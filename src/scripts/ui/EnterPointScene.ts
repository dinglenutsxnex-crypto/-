import { Vector3 } from "@babylonjs/core";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { SceneManager, SceneConfig, SceneNode } from "../core/SceneManager";
import { AtlasManager } from "./AtlasManager";
import { UserDataController } from "../SF3/UserData/UserDataController";
import "../../ui/styles/screens/currency-bar.css";
import "../../ui/styles/screens/home-menu.css";

function createBabylonCamera(mgr: SceneManager, node: SceneNode): void {
  const camComponent = node.components?.find((c: { type: string }) => c.type === "camera");
  if (!camComponent || !camComponent.data) return;

  const cd = camComponent.data;
  const t = node.transform || { position: { x: 0, y: 0, z: -70 } };
  const pos = t.position;

  const cam = new FreeCamera(node.name, new Vector3(pos.x, pos.y, pos.z ?? -70), mgr.scene);
  cam.minZ = cd.nearClip ?? 0.3;
  cam.maxZ = cd.farClip ?? 1000;

  if (cd.type === "orthographic") {
    cam.mode = FreeCamera.ORTHOGRAPHIC_CAMERA;
  }

  if (cd.clearFlags === 1) {
    const bg = cd.backgroundColor || {};
    mgr.scene.clearColor.set(bg.r ?? 0, bg.g ?? 0, bg.b ?? 0, bg.a ?? 1);
  } else if (cd.clearFlags === 3) {
    mgr.scene.clearColor.set(0, 0, 0, 0);
  }

  if (cd.depth === -1 || cd.depth === undefined) {
    mgr.scene.activeCamera = cam;
  }
}

interface ScaledSprite {
  name: string;
  w: number;
  h: number;
}

const MENU_ICON_SIZE = { w: 60, h: 60 };
const HOME_ICON_SIZE = { w: 50, h: 32 };
const BADGE_ICON_SIZE = { w: 30, h: 30 };

const SCALED_SPRITES: Record<string, ScaledSprite> = {
  "sp-menu_icon bg-only": { name: "menu_icon", ...HOME_ICON_SIZE },
  "sp-map_icon bg-only": { name: "map_icon", ...MENU_ICON_SIZE },
  "sp-shop_icon bg-only": { name: "shop_icon", ...MENU_ICON_SIZE },
  "sp-dojo_icon bg-only": { name: "dojo_icon", ...MENU_ICON_SIZE },
  "sp-inventory_icon bg-only": { name: "inventory_icon", ...MENU_ICON_SIZE },
  "sp-circle bg-only": { name: "circle", ...BADGE_ICON_SIZE },
};

export class EnterPointScene {
  private _mgr: SceneManager;
  private _config: SceneConfig;
  private _rootEl: HTMLDivElement | null = null;
  private _atlas: AtlasManager;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr = mgr;
    this._config = config;
    this._atlas = new AtlasManager();
  }

  async mount(): Promise<void> {
    console.log("[EnterPointScene] mounting");

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
      const fallbackCam = new FreeCamera("fallback", new Vector3(0, 0, -10), this._mgr.scene);
      this._mgr.scene.activeCamera = fallbackCam;
    }

    await this._atlas.load();

    const root = document.getElementById("enter-point-ui") as HTMLDivElement;
    if (!root) return;
    this._rootEl = root;

    root.innerHTML = "";

    await Promise.all([
      this._loadTemplate("screens/currency-bar.html"),
      this._loadTemplate("screens/home-menu.html"),
    ]);

    this._applyAtlasSprites();
    this._injectPlayerData();
    this._wireHomeMenu();
  }

  private async _loadTemplate(path: string): Promise<void> {
    try {
      const res = await fetch(`ui/${path}`);
      const html = await res.text();
      if (this._rootEl) {
        this._rootEl.insertAdjacentHTML("beforeend", html);
      }
    } catch (e) {
      console.warn(`[EnterPointScene] failed to load ${path}:`, e);
    }
  }

  private _applyAtlasSprites(): void {
    if (!this._rootEl) return;

    const scaledKeys = new Set(Object.keys(SCALED_SPRITES));

    const els = this._rootEl.querySelectorAll<HTMLElement>("[class]");
    for (const el of els) {
      const classKey = Array.from(el.classList).join(" ");
      if (scaledKeys.has(classKey)) {
        const spec = SCALED_SPRITES[classKey];
        this._atlas.applyScaled(el, spec.name, spec.w, spec.h);
        continue;
      }
      for (const cls of el.classList) {
        if (cls.startsWith("sp-")) {
          const name = cls.replace("sp-", "");
          this._atlas.apply(el, name);
        }
      }
    }
  }

  private _injectPlayerData(): void {
    const player = UserDataController.player;
    if (!player) return;

    const nameEl = document.getElementById("currency-name");
    if (nameEl) nameEl.textContent = player.Name;

    const levelEl = document.getElementById("currency-level");
    if (levelEl) levelEl.textContent = String(player.Level);

    const progressFill = document.querySelector(".progress-fill") as HTMLElement;
    if (progressFill && player.LevelExperience > 0) {
      const pct = Math.min(100, (player.Experience / player.LevelExperience) * 100);
      progressFill.style.width = `${pct}%`;
    }

    const currency = player.Currency;
    if (currency) {
      const bonusEl = document.querySelector(".currency-item.bonus .currency-value");
      if (bonusEl) bonusEl.textContent = String(currency.Bonus);

      const coinEl = document.querySelector(".currency-item.coin .currency-value");
      if (coinEl) coinEl.textContent = String(currency.Coin);

      const shadowEl = document.querySelector(".currency-item.shadow .currency-value");
      if (shadowEl) shadowEl.textContent = String(currency.Shadow);
    }
  }

  private _wireHomeMenu(): void {
    const homeBtn = document.getElementById("home-button");
    const homeMenu = document.getElementById("home-menu");
    const screenBg = document.getElementById("home-screen-background");

    if (!homeBtn || !homeMenu || !screenBg) return;

    homeBtn.addEventListener("click", () => {
      homeMenu.classList.add("open");
    });

    screenBg.addEventListener("click", () => {
      homeMenu.classList.remove("open");
      this._deselectAllMenuButtons();
    });

    const menuBtns = homeMenu.querySelectorAll<HTMLElement>(".menu-btn");
    for (const btn of menuBtns) {
      btn.addEventListener("click", (e) => {
        e.stopPropagation();
        const menu = btn.dataset.menu;
        if (menu) {
          console.log(`[EnterPointScene] OpenMenu("${menu}")`);
        }
        homeMenu.classList.remove("open");
        this._deselectAllMenuButtons();
      });

      btn.addEventListener("mouseenter", () => {
        this._deselectAllMenuButtons();
        btn.classList.add("selected");
      });

      btn.addEventListener("mouseleave", () => {
        btn.classList.remove("selected");
      });
    }
  }

  private _deselectAllMenuButtons(): void {
    const selected = document.querySelectorAll(".menu-btn.selected");
    for (const el of selected) {
      el.classList.remove("selected");
    }
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

  unmount(): void {
    this._rootEl?.remove();
    this._rootEl = null;
  }
}
