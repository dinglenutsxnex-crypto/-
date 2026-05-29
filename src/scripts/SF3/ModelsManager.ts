import { Scene, TransformNode, Vector3, Skeleton, AbstractMesh } from "@babylonjs/core";
import { EquipmentType }      from "./Items/EquipmentType";
import { assembleCharacter }  from "./GameModels/ModelComponents";
import { ModelInfo }          from "./GameModels/ModelInfo";
import { UserDataController } from "./UserData/UserDataController";

export interface IPlayerModel {
  root:     TransformNode;
  skeleton: Skeleton | null;
  meshes:   AbstractMesh[];
  info:     ModelInfo;          // keep a ref so BattleController can read it later
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

  // ── Skeleton config ────────────────────────────────────────────────────────

  async loadModels(): Promise<void> {
    const xml = await fetch("assets/configs/content/bones/configs/skeleton.txt")
      .then(r => r.text());
    this._skeletonIdToBoneName = parseSkeletonIds(xml);
  }

  get skeletonIdToBoneName(): Map<number, string> {
    return this._skeletonIdToBoneName;
  }

  // ── Primary spawn path (used by FightScene) ────────────────────────────────

  /**
   * Spawn both characters from explicit ModelInfo objects.
   * This is the main path — FightScene always calls this.
   */
  async createBattleModels(playerInfo: ModelInfo, enemyInfo: ModelInfo): Promise<void> {
    await Promise.all([
      this._spawnFromModelInfo(playerInfo, true),
      this._spawnFromModelInfo(enemyInfo,  false),
    ]);
    console.log("[ModelsManager] player + enemy spawned");
  }

  // ── Convenience spawns (fall back to UserDataController) ──────────────────

  /**
   * Spawn just the player — reads from UserDataController if loaded,
   * otherwise uses safe hardcoded defaults.
   */
  async spawnPlayer(): Promise<void> {
    const info = UserDataController.isReady
      ? UserDataController.getPlayerModelInfo()
      : ModelInfo.createPlayer();
    await this._spawnFromModelInfo(info, true);
  }

  /**
   * Spawn just the enemy — always uses the default enemy config
   * (no server needed).
   */
  async spawnEnemy(): Promise<void> {
    const info = UserDataController.isReady
      ? UserDataController.getDefaultEnemyModelInfo()
      : ModelInfo.createEnemy();
    await this._spawnFromModelInfo(info, false);
  }

  // ── Internal ───────────────────────────────────────────────────────────────

  private async _spawnFromModelInfo(info: ModelInfo, isPlayer: boolean): Promise<void> {
    const armor  = info.getEquippedModel(EquipmentType.Armor)  ?? "arm__base";
    const helmet = info.getEquippedModel(EquipmentType.Helmet) ?? "hair-01";
    const head   = info.head || "head__01a";

    console.log(
      `[ModelsManager] spawning ${isPlayer ? "player" : "enemy"} — ` +
      `head:${head} armor:${armor} helmet:${helmet}`,
    );

    const result = await assembleCharacter(
      this._scene,
      info.gender,
      head,
      [
        { type: EquipmentType.Armor,  model: armor  },
        { type: EquipmentType.Helmet, model: helmet },
      ],
    );

    const root = result.rootNodes[0];
    if (!root) {
      console.error(`[ModelsManager] assembleCharacter returned no root node for ${isPlayer ? "player" : "enemy"}`);
      return;
    }

    if (isPlayer) {
      root.position        = new Vector3(-250, 0, 0);
      root.scaling         = new Vector3(-1, 1, 1);   // mirror to face right
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
