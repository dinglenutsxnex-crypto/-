/**
 * ModelHPStatus.ts
 * Mirror of SF3/GameModels/ModelHPStatus.cs
 *
 * Holds the live HP state for one fighter.
 * FightController.update() reads isDead each frame to detect round-end.
 * FightHUD reads currentHPRatio to update bars.
 */

export class ModelHPStatus {
  private _maxHP:     number;
  private _currentHP: number;

  constructor(maxHP: number = 1) {
    this._maxHP     = maxHP;
    this._currentHP = maxHP;
  }

  get maxHP():         number  { return this._maxHP; }
  get currentHP():     number  { return this._currentHP; }
  get currentHPRatio():number  { return this._maxHP > 0 ? this._currentHP / this._maxHP : 0; }
  get isDead():        boolean { return this._currentHP <= 0; }

  /** Apply raw damage (positive = lose HP). Clamps to [0, maxHP]. */
  applyDamage(amount: number): void {
    this._currentHP = Math.max(0, Math.min(this._maxHP, this._currentHP - amount));
  }

  /** Fully restore HP — called at the start of each round. */
  reset(): void {
    this._currentHP = this._maxHP;
  }

  /** Hard-set current HP (used by debug / cheat commands). */
  setHP(value: number): void {
    this._currentHP = Math.max(0, Math.min(this._maxHP, value));
  }

  /** Hard-set from a 0–1 ratio. */
  setHPRatio(ratio: number): void {
    this.setHP(ratio * this._maxHP);
  }
}
