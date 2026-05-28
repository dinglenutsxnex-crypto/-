import {
  Scene,
  SceneLoader,
  AbstractMesh,
  Vector3,
  Color3,
  Color4,
  DirectionalLight,
  HemisphericLight,
  ShadowGenerator,
  FreeCamera,
  TransformNode,
} from "@babylonjs/core";

import "@babylonjs/loaders";

import { BattleController } from "../BattleController";
import { BattleCamera } from "../BattleCamera";
import { EffectsManager } from "../EffectsManager";
import { FightController, IFightInfo } from "../FightController";
import { CameraConfiguration } from "../CameraConfiguration";
import { ModelsManager } from "../SF3/ModelsManager";
import { UserDataController } from "../SF3/UserData/UserDataController";

export class FightScene {
  private readonly _scene: Scene;

  private _locationMeshes: AbstractMesh[] = [];
  private _locationName = "";

  private _battleController!: BattleController;
  private _fightController!: FightController;
  private _battleCamera!: BattleCamera;
  private _effectsManager!: EffectsManager;

  private _cameraConfig!: CameraConfiguration;
  private _cameraNode!: TransformNode;
  private _mainCamera!: FreeCamera;

  private _directionalLight!: DirectionalLight;
  private _hemisphericLight!: HemisphericLight;
  private _shadowGenerator!: ShadowGenerator;

  constructor(scene: Scene) {
    this._scene = scene;
  }

  async initialize(
    locationName: string,
    fightInfo: IFightInfo
  ): Promise<void> {

    this._locationName = locationName;

    this._setupScene();

    await this._loadLocation(locationName);

    if (!UserDataController.isReady) {
      await UserDataController.create();
      UserDataController.initPlayer();
    }

    const modelsManager = new ModelsManager(this._scene);

    await modelsManager.loadModels();

    const playerInfo =
      UserDataController.getPlayerModelInfo();

    const enemyInfo =
      UserDataController.getDefaultEnemyModelInfo();

    await modelsManager.createBattleModels(
      playerInfo,
      enemyInfo
    );

    // Proper fight spacing like Unity
    if (modelsManager.player?.root) {
      modelsManager.player.root.position =
        new Vector3(-180, 0, 0);

      modelsManager.player.root.rotation.y =
        Math.PI / 2;
    }

    if (modelsManager.enemy?.root) {
      modelsManager.enemy.root.position =
        new Vector3(180, 0, 0);

      modelsManager.enemy.root.rotation.y =
        -Math.PI / 2;
    }

    this._cameraConfig =
      this._createDefaultCameraConfig();

    this._battleCamera =
      BattleCamera.createInstance(
        this._cameraNode,
        this._mainCamera,
        this._cameraConfig,
        this._scene
      );

    this._battleCamera.initialize(
      Vector3.Zero()
    );

    // Critical part:
    // connect actual fighter positions to camera tracking
    this._battleCamera.setModels(
      {
        modelBackPosX:
          modelsManager.player?.root.position.x ?? -180,

        centerOfMassY: 120,

        centerOfMassPosition:
          new Vector3(
            modelsManager.player?.root.position.x ?? -180,
            120,
            0
          ),
      },

      {
        modelBackPosX:
          modelsManager.enemy?.root.position.x ?? 180,

        centerOfMassY: 120,

        centerOfMassPosition:
          new Vector3(
            modelsManager.enemy?.root.position.x ?? 180,
            120,
            0
          ),
      }
    );

    // THIS is what actually enables
    // follow + framing behavior
    this._battleCamera.activateBattleCamera(true);

    this._battleController =
      BattleController.createInstance(this._scene);

    this._fightController =
      new FightController();

    this._effectsManager =
      EffectsManager.createInstance(this._scene);

    this._battleController.initialize();

    this._effectsManager.initialize();

    await this._battleController.initBattle(
      fightInfo
    );

    // realtime follow update
    this._scene.onBeforeRenderObservable.add(() => {

      const player =
        modelsManager.player?.root;

      const enemy =
        modelsManager.enemy?.root;

      if (!player || !enemy) {
        return;
      }

      const middle =
        player.position
          .add(enemy.position)
          .scale(0.5);

      const distance =
        Vector3.Distance(
          player.position,
          enemy.position
        );

      this._cameraNode.position.x =
        middle.x;

      this._cameraNode.position.y =
        120 + distance * 0.05;

      this._mainCamera.position.z =
        -560 - distance * 0.35;
    });
  }

  private _setupScene(): void {

    this._scene.clearColor =
      new Color4(0.32, 0.32, 0.32, 0.02);

    this._scene.ambientColor =
      new Color3(0.6, 0.6, 0.6);

    const hemi =
      new HemisphericLight(
        "hemi",
        new Vector3(0, 1, 0.5),
        this._scene
      );

    hemi.intensity = 0.7;

    this._hemisphericLight = hemi;

    const dir =
      new DirectionalLight(
        "dir",
        new Vector3(-0.5, -1, -0.3),
        this._scene
      );

    dir.position =
      new Vector3(300, 500, 300);

    dir.intensity = 1.2;

    this._directionalLight = dir;

    const sg =
      new ShadowGenerator(2048, dir);

    sg.useBlurExponentialShadowMap = true;
    sg.blurKernel = 32;

    this._shadowGenerator = sg;

    const parentNode =
      new TransformNode(
        "battle_camera",
        this._scene
      );

    this._cameraNode = parentNode;

    const cam =
      new FreeCamera(
        "Main Camera",
        new Vector3(0, 120, -560),
        this._scene
      );

    cam.parent = parentNode;

    cam.setTarget(
      new Vector3(0, 110, 0)
    );

    cam.minZ = 10;
    cam.maxZ = 5000;

    cam.fov = 30 * Math.PI / 180;

    this._mainCamera = cam;

    this._scene.activeCamera = cam;
  }

  private _createDefaultCameraConfig():
    CameraConfiguration {

    return CameraConfiguration.fromJSON({
      settings: [
        {
          aspect: 1,

          minXPosition: -9999,
          maxXPosition: 9999,

          minYPosition: 0,
          maxYPosition: 400,

          startFollowYUp: 150,
          startFollowYBot: 50,

          startRotateYUp: 180,
          startRotateYBot: 20,

          farClipPlane: 5000,

          botVerticalAngle: 10,
          upVerticalAngle: 30,

          leftHorizontalAngle: 30,
          rightHorizontalAngle: 30,

          camZOffset: 0,
        },
      ],
    });
  }

  private async _loadLocation(
    locationName: string
  ): Promise<void> {

    try {

      const result =
        await SceneLoader.ImportMeshAsync(
          "",
          "assets/locations/",
          `${locationName}.glb`,
          this._scene
        );

      this._locationMeshes =
        result.meshes;

      for (const mesh of this._locationMeshes) {

        if (
          mesh.name === "ShadowReciever" ||
          mesh.name === "ShadowReceiver"
        ) {
          mesh.isVisible = false;
          continue;
        }

        mesh.receiveShadows = true;
      }

    } catch (err) {

      console.warn(
        `[FightScene] Could not load location "${locationName}": ${err}`
      );
    }
  }

  dispose(): void {

    for (const mesh of this._locationMeshes) {
      mesh.dispose();
    }

    this._locationMeshes = [];
  }
}
