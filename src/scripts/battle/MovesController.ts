import { Scene } from "@babylonjs/core";
import { EFightStage } from "../FightController";

export class MovesController {
  private readonly _scene: Scene;
  private _active = false;

  constructor(scene: Scene) {
    this._scene = scene;
  }

  initialize(): void {
    this._active = false;
  }

  update(dt: number): void {
    if (!this._active) return;
  }

  enable(): void {
    this._active = true;
  }

  disable(): void {
    this._active = false;
  }

  dispose(): void {
    this._active = false;
  }
}
