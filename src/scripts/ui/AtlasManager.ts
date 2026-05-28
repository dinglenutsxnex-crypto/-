/* AtlasManager — loads sprite atlas JSON/PNG pairs and applies CSS background sprites.
   Atlas source data:
     Common.png   2048×2048  — CommonJSON.json
     Currency.png  512×512   — CurrencyJSON.json
     DojoMenu.png  512×512   — no JSON, hard-coded frames
*/

interface AtlasFrame { x: number; y: number; w: number; h: number; }

interface AtlasInfo {
  path:   string;
  width:  number;
  height: number;
  json?:  string;   // JSON filename (no extension) to load dynamically
}

const ATLAS_DEFS: Record<string, AtlasInfo> = {
  Common: {
    path:   "assets/textures/ui/nativeui/atlases/Common.png",
    width:  2048, height: 2048,
    json:   "CommonJSON",
  },
  Currency: {
    path:   "assets/textures/ui/nativeui/atlases/Currency.png",
    width:  512, height: 512,
    json:   "CurrencyJSON",
  },
  DojoMenu: {
    path:   "assets/textures/ui/nativeui/atlases/DojoMenu.png",
    width:  512, height: 512,
  },
};

/** Hard-coded frames for DojoMenu.png (512×512, no JSON).
 *  Sprite coords measured from the 512px atlas (source art @ 2048px → 4× downscale).
 */
const DOJOMENU_FRAMES: Record<string, AtlasFrame> = {
  menu_icon:      { x: 1,   y: 137, w: 94,  h: 58  },
  dojo_icon:      { x: 5,   y: 223, w: 142, h: 111 },
  map_icon:       { x: 308, y: 226, w: 141, h: 99  },
  shop_icon:      { x: 255, y: 55,  w: 134, h: 134 },
  inventory_icon: { x: 169, y: 211, w: 116, h: 129 },
};

interface SpriteRecord { atlasKey: string; frame: AtlasFrame; }

export class AtlasManager {
  private _sprites = new Map<string, SpriteRecord>();

  async load(): Promise<void> {
    // Load JSON atlases
    for (const [key, def] of Object.entries(ATLAS_DEFS)) {
      if (def.json) {
        await this._loadJSON(key, def.json);
      }
    }
    // Register hard-coded DojoMenu frames
    for (const [name, frame] of Object.entries(DOJOMENU_FRAMES)) {
      this._sprites.set(name, { atlasKey: "DojoMenu", frame });
    }
    console.log(`[AtlasManager] ${this._sprites.size} sprites ready`);
  }

  private async _loadJSON(atlasKey: string, jsonName: string): Promise<void> {
    try {
      const url = `assets/textures/ui/nativeui/atlases/${jsonName}.json`;
      const res  = await fetch(url);
      const data = await res.json();
      for (const [key, val] of Object.entries(data.frames as Record<string, any>)) {
        const f = val.frame as { x: number; y: number; w: number; h: number };
        const name = key.replace(/\.png$/i, "");
        this._sprites.set(name, { atlasKey, frame: { x: f.x, y: f.y, w: f.w, h: f.h } });
      }
    } catch (e) {
      console.warn(`[AtlasManager] failed to load ${jsonName}:`, e);
    }
  }

  /** Apply sprite at its native atlas pixel size. */
  apply(el: HTMLElement, name: string): void {
    const rec = this._sprites.get(name);
    if (!rec) { console.warn(`[AtlasManager] sprite not found: "${name}"`); return; }
    const atlas = ATLAS_DEFS[rec.atlasKey];
    const f     = rec.frame;
    el.style.backgroundImage    = `url(${atlas.path})`;
    el.style.backgroundPosition = `-${f.x}px -${f.y}px`;
    el.style.backgroundSize     = `${atlas.width}px ${atlas.height}px`;
    el.style.backgroundRepeat   = "no-repeat";
    el.style.width              = `${f.w}px`;
    el.style.height             = `${f.h}px`;
    el.style.display            = "block";
    el.style.flexShrink         = "0";
  }

  /** Apply sprite scaled to a specific display size (preserves aspect ratio by fitting). */
  applyScaled(el: HTMLElement, name: string, displayW: number, displayH: number): void {
    const rec = this._sprites.get(name);
    if (!rec) { console.warn(`[AtlasManager] sprite not found: "${name}"`); return; }
    const atlas = ATLAS_DEFS[rec.atlasKey];
    const f     = rec.frame;

    const scale = Math.min(displayW / f.w, displayH / f.h);
    const bgW   = atlas.width  * scale;
    const bgH   = atlas.height * scale;
    const posX  = -f.x * scale;
    const posY  = -f.y * scale;

    el.style.backgroundImage    = `url(${atlas.path})`;
    el.style.backgroundPosition = `${posX}px ${posY}px`;
    el.style.backgroundSize     = `${bgW}px ${bgH}px`;
    el.style.backgroundRepeat   = "no-repeat";
    el.style.width              = `${displayW}px`;
    el.style.height             = `${displayH}px`;
    el.style.display            = "block";
    el.style.flexShrink         = "0";
  }

  /** Apply sprite stretched to exact display size (no aspect correction). */
  applyStretched(el: HTMLElement, name: string, displayW: number, displayH: number): void {
    const rec = this._sprites.get(name);
    if (!rec) return;
    const atlas = ATLAS_DEFS[rec.atlasKey];
    const f     = rec.frame;

    const scaleX = displayW / f.w;
    const scaleY = displayH / f.h;
    const bgW    = atlas.width  * scaleX;
    const bgH    = atlas.height * scaleY;

    el.style.backgroundImage    = `url(${atlas.path})`;
    el.style.backgroundPosition = `${-f.x * scaleX}px ${-f.y * scaleY}px`;
    el.style.backgroundSize     = `${bgW}px ${bgH}px`;
    el.style.backgroundRepeat   = "no-repeat";
    el.style.width              = `${displayW}px`;
    el.style.height             = `${displayH}px`;
    el.style.display            = "block";
    el.style.flexShrink         = "0";
  }

  hasSprite(name: string): boolean {
    return this._sprites.has(name);
  }
}
