import { Scene, TransformNode, Vector3, Skeleton, AbstractMesh } from "@babylonjs/core";
import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter } from "./GameModels/ModelComponents";
import { ModelInfo } from "./GameModels/ModelInfo";

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

  async createBattleModels(playerInfo: ModelInfo, enemyInfo: ModelInfo): Promise<void> {
    await Promise.all([
      this._spawnFromModelInfo(playerInfo, true),
      this._spawnFromModelInfo(enemyInfo, false),
    ]);
  }

  async spawnPlayer(
    gender:      Gender = Gender.Male,
    headModel   = "head__01a",
    armorModel  = "arm__str_15",
    helmetModel = "hair-01",
  ): Promise<void> {
    const info = new ModelInfo();
    info.gender = gender;
    info.head   = headModel;
    info.setEquipment(EquipmentType.Armor,  armorModel);
    info.setEquipment(EquipmentType.Helmet, helmetModel);
    await this._spawnFromModelInfo(info, true);
  }

  async spawnEnemy(
    gender:      Gender = Gender.Male,
    headModel   = "head__01a",
    armorModel  = "arm__base",
    helmetModel = "hair-01",
  ): Promise<void> {
    const info = new ModelInfo();
    info.gender = gender;
    info.head   = headModel;
    info.setEquipment(EquipmentType.Armor,  armorModel);
    info.setEquipment(EquipmentType.Helmet, helmetModel);
    await this._spawnFromModelInfo(info, false);
  }

  private async _spawnFromModelInfo(info: ModelInfo, isPlayer: boolean): Promise<void> {
    const armorModel  = info.getEquippedModel(EquipmentType.Armor)  ?? (isPlayer ? "arm__str_15" : "arm__base");
    const helmetModel = info.getEquippedModel(EquipmentType.Helmet) ?? "hair-01";

    const result = await assembleCharacter(
      this._scene,
      info.gender,
      info.head || "head__01a",
      [
        { type: EquipmentType.Armor,  model: armorModel  },
        { type: EquipmentType.Helmet, model: helmetModel },
      ],
    );

    const root = result.rootNodes[0];
    if (!root) return;

    if (isPlayer) {
      root.position = new Vector3(-250, 0, 0);
      root.scaling  = new Vector3(-1, 1, 1);
      this._player  = { root, skeleton: result.skeleton, meshes: result.meshes };
    } else {
      root.position = new Vector3(250, 0, 0);
      this._enemy   = { root, skeleton: result.skeleton, meshes: result.meshes };
    }
  }
}

async function loadBytes(path: string): Promise<Uint8Array> {
  const res = await fetch(path);
  if (!res.ok) throw new Error(`Failed to load ${path}`);
  return new Uint8Array(await res.arrayBuffer());
}

function parseSkeletonIds(xmlText: string): Map<number, string> {
  const map   = new Map<number, string>();
  const regex = /<Bone\s+Name="([^"]+)"\s+ID="(\d+)"/g;
  let match: RegExpExecArray | null;
  while ((match = regex.exec(xmlText)) !== null) {
    map.set(parseInt(match[2], 10), match[1]);
  }
  return map;
}
