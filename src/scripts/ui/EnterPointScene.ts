import { Vector3 } from "@babylonjs/core";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { SceneManager, SceneConfig, SceneNode } from "../core/SceneManager";
import { UserDataController } from "../SF3/UserData/UserDataController";

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

export class EnterPointScene {
  private _mgr: SceneManager;
  private _config: SceneConfig;
  private _el: HTMLDivElement | null = null;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr = mgr;
    this._config = config;
  }

  mount(): void {
    console.log("[EnterPointScene] mounting UI");

    if (this._config.hierarchy) {
      for (const node of this._config.hierarchy) {
        if (!node.isActive) continue;
        if (node.components?.some((c: { type: string }) => c.type === "camera")) {
          createBabylonCamera(this._mgr, node);
        }
      }
    }

    if (!this._mgr.scene.activeCamera) {
      new FreeCamera("fallback", new Vector3(0, 0, -10), this._mgr.scene);
    }

    const el = document.getElementById("enter-point") as HTMLDivElement | null;
    if (!el) return;
    el.style.display = "block";
    el.classList.add("active");
    this._el = el;

    const player = UserDataController.player;
    const nameEl = document.getElementById("ep-player-name");
    const lvlEl = document.getElementById("ep-player-level");
    if (nameEl) nameEl.textContent = player?.Name ?? "Player";
    if (lvlEl) lvlEl.textContent = player ? `Lv.${player.Level}` : "";

    const verEl = document.getElementById("ep-version");
    if (verEl) verEl.textContent = "BETA VERSION (CBT#1f2)";
  }

  unmount(): void {
    if (this._el) {
      this._el.style.display = "none";
      this._el.classList.remove("active");
    }
    this._el = null;
  }
}
