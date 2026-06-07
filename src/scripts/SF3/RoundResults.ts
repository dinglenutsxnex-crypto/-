/**
 * RoundResults.ts
 * Mirror of SF3/RoundResults.cs
 *
 * Tracks per-round outcome and bonus flags (perfect / great).
 * Owned by RoundController; read by FightController to drive HUD banners.
 */

import { ERoundResult } from "./ERoundResult";

export class RoundResults {
  private static readonly GREAT_HEALTH_THRESHOLD = 0.1;

  result:         ERoundResult = ERoundResult.IN_PROGRESS;
  winnerIsPlayer: boolean      = false;

  /** Winner kept full HP — no damage taken all round. */
  get isPerfect(): boolean {
    return this.winnerHP >= 1.0 && this.result !== ERoundResult.TIME_OUT;
  }

  /** Winner had ≤10 % HP left when the round ended. */
  get isGreat(): boolean {
    return this.winnerHP <= RoundResults.GREAT_HEALTH_THRESHOLD;
  }

  /** 0–1 HP ratio of the round winner, snapshotted in RoundController.endRoundFight(). */
  winnerHP = 1.0;

  reset(): void {
    this.result         = ERoundResult.IN_PROGRESS;
    this.winnerIsPlayer = false;
    this.winnerHP       = 1.0;
  }
}
