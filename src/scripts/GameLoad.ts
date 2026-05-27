import { SceneManager } from "./core/SceneManager";
import { LoadScreen } from "./LoadScreen";
import { EnterPoint } from "./EnterPoint";

export class GameLoad {
  private _mgr: SceneManager;
  private _logoEl: HTMLImageElement | null = null;

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
    const img = document.createElement("img");
    img.src = "assets/textures/ui/logSF3.png";
    img.id = "nekki-logo";
    Object.assign(img.style, {
      position: "fixed", top: "50%", left: "50%",
      transform: "translate(-50%, -50%)",
      width: "180px", zIndex: "10000", pointerEvents: "none",
      opacity: "0", transition: "opacity 0.3s ease",
    });
    document.body.appendChild(img);
    this._logoEl = img;
    requestAnimationFrame(() => { img.style.opacity = "1"; });
  }

  private _hideLogo(): void {
    if (!this._logoEl) return;
    const el = this._logoEl;
    el.style.opacity = "0";
    setTimeout(() => el.remove(), 320);
    this._logoEl = null;
  }
}
