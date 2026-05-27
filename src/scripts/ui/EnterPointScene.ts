import { Vector3 } from "@babylonjs/core";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { SceneManager, SceneConfig, SceneNode } from "../core/SceneManager";

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
  private _rootEl: HTMLDivElement | null = null;

  constructor(mgr: SceneManager, config: SceneConfig) {
    this._mgr = mgr;
    this._config = config;
  }

  mount(): void {
    console.log("[EnterPointScene] mounting hierarchy:", 
      this._config.hierarchy?.map(h => h.name));

    if (this._config.hierarchy) {
      for (const node of this._config.hierarchy) {
        if (!node.isActive) continue;
        if (node.components?.some((c: { type: string }) => c.type === "camera")) {
          createBabylonCamera(this._mgr, node);
        }
        this._mountRecursive(node);
      }
    }

    const root = document.createElement("div");
    root.id = "enter-point-ui";
    Object.assign(root.style, {
      position: "fixed", inset: "0", zIndex: "100",
      pointerEvents: "none",
    });
    document.body.appendChild(root);
    this._rootEl = root;

    if (!this._mgr.scene.activeCamera) {
      const fallbackCam = new FreeCamera("fallback", new Vector3(0, 0, -10), this._mgr.scene);
      this._mgr.scene.activeCamera = fallbackCam;
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
