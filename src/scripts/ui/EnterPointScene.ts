import { SceneManager, SceneConfig } from "../core/SceneManager";

export class EnterPointScene {
  private _mgr: SceneManager;
  private _config: SceneConfig;
  private _rootEl: HTMLDivElement | null = null;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr = mgr;
    this._config = config;
  }

  mount(): void {
    console.log("[EnterPointScene] mounting hierarchy:", 
      this._config.hierarchy?.map(h => h.name));

    const root = document.createElement("div");
    root.id = "enter-point-ui";
    Object.assign(root.style, {
      position: "fixed", inset: "0", zIndex: "100",
      display: "flex", flexDirection: "column",
      alignItems: "center", justifyContent: "center",
      pointerEvents: "none",
    });

    this._config.hierarchy?.forEach(node => {
      if (!node.isActive) return;
      const el = document.createElement("div");
      el.dataset.sceneName = node.name;
      el.style.cssText = "position:absolute;";
      root.appendChild(el);
      console.log(`[EnterPointScene] node mounted: ${node.name}`);
    });

    document.body.appendChild(root);
    this._rootEl = root;
  }

  unmount(): void {
    this._rootEl?.remove();
    this._rootEl = null;
  }
}
