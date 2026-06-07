import { Scene, SceneLoader, AbstractMesh, Vector3, Color3, Color4, DirectionalLight, HemisphericLight, ShadowGenerator, FreeCamera, TransformNode } from "@babylonjs/core";
import "@babylonjs/loaders";
import { BattleController } from "./BattleController";
import { BattleCamera, ICameraModel } from "./BattleCamera";
import { EffectsManager } from "./EffectsManager";
import { IFightInfo } from "./FightController";
import { CameraConfiguration } from "./CameraConfiguration";
import { ModelsManager } from "./SF3/ModelsManager";
import { FightHUD } from "./ui/FightHUD";

// Fallback spawn positions (Unity dojo_Legion: ~2.5 Unity units = 250 Babylon units apart)
const FALLBACK_SPAWN_PLAYER = new Vector3(-250, 0, 0);
const FALLBACK_SPAWN_ENEMY  = new Vector3( 250, 0, 0);
const SPAWN_POINT           = new Vector3(   0, 0, 0);

class ModelTracker implements ICameraModel {
  private readonly _root: TransformNode;
  constructor(root: TransformNode) { this._root = root; }
  get modelBackPosX(): number { return this._root.position.x; }
  get centerOfMassY(): number { return this._root.position.y + 90; }
  get centerOfMassPosition(): Vector3 {
    return new Vector3(this._root.position.x, this.centerOfMassY, this._root.position.z);
  }
}

export class SceneInitializer {
  private _locationMeshes: AbstractMesh[] = [];
  private readonly _scene: Scene;

  private _directionalLight!: DirectionalLight;
  private _hemisphericLight!: HemisphericLight;
  private _shadowGenerator!: ShadowGenerator;
  private _cameraNode!: TransformNode;
  private _mainCamera!: FreeCamera;

  // Spawn positions — read from GLB nodes, or fallback to defaults
  private _spawnPlayer = FALLBACK_SPAWN_PLAYER.clone();
  private _spawnEnemy  = FALLBACK_SPAWN_ENEMY.clone();

  constructor(scene: Scene) { this._scene = scene; }

  /**
   * @param hud  Optional FightHUD to wire into the fight stage machine.
   *             For the Dojo this should be provided so HP bars and the timer
   *             activate as soon as RoundFightStart fires.
   */
  async initializeNewLocationScene(
    locationName: string,
    fightInfo: IFightInfo,
    hud?: FightHUD,
    onReady?: () => void,
  ): Promise<void> {
    this._disposePrevious();

    this._setupScene();
    await this._loadLocation(locationName);

    new ModelsManager(this._scene);
    await ModelsManager.instance.Initialize();
    this._positionModels();

    this._initCamera();
    this._wireCameraTrackers();

    const bc = BattleController.createInstance(this._scene);
    EffectsManager.createInstance(this._scene);
    bc.initialize();

    await bc.initBattle(fightInfo, hud);

    onReady?.();
  }

  dispose(): void {
    this._disposePrevious();
    for (const mesh of this._locationMeshes) mesh.dispose();
    this._locationMeshes = [];
  }

  private _disposePrevious(): void {
    if (this._locationMeshes.length === 0) return;
    BattleController.instance?.disposePreviousLocation();
    BattleCamera.instance?.disposePreviousLocation();
    EffectsManager.instance?.disposePreviousLocation();
    for (const mesh of this._locationMeshes) mesh.dispose();
    this._locationMeshes = [];
  }

  private _setupScene(): void {
    this._scene.clearColor   = new Color4(0.22, 0.22, 0.25, 1);
    this._scene.ambientColor = new Color3(0.55, 0.55, 0.55);

    const hemi = new HemisphericLight("hemi", new Vector3(0, 1, 0.3), this._scene);
    hemi.intensity = 0.65;
    this._hemisphericLight = hemi;

    const dir = new DirectionalLight("dir", new Vector3(-0.5, -1, -0.3), this._scene);
    dir.position  = new Vector3(300, 500, 300);
    dir.intensity = 1.2;
    this._directionalLight = dir;

    const sg = new ShadowGenerator(2048, dir);
    sg.useBlurExponentialShadowMap = true;
    sg.blurKernel = 32;
    this._shadowGenerator = sg;

    const cameraRig = new TransformNode("battle_camera_rig", this._scene);
    this._cameraNode = cameraRig;

    const cam = new FreeCamera("Main Camera", new Vector3(0, 155, -950), this._scene);
    cam.parent  = cameraRig;
    cam.setTarget(new Vector3(0, 155, 0));
    cam.minZ = 10;
    cam.maxZ = 5000;
    cam.fov  = 30 * (Math.PI / 180);
    cam.mode = FreeCamera.PERSPECTIVE_CAMERA;
    this._mainCamera      = cam;
    this._scene.activeCamera = cam;
    this._scene.fogEnabled = false;
  }

  private async _loadLocation(locationName: string): Promise<void> {
    // Reset to fallbacks each load
    this._spawnPlayer = FALLBACK_SPAWN_PLAYER.clone();
    this._spawnEnemy  = FALLBACK_SPAWN_ENEMY.clone();

    try {
      const result = await SceneLoader.ImportMeshAsync(
        "", "assets/locations/", `${locationName}.glb`, this._scene,
      );
      this._locationMeshes = result.meshes;

      // Read spawn positions from named nodes baked into the GLB by Unity export
      // Unity exports SpawnPointA (player, left side) and SpawnPointB (enemy, right side)
      for (const node of result.transformNodes) {
        const nm = node.name;
        if (nm === "SpawnPointA" || nm === "SpawnPoint_Player" || nm === "spawn_point_a") {
          const pos = node.getAbsolutePosition();
          if (pos.x !== 0 || pos.y !== 0 || pos.z !== 0) {
            this._spawnPlayer.copyFrom(pos);
            console.log("[SceneInitializer] SpawnPointA from GLB:", pos.toString());
          }
          node.setEnabled(false);
        } else if (nm === "SpawnPointB" || nm === "SpawnPoint_Enemy" || nm === "spawn_point_b") {
          const pos = node.getAbsolutePosition();
          if (pos.x !== 0 || pos.y !== 0 || pos.z !== 0) {
            this._spawnEnemy.copyFrom(pos);
            console.log("[SceneInitializer] SpawnPointB from GLB:", pos.toString());
          }
          node.setEnabled(false);
        }
      }

      for (const mesh of this._locationMeshes) {
        if (mesh.name === "ShadowReciever" || mesh.name === "ShadowReceiver") {
          mesh.isVisible = false;
          continue;
        }
        mesh.receiveShadows = true;
      }
    } catch (err) {
      console.warn(`[SceneInitializer] Could not load location "${locationName}": ${err}`);
    }

    console.log("[SceneInitializer] Final spawn — Player:", this._spawnPlayer.toString(), "Enemy:", this._spawnEnemy.toString());
  }

  private _positionModels(): void {
    const player = ModelsManager.instance.player;
    const enemy  = ModelsManager.instance.enemy;
    if (player?.root) {
      player.root.position.copyFrom(this._spawnPlayer);
      // Mirror X so the player (on the left) faces right toward the enemy
      player.root.scaling = new Vector3(-1, 1, 1);
    }
    if (enemy?.root) {
      enemy.root.position.copyFrom(this._spawnEnemy);
      // Enemy naturally faces left (toward the player) — no mirror needed
    }
  }

  private _initCamera(): void {
    // camZOffset: 465 matches dojo_Legion.prefab CamZOffset property
    const config = CameraConfiguration.fromJSON({
      settings: [{
        aspect: 1, minXPosition: -9999, maxXPosition: 9999,
        minYPosition: 0, maxYPosition: 400,
        startFollowYUp: 150, startFollowYBot: 50,
        startRotateYUp: 180, startRotateYBot: 20,
        farClipPlane: 5000, botVerticalAngle: 10, upVerticalAngle: 30,
        leftHorizontalAngle: 30, rightHorizontalAngle: 30, camZOffset: 465,
      }],
    });
    const bCam = BattleCamera.createInstance(
      this._cameraNode, this._mainCamera, config, this._scene,
    );
    bCam.initialize(SPAWN_POINT);
    bCam.activateBattleCamera(true);
  }

  private _wireCameraTrackers(): void {
    const player = ModelsManager.instance.player;
    const enemy  = ModelsManager.instance.enemy;
    if (!player?.root || !enemy?.root) return;
    BattleCamera.setModels(new ModelTracker(player.root), new ModelTracker(enemy.root));
  }
}
