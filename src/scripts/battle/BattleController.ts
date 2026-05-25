import { Scene } from "@babylonjs/core";
import { FightController, IFightInfo } from "../FightController";
import { EffectsManager } from "../EffectsManager";

export class BattleController {
  private readonly _scene: Scene;
  private readonly _fightController: FightController;
  private _battleEnabled = false;
  private _eventsEnabled = false;
  private _paused = false;

  get fightController(): FightController { return this._fightController; }
  get battleEnabled(): boolean { return this._battleEnabled; }

  constructor(scene: Scene) {
    this._scene = scene;
    this._fightController = new FightController();
  }

  initialize(): void {
    this._battleEnabled = false;
    this._paused = false;
  }

  async initBattle(fightInfo: IFightInfo): Promise<void> {
    this._battleEnabled = true;
    await this._fightController.initFight(fightInfo);
  }

  update(dt: number): void {
    if (!this._battleEnabled || this._paused) return;
  }

  pause(): void {
    this._paused = true;
  }

  resume(): void {
    this._paused = false;
  }

  dispose(): void {
    this._battleEnabled = false;
    this._eventsEnabled = false;
  }
}
