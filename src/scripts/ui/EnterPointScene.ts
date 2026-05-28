import { Vector3 }    from "@babylonjs/core";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { SceneManager, SceneConfig, SceneNode } from "../core/SceneManager";
import { AtlasManager }       from "./AtlasManager";
import { FightHUD }           from "./FightHUD";
import { LoadScreen }         from "../LoadScreen";
import { UserDataController } from "../SF3/UserData/UserDataController";
import { FightScene as BattleFightScene } from "../battle/FightScene";
import type { IFightInfo }    from "../FightController";
import "../../ui/styles/screens/currency-bar.css";
import "../../ui/styles/screens/home-menu.css";
import "../../ui/styles/screens/fight-hud.css";

interface SpriteSpec { w: number; h: number; stretch?: boolean; }

const SPRITE_SIZES: Record<string, SpriteSpec> = {
  // DojoMenu atlas
  "menu_icon":      { w: 44,  h: 28  },
  "dojo_icon":      { w: 52,  h: 40  },
  "shop_icon":      { w: 46,  h: 52  },
  "map_icon":       { w: 56,  h: 40  },
  "inventory_icon": { w: 52,  h: 52  },
  "booster_icon":   { w: 42,  h: 56  },
  "settings_icon":  { w: 52,  h: 52  },

  // Currency atlas
  "chat":           { w: 36,  h: 24  },
  "cross":          { w: 22,  h: 22  },
  "progress_empty": { w: 180, h: 14, stretch: true },
  "progress_full":  { w: 180, h: 14, stretch: true },

  // Common atlas
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
  private _mgr:    SceneManager;
  private _config: SceneConfig;
  private _root:   HTMLDivElement | null = null;
  private _atlas:  AtlasManager;
  private _hud:    FightHUD | null = null;
  private _fightScene: BattleFightScene | null = null;
  private _inFight = false;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr    = mgr;
    this._config = config;
    this._atlas  = new AtlasManager();
  }

  async mount(): Promise<void> {
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
      this._mgr.scene.activeCamera = new FreeCamera("fallback", new Vector3(0, 0, -10), this._mgr.scene);
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

    // Auto-start in training
    this._launchDojo();
  }

  // ─────────────────────────────────────────────────────────────────────────
  //  Fight launch (mirrors DojoHolderModule + FightHolderModule)
  // ─────────────────────────────────────────────────────────────────────────

  private async _launchDojo(): Promise<void> {
    if (this._inFight) return;
    this._inFight = true;

    // Show loading screen during scene bootstrap
    LoadScreen.show();

    // Build a minimal training FightInfo (no server needed)
    const trainingFightInfo: IFightInfo = {
      battleID:    "training_dojo",
      fightID:     "training_0",
      roundsToWin: 2,
      roundsToLose:2,
    };

    try {
      // Dispose previous fight scene if any
      this._fightScene?.dispose();

      // Initialize the fight scene (loads location + models + camera + BattleController)
      this._fightScene = new BattleFightScene(this._mgr.scene);
      await this._fightScene.initialize("dojo_Legion", trainingFightInfo);

      // Hide currency bar while in fight (mirrors CurrencyUI.SetActive(true) on dojo)
      const currBar = document.getElementById("currency-bar");
      if (currBar) currBar.style.display = "none";

      LoadScreen.hide(() => {
        // Activate fight HUD and run the round-start sequence
        this._activateFightHUD(trainingFightInfo);
      });

    } catch (err) {
      console.error("[EnterPointScene] Fight launch failed:", err);
      LoadScreen.hide();
      this._inFight = false;
    }
  }

  private _activateFightHUD(info: IFightInfo): void {
    if (!this._root) return;

    // Create HUD controller and bind to DOM
    this._hud = new FightHUD();
    this._hud.bind(this._root);

    // Set names from user data
    const player = UserDataController.player;
    this._hud.setPlayerName(player?.Name ?? "PLAYER");
    this._hud.setEnemyName("ENEMY");

    // Init round state
    this._hud.initRound(1, info.roundsToWin, 99);
    this._hud.show();

    // ── Sequence: ROUND 1 → FIGHT! → start timer ──
    // Mirrors BattleInterface.ShowStartRound + ShowStartRoundFight
    this._hud.showRoundStart(() => {
      this._hud!.showFightStart(() => {
        this._hud!.startTimer();
        console.log("[EnterPointScene] Fight active – RoundFightStart");
      });
    });
  }

  // ─────────────────────────────────────────────────────────────────────────
  //  Private helpers (unchanged from original)
  // ─────────────────────────────────────────────────────────────────────────

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
        if (!spec) {
          this._atlas.apply(el, name);
        } else if (spec.stretch) {
          this._atlas.applyStretched(el, name, spec.w, spec.h);
        } else {
          this._atlas.applyScaled(el, name, spec.w, spec.h);
        }
        return;
      }
    });
  }

  private _injectPlayerData(): void {
    const p = UserDataController.player;

    const nameEl = document.getElementById("currency-name");
    if (nameEl) nameEl.textContent = p?.Name ?? "PLAYER";

    const lvEl = document.getElementById("currency-level");
    if (lvEl) lvEl.textContent = String(p?.Level ?? 1);

    const cur = p?.Currency;
    const b = document.getElementById("val-bonus");
    const c = document.getElementById("val-coin");
    const s = document.getElementById("val-shadow");
    if (b) b.textContent = String(cur?.Bonus  ?? 0);
    if (c) c.textContent = String(cur?.Coin   ?? 0);
    if (s) s.textContent = String(cur?.Shadow ?? 0);
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

        // ── Route menu buttons ──
        if (menu === "dojo") {
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
    document.querySelectorAll(".menu-btn.selected").forEach(el => el.classList.remove("selected"));
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
