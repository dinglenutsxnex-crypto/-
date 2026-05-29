/**
 * FightScene.ts
 *
 * Bootstraps a full fight: location load, character spawn, camera, controllers.
 *
 * Unity mirror path:
 *   SceneInitializer.InitializeNewLocationScene()
 *     → ModelsManager.Initialize()  (spawnPlayer + spawnEnemy)
 *     → BattleCamera.Initialize(SpawnPointPlayer)
 *     → BattleController.InitBattle()
 *
 * Key differences from Unity:
 *  - No server; player info comes from user_default.json via UserDataController.
 *  - Enemy always comes from TrainingEnemyConfig.
 *  - Camera trackers are lightweight objects reading the model root position,
 *    mirroring Unity's modelCollisions.modelBackPos / centerOfMassBone.position.y
 */

import {
  Scene, SceneLoader, AbstractMesh,
  Vector3, Color3, Color4,
  DirectionalLight, HemisphericLight, ShadowGenerator,
  FreeCamera, TransformNode,
} from "@babylonjs/core";
import "@babylonjs/loaders";

import { BattleController }              from "../BattleController";
import { BattleCamera, ICameraModel }    from "../BattleCamera";
import { EffectsManager }                from "../EffectsManager";
import { FightController, IFightInfo }   from "../FightController";
import { CameraConfiguration }           from "../CameraConfiguration";
import { ModelsManager, IPlayerModel }   from "../SF3/ModelsManager";
import { UserDataController }            from "../SF3/UserData/UserDataController";
import { buildTrainingEnemyModelInfo }   from "../SF3/TrainingEnemyConfig";
import { ModelInfo }                     from "../SF3/GameModels/ModelInfo";

// ── Spawn positions (mirrors Unity SceneConfig.SpawnPointPlayer/Enemy) ────────
// Player faces right (mirrored X), enemy faces left. Dojo standard.
const SPAWN_PLAYER = new Vector3(-250, 0, 0);
const SPAWN_ENEMY  = new Vector3( 250, 0, 0);
const SPAWN_POINT  = new Vector3(   0, 0, 0);  // camera reference point

// ── Camera tracker adaptor ───────────────────────────────────────────────────
// Unity used modelCollisions.modelBackPos and centerOfMassBone.position.y.
// We derive equivalent values from the model root transform node.

class ModelTracker implements ICameraModel {
  private readonly _root: TransformNode;

  constructor(root: TransformNode) {
    this._root = root;
  }

  /** Horizontal "back" edge position — we use root X as a proxy. */
  get modelBackPosX(): number { return this._root.position.x; }

  /**
   * Vertical centre of mass — Unity used a dedicated bone; we approximate
   * by taking root.y + a fixed mid-body offset (roughly hip height).
   */
  get centerOfMassY(): number { return this._root.position.y + 90; }

  get centerOfMassPosition(): Vector3 {
    return new Vector3(this._root.position.x, this.centerOfMassY, this._root.position.z);
  }
}

// ── FightScene ────────────────────────────────────────────────────────────────

export class FightScene {
  private readonly _scene: Scene;
  private _locationMeshes: AbstractMesh[] = [];

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

  private _modelsManager!: ModelsManager;

  constructor(scene: Scene) {
    this._scene = scene;
  }

  // ── Main entry point ─────────────────────────────────────────────────────

  async initialize(locationName: string, fightInfo: IFightInfo): Promise<void> {
    // 1. Lighting, camera rig
    this._setupScene();

    // 2. Load location mesh
    await this._loadLocation(locationName);

    // 3. Determine what to spawn
    //    Unity: ModelsManager.Initialize → FightFiller → RoundController.InitNewRound
    //    We: always read player from user_default.json, enemy from TrainingEnemyConfig.
    this._modelsManager = new ModelsManager(this._scene);
    await this._modelsManager.loadModels();

    const playerInfo: ModelInfo = UserDataController.isReady
      ? UserDataController.getPlayerModelInfo()
      : ModelInfo.createPlayer();

    const enemyInfo: ModelInfo = buildTrainingEnemyModelInfo();

    // 4. Spawn characters at fight positions (mirrors SceneConfig.SpawnPoint*)
    await this._modelsManager.createBattleModels(playerInfo, enemyInfo);
    this._positionModels();

    // 5. Camera — mirrors BattleCamera.Initialize(SpawnPointPlayer)
    this._cameraConfig  = this._createDefaultCameraConfig();
    this._battleCamera  = BattleCamera.createInstance(
      this._cameraNode, this._mainCamera, this._cameraConfig, this._scene,
    );
    this._battleCamera.initialize(SPAWN_POINT);

    // Wire model trackers so the camera can follow both fighters
    this._wireCameraTrackers();

    // Activate battle mode (mirrors DojoRound → InitBattleCamera)
    this._battleCamera.activateBattleCamera(true);

    // 6. Fight controllers
    this._battleController = BattleController.createInstance(this._scene);
    this._fightController  = new FightController();
    this._effectsManager   = EffectsManager.createInstance(this._scene);

    this._battleController.initialize();
    this._effectsManager.initialize();

    await this._battleController.initBattle(fightInfo);

    console.log("[FightScene] initialized ✔");
  }

  // ── Private helpers ───────────────────────────────────────────────────────

  /**
   * Set spawn positions for both models after assembly.
   * Mirrors Unity SceneConfig.SpawnPointPlayer / SpawnPointEnemy.
   * Player is mirrored on X so it faces right; enemy faces left naturally.
   */
  private _positionModels(): void {
    const player = this._modelsManager.player;
    const enemy  = this._modelsManager.enemy;

    if (player?.root) {
      player.root.position.copyFrom(SPAWN_PLAYER);
      // Mirror X so the player faces towards the enemy (Unity: scale.x = -1)
      player.root.scaling = new Vector3(-1, 1, 1);
    }
    if (enemy?.root) {
      enemy.root.position.copyFrom(SPAWN_ENEMY);
    }

    console.log(
      `[FightScene] player @ ${SPAWN_PLAYER.toString()}  ` +
      `enemy @ ${SPAWN_ENEMY.toString()}`,
    );
  }

  /**
   * Create ICameraModel adaptors from the spawned models and give them to
   * BattleCamera.setModels() — mirrors BattleCamera.SetModels(player, enemy).
   */
  private _wireCameraTrackers(): void {
    const player = this._modelsManager.player;
    const enemy  = this._modelsManager.enemy;

    if (!player?.root || !enemy?.root) {
      console.warn("[FightScene] cannot wire camera — models not ready");
      return;
    }

    const pt = new ModelTracker(player.root);
    const et = new ModelTracker(enemy.root);

    BattleCamera.setModels(pt, et);
    console.log("[FightScene] camera trackers wired ✔");
  }

  private _setupScene(): void {
    this._scene.clearColor   = new Color4(0.22, 0.22, 0.25, 1);
    this._scene.ambientColor = new Color3(0.55, 0.55, 0.55);

    // Hemisphere fill — soft upward ambient (mirrors Unity's ambient light)
    const hemi = new HemisphericLight("hemi", new Vector3(0, 1, 0.3), this._scene);
    hemi.intensity = 0.65;
    this._hemisphericLight = hemi;

    // Directional key light + shadows
    const dir = new DirectionalLight("dir", new Vector3(-0.5, -1, -0.3), this._scene);
    dir.position  = new Vector3(300, 500, 300);
    dir.intensity = 1.2;
    this._directionalLight = dir;

    const sg = new ShadowGenerator(2048, dir);
    sg.useBlurExponentialShadowMap = true;
    sg.blurKernel = 32;
    this._shadowGenerator = sg;

    // Camera rig — parent node (TransformNode) + FreeCamera child
    // Mirrors Unity's BattleCamera MonoBehaviour attached to a GO with a Camera component.
    const cameraRig = new TransformNode("battle_camera_rig", this._scene);
    this._cameraNode = cameraRig;

    // Unity default: position (0, 155, -950), look at (0, 155, 0)
    const cam = new FreeCamera("Main Camera", new Vector3(0, 155, -950), this._scene);
    cam.parent  = cameraRig;
    cam.setTarget(new Vector3(0, 155, 0));
    cam.minZ = 10;
    cam.maxZ = 5000;
    // Unity FOV 30° — Babylon uses radians
    cam.fov  = 30 * (Math.PI / 180);
    cam.mode = FreeCamera.PERSPECTIVE_CAMERA;
    this._mainCamera      = cam;
    this._scene.activeCamera = cam;

    this._scene.fogEnabled = false;
  }

  private _createDefaultCameraConfig(): CameraConfiguration {
    // Mirrors CameraConfiguration values from Unity's inspector.
    return CameraConfiguration.fromJSON({
      settings: [
        {
          aspect:               1,
          minXPosition:        -9999,
          maxXPosition:         9999,
          minYPosition:         0,
          maxYPosition:         400,
          startFollowYUp:       150,
          startFollowYBot:      50,
          startRotateYUp:       180,
          startRotateYBot:      20,
          farClipPlane:         5000,
          botVerticalAngle:     10,
          upVerticalAngle:      30,
          leftHorizontalAngle:  30,
          rightHorizontalAngle: 30,
          camZOffset:           0,
        },
      ],
    });
  }

  private async _loadLocation(locationName: string): Promise<void> {
    try {
      const result = await SceneLoader.ImportMeshAsync(
        "", "assets/locations/", `${locationName}.glb`, this._scene,
      );
      this._locationMeshes = result.meshes;
      for (const mesh of this._locationMeshes) {
        if (mesh.name === "ShadowReciever" || mesh.name === "ShadowReceiver") {
          mesh.isVisible = false;
          continue;
        }
        mesh.receiveShadows = true;
      }
      console.log(`[FightScene] loaded location "${locationName}" (${result.meshes.length} meshes)`);
    } catch (err) {
      console.warn(`[FightScene] Could not load location "${locationName}": ${err}`);
    }
  }

  dispose(): void {
    for (const mesh of this._locationMeshes) mesh.dispose();
    this._locationMeshes = [];
  }
}
