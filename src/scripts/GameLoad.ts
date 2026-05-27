import { SceneManager } from "./core/SceneManager";
import { LoadScreen } from "./LoadScreen";
import { EnterPoint } from "./EnterPoint";

export class GameLoad {
  private _mgr: SceneManager;
  private _logoEl: HTMLElement | null = null;

  constructor(mgr: SceneManager) {
    this._mgr = mgr;
  }

  async start(): Promise<void> {
    this._showLogo();
    const ep = new EnterPoint(this._mgr);
    await ep.init();
    this._hideLogo();
  }

  private _showLogo(): void {
    const el = document.getElementById("nekki-logo");
    if (!el) return;
    el.style.removeProperty("display");
    el.classList.add("active");
    requestAnimationFrame(() => { el.style.opacity = "1"; });
    this._logoEl = el;
  }

  private _hideLogo(): void {
    if (!this._logoEl) return;
    const el = this._logoEl;
    el.style.opacity = "0";
    setTimeout(() => {
      el.classList.remove("active");
      el.style.opacity = "0";
    }, 320);
    this._logoEl = null;
  }
}
