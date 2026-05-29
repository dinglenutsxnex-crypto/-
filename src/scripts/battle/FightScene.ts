import { Scene } from "@babylonjs/core";
import { SceneManager } from "../SceneManager";
import { IFightInfo } from "../FightController";

export class FightScene {
  private readonly _scene: Scene;
  private _sceneManager!: SceneManager;

  constructor(scene: Scene) { this._scene = scene; }

  async initialize(locationName: string, fightInfo: IFightInfo): Promise<void> {
    this._sceneManager = SceneManager.createInstance(this._scene);
    await this._sceneManager.loadLocationScene(locationName, fightInfo);
    console.log("[FightScene] initialized ✔");
  }

  dispose(): void {
    this._sceneManager?.dispose();
  }
}
