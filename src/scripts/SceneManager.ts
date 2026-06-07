/**
 * SceneManager.ts
 * Mirror of SF3/SceneManager.cs
 *
 * Changes:
 *  - loadLocationScene() now accepts optional IFightHUDCallbacks and
 *    forwards them to SceneInitializer so BattleController gets wired
 *    to the HUD before initBattle() runs.
 */

import { Scene }          from "@babylonjs/core";
import { SceneInitializer } from "./SceneInitializer";
import type { IFightInfo, IFightHUDCallbacks } from "./SF3/FightController";

export enum ESceneType { None = 0, Fight = 1 }

export class SceneManager {
  private static _instance: SceneManager;
  static get instance(): SceneManager { return SceneManager._instance; }

  private _sceneType:        ESceneType = ESceneType.None;
  private _locationName      = "";
  private _sceneInitializer: SceneInitializer;

  get sceneType():    ESceneType { return this._sceneType; }
  get locationName(): string     { return this._locationName; }

  private constructor(scene: Scene) {
    SceneManager._instance    = this;
    this._sceneType           = ESceneType.Fight;
    this._sceneInitializer    = new SceneInitializer(scene);
  }

  static createInstance(scene: Scene): SceneManager {
    return new SceneManager(scene);
  }

  async loadLocationScene(
    locationName:  string,
    fightInfo?:    IFightInfo,
    hudCallbacks?: IFightHUDCallbacks,
  ): Promise<void> {
    this._locationName  = locationName;
    const fi: IFightInfo = fightInfo ?? {
      battleID:     "default",
      fightID:      "default",
      roundsToWin:  2,
      roundsToLose: 3,
    };
    await this._sceneInitializer.initializeNewLocationScene(locationName, fi, hudCallbacks);
  }

  dispose(): void {
    this._sceneInitializer.dispose();
  }
}
