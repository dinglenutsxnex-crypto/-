import { Scene, SceneLoader, AbstractMesh, Vector3, Color3, Color4, DirectionalLight, HemisphericLight, ShadowGenerator, FreeCamera, TransformNode } from "@babylonjs/core";
import "@babylonjs/loaders";
import { BattleController } from "./BattleController";
import { BattleCamera, ICameraModel } from "./BattleCamera";
import { EffectsManager } from "./EffectsManager";
import { IFightInfo } from "./FightController";
import { CameraConfiguration } from "./CameraConfiguration";
import { ModelsManager } from "./SF3/ModelsManager";
import { FightHUD } from "./ui/FightHUD";

// Fallback spawn positions.
// The GLTF loader converts right-hand→left-hand by negating Z, which (combined
// with the 270° Y rotation baked into the dojo root) makes the arena load
// facing backwards from the camera.  We compensate by flipping the location
// mesh root on X (see _loadLocation) and swapping the spawn sides so characters
// end up on the same sides as in Unity: player LEFT (+X screen-right is X+
// world, but after the root X-flip the visual is corrected → player appears
// LEFT, enemy appears RIGHT).
const FALLBACK_SPAWN_PLAYER = new Vector3( 250, 0, 0);
const FALLBACK_SPAWN_ENEMY  = new Vector3(-250, 0, 0);
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

    // ── Tear down the previous lighting/camera rig ──────────────────────────
    // _setupScene() always creates NEW HemisphericLight/DirectionalLight/
    // ShadowGenerator/camera instances. Without disposing the old ones here,
    // re-entering the dojo (or any location reload) stacks a second set of
    // lights on top of the first — same geometry, but double ambient +
    // double directional intensity, which reads as flatter/grayer/lower
    // contrast and noisier shadows on the second run.
    this._shadowGenerator?.dispose();
    this._hemisphericLight?.dispose();
    this._directionalLight?.dispose();
    this._mainCamera?.dispose();
    this._cameraNode?.dispose();
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

    const cam = new FreeCamera("Main Camera", new Vector3(0, 190, -1050), this._scene);
    cam.parent  = cameraRig;
    cam.setTarget(new Vector3(0, 160, 0));
    cam.minZ = 10;
    cam.maxZ = 5000;
    cam.fov  = 26 * (Math.PI / 180);
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
            // Negate X: Unity→GLTF→BabylonJS preserves X, but the GLTF loader's
            // Z-flip + the dojo root's baked Y-rotation effectively inverts the
            // visual X axis — so the player spawn (left in Unity) arrives as
            // right in BabylonJS and needs to be mirrored back.
            pos.x *= -1;
            this._spawnPlayer.copyFrom(pos);
            console.log("[SceneInitializer] SpawnPointA from GLB:", pos.toString());
          }
          node.setEnabled(false);
        } else if (nm === "SpawnPointB" || nm === "SpawnPoint_Enemy" || nm === "spawn_point_b") {
          const pos = node.getAbsolutePosition();
          if (pos.x !== 0 || pos.y !== 0 || pos.z !== 0) {
            pos.x *= -1;
            this._spawnEnemy.copyFrom(pos);
            console.log("[SceneInitializer] SpawnPointB from GLB:", pos.toString());
          }
          node.setEnabled(false);
        }
      }

      // ── Correct GLTF→BabylonJS X-mirror ───────────────────────────────────
      // The loader negates Z to convert right-hand→left-hand.  The dojo root's
      // baked 270° Y rotation means the arena ends up X-flipped from what the
      // Unity camera sees.  Negate X on the GLB root to restore the correct
      // orientation.  After negating X the mesh normals are reversed, so
      // disable backface culling so arena walls remain visible from inside.
      const locationRoot =
        result.transformNodes.find((n: TransformNode) => n.name === "__root__") ??
        result.transformNodes.filter((n: TransformNode) => !n.parent)[0];
      if (locationRoot) {
        locationRoot.scaling.x *= -1;
      }

      for (const mesh of this._locationMeshes) {
        if (mesh.name === "ShadowReciever" || mesh.name === "ShadowReceiver") {
          mesh.isVisible = false;
          continue;
        }
        mesh.receiveShadows = true;
        if (mesh.material) mesh.material.backFaceCulling = false;
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
