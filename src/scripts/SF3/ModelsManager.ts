import { Scene, TransformNode, Vector3, Skeleton, AbstractMesh } from "@babylonjs/core";
import { EquipmentType }                from "./Items/EquipmentType";
import { assembleCharacter }            from "./GameModels/ModelComponents";
import { ModelInfo }                    from "./GameModels/ModelInfo";
import { UserDataController }           from "./UserData/UserDataController";
import { buildTrainingEnemyModelInfo }  from "./TrainingEnemyConfig";

export interface IPlayerModel {
  root:     TransformNode;
  skeleton: Skeleton | null;
  meshes:   AbstractMesh[];
  info:     ModelInfo;
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

  get skeletonIdToBoneName(): Map<number, string> {
    return this._skeletonIdToBoneName;
  }

  /** Mirror of Unity's ModelsManager.Initialize() — loads skeleton config,
   *  creates ModelInfo for player (from UserData) and enemy (from TrainingEnemyConfig),
   *  then spawns both characters. */
  async Initialize(): Promise<void> {
    await this._loadSkeletonConfig();

    const playerInfo = UserDataController.isReady
      ? UserDataController.getPlayerModelInfo()
      : ModelInfo.createPlayer();

    const enemyInfo = buildTrainingEnemyModelInfo();

    await Promise.all([
      this._spawnFromModelInfo(playerInfo, true),
      this._spawnFromModelInfo(enemyInfo,  false),
    ]);

    console.log("[ModelsManager] Initialize — player + enemy spawned");
  }

  /** Load skeleton bone-ID mapping (used by animation system). */
  private async _loadSkeletonConfig(): Promise<void> {
    const xml = await fetch("assets/configs/content/bones/configs/skeleton.txt")
      .then(r => r.text());
    this._skeletonIdToBoneName = parseSkeletonIds(xml);
  }

  /** Legacy spawn path kept for external callers. */
  async createBattleModels(playerInfo: ModelInfo, enemyInfo: ModelInfo): Promise<void> {
    await Promise.all([
      this._spawnFromModelInfo(playerInfo, true),
      this._spawnFromModelInfo(enemyInfo,  false),
    ]);
  }

  private async _spawnFromModelInfo(info: ModelInfo, isPlayer: boolean): Promise<void> {
    const head  = info.head || "head__01a";
    const equipment = info.getEquipmentSlots();

    console.log(
      `[ModelsManager] spawning ${isPlayer ? "player" : "enemy"} — ` +
      `head:${head} ` +
      equipment.map(e => `${EquipmentType[e.type]}:${e.model}`).join(" "),
    );

    const result = await assembleCharacter(this._scene, info.gender, head, equipment);

    const root = result.rootNodes[0];
    if (!root) {
      console.error(`[ModelsManager] assembleCharacter returned no root for ${isPlayer ? "player" : "enemy"}`);
      return;
    }

    if (isPlayer) {
      root.position        = new Vector3(-250, 0, 0);
      root.scaling         = new Vector3(-1, 1, 1);
      this._player         = { root, skeleton: result.skeleton, meshes: result.meshes, info };
    } else {
      root.position        = new Vector3(250, 0, 0);
      this._enemy          = { root, skeleton: result.skeleton, meshes: result.meshes, info };
    }
  }
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
