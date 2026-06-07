/**
 * FightScene.ts
 *
 * Thin wrapper that boots a full fight via SceneInitializer.
 *
 * Changes from the old stub:
 *  - Accepts optional IFightHUDCallbacks and passes them through to
 *    SceneInitializer so BattleController is wired to the HUD before
 *    initBattle() is called.
 */

import { Scene }             from "@babylonjs/core";
import { SceneManager }      from "../SceneManager";
import type { IFightInfo, IFightHUDCallbacks } from "../SF3/FightController";

export class FightScene {
  private readonly _scene:   Scene;
  private _sceneManager!:    SceneManager;

  constructor(scene: Scene) { this._scene = scene; }

  async initialize(
    locationName: string,
    fightInfo:    IFightInfo,
    hudCallbacks?: IFightHUDCallbacks,
  ): Promise<void> {
    this._sceneManager = SceneManager.createInstance(this._scene);
    await this._sceneManager.loadLocationScene(locationName, fightInfo, hudCallbacks);
    console.log("[FightScene] initialized ✔");
  }

  dispose(): void {
    this._sceneManager?.dispose();
  }
}
