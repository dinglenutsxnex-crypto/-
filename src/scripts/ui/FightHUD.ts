/**
 * FightHUD.ts
 * Mirror of BattleInterface.cs + RoundsUI.cs + RoundTimer.cs
 *
 * Drives all in-fight HTML UI:
 *  - Player/enemy HP bars (with fade-drain effect)
 *  - Round-win dots
 *  - Countdown timer (display only — tick driven by FightController)
 *  - Centre banners: ROUND N, FIGHT!, KO, PERFECT, GREAT, WIN, LOSE
 *
 * Changes from the old version:
 *  - startTimer() / stopTimer() removed — timer display is now driven by
 *    FightController via onTimerUpdate(secondsLeft).
 *  - buildHUDCallbacks() returns an IFightHUDCallbacks object ready to
 *    hand directly to BattleController.setHUDCallbacks().
 *  - onRoundEnd handles KO / PERFECT / GREAT banner sequencing.
 *  - onFightEnd shows WIN / LOSE banner.
 */

import "../../ui/styles/screens/fight-hud.css";
import type { IFightHUDCallbacks } from "../SF3/FightController";

// Timing constants (mirrors BattleInterface.cs)
const TIME_ROUND_START = 2500;
const TIME_FIGHT_START = 1500;
const TIME_ROUND_END   = 750;
const TIME_KO          = 750;
const TIME_GREAT       = 750;
const TIME_PERFECT     = 750;
const TIME_FIGHT_END   = 2000;

const DELAY_FIRST_ROUND = 1000;

interface HUDState {
  playerHP:    number; // 0–1
  enemyHP:     number;
  playerWins:  number;
  enemyWins:   number;
  roundsToWin: number;
  round:       number;
}

export class FightHUD {
  private _root:       HTMLElement | null = null;
  private _banner:     HTMLElement | null = null;
  private _bannerText: HTMLElement | null = null;
  private _timer:      HTMLElement | null = null;

  private _playerHP:      HTMLElement | null = null;
  private _playerHPFade:  HTMLElement | null = null;
  private _enemyHP:       HTMLElement | null = null;
  private _enemyHPFade:   HTMLElement | null = null;
  private _playerRounds:  HTMLElement | null = null;
  private _enemyRounds:   HTMLElement | null = null;

  private _bannerTimer: ReturnType<typeof setTimeout> | null = null;

  private _state: HUDState = {
    playerHP: 1, enemyHP: 1,
    playerWins: 0, enemyWins: 0,
    roundsToWin: 2,
    round: 1,
  };

  // ─── DOM wiring ──────────────────────────────────────────────────────────

  bind(container: HTMLElement): void {
    this._root        = container.querySelector("#fight-hud");
    this._banner      = container.querySelector("#hud-banner");
    this._bannerText  = container.querySelector("#hud-banner-text");
    this._timer       = container.querySelector("#hud-timer");
    this._playerHP    = container.querySelector("#hud-hp-player");
    this._playerHPFade= container.querySelector("#hud-hp-fade-player");
    this._enemyHP     = container.querySelector("#hud-hp-enemy");
    this._enemyHPFade = container.querySelector("#hud-hp-fade-enemy");
    this._playerRounds= container.querySelector("#hud-rounds-player");
    this._enemyRounds = container.querySelector("#hud-rounds-enemy");

    container.querySelector("#hud-pause-btn")
      ?.addEventListener("click", () => this._onPause());
  }

  show(): void { this._root?.classList.add("active"); }
  hide(): void { this._root?.classList.remove("active"); }

  // ─── IFightHUDCallbacks factory ──────────────────────────────────────────

  /**
   * Returns a callbacks object ready to pass to BattleController.setHUDCallbacks().
   * This keeps FightHUD decoupled — FightController drives it, not the other way.
   */
  buildHUDCallbacks(): IFightHUDCallbacks {
    return {
      onRoundStart: (roundNumber, roundsToWin, roundTime) => {
        this._state.round       = roundNumber;
        this._state.roundsToWin = roundsToWin;
        this._state.playerHP    = 1;
        this._state.enemyHP     = 1;
        this._setHP("player", 1, false);
        this._setHP("enemy",  1, false);
        this._rebuildRoundDots();
        this._setTimer(roundTime);
        this.showRoundStart();
      },

      onFightStart: (cb) => {
        this.showFightStart(cb);
      },

      onRoundEnd: (playerWon, isPerfect, isGreat, cb) => {
        // Sequence: KO / PERFECT / GREAT → then call cb
        const afterBanner = () => {
          if (playerWon) {
            this._state.playerWins++;
            this._rebuildRoundDots();
          } else {
            this._state.enemyWins++;
            this._rebuildRoundDots();
          }
          cb();
        };

        if (isPerfect) {
          this.showPerfect(() => this.showKO(afterBanner));
        } else if (isGreat) {
          this.showGreat(() => this.showKO(afterBanner));
        } else {
          this.showKO(afterBanner);
        }
      },

      onFightEnd: (playerWon) => {
        this._showBanner(
          playerWon ? "YOU WIN" : "YOU LOSE",
          playerWon ? "hud-banner--win" : "hud-banner--lose",
          TIME_FIGHT_END,
        );
      },

      onHPUpdate: (playerRatio, enemyRatio) => {
        if (Math.abs(playerRatio - this._state.playerHP) > 0.001) {
          this._state.playerHP = playerRatio;
          this._setHP("player", playerRatio, true);
        }
        if (Math.abs(enemyRatio - this._state.enemyHP) > 0.001) {
          this._state.enemyHP = enemyRatio;
          this._setHP("enemy", enemyRatio, true);
        }
      },

      onTimerUpdate: (secondsLeft) => {
        this._setTimer(secondsLeft);
      },
    };
  }

  // ─── Banner helpers (mirrors BattleInterface show* methods) ─────────────

  setPlayerName(name: string): void {
    const el = this._root?.querySelector("#hud-name-player") as HTMLElement | null;
    if (el) el.textContent = name.toUpperCase();
  }

  setEnemyName(name: string): void {
    const el = this._root?.querySelector("#hud-name-enemy") as HTMLElement | null;
    if (el) el.textContent = name.toUpperCase();
  }

  showRoundStart(cb?: () => void): void {
    const delay = this._state.round === 1 ? DELAY_FIRST_ROUND : 0;
    setTimeout(() => {
      this._showBanner(`ROUND ${this._state.round}`, "", TIME_ROUND_START, cb);
    }, delay);
  }

  showFightStart(cb?: () => void): void {
    this._showBanner("FIGHT!", "hud-banner--fight", TIME_FIGHT_START, cb);
  }

  showKO(cb?: () => void):      void { this._showBanner("KO",      "hud-banner--ko",     TIME_KO,      cb); }
  showGreat(cb?: () => void):   void { this._showBanner("GREAT",   "hud-banner--great",  TIME_GREAT,   cb); }
  showPerfect(cb?: () => void): void { this._showBanner("PERFECT", "hud-banner--perfect",TIME_PERFECT, cb); }

  // ─── Private ─────────────────────────────────────────────────────────────

  private _setHP(side: "player" | "enemy", ratio: number, animate: boolean): void {
    const fill = side === "player" ? this._playerHP    : this._enemyHP;
    const fade = side === "player" ? this._playerHPFade: this._enemyHPFade;
    if (!fill || !fade) return;

    const pct = Math.max(0, Math.min(1, ratio));
    fill.style.transform = `scaleX(${pct})`;

    if (animate) {
      const prevPct = parseFloat(
        (fade.style.transform ?? "scaleX(1)").replace("scaleX(", "")
      );
      if (pct < prevPct) {
        setTimeout(() => { fade.style.transform = `scaleX(${pct})`; }, 350);
      }
    } else {
      fade.style.transform = `scaleX(${pct})`;
    }
  }

  private _setTimer(secs: number): void {
    if (!this._timer) return;
    this._timer.textContent  = String(Math.ceil(secs));
    this._timer.style.color  = secs <= 10 ? "#ff5555" : "#fff";
  }

  private _rebuildRoundDots(): void {
    const n = this._state.roundsToWin;
    [
      { el: this._playerRounds, wins: this._state.playerWins, rtl: false },
      { el: this._enemyRounds,  wins: this._state.enemyWins,  rtl: true  },
    ].forEach(({ el, wins, rtl }) => {
      if (!el) return;
      el.innerHTML = "";
      const dots: HTMLElement[] = [];
      for (let i = 0; i < n; i++) {
        const dot = document.createElement("div");
        dot.className = "hud-round-dot" + (i < wins ? " won" : "");
        dots.push(dot);
      }
      if (rtl) dots.reverse();
      dots.forEach(d => el.appendChild(d));
    });
  }

  private _showBanner(
    text:       string,
    extraClass: string,
    duration:   number,
    cb?:        () => void,
  ): void {
    if (!this._banner || !this._bannerText) { cb?.(); return; }
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);

    this._banner.className   = "hud-banner " + extraClass;
    this._bannerText.textContent = text;

    requestAnimationFrame(() => this._banner!.classList.remove("hidden"));

    this._bannerTimer = setTimeout(() => {
      this._banner!.classList.add("hidden");
      this._bannerTimer = setTimeout(() => {
        cb?.();
        this._bannerTimer = null;
      }, 300);
    }, duration);
  }

  private _onPause(): void {
    console.log("[FightHUD] Pause pressed");
    // TODO: wire to PauseController / BattleController.pauseGame()
  }

  dispose(): void {
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);
    this._root = null;
  }
}
