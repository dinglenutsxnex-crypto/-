import { Scene } from "@babylonjs/core";
import { SceneInitializer } from "../SceneInitializer";
import { IFightInfo } from "../FightController";

export class FightScene {
  private readonly _scene: Scene;
  private _initializer: SceneInitializer | null = null;

  constructor(scene: Scene) { this._scene = scene; }

  async initialize(locationName: string, fightInfo: IFightInfo): Promise<void> {
    this._initializer = new SceneInitializer(this._scene);
    await this._initializer.initializeNewLocationScene(locationName, fightInfo);
    console.log("[FightScene] initialized \u2714");
  }

  dispose(): void {
    this._initializer?.dispose();
    this._initializer = null;
  }
}
