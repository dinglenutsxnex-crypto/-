import "../../ui/styles/screens/fight-hud.css";
import { AtlasManager } from "./AtlasManager";

// ── Sprite display sizes (design-space pixels, 1280×720) ────────────────────
const HP_BAR_W       = 480;  const HP_BAR_H       = 20;
const SHADOW_BAR_W   = 480;  const SHADOW_BAR_H   = 12;
const ROUND_DOT_W    = 22;   const ROUND_DOT_H    = 10;

// Joystick
const RING_SIZE  = 152;   // stick_circle  165×166 → 152px
const KNOB_SIZE  = 90;    // stick_btn     165×165 → 90px

// Dir buttons row
const DIR_BTN_SIZE = 78;  // arrow.png 125×125 → 78px

// Attack buttons triangle
const ATK_BTN_SHURIKEN = 88;   // shuriken_button 217×216 → 88px
const ATK_BTN_FIST     = 88;   // fist  192×192 → 88px
const ATK_BTN_FOOT     = 88;   // foot  192×192 → 88px

export type FightAction = "punch" | "kick" | "ranged" | "dir_back" | "dir_down" | "dir_up" | "dir_forward";
export type ActionCallback = (action: FightAction) => void;

export class FightHUD {
  private _atlas: AtlasManager | null;

  private _root:          HTMLElement | null = null;
  private _banner:        HTMLElement | null = null;
  private _bannerText:    HTMLElement | null = null;
  private _timer:         HTMLElement | null = null;
  private _playerHP:      HTMLElement | null = null;
  private _playerHPFade:  HTMLElement | null = null;
  private _enemyHP:       HTMLElement | null = null;
  private _enemyHPFade:   HTMLElement | null = null;
  private _playerRounds:  HTMLElement | null = null;
  private _enemyRounds:   HTMLElement | null = null;
  private _shadowFill:    HTMLElement | null = null;
  private _knob:          HTMLElement | null = null;

  private _timerInterval: number | null = null;
  private _bannerTimer:   number | null = null;
  private _timeLeft     = 99;
  private _timerRunning = false;

  private _playerWins  = 0;
  private _enemyWins   = 0;
  private _roundsToWin = 2;

  // Joystick state
  private _stickActive   = false;
  private _stickOriginX  = 0;
  private _stickOriginY  = 0;
  private readonly _stickMaxRadius = 48;

  // Input callbacks
  private _actionCb: ActionCallback | null = null;
  private _stickCb: ((dx: number, dy: number) => void) | null = null;

  constructor(atlas?: AtlasManager | null) {
    this._atlas = atlas ?? AtlasManager.instance;
  }

  // ── Callbacks ──────────────────────────────────────────────────────────────

  onAction(cb: ActionCallback): void       { this._actionCb = cb; }
  onStick(cb: (dx: number, dy: number) => void): void { this._stickCb = cb; }

  // ── Bind ───────────────────────────────────────────────────────────────────

  bind(container: HTMLElement): void {
    const q = <T extends HTMLElement>(sel: string) =>
      container.querySelector<T>(sel);

    this._root         = q("#fight-hud");
    this._banner       = q("#hud-banner");
    this._bannerText   = q("#hud-banner-text");
    this._timer        = q("#hud-timer");
    this._playerHP     = q("#hud-hp-player");
    this._playerHPFade = q("#hud-hp-fade-player");
    this._enemyHP      = q("#hud-hp-enemy");
    this._enemyHPFade  = q("#hud-hp-fade-enemy");
    this._playerRounds = q("#hud-rounds-player");
    this._enemyRounds  = q("#hud-rounds-enemy");
    this._shadowFill   = q("#hud-shadow-fill");
    this._knob         = q("#hud-stick-knob");

    const a = this._atlas;
    if (a) {
      // ── HP bars ──
      const hp = (id: string) => q<HTMLElement>(id);
      if (this._playerHP)     a.applyBackground(this._playerHP,     "hp_bar",       HP_BAR_W, HP_BAR_H);
      if (this._playerHPFade) a.applyBackground(this._playerHPFade, "hp_bar_fade",  HP_BAR_W, HP_BAR_H);
      if (this._enemyHP)      a.applyBackground(this._enemyHP,      "hp_bar",       HP_BAR_W, HP_BAR_H);
      if (this._enemyHPFade)  a.applyBackground(this._enemyHPFade,  "hp_bar_fade",  HP_BAR_W, HP_BAR_H);

      // ── Shadow bar ──
      const shadowBg = q<HTMLElement>("#hud-shadow-bg");
      if (shadowBg)          a.applyBackground(shadowBg,          "shadow_bar_empty", SHADOW_BAR_W, SHADOW_BAR_H);
      if (this._shadowFill)  a.applyBackground(this._shadowFill,  "shadow_bar_full",  SHADOW_BAR_W, SHADOW_BAR_H);

      // ── Joystick ──
      const ring  = q<HTMLElement>("#hud-stick-ring");
      if (ring)  a.applyScaled(ring,  "stick_circle", RING_SIZE, RING_SIZE);
      if (this._knob) a.applyScaled(this._knob, "stick_btn", KNOB_SIZE, KNOB_SIZE);

      // ── 4 directional attack buttons ──
      const DIRS = ["back", "down", "up", "forward"] as const;
      for (let i = 0; i < 4; i++) {
        const btn = q<HTMLElement>(`#hud-dir-${i}`);
        if (btn) a.applyScaled(btn, "arrow", DIR_BTN_SIZE, DIR_BTN_SIZE);
      }

      // ── Attack triangle ──
      const shuriken = q<HTMLElement>("#hud-btn-shuriken");
      const fist     = q<HTMLElement>("#hud-btn-fist");
      const foot     = q<HTMLElement>("#hud-btn-foot");
      if (shuriken) a.applyScaled(shuriken, "shuriken_button", ATK_BTN_SHURIKEN, ATK_BTN_SHURIKEN);
      if (fist)     a.applyScaled(fist,     "fist",            ATK_BTN_FIST,     ATK_BTN_FIST);
      if (foot)     a.applyScaled(foot,     "foot",            ATK_BTN_FOOT,     ATK_BTN_FOOT);
    }

    this._wireButtons(container);
    this._wireJoystick(container);
  }

  // ── Show / hide ────────────────────────────────────────────────────────────

  show(): void  { this._root?.classList.add("active"); }
  hide(): void  { this._root?.classList.remove("active"); }

  // ── Round setup ────────────────────────────────────────────────────────────

  initRound(round: number, roundsToWin: number, roundTime: number): void {
    this._roundsToWin = roundsToWin;
    this._timeLeft    = roundTime;
    this._setHP("player", 1, false);
    this._setHP("enemy",  1, false);
    this._rebuildRoundDots();
    this._setTimer(roundTime);
    this.stopTimer();
  }

  // ── Banner sequence ────────────────────────────────────────────────────────

  showRoundStart(round: number, cb?: () => void): void {
    this._showBanner(`ROUND ${round}`, 2500, cb);
  }
  showFightStart(cb?: () => void): void { this._showBanner("FIGHT!", 1500, cb); }
  showKO(cb?: () => void):         void { this._showBanner("KO",     750,  cb); }
  showGreat(cb?: () => void):      void { this._showBanner("GREAT!", 750,  cb); }
  showPerfect(cb?: () => void):    void { this._showBanner("PERFECT!", 750, cb); }

  // ── HP ─────────────────────────────────────────────────────────────────────

  setPlayerHP(ratio: number): void { this._setHP("player", ratio, true); }
  setEnemyHP (ratio: number): void { this._setHP("enemy",  ratio, true); }

  // ── Shadow energy ──────────────────────────────────────────────────────────

  setShadow(ratio: number): void {
    if (!this._shadowFill) return;
    this._shadowFill.style.transform = `scaleX(${Math.max(0, Math.min(1, ratio))})`;
  }

  // ── Round wins ─────────────────────────────────────────────────────────────

  addPlayerWin(): void { this._playerWins++; this._rebuildRoundDots(); }
  addEnemyWin():  void { this._enemyWins++;  this._rebuildRoundDots(); }

  // ── Timer ──────────────────────────────────────────────────────────────────

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

  // ── Names ──────────────────────────────────────────────────────────────────

  setPlayerName(name: string): void {
    const el = this._root?.querySelector<HTMLElement>("#hud-name-player");
    if (el) el.textContent = name.toUpperCase();
  }
  setEnemyName(name: string): void {
    const el = this._root?.querySelector<HTMLElement>("#hud-name-enemy");
    if (el) el.textContent = name.toUpperCase();
  }

  // ── Private: HP ────────────────────────────────────────────────────────────

  private _setHP(side: "player" | "enemy", ratio: number, animate: boolean): void {
    const fill = side === "player" ? this._playerHP    : this._enemyHP;
    const fade = side === "player" ? this._playerHPFade : this._enemyHPFade;
    const pct  = Math.max(0, Math.min(1, ratio));
    if (fill) fill.style.transform = `scaleX(${pct})`;
    if (fade) {
      if (!animate) { fade.style.transform = `scaleX(${pct})`; return; }
      const prev = parseFloat(fade.style.transform?.replace(/scaleX\(/, "").replace(/\)/, "") ?? "1") || 1;
      if (pct < prev) {
        setTimeout(() => { if (fade) fade.style.transform = `scaleX(${pct})`; }, 350);
      }
    }
  }

  // ── Private: timer ─────────────────────────────────────────────────────────

  private _setTimer(secs: number): void {
    if (!this._timer) return;
    this._timer.textContent = String(Math.ceil(secs));
    this._timer.classList.toggle("low", secs <= 10);
  }

  // ── Private: round dots ────────────────────────────────────────────────────

  private _rebuildRoundDots(): void {
    const a = this._atlas;
    [
      { el: this._playerRounds, wins: this._playerWins, rtl: false },
      { el: this._enemyRounds,  wins: this._enemyWins,  rtl: true  },
    ].forEach(({ el, wins, rtl }) => {
      if (!el) return;
      el.innerHTML = "";
      const n = this._roundsToWin;
      for (let i = 0; i < n; i++) {
        const won = i < wins;
        const d = document.createElement("div");
        d.className = "hud-round-dot" + (won ? " won" : "");
        if (a) a.applyScaled(d, won ? "round_full" : "round_empty", ROUND_DOT_W, ROUND_DOT_H);
        el.appendChild(d);
      }
      if (rtl) {
        const kids = Array.from(el.children) as HTMLElement[];
        kids.reverse().forEach(k => el.appendChild(k));
      }
    });
  }

  // ── Private: banner ────────────────────────────────────────────────────────

  private _showBanner(text: string, duration: number, cb?: () => void): void {
    if (!this._banner || !this._bannerText) { cb?.(); return; }
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);

    this._bannerText.textContent = text;
    this._banner.classList.remove("hidden");

    this._bannerTimer = window.setTimeout(() => {
      this._banner!.classList.add("hidden");
      this._bannerTimer = window.setTimeout(() => { cb?.(); this._bannerTimer = null; }, 260);
    }, duration);
  }

  // ── Private: button wiring ─────────────────────────────────────────────────

  private _wireButtons(container: HTMLElement): void {
    const bind = (sel: string, action: FightAction) => {
      const el = container.querySelector<HTMLElement>(sel);
      if (!el) return;
      const fire = (e: Event) => {
        e.preventDefault();
        this._actionCb?.(action);
        console.log(`[FightHUD] action: ${action}`);
      };
      el.addEventListener("pointerdown", fire, { passive: false });
    };

    bind("#hud-btn-shuriken", "ranged");
    bind("#hud-btn-fist",     "punch");
    bind("#hud-btn-foot",     "kick");

    const dirActions: FightAction[] = ["dir_back", "dir_down", "dir_up", "dir_forward"];
    for (let i = 0; i < 4; i++) {
      bind(`#hud-dir-${i}`, dirActions[i]);
    }
  }

  // ── Private: joystick wiring ───────────────────────────────────────────────

  private _wireJoystick(container: HTMLElement): void {
    const stick = container.querySelector<HTMLElement>("#hud-joystick");
    const knob  = this._knob;
    if (!stick || !knob) return;

    const R = this._stickMaxRadius;

    const moveKnob = (dx: number, dy: number) => {
      const len = Math.sqrt(dx * dx + dy * dy);
      const clamped = len > R ? R / len : 1;
      const nx = dx * clamped;
      const ny = dy * clamped;
      // Offset from rest position center (KNOB_SIZE/2 already centered by CSS top/left)
      knob.style.transform = `translate(${nx}px, ${ny}px)`;
      this._stickCb?.(nx / R, ny / R);
    };

    const resetKnob = () => {
      knob.style.transform = "";
      this._stickActive = false;
      this._stickCb?.(0, 0);
    };

    stick.addEventListener("pointerdown", (e: PointerEvent) => {
      e.preventDefault();
      this._stickActive  = true;
      this._stickOriginX = e.clientX;
      this._stickOriginY = e.clientY;
      stick.setPointerCapture(e.pointerId);
    }, { passive: false });

    stick.addEventListener("pointermove", (e: PointerEvent) => {
      if (!this._stickActive) return;
      e.preventDefault();
      moveKnob(e.clientX - this._stickOriginX, e.clientY - this._stickOriginY);
    }, { passive: false });

    stick.addEventListener("pointerup",     resetKnob, { passive: true });
    stick.addEventListener("pointercancel", resetKnob, { passive: true });
  }

  // ── Dispose ────────────────────────────────────────────────────────────────

  dispose(): void {
    this.stopTimer();
    if (this._bannerTimer !== null) clearTimeout(this._bannerTimer);
    this._root = null;
  }
}
