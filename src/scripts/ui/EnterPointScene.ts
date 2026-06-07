/**
 * EnterPointScene.ts
 *
 * Mounts all HTML UI and bootstraps the Babylon fight scene.
 *
 * Key changes from the old version:
 *  - FightHUD.buildHUDCallbacks() is passed into BattleController before
 *    initBattle so FightController drives the HUD automatically every frame.
 *  - _activateFightHUD() no longer manually fires showRoundStart / showFightStart
 *    or calls startTimer() — all of that is now owned by FightController.
 *  - EnterPointScene just wires names, shows the HUD, and gets out of the way.
 */

import { Vector3 }         from "@babylonjs/core";
import { FreeCamera }      from "@babylonjs/core/Cameras/freeCamera";
import { SceneManager, SceneConfig, SceneNode } from "../core/SceneManager";
import { AtlasManager }    from "./AtlasManager";
import { FightHUD }        from "./FightHUD";
import { LoadScreen }      from "../LoadScreen";
import { UserDataController } from "../SF3/UserData/UserDataController";
import { FightScene as BattleFightScene } from "../battle/FightScene";
import type { IFightInfo } from "../SF3/FightController";
import "../../ui/styles/screens/currency-bar.css";
import "../../ui/styles/screens/home-menu.css";
import "../../ui/styles/screens/fight-hud.css";

interface SpriteSpec { w: number; h: number; stretch?: boolean; }

const SPRITE_SIZES: Record<string, SpriteSpec> = {
  "menu_icon":      { w: 44,  h: 28  },
  "dojo_icon":      { w: 52,  h: 40  },
  "shop_icon":      { w: 46,  h: 52  },
  "map_icon":       { w: 56,  h: 40  },
  "inventory_icon": { w: 52,  h: 52  },
  "booster_icon":   { w: 42,  h: 56  },
  "settings_icon":  { w: 52,  h: 52  },
  "chat":           { w: 36,  h: 24  },
  "cross":          { w: 22,  h: 22  },
  "progress_empty": { w: 180, h: 14, stretch: true },
  "progress_full":  { w: 180, h: 14, stretch: true },
  "coin":           { w: 34,  h: 34  },
  "bonus":          { w: 26,  h: 31  },
  "shadow_currency":{ w: 34,  h: 34  },
  "circle":         { w: 24,  h: 24  },
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
  private _mgr:       SceneManager;
  private _config:    SceneConfig;
  private _root:      HTMLDivElement | null = null;
  private _atlas:     AtlasManager;
  private _hud:       FightHUD | null = null;
  private _fightScene: BattleFightScene | null = null;
  private _inFight    = false;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr    = mgr;
    this._config = config;
    this._atlas  = new AtlasManager();
  }

  async mount(): Promise<void> {
    // Process scene hierarchy / cameras from JSON config
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
      this._mgr.scene.activeCamera = new FreeCamera(
        "fallback", new Vector3(0, 0, -10), this._mgr.scene,
      );
    }

    await this._atlas.load();

    const root = document.getElementById("enter-point-ui") as HTMLDivElement | null;
    if (!root) return;
    this._root = root;
    root.innerHTML = "";

    await this._inject("screens/currency-bar.html");
    await this._inject("screens/home-menu.html");
    await this._inject("screens/fight-hud.html");

    this._applySprites();
    this._injectPlayerData();
    this._wireHomeMenu();

    console.log("[EnterPointScene] mounted");

    // Auto-start training dojo on boot (mirrors Unity's default entry point)
    this._launchDojo();
  }

  // ─── Dojo launch ──────────────────────────────────────────────────────────

  private async _launchDojo(): Promise<void> {
    if (this._inFight) return;
    this._inFight = true;

    LoadScreen.show();

    const trainingFightInfo: IFightInfo = {
      battleID:     "training_dojo",
      fightID:      "training_0",
      roundsToWin:  2,
      roundsToLose: 2,
      roundTime:    99,
    };

    try {
      this._fightScene?.dispose();

      // 1. Create HUD and build callbacks before the scene initialises
      //    so BattleController.setHUDCallbacks() is called before initBattle.
      this._hud = new FightHUD();
      if (this._root) this._hud.bind(this._root);
      const hudCallbacks = this._hud.buildHUDCallbacks();

      // 2. Boot the fight scene — this creates BattleController internally
      this._fightScene = new BattleFightScene(this._mgr.scene);
      await this._fightScene.initialize("dojo_Legion", trainingFightInfo, hudCallbacks);

      // 3. Set player / enemy names
      const player = UserDataController.player;
      this._hud.setPlayerName(player?.Name ?? "PLAYER");
      this._hud.setEnemyName("ENEMY");

      // 4. Hide currency bar while in fight (mirrors DojoHolderModule)
      const currBar = document.getElementById("currency-bar");
      if (currBar) currBar.style.display = "none";

      // 5. Show HUD — FightController owns the banner / timer sequence from here
      this._hud.show();

      LoadScreen.hide();
      console.log("[EnterPointScene] dojo launched — FightController owns HUD from here");

    } catch (err) {
      console.error("[EnterPointScene] Fight launch failed:", err);
      LoadScreen.hide();
      this._inFight = false;
    }
  }

  // ─── Private helpers ─────────────────────────────────────────────────────

  private async _inject(path: string): Promise<void> {
    try {
      const html = await (await fetch(`ui/${path}`)).text();
      this._root!.insertAdjacentHTML("beforeend", html);
    } catch (e) {
      console.warn(`[EnterPointScene] failed to load ${path}`, e);
    }
  }

  private _applySprites(): void {
    if (!this._root) return;
    this._root.querySelectorAll<HTMLElement>("[class]").forEach(el => {
      for (const cls of el.classList) {
        if (!cls.startsWith("sp-")) continue;
        const name = cls.replace("sp-", "");
        const spec = SPRITE_SIZES[name];
        if (!spec)               { this._atlas.apply(el, name); }
        else if (spec.stretch)   { this._atlas.applyStretched(el, name, spec.w, spec.h); }
        else                     { this._atlas.applyScaled(el, name, spec.w, spec.h); }
        return;
      }
    });
  }

  private _injectPlayerData(): void {
    const p   = UserDataController.player;
    const set = (id: string, val: string) => {
      const el = document.getElementById(id);
      if (el) el.textContent = val;
    };
    set("currency-name",  p?.Name  ?? "PLAYER");
    set("currency-level", String(p?.Level ?? 1));
    set("val-bonus",  String(p?.Currency?.Bonus  ?? 0));
    set("val-coin",   String(p?.Currency?.Coin   ?? 0));
    set("val-shadow", String(p?.Currency?.Shadow ?? 0));
  }

  private _wireHomeMenu(): void {
    const homeBtn  = document.getElementById("home-button");
    const homeMenu = document.getElementById("home-menu");
    const screenBg = document.getElementById("home-screen-background");
    if (!homeBtn || !homeMenu || !screenBg) return;

    const open  = () => homeMenu.classList.add("open");
    const close = () => { homeMenu.classList.remove("open"); this._deselectMenuBtns(); };

    homeBtn.addEventListener("click", open);
    screenBg.addEventListener("click", close);
    document.addEventListener("keydown", e => { if (e.key === "Escape") close(); });

    homeMenu.querySelectorAll<HTMLElement>(".menu-btn").forEach(btn => {
      btn.addEventListener("click", e => {
        e.stopPropagation();
        const menu = btn.dataset.menu ?? "";
        close();
        if (menu === "dojo") {
          this._inFight = false; // allow re-launch
          this._launchDojo();
        } else {
          console.log(`[SlideMenu] OpenMenu("${menu}") – not yet implemented`);
        }
      });
      btn.addEventListener("mouseenter", () => { this._deselectMenuBtns(); btn.classList.add("selected"); });
      btn.addEventListener("mouseleave", () => btn.classList.remove("selected"));
    });
  }

  private _deselectMenuBtns(): void {
    document.querySelectorAll(".menu-btn.selected")
      .forEach(el => el.classList.remove("selected"));
  }

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

  unmount(): void { this._root?.remove(); this._root = null; }
}
