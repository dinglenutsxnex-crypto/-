import { Scene } from "@babylonjs/core";
import { IFightInfo } from "./SF3/FightController";

export class SceneInitializer {

  constructor(
    private readonly _scene: Scene
  ) {}

  async initializeNewLocationScene(
    locationName: string,
    fightInfo: IFightInfo,
    onReady?: () => void
  ): Promise<void> {

    onReady?.();
  }
}
