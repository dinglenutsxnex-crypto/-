import { Scene } from "@babylonjs/core";

export class RoundController {
  private readonly _scene: Scene;
  private _roundNumber  = 0;
  private _maxRounds    = 3;
  private _roundActive  = false;

  get roundNumber():  number  { return this._roundNumber; }
  get roundActive():  boolean { return this._roundActive; }

  constructor(scene: Scene, maxRounds = 3) {
    this._scene     = scene;
    this._maxRounds = maxRounds;
  }

  initialize(): void {
    this._roundNumber = 0;
    this._roundActive = false;
  }

  startRound(): void {
    this._roundNumber++;
    this._roundActive = true;
  }

  endRound(): void {
    this._roundActive = false;
  }

  allRoundsComplete(): boolean {
    return this._roundNumber >= this._maxRounds;
  }

  dispose(): void {
    this._roundActive = false;
  }
}
