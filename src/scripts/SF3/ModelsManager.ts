/**
 * ModelsManager.ts
 * Mirror of SF3/ModelsManager.cs
 *
 * Owns both fighter model assemblies (player + enemy).
 * SceneInitializer calls Initialize() once per scene load.
 * RoundController.initNewRound() reads player.info and enemy.info for HP.
 *
 * Changes:
 *  - Exposes typed `player` and `enemy` handles so RoundController and
 *    BattleCamera can read live positions / info without any casting.
 *  - createBattleModels() is separated so future per-round resets
 *    can call it without re-running Initialize().
 */

import { Scene, TransformNode } from "@babylonjs/core";
import { ModelInfo }            from "./GameModels/ModelInfo";
import { assembleCharacter }    from "./GameModels/ModelComponents";
import { Gender }               from "../sf3DTO/Gender";
import { EquipmentType }        from "./Items/EquipmentType";

export interface IFighterHandle {
  root: TransformNode;
  info: ModelInfo;
}

export class ModelsManager {
  private static _instance: ModelsManager;
  static get instance(): ModelsManager { return ModelsManager._instance; }

  private _scene:  Scene;
  private _player: IFighterHandle | null = null;
  private _enemy:  IFighterHandle | null = null;

  get player(): IFighterHandle | null { return this._player; }
  get enemy():  IFighterHandle | null { return this._enemy; }

  constructor(scene: Scene) {
    ModelsManager._instance = this;
    this._scene = scene;
  }

  async Initialize(): Promise<void> {
    await this.createBattleModels();
  }

  /**
   * Assembles both fighters from their ModelInfo.
   * Called once by Initialize() and can be called again per-round
   * to respawn models if needed.
   */
  async createBattleModels(): Promise<void> {
    const playerInfo = ModelInfo.createPlayer({ maxLife: 1, warriorPower: 50 });
    const enemyInfo  = ModelInfo.createEnemy ({ maxLife: 1, warriorPower: 40 });

    const [playerResult, enemyResult] = await Promise.all([
      assembleCharacter(
        this._scene,
        playerInfo.gender,
        playerInfo.head,
        playerInfo.getEquipmentSlots(),
      ),
      assembleCharacter(
        this._scene,
        enemyInfo.gender,
        enemyInfo.head,
        enemyInfo.getEquipmentSlots(),
      ),
    ]);

    // Player root
    const pRoot = playerResult.rootNodes[0]
      ?? new TransformNode("player_root", this._scene);
    pRoot.name = "player_root";
    this._player = { root: pRoot, info: playerInfo };

    // Enemy root
    const eRoot = enemyResult.rootNodes[0]
      ?? new TransformNode("enemy_root", this._scene);
    eRoot.name = "enemy_root";
    this._enemy = { root: eRoot, info: enemyInfo };

    console.log("[ModelsManager] Battle models created ✔");
  }

  dispose(): void {
    this._player?.root.dispose();
    this._enemy?.root.dispose();
    this._player = null;
    this._enemy  = null;
  }
}
