import { Scene, SceneLoader, AbstractMesh, Vector3 } from "@babylonjs/core";
import { BattleController } from "./BattleController";
import { BattleCamera } from "./BattleCamera";
import { EffectsManager } from "./EffectsManager";
import { IFightInfo } from "./FightController";

import { ModelsManager } from "./SF3/ModelsManager";
import { UserDataController } from "./SF3/UserData/UserDataController";

export interface ISceneInitializationObject {
  initialize(): void;
  disposePreviousLocation(): void;
}

export class SceneInitializer {
  private _locationMeshes: AbstractMesh[] = [];
  private readonly _scene: Scene;

  constructor(scene: Scene) {
    this._scene = scene;
  }

  private _getInitObjects(): ISceneInitializationObject[] {
    return [
      BattleController.instance,
      {
        initialize: () =>
          BattleCamera.instance.initialize(
            new Vector3(0, 0, 0)
          ),

        disposePreviousLocation: () =>
          BattleCamera.instance.disposePreviousLocation(),
      },
      EffectsManager.instance,
    ];
  }

  async initializeNewLocationScene(
    locationName: string,
    fightInfo: IFightInfo,
    onReady?: () => void
  ): Promise<void> {

    this._disposePreviousLocationScene();

    await this._loadLocationPrefab(locationName);

    for (const obj of this._getInitObjects()) {
      obj.initialize();
    }

    if (!UserDataController.isReady) {
      await UserDataController.create();
      UserDataController.initPlayer();
    }

    const modelsManager =
      new ModelsManager(this._scene);

    await modelsManager.loadModels();

    await modelsManager.createBattleModels(
      UserDataController.getPlayerModelInfo(),
      UserDataController.getDefaultEnemyModelInfo()
    );

    this._scene.onBeforeRenderObservable.add(() => {

      const player =
        modelsManager.player?.root;

      const enemy =
        modelsManager.enemy?.root;

      if (!player || !enemy) {
        return;
      }

      const midpoint =
        player.position
          .add(enemy.position)
          .scale(0.5);

      BattleCamera.instance
        .setTargetPosition(
          midpoint.add(
            new Vector3(0, 100, 0)
          )
        );
    });

    await BattleController.instance
      .initBattle(fightInfo);

    onReady?.();
  }

  private _disposePreviousLocationScene(): void {

    for (const obj of this._getInitObjects()) {
      obj.disposePreviousLocation();
    }

    for (const mesh of this._locationMeshes) {
      mesh.dispose();
    }

    this._locationMeshes = [];
  }

  private async _loadLocationPrefab(
    locationName: string
  ): Promise<void> {

    const path =
      `assets/models/locations/${locationName}/`;

    const file =
      `${locationName}.glb`;

    const result =
      await SceneLoader.ImportMeshAsync(
        "",
        path,
        file,
        this._scene
      );

    this._locationMeshes =
      result.meshes;
  }
}
