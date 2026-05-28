/**
 * FightHUD.ts
 * Mirrors BattleInterface.cs + RoundsUI.cs + RoundTimer.cs
 *
 * Drives all in-fight HTML UI:
 *  - Player/enemy HP bars (with fade-drain effect)
 *  - Round-win dots
 *  - Countdown timer
 *  - Center banners: ROUND 1, FIGHT!, KO, PERFECT, GREAT
 */

import "../../ui/styles/screens/fight-hud.css";

// Timing constants from BattleInterface.cs
const TIME_ROUND_START  = 2500;  // ms
const TIME_FIGHT_START  = 1500;
const TIME_ROUND_END    = 750;
const TIME_GAME_END     = 1000;
const TIME_KO           = 750;
const TIME_GREAT        = 750;
const TIME_PERFECT      = 750;

const DELAY_FIRST_ROUND = 1000;
const DELAY_FIGHT_START = 0;

interface HUDState {
  playerHP:   number;   // 0–1
  enemyHP:    number;
  playerWins: number;
  enemyWins:  number;
  roundsToWin: number;
  roundTime:  number;   // seconds
  round:      number;
}

export class FightHUD {
  private _root:   HTMLElement | null = null;
  private _banner: HTMLElement | null = null;
  private _bannerText: HTMLElement | null = null;
  private _timer:  HTMLElement | null = null;

  private _playerHP: HTMLElement | null = null;
  private _playerHPFade: HTMLElement | null = null;
  private _enemyHP:  HTMLElement | null = null;
  private _enemyHPFade: HTMLElement | null = null;
  private _playerRounds: HTMLElement | null = null;
  private _enemyRounds:  HTMLElement | null = null;

  private _timerInterval: number | null = null;
  private _bannerTimer: number | null = null;
  private _timeLeft = 99;
  private _timerRunning = false;

  private _state: HUDState = {
    playerHP: 1, enemyHP: 1,
    playerWins: 0, enemyWins: 0,
    roundsToWin: 2,
    roundTime: 99,
    round: 1,
  };

  // Called once after inject into DOM
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

    const pauseBtn = container.querySelector("#hud-pause-btn");
    pauseBtn?.addEventListener("click", () => this._onPause());
  }

  show(): void {
    this._root?.classList.add("active");
  }

  hide(): void {
    this._root?.classList.remove("active");
  }

  // ─── Round setup ────────────────────────────────────────────────────────

  initRound(round: number, roundsToWin: number, roundTime: number): void {
    this._state.round       = round;
    this._state.roundsToWin = roundsToWin;
    this._state.roundTime   = roundTime;
    this._state.playerHP    = 1;
    this._state.enemyHP     = 1;
    this._timeLeft          = roundTime;

    this._setHP("player", 1, false);
    this._setHP("enemy",  1, false);
    this._rebuildRoundDots();
    this._setTimer(roundTime);
    this.stopTimer();
  }

  // ─── Sequence helpers (mirrors BattleInterface show* methods) ───────────

  /** Show "ROUND N" then callback */
  showRoundStart(cb?: () => void): void {
    const delay = this._state.round === 1 ? DELAY_FIRST_ROUND : 0;
    setTimeout(() => {
      this._showBanner(`ROUND ${this._state.round}`, "", TIME_ROUND_START, cb);
    }, delay);
  }

  /** Show "FIGHT!" then callback */
  showFightStart(cb?: () => void): void {
    setTimeout(() => {
      this._showBanner("FIGHT!", "hud-banner--fight", TIME_FIGHT_START, cb);
    }, DELAY_FIGHT_START);
  }

  showKO(cb?: () => void):      void { this._showBanner("KO",      "hud-banner--ko",     TIME_KO,      cb); }
  showGreat(cb?: () => void):   void { this._showBanner("GREAT",   "hud-banner--great",  TIME_GREAT,   cb); }
  showPerfect(cb?: () => void): void { this._showBanner("PERFECT", "hud-banner--perfect",TIME_PERFECT, cb); }

  // ─── HP ─────────────────────────────────────────────────────────────────

  setPlayerHP(ratio: number): void { this._setHP("player", ratio, true); }
  setEnemyHP (ratio: number): void { this._setHP("enemy",  ratio, true); }

  // ─── Round wins ─────────────────────────────────────────────────────────

  addPlayerWin(): void { this._state.playerWins++; this._rebuildRoundDots(); }
  addEnemyWin():  void { this._state.enemyWins++;  this._rebuildRoundDots(); }

  // ─── Timer ──────────────────────────────────────────────────────────────

  startTimer(): void {
    if (this._timerRunning) return;
    this._timerRunning = true;
    this._timerInterval = window.setInterval(() => {
      this._timeLeft = Math.max(0, this._timeLeft - 1);
      this._setTimer(this._timeLeft);
      if (this._timeLeft <= 0) this.stopTimer();
    }, 1000);
  }

  stopTimer(): void {
    this._timerRunning = false;
    if (this._timerInterval !== null) {
      clearInterval(this._timerInterval);
      this._timerInterval = null;
    }
  }

  setPlayerName(name: string): void {
    const el = this._root?.querySelector("#hud-name-player") as HTMLElement | null;
    if (el) el.textContent = name.toUpperCase();
  }

  setEnemyName(name: string): void {
    const el = this._root?.querySelector("#hud-name-enemy") as HTMLElement | null;
    if (el) el.textContent = name.toUpperCase();
  }

  // ─── Private ────────────────────────────────────────────────────────────

  private _setHP(side: "player" | "enemy", ratio: number, animate: boolean): void {
    const fill = side === "player" ? this._playerHP : this._enemyHP;
    const fade = side === "player" ? this._playerHPFade : this._enemyHPFade;
    if (!fill || !fade) return;

    const pct = Math.max(0, Math.min(1, ratio)) * 100;
    fill.style.transform = `scaleX(${pct / 100})`;

    // Fade bar stays until HP catches up
    if (animate) {
      const prevFadePct = parseFloat(fade.style.transform?.replace("scaleX(", "") || "1") * 100;
      if (pct < prevFadePct) {
        // HP dropped — fade stays, HP snaps, fade drains after delay
        setTimeout(() => {
          fade.style.transform = `scaleX(${pct / 100})`;
        }, 350);
      }
    } else {
      fade.style.transform = `scaleX(${pct / 100})`;
    }
  }

  private _setTimer(secs: number): void {
    if (!this._timer) return;
    this._timer.textContent = String(Math.ceil(secs));
    if (secs <= 10) this._timer.style.color = "#ff5555";
    else            this._timer.style.color = "#fff";
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
    text: string,
    extraClass: string,
    duration: number,
    cb?: () => void
  ): void {
    if (!this._banner || !this._bannerText) { cb?.(); return; }
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);

    // Reset classes
    this._banner.className = "hud-banner " + extraClass;
    this._bannerText.textContent = text;

    // Show
    requestAnimationFrame(() => {
      this._banner!.classList.remove("hidden");
    });

    this._bannerTimer = window.setTimeout(() => {
      this._banner!.classList.add("hidden");
      this._bannerTimer = window.setTimeout(() => {
        cb?.();
        this._bannerTimer = null;
      }, 300);
    }, duration);
  }

  private _onPause(): void {
    console.log("[FightHUD] Pause pressed");
    // TODO: wire to PauseController
  }

  dispose(): void {
    this.stopTimer();
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);
    this._root = null;
  }
}
