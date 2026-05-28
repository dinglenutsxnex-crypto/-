/* EnterPointScene — mirrors Unity SlideMenu + CurrencyUI logic in HTML/CSS.
   
   Layout (1280×720 design space):
   ┌──────────────────────────────────────────────┐
   │  [☰] │ name · lv · xp ░░░░░░░░░ [💬] │ B C S│  ← currency-bar  108px
   ├──────┼───────────────────────────────────────┤
   │slide │                                       │
   │menu  │       (babylon 3D canvas)             │
   │panel │                                       │
   └──────┴───────────────────────────────────────┘
   The slide menu panel (220px wide) slides in from the left on hamburger click.
*/

import { Vector3 }    from "@babylonjs/core";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { SceneManager, SceneConfig, SceneNode } from "../core/SceneManager";
import { AtlasManager }       from "./AtlasManager";
import { UserDataController } from "../SF3/UserData/UserDataController";
import "../../ui/styles/screens/currency-bar.css";
import "../../ui/styles/screens/home-menu.css";

/* ─── Atlas sprite → display-size map ─── */
interface SpriteSpec { atlas: string; w?: number; h?: number; stretch?: boolean; }

/** Elements whose class starts with sp-<name> get sprite applied automatically.
 *  If w/h are provided  → applyScaled (fit)
 *  If stretch:true      → applyStretched
 *  Otherwise            → apply (native size)
 */
const SPRITE_MAP: Record<string, SpriteSpec> = {
  // ── DojoMenu atlas (512×512, hard-coded frames) ──────────────────────
  "menu_icon":      { atlas: "DojoMenu", w: 50,  h: 32  },  // hamburger icon
  "dojo_icon":      { atlas: "DojoMenu", w: 56,  h: 44  },
  "map_icon":       { atlas: "DojoMenu", w: 56,  h: 40  },
  "shop_icon":      { atlas: "DojoMenu", w: 56,  h: 56  },
  "inventory_icon": { atlas: "DojoMenu", w: 52,  h: 58  },

  // ── Currency atlas (512×512, CurrencyJSON) ───────────────────────────
  // chat.png          → 101×67  displayed at roughly half
  "chat":           { atlas: "Currency", w: 34,  h: 23  },
  // cross.png         → 51×52   the + add button
  "cross":          { atlas: "Currency", w: 18,  h: 18  },
  // progress_empty    → 460×19  XP bar background
  "progress_empty": { atlas: "Currency", w: 180, h: 19, stretch: true },
  // progress_full     → 460×19  XP bar fill
  "progress_full":  { atlas: "Currency", w: 180, h: 19, stretch: true },

  // ── Common atlas (2048×2048, CommonJSON) ─────────────────────────────
  // coin.png          → 90×90
  "coin":           { atlas: "Common",   w: 32,  h: 32  },
  // bonus.png         → 60×72
  "bonus":          { atlas: "Common",   w: 26,  h: 31  },
  // shadow_currency   → 99×98
  "shadow_currency":{ atlas: "Common",   w: 32,  h: 32  },
  // circle.png        → 52×52  used for badge
  "circle":         { atlas: "Common",   w: 28,  h: 28  },
};

function createBabylonCamera(mgr: SceneManager, node: SceneNode): void {
  const cam = node.components?.find((c: { type: string }) => c.type === "camera");
  if (!cam?.data) return;
  const cd  = cam.data;
  const pos = node.transform?.position ?? { x: 0, y: 0, z: -70 };
  const fc  = new FreeCamera(node.name, new Vector3(pos.x, pos.y, pos.z ?? -70), mgr.scene);
  fc.minZ = cd.nearClip ?? 0.3;
  fc.maxZ = cd.farClip  ?? 1000;
  if (cd.type === "orthographic") fc.mode = FreeCamera.ORTHOGRAPHIC_CAMERA;
  if (cd.clearFlags === 1) {
    const bg = cd.backgroundColor || {};
    mgr.scene.clearColor.set(bg.r ?? 0, bg.g ?? 0, bg.b ?? 0, bg.a ?? 1);
  } else if (cd.clearFlags === 3) {
    mgr.scene.clearColor.set(0, 0, 0, 0);
  }
  if (cd.depth === -1 || cd.depth === undefined) mgr.scene.activeCamera = fc;
}

export class EnterPointScene {
  private _mgr:    SceneManager;
  private _config: SceneConfig;
  private _root:   HTMLDivElement | null = null;
  private _atlas:  AtlasManager;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr    = mgr;
    this._config = config;
    this._atlas  = new AtlasManager();
  }

  async mount(): Promise<void> {
    // 1. Set up Babylon camera from scene JSON
    if (this._config.hierarchy) {
      for (const node of this._config.hierarchy) {
        if (!node.isActive) continue;
        if (node.components?.some((c: { type: string }) => c.type === "camera")) {
          createBabylonCamera(this._mgr, node);
        }
        this._mountRecursive(node);
      }
    }
    if (!this._mgr.scene.activeCamera) {
      const fb = new FreeCamera("fallback", new Vector3(0, 0, -10), this._mgr.scene);
      this._mgr.scene.activeCamera = fb;
    }

    // 2. Load sprite atlases
    await this._atlas.load();

    // 3. Inject HTML templates
    const root = document.getElementById("enter-point-ui") as HTMLDivElement | null;
    if (!root) return;
    this._root = root;
    root.innerHTML = "";

    await this._inject("screens/currency-bar.html");
    await this._inject("screens/home-menu.html");

    // 4. Apply atlas sprites to all sp-* elements
    this._applySprites();

    // 5. Wire live player data
    this._injectPlayerData();

    // 6. Wire interactivity (SlideMenu behaviour)
    this._wireHomeMenu();

    console.log("[EnterPointScene] mounted");
  }

  // ── Template loader ──────────────────────────────────────────────
  private async _inject(path: string): Promise<void> {
    try {
      const res  = await fetch(`ui/${path}`);
      const html = await res.text();
      this._root!.insertAdjacentHTML("beforeend", html);
    } catch (e) {
      console.warn(`[EnterPointScene] failed to load ${path}`, e);
    }
  }

  // ── Atlas sprite application ──────────────────────────────────────
  /** Scan all elements with class sp-<name> and apply the matching sprite. */
  private _applySprites(): void {
    if (!this._root) return;

    this._root.querySelectorAll<HTMLElement>("[class]").forEach(el => {
      for (const cls of el.classList) {
        if (!cls.startsWith("sp-")) continue;
        const name = cls.replace("sp-", "");
        const spec = SPRITE_MAP[name];
        if (!spec) {
          this._atlas.apply(el, name);
          return;
        }
        if (spec.stretch && spec.w && spec.h) {
          this._atlas.applyStretched(el, name, spec.w, spec.h);
        } else if (spec.w && spec.h) {
          this._atlas.applyScaled(el, name, spec.w, spec.h);
        } else {
          this._atlas.apply(el, name);
        }
        return;
      }
    });
  }

  // ── Player data ───────────────────────────────────────────────────
  private _injectPlayerData(): void {
    const p = UserDataController.player;

    const nameEl = document.getElementById("currency-name");
    if (nameEl) nameEl.textContent = p?.Name ?? "PLAYER";

    const lvEl = document.getElementById("currency-level");
    if (lvEl) lvEl.textContent = String(p?.Level ?? 1);

    // XP bar: the .xp-fill sprite is 180px wide but we clip it via its parent
    // #currency-xp-bar already has position:relative + overflow:hidden via CSS;
    // the .xp-fill sprite is 180px wide, we just mask it using a wrapper approach:
    // easier to just set the fill element width directly (it has overflow hidden on parent)
    const xpFill = document.getElementById("currency-xp-fill") as HTMLElement | null;
    if (xpFill) {
      const pct = (p && p.LevelExperience > 0)
        ? Math.min(1, p.Experience / p.LevelExperience)
        : 0;
      // override the width that AtlasManager set (180px) to clip the bar
      xpFill.style.width = `${pct * 180}px`;
    }

    const cur = p?.Currency;
    const bonusEl  = document.getElementById("val-bonus");
    const coinEl   = document.getElementById("val-coin");
    const shadowEl = document.getElementById("val-shadow");
    if (bonusEl)  bonusEl.textContent  = String(cur?.Bonus  ?? 0);
    if (coinEl)   coinEl.textContent   = String(cur?.Coin   ?? 0);
    if (shadowEl) shadowEl.textContent = String(cur?.Shadow ?? 0);
  }

  // ── SlideMenu wiring (mirrors Unity SlideMenu.cs MenuMoveController) ──
  private _wireHomeMenu(): void {
    const homeBtn  = document.getElementById("home-button");
    const homeMenu = document.getElementById("home-menu");
    const screenBg = document.getElementById("home-screen-background");
    if (!homeBtn || !homeMenu || !screenBg) return;

    const open  = () => homeMenu.classList.add("open");
    const close = () => {
      homeMenu.classList.remove("open");
      this._deselectMenuBtns();
    };

    homeBtn.addEventListener("click", open);
    screenBg.addEventListener("click", close);

    // Escape key = CloseWithoutCooldown
    document.addEventListener("keydown", (e) => {
      if (e.key === "Escape") close();
    });

    // Menu buttons
    homeMenu.querySelectorAll<HTMLElement>(".menu-btn").forEach(btn => {
      btn.addEventListener("click", (e) => {
        e.stopPropagation();
        const menu = btn.dataset.menu ?? "";
        if (menu) console.log(`[SlideMenu] OpenMenu("${menu}")`);
        close();
      });
      btn.addEventListener("mouseenter", () => {
        this._deselectMenuBtns();
        btn.classList.add("selected");
      });
      btn.addEventListener("mouseleave", () => btn.classList.remove("selected"));
    });
  }

  private _deselectMenuBtns(): void {
    document.querySelectorAll(".menu-btn.selected").forEach(el => el.classList.remove("selected"));
  }

  // ── Babylon hierarchy walker ──────────────────────────────────────
  private _mountRecursive(node: SceneNode): void {
    if (!node.isActive || !node.children) return;
    for (const child of node.children) {
      if (!child.isActive) continue;
      if (child.components?.some((c: { type: string }) => c.type === "camera")) {
        createBabylonCamera(this._mgr, child);
      }
      this._mountRecursive(child);
    }
  }

  unmount(): void {
    this._root?.remove();
    this._root = null;
  }
}
