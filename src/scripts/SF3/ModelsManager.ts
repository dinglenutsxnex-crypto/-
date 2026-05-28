import {
  Scene,
  TransformNode,
  Vector3,
  Skeleton,
  AbstractMesh
} from "@babylonjs/core";

import { Gender } from "../sf3DTO/Gender";
import { assembleCharacter } from "./GameModels/ModelComponents";
import { ModelInfo } from "./GameModels/ModelInfo";

export interface IPlayerModel {
  root: TransformNode;
  skeleton: Skeleton | null;
  meshes: AbstractMesh[];
}

export class ModelsManager {

  private static _instance: ModelsManager;

  static get instance():
    ModelsManager {

    return ModelsManager._instance;
  }

  readonly playerInfo =
    new ModelInfo();

  readonly enemyInfo =
    new ModelInfo();

  private readonly _scene: Scene;

  private _player?:
    IPlayerModel;

  private _enemy?:
    IPlayerModel;

  constructor(scene: Scene) {

    ModelsManager._instance =
      this;

    this._scene =
      scene;
  }

  get player():
    IPlayerModel | undefined {

    return this._player;
  }

  get enemy():
    IPlayerModel | undefined {

    return this._enemy;
  }

  async loadModels():
    Promise<void> {}

  async createBattleModels(
    playerInfo: ModelInfo,
    enemyInfo: ModelInfo
  ): Promise<void> {

    await this._spawnFromModelInfo(
      playerInfo,
      true
    );

    await this._spawnFromModelInfo(
      enemyInfo,
      false
    );
  }

  async spawnPlayer():
    Promise<void> {

    await this._spawnFromModelInfo(
      this.playerInfo,
      true
    );
  }

  async spawnEnemy():
    Promise<void> {

    await this._spawnFromModelInfo(
      this.enemyInfo,
      false
    );
  }

  private async _spawnFromModelInfo(
    info: ModelInfo,
    isPlayer: boolean
  ): Promise<void> {

    const result =
      await assembleCharacter(
        this._scene,
        info.gender ?? Gender.Male,
        info.head ?? "head__01a",
        []
      );

    const root =
      result.rootNodes[0];

    if (!root) {
      return;
    }

    root.position =
      isPlayer
        ? new Vector3(-180, 0, 0)
        : new Vector3(180, 0, 0);

    root.rotation.y =
      isPlayer
        ? Math.PI / 2
        : -Math.PI / 2;

    const model = {
      root,
      skeleton:
        result.skeleton,
      meshes:
        result.meshes,
    };

    if (isPlayer) {
      this._player = model;
    } else {
      this._enemy = model;
    }
  }
}
