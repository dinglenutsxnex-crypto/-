/**
 * BattleController.ts
 * Mirror of SF3/BattleController.cs
 *
 * Top-level fight scene controller.
 * Owns FightController (via SF3/FightController), wires the per-frame
 * update loop, and exposes the event bus used by the rest of the system.
 *
 * Changes from the old stub:
 *   – Imports from SF3/FightController (the real one with RoundController).
 *   – update(dt) actually calls fightController.update(dt) every frame.
 *   – Exposes setHUDCallbacks() so EnterPointScene can wire the FightHUD.
 *   – createInstance() registers itself in the engine update loop.
 */

import { Scene }                       from "@babylonjs/core";
import {
  FightController,
  IFightInfo,
  IFightHUDCallbacks,
  EFightStage,
}                                      from "./SF3/FightController";
import { EffectsManager }              from "./EffectsManager";
import { BattleCamera }                from "./BattleCamera";

export class BattleController {
  private static _instance: BattleController;
  static get instance(): BattleController { return BattleController._instance; }

  private readonly _scene:            Scene;
  private readonly _fightController:  FightController;

  private _battleEnabled  = false;
  private _eventsEnabled  = true;

  // ─── Event bus (mirrors BattleEventsControl) ──────────────────────────────
  private _eventCallbacks: Map<number, ((data?: any) => void)[]> = new Map();

  get fightController(): FightController { return this._fightController; }

  private constructor(scene: Scene) {
    BattleController._instance = this;
    this._scene           = scene;
    this._fightController = new FightController();
  }

  static createInstance(scene: Scene): BattleController {
    const bc = new BattleController(scene);
    // Register per-frame update with the Babylon render loop
    scene.onBeforeRenderObservable.add(() => {
      const dt = scene.getEngine().getDeltaTime() / 1000;
      bc.update(dt);
    });
    return bc;
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

  async initBattle(fightInfo: IFightInfo): Promise<void> {
    console.log("[BattleController] initBattle");
    await this._fightController.initFight(fightInfo);
    this._battleEnabled = true;
  }

  // ─── HUD wiring (call before initBattle) ─────────────────────────────────

  setHUDCallbacks(cbs: IFightHUDCallbacks): void {
    this._fightController.setHUDCallbacks(cbs);
  }

  // ─── Per-frame update ────────────────────────────────────────────────────

  private update(dt: number): void {
    if (!this._battleEnabled) return;
    this._fightController.update(dt);
  }

  // ─── Enable / disable ────────────────────────────────────────────────────

  battleEnable(enable: boolean): void  { this._battleEnabled = enable; }
  eventsEnable(enable: boolean): void  { this._eventsEnabled = enable; }

  static get isBattleEnabled(): boolean {
    return BattleController._instance?._battleEnabled ?? false;
  }

  // ─── Event bus ───────────────────────────────────────────────────────────

  static registerEventCallback(
    eventId: number,
    callback: (data?: any) => void,
  ): void {
    const i = BattleController._instance;
    if (!i) return;
    if (!i._eventCallbacks.has(eventId)) i._eventCallbacks.set(eventId, []);
    i._eventCallbacks.get(eventId)!.push(callback);
  }

  static throwEvent(eventId: number, data?: any): void {
    const i = BattleController._instance;
    if (!i?._eventsEnabled) return;
    const cbs = i._eventCallbacks.get(eventId) ?? [];
    for (const cb of cbs) cb(data);
  }

  // ─── Game flow helpers ───────────────────────────────────────────────────

  static resumeGame(): void { BattleController._instance?.battleEnable(true); }
  static pauseGame():  void { BattleController._instance?.battleEnable(false); }
}
