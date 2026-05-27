export class LoadScreen {
  private static _el: HTMLDivElement | null = null;
  private static _tipEl: HTMLParagraphElement | null = null;
  private static _tips: string[] = [];
  private static _visible = false;

  static mount(): void {
    if (this._el) return;
    const el = document.getElementById("load-screen") as HTMLDivElement | null;
    if (!el) return;
    const bg = document.getElementById("load-screen-bg") as HTMLElement | null;
    if (bg) {
      bg.style.background = "url('assets/textures/ui/loadingGame.png') center / cover no-repeat #000";
    }
    el.style.removeProperty("display");
    el.classList.add("active");
    el.style.opacity = "1";
    el.style.pointerEvents = "all";
    this._el = el;
    this._tipEl = document.getElementById("load-screen-tip") as HTMLParagraphElement | null;
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
    if (!this._el) this.mount();
    this._visible = true;
    this._el!.style.opacity = "1";
    this._el!.style.pointerEvents = "all";
  }

  static hide(onDone?: () => void): void {
    if (!this._el) { onDone?.(); return; }
    this._visible = false;
    this._el.style.opacity = "0";
    this._el.style.pointerEvents = "none";
    setTimeout(() => {
      this._el?.classList.remove("active");
      this._el = null;
      this._tipEl = null;
      onDone?.();
    }, 420);
  }

  static get isVisible(): boolean { return this._visible; }
}
