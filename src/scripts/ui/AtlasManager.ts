interface AtlasFrame {
  x: number;
  y: number;
  w: number;
  h: number;
}

interface AtlasInfo {
  path: string;
  width: number;
  height: number;
  json?: string;
}

interface SpriteStyle {
  backgroundImage: string;
  backgroundPosition: string;
  backgroundSize: string;
  width: string;
  height: string;
}

const ATLAS_DEFS: Record<string, AtlasInfo> = {
  Common: {
    path: "assets/textures/ui/nativeui/atlases/Common.png",
    width: 2048, height: 2048,
    json: "CommonJSON",
  },
  Currency: {
    path: "assets/textures/ui/nativeui/atlases/Currency.png",
    width: 512, height: 512,
    json: "CurrencyJSON",
  },
  DojoMenu: {
    path: "assets/textures/ui/nativeui/atlases/DojoMenu.png",
    width: 512, height: 512,
  },
};

const DOJO_SPRITES: Record<string, AtlasFrame> = {
  menu_icon:         { x: 1,   y: 137, w: 94,  h: 58  },
  dojo_icon:         { x: 5,   y: 223, w: 142, h: 111 },
  map_icon:          { x: 308, y: 226, w: 141, h: 99  },
  shop_icon:         { x: 255, y: 55,  w: 134, h: 134 },
  inventory_icon:    { x: 169, y: 211, w: 116, h: 129 },
};

interface SpriteRecord {
  atlasKey: string;
  frame: AtlasFrame;
}

export class AtlasManager {
  private _sprites = new Map<string, SpriteRecord>();

  async load(): Promise<void> {
    for (const [key, def] of Object.entries(ATLAS_DEFS)) {
      if (def.json) {
        await this._loadJSON(key, def.json);
      }
    }
    for (const [name, frame] of Object.entries(DOJO_SPRITES)) {
      this._sprites.set(name, { atlasKey: "DojoMenu", frame });
    }
    console.log(`[AtlasManager] loaded ${this._sprites.size} sprites`);
  }

  private async _loadJSON(atlasKey: string, jsonName: string): Promise<void> {
    try {
      const url = `assets/textures/ui/nativeui/atlases/${jsonName}.json`;
      const res = await fetch(url);
      const data = await res.json();
      for (const [key, val] of Object.entries(data.frames)) {
        const frameData = val as any;
        const frame: AtlasFrame = {
          x: frameData.frame.x,
          y: frameData.frame.y,
          w: frameData.frame.w,
          h: frameData.frame.h,
        };
        const name = key.replace(/\.png$/i, "");
        this._sprites.set(name, { atlasKey, frame });
      }
    } catch (e) {
      console.warn(`[AtlasManager] failed to load ${jsonName}:`, e);
    }
  }

  private _getAtlas(name: string): AtlasInfo | null {
    const rec = this._sprites.get(name);
    if (!rec) return null;
    return ATLAS_DEFS[rec.atlasKey] ?? null;
  }

  private _getFrame(name: string): AtlasFrame | null {
    return this._sprites.get(name)?.frame ?? null;
  }

  private _nativeStyle(name: string): SpriteStyle | null {
    const frame = this._getFrame(name);
    const atlas = this._getAtlas(name);
    if (!frame || !atlas) return null;
    return {
      backgroundImage: `url(${atlas.path})`,
      backgroundPosition: `-${frame.x}px -${frame.y}px`,
      backgroundSize: `${atlas.width}px ${atlas.height}px`,
      width: `${frame.w}px`,
      height: `${frame.h}px`,
    };
  }

  scaledStyle(name: string, displayW: number, displayH: number): SpriteStyle | null {
    const frame = this._getFrame(name);
    const atlas = this._getAtlas(name);
    if (!frame || !atlas) return null;

    const sx = displayW / frame.w;
    const sy = displayH / frame.h;
    const scale = Math.min(sx, sy);

    const bgW = atlas.width * scale;
    const bgH = atlas.height * scale;
    const posX = -frame.x * scale;
    const posY = -frame.y * scale;

    return {
      backgroundImage: `url(${atlas.path})`,
      backgroundPosition: `${posX}px ${posY}px`,
      backgroundSize: `${bgW}px ${bgH}px`,
      width: `${displayW}px`,
      height: `${displayH}px`,
    };
  }

  getStyle(name: string): SpriteStyle | null {
    return this._nativeStyle(name);
  }

  apply(el: HTMLElement, name: string): void {
    const style = this._nativeStyle(name);
    if (!style) return;
    el.style.backgroundImage = style.backgroundImage;
    el.style.backgroundPosition = style.backgroundPosition;
    el.style.backgroundSize = style.backgroundSize;
    el.style.width = style.width;
    el.style.height = style.height;
    el.style.backgroundRepeat = "no-repeat";
  }

  applyScaled(el: HTMLElement, name: string, displayW: number, displayH: number): void {
    const style = this.scaledStyle(name, displayW, displayH);
    if (!style) return;
    el.style.backgroundImage = style.backgroundImage;
    el.style.backgroundPosition = style.backgroundPosition;
    el.style.backgroundSize = style.backgroundSize;
    el.style.width = style.width;
    el.style.height = style.height;
    el.style.backgroundRepeat = "no-repeat";
  }
}
