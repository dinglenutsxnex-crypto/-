import { Scene } from "@babylonjs/core";
import { SceneInitializer } from "../SceneInitializer";
import { IFightInfo } from "../FightController";
import { FightHUD } from "../ui/FightHUD";

export class FightScene {
  private readonly _scene: Scene;
  private _initializer: SceneInitializer | null = null;

  constructor(scene: Scene) { this._scene = scene; }

  /**
   * @param hud  Optional FightHUD to wire into the fight stage machine.
   *             Pass the already-bound HUD so HP bars, timer, and banners
   *             activate in sync with the Unity fight-stage sequence.
   */
  async initialize(locationName: string, fightInfo: IFightInfo, hud?: FightHUD): Promise<void> {
    this._initializer = new SceneInitializer(this._scene);
    await this._initializer.initializeNewLocationScene(locationName, fightInfo, hud);
    console.log("[FightScene] initialized ✔");
  }

  dispose(): void {
    this._initializer?.dispose();
    this._initializer = null;
  }
}
