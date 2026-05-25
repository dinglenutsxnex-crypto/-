/**
 * SceneManager.ts
 * Mirror of SF3/SceneManager.cs
 *
 * Singleton that manages scene transitions and delegates to SceneInitializer.
 * In Unity this wraps UnityEngine.SceneManagement.SceneManager; here it wraps
 * BabylonJS scene + SceneInitializer.
 */

import { Scene } from "@babylonjs/core";
import { SceneInitializer } from "./SceneInitializer";
import { IFightInfo } from "./FightController";

export enum ESceneType {
  None = 0,
  Fight = 1,
}

export class SceneManager {
  private static _instance: SceneManager;
  static get instance(): SceneManager { return SceneManager._instance; }

  private _sceneType: ESceneType = ESceneType.None;
  private _locationName = "";
  private _sceneInitializer: SceneInitializer;

  onSceneLoadedEvent?: () => void;
  onLocationSceneLoadedEvent?: () => void;

  get sceneType(): ESceneType { return this._sceneType; }
  get locationName(): string { return this._locationName; }

  private constructor(scene: Scene) {
    SceneManager._instance = this;
    this._sceneType = ESceneType.Fight; // we are already inside the fight scene
    this._sceneInitializer = new SceneInitializer(scene);
  }

  static createInstance(scene: Scene): SceneManager {
    return new SceneManager(scene);
  }

  /**
   * Load a location and start the fight.
   * Mirrors SceneManager.LoadLocationScene(locationName, onLoad)
   */
  async loadLocationScene(
    locationName: string,
    onLoad?: () => void,
    fightInfo?: IFightInfo
  ): Promise<void> {
    this._locationName = locationName;

    const fi: IFightInfo = fightInfo ?? {
      battleID: "default",
      fightID: "default",
      roundsToWin: 2,
      roundsToLose: 3,
    };

    await this._sceneInitializer.initializeNewLocationScene(
      locationName,
      fi,
      () => {
        onLoad?.();
        this.onLocationSceneLoadedEvent?.();
        this.onLocationSceneLoadedEvent = undefined;
      }
    );
  }
}
