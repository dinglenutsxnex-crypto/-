import { Engine, Scene, Color3, Color4, Vector3, AbstractMesh } from "@babylonjs/core";
import "@babylonjs/loaders";

export interface SceneNode {
  name: string;
  isActive?: number;
  layer?: number;
  tag?: string;
  transform?: { position: { x: number; y: number; z?: number }; rotation?: any; scale?: any };
  components?: Array<{ type: string; data?: any }>;
  children?: SceneNode[];
  [key: string]: any;
}

export interface SceneConfig {
  renderSettings?: {
    fog: number;
    fogColor: { r: number; g: number; b: number; a: number };
    fogMode: number;
    fogDensity: number;
    linearFogStart: number;
    linearFogEnd: number;
    ambientSkyColor: { r: number; g: number; b: number; a: number };
    ambientIntensity: number;
    ambientMode: number;
  };
  hierarchy?: Array<{
    name: string;
    isActive?: number;
    children: any[];
    [key: string]: any;
  }>;
}

export class SceneManager {
  private _engine: Engine;
  private _scene: Scene;
  private _updates: ((dt: number) => void)[] = [];
  private _loadedConfig?: SceneConfig;

  get engine(): Engine { return this._engine; }
  get scene(): Scene { return this._scene; }
  get config(): SceneConfig | undefined { return this._loadedConfig; }

  constructor(canvas: HTMLCanvasElement) {
    // adaptToDeviceRatio keeps the render resolution matched to the
    // device's actual pixel density instead of being locked to whatever
    // ratio was in effect when the engine was created (fixes blurry /
    // mismatched framebuffers when a WebView reports its ratio late).
    this._engine = new Engine(canvas, true, {
      preserveDrawingBuffer: false,
      stencil: true,
      adaptToDeviceRatio: true,
    });
    this._scene = new Scene(this._engine);
  }

  async loadSceneJSON(path: string): Promise<SceneConfig> {
    const r = await fetch(path);
    this._loadedConfig = await r.json();
    return this._loadedConfig!;
  }

  applyConfig(config: SceneConfig): void {
    const rs = config.renderSettings;
    if (!rs) return;

    if (rs.fog !== 0) {
      this._scene.fogEnabled = true;
      this._scene.fogColor = new Color3(rs.fogColor.r, rs.fogColor.g, rs.fogColor.b);
      this._scene.fogDensity = rs.fogDensity;
      this._scene.fogStart = rs.linearFogStart;
      this._scene.fogEnd = rs.linearFogEnd;
      this._scene.fogMode = Scene.FOGMODE_EXP2;
    }

    this._scene.ambientColor = new Color3(
      rs.ambientSkyColor.r, rs.ambientSkyColor.g, rs.ambientSkyColor.b,
    );
    this._scene.clearColor = new Color4(0.3235, 0.3235, 0.3235, 0.02);
  }

  onUpdate(cb: (dt: number) => void): void {
    this._updates.push(cb);
  }

  start(): void {
    this._engine.runRenderLoop(() => {
      const dt = this._engine.getDeltaTime() / 1000;
      for (const cb of this._updates) cb(dt);
      this._scene.render();
    });

    // Multiple triggers because different browsers/WebViews fire different
    // events when the visible area changes (rotation, browser chrome
    // show/hide, split-screen, fullscreen toggles). Any one of these missing
    // is what causes the canvas to get stuck at a stale size and appear
    // cropped or overflowing until the next full resize.
    const doResize = () => this._engine.resize();
    window.addEventListener("resize", doResize);
    window.addEventListener("orientationchange", doResize);
    if (window.visualViewport) {
      window.visualViewport.addEventListener("resize", doResize);
    }
    const renderCanvas = this._engine.getRenderingCanvas();
    if (typeof ResizeObserver !== "undefined" && renderCanvas) {
      new ResizeObserver(doResize).observe(renderCanvas as unknown as Element);
    }
  }
}
