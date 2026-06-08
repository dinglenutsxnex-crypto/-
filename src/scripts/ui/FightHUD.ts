import "../../ui/styles/screens/fight-hud.css";
import { AtlasManager } from "./AtlasManager";

// ─── Display sizes (1280×720 design space) ───────────────────────────────────
const HP_BAR_W     = 480;
const HP_BAR_H     = 18;
const SHADOW_BAR_W = 480;
const SHADOW_BAR_H = 12;
const ROUND_DOT_W  = 26;
const ROUND_DOT_H  = 11;
const PAUSE_BTN_W  = 80;
const PAUSE_BTN_H  = 32;

// Control sprite sizes
const STICK_RING_SIZE  = 160;
const STICK_KNOB_SIZE  = 100;
const ATK_BTN_SIZE     = 90;
const ABILITY_SLOT_SIZE = 60;

// Timing constants (ms) — from BattleInterface.cs
const TIME_ROUND_START  = 2500;
const TIME_FIGHT_START  = 1500;
const TIME_KO           = 750;
const TIME_GREAT        = 750;
const TIME_PERFECT      = 750;

const DELAY_FIRST_ROUND = 1000;
const DELAY_FIGHT_START = 0;

interface HUDState {
  playerHP:    number;
  enemyHP:     number;
  playerWins:  number;
  enemyWins:   number;
  roundsToWin: number;
  roundTime:   number;
  round:       number;
  shadow:      number;
}

export class FightHUD {
  private _atlas: AtlasManager | null;

  private _root:         HTMLElement | null = null;
  private _banner:       HTMLElement | null = null;
  private _bannerText:   HTMLElement | null = null;
  private _timer:        HTMLElement | null = null;
  private _playerHP:     HTMLElement | null = null;
  private _playerHPFade: HTMLElement | null = null;
  private _enemyHP:      HTMLElement | null = null;
  private _enemyHPFade:  HTMLElement | null = null;
  private _playerRounds: HTMLElement | null = null;
  private _enemyRounds:  HTMLElement | null = null;
  private _shadowFill:   HTMLElement | null = null;

  private _timerInterval: number | null = null;
  private _bannerTimer:   number | null = null;
  private _timeLeft    = 99;
  private _timerRunning = false;

  private _state: HUDState = {
    playerHP: 1, enemyHP: 1,
    playerWins: 0, enemyWins: 0,
    roundsToWin: 2, roundTime: 99, round: 1,
    shadow: 0,
  };

  /** atlas is optional; falls back to AtlasManager.instance if not provided */
  constructor(atlas?: AtlasManager | null) {
    this._atlas = atlas ?? AtlasManager.instance;
  }

  // ─── Bind ────────────────────────────────────────────────────────────────────

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
    this._shadowFill  = container.querySelector("#hud-shadow-fill");

    const a = this._atlas;

    // ── HP bars ──
    if (a) {
      if (this._playerHP)     a.applyBackground(this._playerHP,     "hp_bar",      HP_BAR_W, HP_BAR_H);
      if (this._playerHPFade) a.applyBackground(this._playerHPFade, "hp_bar_fade", HP_BAR_W, HP_BAR_H);
      if (this._enemyHP)      a.applyBackground(this._enemyHP,      "hp_bar",      HP_BAR_W, HP_BAR_H);
      if (this._enemyHPFade)  a.applyBackground(this._enemyHPFade,  "hp_bar_fade", HP_BAR_W, HP_BAR_H);

      // ── Shadow energy bar ──
      const shadowBg   = container.querySelector<HTMLElement>("#hud-shadow-bg");
      if (shadowBg)          a.applyBackground(shadowBg,           "shadow_bar_empty", SHADOW_BAR_W, SHADOW_BAR_H);
      if (this._shadowFill)  a.applyBackground(this._shadowFill,   "shadow_bar_full",  SHADOW_BAR_W, SHADOW_BAR_H);

      // ── Pause button ──
      const pauseSprite = container.querySelector<HTMLElement>("#hud-pause-sprite");
      if (pauseSprite) a.applyStretched(pauseSprite, "pause_button", PAUSE_BTN_W, PAUSE_BTN_H);

      // ── Joystick ──
      const stickRing = container.querySelector<HTMLElement>("#hud-stick-ring");
      const stickKnob = container.querySelector<HTMLElement>("#hud-stick-knob");
      if (stickRing) a.applyScaled(stickRing, "stick_circle", STICK_RING_SIZE, STICK_RING_SIZE);
      if (stickKnob) a.applyScaled(stickKnob, "stick_btn",    STICK_KNOB_SIZE, STICK_KNOB_SIZE);

      // ── Attack buttons ──
      const btnShuriken = container.querySelector<HTMLElement>("#hud-btn-shuriken");
      const btnFist     = container.querySelector<HTMLElement>("#hud-btn-fist");
      const btnFoot     = container.querySelector<HTMLElement>("#hud-btn-foot");
      if (btnShuriken) a.applyScaled(btnShuriken, "shuriken_button", ATK_BTN_SIZE, ATK_BTN_SIZE);
      if (btnFist)     a.applyScaled(btnFist,     "fist",            ATK_BTN_SIZE, ATK_BTN_SIZE);
      if (btnFoot)     a.applyScaled(btnFoot,     "foot",            ATK_BTN_SIZE, ATK_BTN_SIZE);

      // ── Shadow ability slots ──
      for (let i = 0; i < 3; i++) {
        const slot = container.querySelector<HTMLElement>(`#hud-ability-${i}`);
        if (slot) a.applyScaled(slot, "ability_slot", ABILITY_SLOT_SIZE, ABILITY_SLOT_SIZE);
      }
    }

    const pauseBtn = container.querySelector("#hud-pause-btn");
    pauseBtn?.addEventListener("click", () => this._onPause());
  }

  show(): void { this._root?.classList.add("active"); }
  hide(): void { this._root?.classList.remove("active"); }

  // ─── Round setup ─────────────────────────────────────────────────────────────

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

  // ─── Sequence (mirrors BattleInterface show* methods) ────────────────────────

  showRoundStart(cb?: () => void): void {
    const delay = this._state.round === 1 ? DELAY_FIRST_ROUND : 0;
    setTimeout(() => {
      this._showBanner(`ROUND ${this._state.round}`, "", TIME_ROUND_START, cb);
    }, delay);
  }

  showFightStart(cb?: () => void): void {
    setTimeout(() => {
      this._showBanner("FIGHT!", "hud-banner--fight", TIME_FIGHT_START, cb);
    }, DELAY_FIGHT_START);
  }

  showKO(cb?: () => void):      void { this._showBanner("KO",      "hud-banner--ko",      TIME_KO,      cb); }
  showGreat(cb?: () => void):   void { this._showBanner("GREAT!",  "hud-banner--great",   TIME_GREAT,   cb); }
  showPerfect(cb?: () => void): void { this._showBanner("PERFECT!","hud-banner--perfect", TIME_PERFECT, cb); }

  // ─── HP ──────────────────────────────────────────────────────────────────────

  setPlayerHP(ratio: number): void { this._setHP("player", ratio, true); }
  setEnemyHP (ratio: number): void { this._setHP("enemy",  ratio, true); }

  // ─── Shadow energy ────────────────────────────────────────────────────────────

  setShadow(ratio: number): void {
    if (!this._shadowFill) return;
    const pct = Math.max(0, Math.min(1, ratio));
    this._state.shadow = pct;
    this._shadowFill.style.transform = `scaleX(${pct})`;
  }

  // ─── Round wins ──────────────────────────────────────────────────────────────

  addPlayerWin(): void { this._state.playerWins++; this._rebuildRoundDots(); }
  addEnemyWin():  void { this._state.enemyWins++;  this._rebuildRoundDots(); }

  // ─── Timer ───────────────────────────────────────────────────────────────────

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
    const el = this._root?.querySelector<HTMLElement>("#hud-name-player");
    if (el) el.textContent = name.toUpperCase();
  }

  setEnemyName(name: string): void {
    const el = this._root?.querySelector<HTMLElement>("#hud-name-enemy");
    if (el) el.textContent = name.toUpperCase();
  }

  // ─── Private ─────────────────────────────────────────────────────────────────

  private _setHP(side: "player" | "enemy", ratio: number, animate: boolean): void {
    const fill = side === "player" ? this._playerHP : this._enemyHP;
    const fade = side === "player" ? this._playerHPFade : this._enemyHPFade;
    if (!fill || !fade) return;

    const pct = Math.max(0, Math.min(1, ratio));
    fill.style.transform = `scaleX(${pct})`;

    if (animate) {
      const prevStr  = fade.style.transform?.replace("scaleX(", "").replace(")", "") ?? "1";
      const prevFade = parseFloat(prevStr) || 1;
      if (pct < prevFade) {
        setTimeout(() => {
          fade.style.transform = `scaleX(${pct})`;
        }, 350);
      }
    } else {
      fade.style.transform = `scaleX(${pct})`;
    }
  }

  private _setTimer(secs: number): void {
    if (!this._timer) return;
    this._timer.textContent = String(Math.ceil(secs));
    this._timer.style.color = secs <= 10 ? "#ff5555" : "#fff";
  }

  private _rebuildRoundDots(): void {
    const n = this._state.roundsToWin;
    const a = this._atlas;
    [
      { el: this._playerRounds, wins: this._state.playerWins, rtl: false },
      { el: this._enemyRounds,  wins: this._state.enemyWins,  rtl: true  },
    ].forEach(({ el, wins, rtl }) => {
      if (!el) return;
      el.innerHTML = "";
      const dots: HTMLElement[] = [];
      for (let i = 0; i < n; i++) {
        const won = i < wins;
        const dot = document.createElement("div");
        dot.className = "hud-round-dot" + (won ? " won" : "");
        if (a) {
          a.applyScaled(dot, won ? "round_full" : "round_empty", ROUND_DOT_W, ROUND_DOT_H);
        }
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
    cb?: () => void,
  ): void {
    if (!this._banner || !this._bannerText) { cb?.(); return; }
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);

    this._banner.className = "hud-banner " + extraClass;
    this._bannerText.textContent = text;

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
  }

  dispose(): void {
    this.stopTimer();
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);
    this._root = null;
  }
}
