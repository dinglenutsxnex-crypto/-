interface AtlasFrame { x: number; y: number; w: number; h: number; }
interface AtlasInfo  { path: string; width: number; height: number; json?: string; }

const ATLAS_DEFS: Record<string, AtlasInfo> = {
  Common:   { path: "assets/textures/ui/nativeui/atlases/Common.png",   width: 2048, height: 2048, json: "CommonJSON"   },
  Currency: { path: "assets/textures/ui/nativeui/atlases/Currency.png", width: 512,  height: 512,  json: "CurrencyJSON" },
  DojoMenu: { path: "assets/textures/ui/nativeui/atlases/DojoMenu.png", width: 512,  height: 512  },
};

// Correct DojoMenu.png (512×512) frame coords — verified by pixel inspection
const DOJOMENU_FRAMES: Record<string, AtlasFrame> = {
  menu_icon:      { x: 1,   y: 317, w: 94,  h: 35  },  // hamburger ≡ lines
  dojo_icon:      { x: 7,   y: 180, w: 138, h: 107 },  // tent
  shop_icon:      { x: 171, y: 174, w: 112, h: 125 },  // bag/dress
  map_icon:       { x: 310, y: 189, w: 137, h: 95  },  // mountains
  booster_icon:   { x: 284, y: 13,  w: 98,  h: 130 },  // cards
  settings_icon:  { x: 106, y: 327, w: 129, h: 129 },  // gear
  inventory_icon: { x: 257, y: 325, w: 130, h: 130 },  // coin circle
};

interface SpriteRecord { atlasKey: string; frame: AtlasFrame; }

export class AtlasManager {
  private _sprites = new Map<string, SpriteRecord>();

  async load(): Promise<void> {
    for (const [key, def] of Object.entries(ATLAS_DEFS)) {
      if (def.json) await this._loadJSON(key, def.json);
    }
    for (const [name, frame] of Object.entries(DOJOMENU_FRAMES)) {
      this._sprites.set(name, { atlasKey: "DojoMenu", frame });
    }
    console.log(`[AtlasManager] ${this._sprites.size} sprites ready`);
  }

  private async _loadJSON(atlasKey: string, jsonName: string): Promise<void> {
    try {
      const res  = await fetch(`assets/textures/ui/nativeui/atlases/${jsonName}.json`);
      const data = await res.json();
      for (const [key, val] of Object.entries(data.frames as Record<string, any>)) {
        const f = val.frame as { x:number; y:number; w:number; h:number };
        this._sprites.set(key.replace(/\.png$/i, ""), { atlasKey, frame: f });
      }
    } catch (e) {
      console.warn(`[AtlasManager] failed: ${jsonName}`, e);
    }
  }

  /** Native atlas pixel size */
  apply(el: HTMLElement, name: string): void {
    const rec = this._sprites.get(name);
    if (!rec) { console.warn(`[AtlasManager] missing: "${name}"`); return; }
    const atlas = ATLAS_DEFS[rec.atlasKey];
    const f = rec.frame;
    Object.assign(el.style, {
      backgroundImage:    `url(${atlas.path})`,
      backgroundPosition: `-${f.x}px -${f.y}px`,
      backgroundSize:     `${atlas.width}px ${atlas.height}px`,
      backgroundRepeat:   "no-repeat",
      width:  `${f.w}px`,
      height: `${f.h}px`,
      display:    "block",
      flexShrink: "0",
    });
  }

  /** Scale to fit displayW×displayH, preserving aspect */
  applyScaled(el: HTMLElement, name: string, displayW: number, displayH: number): void {
    const rec = this._sprites.get(name);
    if (!rec) { console.warn(`[AtlasManager] missing: "${name}"`); return; }
    const atlas = ATLAS_DEFS[rec.atlasKey];
    const f = rec.frame;
    const scale = Math.min(displayW / f.w, displayH / f.h);
    Object.assign(el.style, {
      backgroundImage:    `url(${atlas.path})`,
      backgroundPosition: `${-f.x * scale}px ${-f.y * scale}px`,
      backgroundSize:     `${atlas.width * scale}px ${atlas.height * scale}px`,
      backgroundRepeat:   "no-repeat",
      width:  `${displayW}px`,
      height: `${displayH}px`,
      display:    "block",
      flexShrink: "0",
    });
  }

  /** Stretch to exact displayW×displayH */
  applyStretched(el: HTMLElement, name: string, displayW: number, displayH: number): void {
    const rec = this._sprites.get(name);
    if (!rec) { console.warn(`[AtlasManager] missing: "${name}"`); return; }
    const atlas = ATLAS_DEFS[rec.atlasKey];
    const f = rec.frame;
    const sx = displayW / f.w;
    const sy = displayH / f.h;
    Object.assign(el.style, {
      backgroundImage:    `url(${atlas.path})`,
      backgroundPosition: `${-f.x * sx}px ${-f.y * sy}px`,
      backgroundSize:     `${atlas.width * sx}px ${atlas.height * sy}px`,
      backgroundRepeat:   "no-repeat",
      width:  `${displayW}px`,
      height: `${displayH}px`,
      display:    "block",
      flexShrink: "0",
    });
  }

  hasSprite(name: string): boolean { return this._sprites.has(name); }
}
