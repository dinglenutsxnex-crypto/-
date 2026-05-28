import { Scene } from "@babylonjs/core";

export class FightScene {

  constructor(
    private readonly _scene: Scene,
    private readonly _location?: string,
    private readonly _fightInfo?: any,
  ) {}

  async initialize():
    Promise<void> {}

  dispose():
    void {}
}
