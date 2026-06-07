/**
 * BattleController.ts
 * Mirror of SF3/BattleController.cs
 *
 * Top-level fight scene controller. Lives on the "battle_controller" node.
 * Owns FightController and wires scene-level lifecycle calls.
 */

import { Scene } from "@babylonjs/core";
import { FightController, IFightInfo } from "./FightController";
import { EffectsManager } from "./EffectsManager";
import { BattleCamera } from "./BattleCamera";
import { FightHUD } from "./ui/FightHUD";

export class BattleController {
  private static _instance: BattleController;
  static get instance(): BattleController { return BattleController._instance; }

  private readonly _scene: Scene;
  private readonly _fightController: FightController;

  private _battleEnabled = false;
  private _eventsEnabled = false;

  // ─── Event system (minimal port of BattleEventsControl) ──────────────────
  private _eventCallbacks: Map<string, ((data?: any) => void)[]> = new Map();

  get fightController(): FightController { return this._fightController; }

  private constructor(scene: Scene) {
    BattleController._instance = this;
    this._scene = scene;
    this._fightController = new FightController();
    this._battleEnabled = false;
    this._eventsEnabled = true;
  }

  static createInstance(scene: Scene): BattleController {
    return new BattleController(scene);
  }

  // ─── ISceneInitializationObject ──────────────────────────────────────────

  initialize(): void {
    this._fightController.initialize();
    this._battleEnabled = false;
  }

  disposePreviousLocation(): void {
    this._battleEnabled = false;
    this._eventCallbacks.clear();
  }

  /**
   * Mirrors BattleController.InitBattle().
   * @param hud Optional FightHUD to wire into the fight stage machine.
   *            For the Dojo this is provided; for scenes without HUD pass undefined.
   */
  async initBattle(fightInfo: IFightInfo, hud?: FightHUD): Promise<void> {
    console.log("[BattleController] InitBattle");
    await this._fightController.initFight(fightInfo, hud);
    this._battleEnabled = true;
  }

  // ─── Enable / disable ────────────────────────────────────────────────────

  battleEnable(enable: boolean): void {
    this._battleEnabled = enable;
  }

  eventsEnable(enable: boolean): void {
    this._eventsEnabled = enable;
  }

  static get isBattleEnabled(): boolean {
    return BattleController._instance._battleEnabled;
  }

  // ─── Event bus ───────────────────────────────────────────────────────────

  static registerEventCallback(
    eventName: string,
    callback: (data?: any) => void
  ): void {
    const i = BattleController._instance;
    if (!i._eventCallbacks.has(eventName)) {
      i._eventCallbacks.set(eventName, []);
    }
    i._eventCallbacks.get(eventName)!.push(callback);
  }

  static throwEvent(eventName: string, data?: any): void {
    const i = BattleController._instance;
    if (!i._eventsEnabled) return;
    const cbs = i._eventCallbacks.get(eventName) ?? [];
    for (const cb of cbs) cb(data);
  }

  // ─── Game flow helpers ───────────────────────────────────────────────────

  static resumeGame(): void {
    console.log("[BattleController] ResumeGame");
  }

  static pauseGame(): void {
    console.log("[BattleController] PauseGame");
  }
}
