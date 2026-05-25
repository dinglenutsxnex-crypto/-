import { Scene, TransformNode, Vector3, Skeleton, Bone, Quaternion, AbstractMesh } from "@babylonjs/core";
import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter, ModelAssemblyResult } from "./GameModels/ModelComponents";
import { AnimationBinaries } from "./Moves/AnimationBinaries";

export interface IPlayerModel {
  root: TransformNode;
  skeleton: Skeleton | null;
  meshes: AbstractMesh[];
}

export class ModelsManager {
  private static _instance: ModelsManager;
  static get instance(): ModelsManager { return ModelsManager._instance; }

  private readonly _scene: Scene;
  private _player?: IPlayerModel;
  private _enemy?: IPlayerModel;

  private _skeletonIdToBoneName: Map<number, string> = new Map();

  constructor(scene: Scene) {
    ModelsManager._instance = this;
    this._scene = scene;
  }

  get player(): IPlayerModel | undefined { return this._player; }
  get enemy(): IPlayerModel | undefined { return this._enemy; }

  async loadModels(): Promise<void> {
    const [skeletonXml] = await Promise.all([
      loadBytes("assets/configs/content/bones/configs/skeleton.txt").then(b => new TextDecoder().decode(b)),
    ]);
    this._skeletonIdToBoneName = parseSkeletonIds(skeletonXml);
  }

  async spawnPlayer(
    gender: Gender = Gender.Male,
    headModel = "head__01a",
    armorModel = "arm__str_15",
    helmetModel = "hair-01",
  ): Promise<void> {
    const result = await assembleCharacter(this._scene, gender, headModel, [
      { type: EquipmentType.Armor, model: armorModel },
      { type: EquipmentType.Helmet, model: helmetModel },
    ]);
    const root = result.rootNodes[0];
    if (!root) return;
    root.position = new Vector3(-250, 0, 0);
    root.scaling = new Vector3(-1, 1, 1);
    this._player = { root, skeleton: result.skeleton, meshes: result.meshes };
  }

  async spawnEnemy(
    gender: Gender = Gender.Male,
    headModel = "head__01a",
    armorModel = "arm__base",
    helmetModel = "hair-01",
  ): Promise<void> {
    const result = await assembleCharacter(this._scene, gender, headModel, [
      { type: EquipmentType.Armor, model: armorModel },
      { type: EquipmentType.Helmet, model: helmetModel },
    ]);
    const root = result.rootNodes[0];
    if (!root) return;
    root.position = new Vector3(250, 0, 0);
    this._enemy = { root, skeleton: result.skeleton, meshes: result.meshes };
  }
}

async function loadBytes(path: string): Promise<Uint8Array> {
  const res = await fetch(path);
  if (!res.ok) throw new Error(`Failed to load ${path}`);
  return new Uint8Array(await res.arrayBuffer());
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
