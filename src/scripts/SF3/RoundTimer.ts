/**
 * RoundTimer.ts
 * Mirror of SF3/RoundTimer.cs
 *
 * Pure display helper — owns no tick logic.
 * FightController drives it via IFightHUDCallbacks.onTimerUpdate(secondsLeft).
 * Call RoundTimer.updateLabel() from FightHUD.onTimerUpdate to render.
 */

export class RoundTimer {
  private _el: HTMLElement | null = null;

  bind(el: HTMLElement): void {
    this._el = el;
  }

  updateLabel(secondsLeft: number): void {
    if (!this._el) return;
    const display = Math.ceil(secondsLeft);
    this._el.textContent = String(display);
    this._el.style.color = display <= 10 ? "#ff5555" : "#fff";
  }

  reset(totalSeconds: number): void {
    this.updateLabel(totalSeconds);
  }
}
