import { Scene, TransformNode, Vector3, Skeleton, AbstractMesh } from "@babylonjs/core";
import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter } from "./GameModels/ModelComponents";
import { ModelInfo } from "./GameModels/ModelInfo";
import { UserDataController } from "./UserData/UserDataController";

export interface IPlayerModel {
  root:     TransformNode;
  skeleton: Skeleton | null;
  meshes:   AbstractMesh[];
}

export class ModelsManager {
  private static _instance: ModelsManager;
  static get instance(): ModelsManager { return ModelsManager._instance; }

  private readonly _scene: Scene;
  private _player?: IPlayerModel;
  private _enemy?:  IPlayerModel;
  private _skeletonIdToBoneName: Map<number, string> = new Map();

  constructor(scene: Scene) {
    ModelsManager._instance = this;
    this._scene = scene;
  }

  get player(): IPlayerModel | undefined { return this._player; }
  get enemy():  IPlayerModel | undefined { return this._enemy; }

  async loadModels(): Promise<void> {
    const xml = await fetch("assets/configs/content/bones/configs/skeleton.txt")
      .then(r => r.text());

    this._skeletonIdToBoneName = parseSkeletonIds(xml);
  }

  async createBattleModels(
    playerInfo?: ModelInfo,
    enemyInfo?: ModelInfo
  ): Promise<void> {

    const finalPlayer =
      playerInfo ?? UserDataController.getPlayerModelInfo();

    const finalEnemy =
      enemyInfo ?? UserDataController.getDefaultEnemyModelInfo();

    await Promise.all([
      this._spawnFromModelInfo(finalPlayer, true),
      this._spawnFromModelInfo(finalEnemy, false),
    ]);
  }

  async spawnPlayer(
    gender?: Gender,
    headModel?: string,
    armorModel?: string,
    helmetModel?: string,
  ): Promise<void> {

    if (
      gender === undefined &&
      headModel === undefined &&
      armorModel === undefined &&
      helmetModel === undefined
    ) {
      await this._spawnFromModelInfo(
        UserDataController.getPlayerModelInfo(),
        true
      );

      return;
    }

    const info = new ModelInfo();

    info.gender = gender ?? Gender.Male;
    info.head = headModel ?? "head__01a";

    info.setEquipment(
      EquipmentType.Armor,
      armorModel ?? "arm-base"
    );

    info.setEquipment(
      EquipmentType.Helmet,
      helmetModel ?? "hair-01"
    );

    await this._spawnFromModelInfo(info, true);
  }

  async spawnEnemy(
    gender?: Gender,
    headModel?: string,
    armorModel?: string,
    helmetModel?: string,
  ): Promise<void> {

    if (
      gender === undefined &&
      headModel === undefined &&
      armorModel === undefined &&
      helmetModel === undefined
    ) {
      await this._spawnFromModelInfo(
        UserDataController.getDefaultEnemyModelInfo(),
        false
      );

      return;
    }

    const info = new ModelInfo();

    info.gender = gender ?? Gender.Male;
    info.head = headModel ?? "head__01a";

    info.setEquipment(
      EquipmentType.Armor,
      armorModel ?? "arm-base"
    );

    info.setEquipment(
      EquipmentType.Helmet,
      helmetModel ?? "hair-01"
    );

    await this._spawnFromModelInfo(info, false);
  }

  private async _spawnFromModelInfo(
    info: ModelInfo,
    isPlayer: boolean
  ): Promise<void> {

    const armorModel =
      info.getEquippedModel(EquipmentType.Armor)
      ?? "arm-base";

    const helmetModel =
      info.getEquippedModel(EquipmentType.Helmet)
      ?? "hair-01";

    const result = await assembleCharacter(
      this._scene,
      info.gender,
      info.head || "head__01a",
      [
        { type: EquipmentType.Armor, model: armorModel },
        { type: EquipmentType.Helmet, model: helmetModel },
      ],
    );

    const root = result.rootNodes[0];

    if (!root) {
      return;
    }

    if (isPlayer) {
      root.position = new Vector3(-250, 0, 0);
      root.scaling = new Vector3(-1, 1, 1);

      this._player = {
        root,
        skeleton: result.skeleton,
        meshes: result.meshes,
      };
    } else {
      root.position = new Vector3(250, 0, 0);

      this._enemy = {
        root,
        skeleton: result.skeleton,
        meshes: result.meshes,
      };
    }
  }
}

function parseSkeletonIds(xmlText: string): Map<number, string> {
  const map = new Map<number, string>();

  const regex = /<Bone\s+Name="([^"]+)"\s+ID="(\d+)"/g;

  let match: RegExpExecArray | null;

  while ((match = regex.exec(xmlText)) !== null) {
    map.set(parseInt(match[2], 10), match[1]);
  }

  return map;
}
