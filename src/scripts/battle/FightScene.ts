import { Scene, SceneLoader, AbstractMesh, Vector3, Color3, Color4, DirectionalLight, HemisphericLight, ShadowGenerator, FreeCamera, TransformNode } from "@babylonjs/core";
import "@babylonjs/loaders";
import { BattleController } from "../BattleController";
import { BattleCamera } from "../BattleCamera";
import { EffectsManager } from "../EffectsManager";
import { FightController, IFightInfo } from "../FightController";
import { CameraConfiguration } from "../CameraConfiguration";
import { ModelsManager } from "../SF3/ModelsManager";

export class FightScene {
  private readonly _scene: Scene;
  private _locationMeshes: AbstractMesh[] = [];
  private _locationName = "";
  private _battleController!: BattleController;
  private _fightController!: FightController;
  private _battleCamera!: BattleCamera;
  private _effectsManager!: EffectsManager;
  private _cameraConfig!: CameraConfiguration;
  private _cameraNode!: TransformNode;
  private _mainCamera!: FreeCamera;
  private _directionalLight!: DirectionalLight;
  private _hemisphericLight!: HemisphericLight;
  private _shadowGenerator!: ShadowGenerator;

  constructor(scene: Scene) {
    this._scene = scene;
  }

  async initialize(locationName: string, fightInfo: IFightInfo): Promise<void> {
    this._locationName = locationName;

    this._setupScene();

    await this._loadLocation(locationName);

    const modelsManager = new ModelsManager(this._scene);
    await modelsManager.loadModels();
    await Promise.all([
      modelsManager.spawnPlayer(),
      modelsManager.spawnEnemy(),
    ]);

    this._cameraConfig = this._createDefaultCameraConfig();
    this._battleCamera = BattleCamera.createInstance(
      this._cameraNode,
      this._mainCamera,
      this._cameraConfig,
      this._scene
    );
    this._battleController = BattleController.createInstance(this._scene);
    this._fightController = new FightController();
    this._effectsManager = EffectsManager.createInstance(this._scene);

    this._battleController.initialize();
    this._battleCamera.initialize(new Vector3(0, 0, 0));
    this._effectsManager.initialize();

    await this._battleController.initBattle(fightInfo);
  }

  private _setupScene(): void {
    this._scene.clearColor = new Color4(0.32, 0.32, 0.32, 0.02);
    this._scene.ambientColor = new Color3(0.6, 0.6, 0.6);

    const hemi = new HemisphericLight("hemi", new Vector3(0, 1, 0.5), this._scene);
    hemi.intensity = 0.7;
    this._hemisphericLight = hemi;

    const dir = new DirectionalLight("dir", new Vector3(-0.5, -1, -0.3), this._scene);
    dir.position = new Vector3(300, 500, 300);
    dir.intensity = 1.2;
    this._directionalLight = dir;

    const sg = new ShadowGenerator(2048, dir);
    sg.useBlurExponentialShadowMap = true;
    sg.blurKernel = 32;
    this._shadowGenerator = sg;

    const parentNode = new TransformNode("battle_camera", this._scene);
    this._cameraNode = parentNode;

    const cam = new FreeCamera("Main Camera", new Vector3(0, 98.5, -562), this._scene);
    cam.parent = parentNode;
    cam.setTarget(new Vector3(0, 98.5, 0));
    cam.minZ = 10;
    cam.maxZ = 5000;
    cam.fov = 30 * Math.PI / 180;
    cam.mode = FreeCamera.PERSPECTIVE_CAMERA;
    this._mainCamera = cam;
    this._scene.activeCamera = cam;

    this._scene.fogEnabled = false;
  }

  private _createDefaultCameraConfig(): CameraConfiguration {
    return CameraConfiguration.fromJSON({
      settings: [
        {
          aspect: 1,
          minXPosition: -9999,
          maxXPosition: 9999,
          minYPosition: 0,
          maxYPosition: 400,
          startFollowYUp: 150,
          startFollowYBot: 50,
          startRotateYUp: 180,
          startRotateYBot: 20,
          farClipPlane: 5000,
          botVerticalAngle: 10,
          upVerticalAngle: 30,
          leftHorizontalAngle: 30,
          rightHorizontalAngle: 30,
          camZOffset: 0,
        },
      ],
    });
  }

  private async _loadLocation(locationName: string): Promise<void> {
    try {
      const result = await SceneLoader.ImportMeshAsync("", "assets/locations/", `${locationName}.glb`, this._scene);
      this._locationMeshes = result.meshes;
      for (const mesh of this._locationMeshes) {
        // Hide the Unity shadow receiver plane — it's a Unity-only shadow catcher
        // with no texture, shows up as white/grey in BabylonJS
        if (mesh.name === "ShadowReciever" || mesh.name === "ShadowReceiver") {
          mesh.isVisible = false;
          continue;
        }
        // Location meshes receive shadows but shouldn't be casters themselves
        mesh.receiveShadows = true;
      }
    } catch (err) {
      console.warn(`[FightScene] Could not load location "${locationName}": ${err}`);
    }
  }

  dispose(): void {
    for (const mesh of this._locationMeshes) {
      mesh.dispose();
    }
    this._locationMeshes = [];
  }
}
