/**
 * FightScene.ts
 * Mirror of fight.unity – bootstraps the BabylonJS scene from fight.scene.json.
 *
 * Unity hierarchy:
 *   ColorManager          → ColorManager script
 *   EffectsManager        → EffectsManager script
 *   BattleCamera          → BattleCamera script
 *     └─ Main Camera      → perspective camera + CameraConfiguration + misc scripts
 *   battle_controller     → BattleController script
 *   models_container      → ModelsContainer node (models loaded at runtime)
 *   game_cheats_controller→ (stub)
 */

import {
  Engine,
  Scene,
  Color3,
  Color4,
  Vector3,
  TransformNode,
  FreeCamera,
  HemisphericLight,
} from "@babylonjs/core";
import "@babylonjs/loaders";

import { ColorManager } from "./ColorManager";
import { EffectsManager } from "./EffectsManager";
import { BattleCamera } from "./BattleCamera";
import { BattleController } from "./BattleController";
import { SceneManager } from "./SceneManager";
import { CameraConfiguration } from "./CameraConfiguration";

import fightSceneData from "../assets/scenes/fight.scene.json";

export class FightScene {
  readonly engine: Engine;
  readonly scene: Scene;

  private _sceneManager: SceneManager;

  constructor(canvas: HTMLCanvasElement) {
    this.engine = new Engine(canvas, true, { preserveDrawingBuffer: true });
    this.scene = new Scene(this.engine);

    this._applyRenderSettings();
    this._buildHierarchy(canvas);

    this._sceneManager = SceneManager.instance;

    this.engine.runRenderLoop(() => {
      this.scene.render();
    });

    window.addEventListener("resize", () => this.engine.resize());
  }

  // ─────────────────────────────────────────────────────────────────────────
  //  Private helpers
  // ─────────────────────────────────────────────────────────────────────────

  /** Applies renderSettings + lightmapSettings from fight.scene.json. */
  private _applyRenderSettings(): void {
    const rs = (fightSceneData as any).renderSettings;

    // Background / clear colour (Main Camera backgroundColor)
    this.scene.clearColor = new Color4(0.324, 0.324, 0.324, 0.02);

    // Fog – Unity fogMode 1 = Linear → BabylonJS FOGMODE_LINEAR = 3
    if (rs.fog) {
      this.scene.fogMode = Scene.FOGMODE_LINEAR;
      this.scene.fogStart = rs.linearFogStart ?? 180;
      this.scene.fogEnd = rs.linearFogEnd ?? 2000;
      this.scene.fogColor = new Color3(
        rs.fogColor.r,
        rs.fogColor.g,
        rs.fogColor.b
      );
    }

    // Ambient light – Unity ambientMode 3 = flat colour from ambientSkyColor
    // Represented as a HemisphericLight with intensity and no diffuse variation.
    const ambient = new HemisphericLight(
      "ambientLight",
      new Vector3(0, 1, 0),
      this.scene
    );
    ambient.intensity = rs.ambientIntensity ?? 1;
    ambient.diffuse = new Color3(
      rs.ambientSkyColor.r,
      rs.ambientSkyColor.g,
      rs.ambientSkyColor.b
    );
    ambient.specular = Color3.Black();
    ambient.groundColor = new Color3(
      rs.ambientSkyColor.r,
      rs.ambientSkyColor.g,
      rs.ambientSkyColor.b
    );
  }

  /**
   * Builds the Unity scene hierarchy as BabylonJS TransformNodes and
   * attaches script-component singletons to each node.
   */
  private _buildHierarchy(canvas: HTMLCanvasElement): void {
    const hierarchyData = (fightSceneData as any).hierarchy as any[];

    for (const obj of hierarchyData) {
      const node = this._createNode(obj, null);
      this._attachComponents(obj, node, canvas);
    }
  }

  /** Recursively mirrors a Unity hierarchy entry as a TransformNode. */
  private _createNode(
    obj: any,
    parent: TransformNode | null
  ): TransformNode {
    const node = new TransformNode(obj.name, this.scene);
    if (parent) node.parent = parent;

    const t = obj.transform;
    node.position = new Vector3(t.position.x, t.position.y, t.position.z);
    node.scaling = new Vector3(t.scale.x, t.scale.y, t.scale.z);

    if (obj.children) {
      for (const child of obj.children) {
        this._createNode(child, node);
      }
    }

    return node;
  }

  /**
   * Attaches script-component instances to their matching scene nodes.
   * All singletons are initialised here so they can reference each other
   * once the full hierarchy exists (mirrors Unity Awake() order).
   */
  private _attachComponents(
    obj: any,
    node: TransformNode,
    canvas: HTMLCanvasElement
  ): void {
    switch (obj.name) {
      case "ColorManager":
        ColorManager.createInstance();
        break;

      case "EffectsManager":
        EffectsManager.createInstance(this.scene);
        break;

      case "BattleCamera": {
        // Find the "Main Camera" child node that was already created.
        const camNode = this.scene.getTransformNodeByName("Main Camera");

        // Build BabylonJS camera on that node.
        const camData = obj.children?.[0];
        const camComp = camData?.components?.find(
          (c: any) => c.type === "camera"
        );
        const fov = 30; // set in BattleCamera.FOV field

        const bCam = new FreeCamera(
          "MainCamera",
          new Vector3(0, 155, -950), // BattleCamera._defaultPosition
          this.scene
        );
        bCam.fov = (fov * Math.PI) / 180;
        bCam.minZ = camComp?.data?.nearClip ?? 10;
        bCam.maxZ = camComp?.data?.farClip ?? 5000;
        // Look towards fight floor
        bCam.setTarget(Vector3.Zero());

        const aspect = canvas.width / canvas.height;
        const config = CameraConfiguration.fromJSON({ settings: [] });
        BattleCamera.createInstance(node, bCam, config, this.scene);
        break;
      }

      case "battle_controller":
        BattleController.createInstance(this.scene);
        break;

      case "models_container":
        // Kept as a plain TransformNode; models are loaded at round-start.
        node.metadata = { role: "models_container" };
        break;

      case "game_cheats_controller":
        // Stub – no-op in production.
        break;
    }
  }

  /** Convenience: load a fight for the given battle / location name. */
  loadFight(locationName: string, onReady?: () => void): void {
    SceneManager.instance.loadLocationScene(locationName, onReady);
  }
}
