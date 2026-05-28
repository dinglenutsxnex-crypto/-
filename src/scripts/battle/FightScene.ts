import { Scene } from "@babylonjs/core";
import { ModelsManager } from "../SF3/ModelsManager";

export class FightScene {

  constructor(
    private readonly _scene: Scene
  ) {}

  async initialize(): Promise<void> {

    const manager =
      ModelsManager.instance;

    if (!manager) {
      return;
    }

    await manager.spawnPlayer();
    await manager.spawnEnemy();
  }
}
