export class LoadScreen {
  private static _overlay: HTMLDivElement | null = null;
  private static _tipEl: HTMLParagraphElement | null = null;
  private static _tips: string[] = [];
  private static _visible = false;

  static mount(): void {
    if (this._overlay) return;

    const overlay = document.createElement("div");
    overlay.id = "load-screen";
    Object.assign(overlay.style, {
      position: "fixed", inset: "0", zIndex: "9999",
      display: "flex", flexDirection: "column",
      alignItems: "center", justifyContent: "center",
      background: `url('assets/textures/ui/loadingGame.png') center/cover no-repeat #000`,
      opacity: "1", transition: "opacity 0.4s ease",
    });

    const logo = document.createElement("img");
    logo.src = "assets/textures/ui/logSF3.png";
    Object.assign(logo.style, { width: "200px", marginBottom: "40px" });

    const spinner = document.createElement("div");
    Object.assign(spinner.style, {
      width: "48px", height: "48px", border: "4px solid rgba(255,255,255,0.2)",
      borderTopColor: "#fff", borderRadius: "50%",
      animation: "ls-spin 0.8s linear infinite",
    });

    const tipEl = document.createElement("p");
    Object.assign(tipEl.style, {
      color: "#ccc", fontSize: "13px", marginTop: "24px",
      maxWidth: "340px", textAlign: "center", fontFamily: "sans-serif",
      lineHeight: "1.5",
    });

    const style = document.createElement("style");
    style.textContent = `@keyframes ls-spin { to { transform: rotate(360deg); } }`;
    document.head.appendChild(style);

    overlay.appendChild(logo);
    overlay.appendChild(spinner);
    overlay.appendChild(tipEl);
    document.body.appendChild(overlay);

    this._overlay = overlay;
    this._tipEl = tipEl;
    this._visible = true;

    this._loadTips();
    this._rotateTip();
  }

  private static _stripMarkup(raw: string): string {
    return raw.replace(/<[^>]*>/g, "").replace(/\s+/g, " ").trim();
  }

  private static async _loadTips(): Promise<void> {
    try {
      const [yamlRes, xmlRes] = await Promise.all([
        fetch("assets/configs/gamesettings/lore_hint_config.txt"),
        fetch("assets/configs/archive_extracted/localization/English.xml"),
      ]);
      const yaml = await yamlRes.text();
      const xml = await xmlRes.text();
      const parser = new DOMParser();
      const doc = parser.parseFromString(xml, "text/xml");

      const keys = (yaml.match(/^\s+-\s+(?!#)(\S+)/gm) || [])
        .map(l => l.replace(/^\s+-\s+/, "").trim());

      this._tips = keys.map(key => {
        const node = doc.querySelector(`Word[Title="${key}"]`);
        return node ? this._stripMarkup(node.textContent ?? "") : "";
      }).filter(Boolean);
    } catch {
      this._tips = ["Master the shadow form to unleash hidden abilities."];
    }
  }

  private static _rotateTip(): void {
    const tick = () => {
      if (!this._visible || !this._tipEl || this._tips.length === 0) return;
      const idx = Math.floor(Math.random() * this._tips.length);
      this._tipEl.textContent = this._tips[idx];
      setTimeout(tick, 4000);
    };
    setTimeout(tick, 600);
  }

  static show(): void {
    if (!this._overlay) this.mount();
    this._visible = true;
    this._overlay!.style.opacity = "1";
    this._overlay!.style.pointerEvents = "all";
  }

  static hide(onDone?: () => void): void {
    if (!this._overlay) { onDone?.(); return; }
    this._visible = false;
    this._overlay.style.opacity = "0";
    this._overlay.style.pointerEvents = "none";
    setTimeout(() => {
      this._overlay?.remove();
      this._overlay = null;
      this._tipEl = null;
      onDone?.();
    }, 420);
  }

  static get isVisible(): boolean { return this._visible; }
}
