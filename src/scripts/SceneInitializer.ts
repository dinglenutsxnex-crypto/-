import { Scene } from "@babylonjs/core";

export class SceneInitializer {

  constructor(
    private readonly _scene: Scene
  ) {}

  async initializeNewLocationScene():
    Promise<void> {}
}
