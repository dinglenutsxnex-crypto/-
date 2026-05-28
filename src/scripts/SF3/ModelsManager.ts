import {
  Scene,
  TransformNode,
  Vector3,
  Skeleton,
  AbstractMesh
} from "@babylonjs/core";

import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter } from "./GameModels/ModelComponents";
import { ModelInfo } from "./GameModels/ModelInfo";

export interface IPlayerModel {
  root: TransformNode;
  skeleton: Skeleton | null;
  meshes: AbstractMesh[];
}

export class ModelsManager {

  private static _instance: ModelsManager;

  static get instance(): ModelsManager {
    return ModelsManager._instance;
  }

  private readonly _scene: Scene;

  private _player?: IPlayerModel;
  private _enemy?: IPlayerModel;

  constructor(scene: Scene) {

    ModelsManager._instance = this;

    this._scene = scene;
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
    Promise<void> {

    // preload hook
  }

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

  private async _spawnFromModelInfo(
    info: ModelInfo,
    isPlayer: boolean
  ): Promise<void> {

    const armorModel =
      info.getEquippedModel(
        EquipmentType.Armor
      ) ?? "arm__base";

    const helmetModel =
      info.getEquippedModel(
        EquipmentType.Helmet
      ) ?? "hair-01";

    const result = await assembleCharacter(
      this._scene,
      info.gender ?? Gender.Male,
      info.head || "head__01a",
      [
        {
          type: EquipmentType.Armor,
          model: armorModel,
        },
        {
          type: EquipmentType.Helmet,
          model: helmetModel,
        },
      ],
    );

    const root = result.rootNodes[0];

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

    if (isPlayer) {

      this._player = {
        root,
        skeleton: result.skeleton,
        meshes: result.meshes,
      };

    } else {

      this._enemy = {
        root,
        skeleton: result.skeleton,
        meshes: result.meshes,
      };
    }
  }
}
