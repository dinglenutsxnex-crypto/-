/**
 * BattleCamera.ts
 * Mirror of SF3/BattleCamera.cs
 *
 * Manages all camera states for the fight scene:
 *   BATTLE      – follows both fighters, zooms on distance
 *   INVENTORY   – centres on player
 *   MOVE_TO_POINT / MOVE_TO_OBJECT – animated move
 *   STAY        – frozen
 */

import {
  Scene,
  TransformNode,
  FreeCamera,
  Vector3,
  Quaternion,
} from "@babylonjs/core";
import { CameraConfiguration, CameraSettingsData, AspectRatio } from "./CameraConfiguration";

// ─── Helpers (mirrors of Unity Mathf utilities) ──────────────────────────────

function smoothDamp(
  current: number,
  target: number,
  velocity: { value: number },
  smoothTime: number,
  dt: number
): number {
  const omega = 2 / smoothTime;
  const x = omega * dt;
  const exp = 1 / (1 + x + 0.48 * x * x + 0.235 * x * x * x);
  const delta = current - target;
  const temp = (velocity.value + omega * delta) * dt;
  velocity.value = (velocity.value - omega * temp) * exp;
  return target + (delta + temp) * exp;
}

function smoothDampVector3(
  current: Vector3,
  target: Vector3,
  velocity: { x: number; y: number; z: number },
  smoothTime: number,
  dt: number
): Vector3 {
  const vx = { value: velocity.x };
  const vy = { value: velocity.y };
  const vz = { value: velocity.z };
  const result = new Vector3(
    smoothDamp(current.x, target.x, vx, smoothTime, dt),
    smoothDamp(current.y, target.y, vy, smoothTime, dt),
    smoothDamp(current.z, target.z, vz, smoothTime, dt)
  );
  velocity.x = vx.value;
  velocity.y = vy.value;
  velocity.z = vz.value;
  return result;
}

function smoothDampAngle(
  current: number,
  target: number,
  velocity: { value: number },
  smoothTime: number,
  dt: number
): number {
  let delta = ((target - current + 540) % 360) - 180;
  return current + smoothDamp(0, delta, velocity, smoothTime, dt);
}

// ─── Public types ─────────────────────────────────────────────────────────────

export enum ECameraState {
  BATTLE = 0,
  INVENTORY = 1,
  MOVE_TO_POINT = 2,
  MOVE_TO_OBJECT = 3,
  STAY = 4,
}

/** Minimal interface the camera needs from Model objects. */
export interface ICameraModel {
  modelBackPosX: number;
  centerOfMassY: number;
  centerOfMassPosition: Vector3;
}

// ─── BattleCamera ─────────────────────────────────────────────────────────────

export class BattleCamera {
  private static _instance: BattleCamera;
  static get instance(): BattleCamera { return BattleCamera._instance; }

  // Unity const
  static readonly DEF_CLIPPING_PLANE = 10;
  private static readonly DEF_MIN_DIST = 350;

  // Core BabylonJS refs
  readonly node: TransformNode;
  readonly camera: FreeCamera;
  private readonly _scene: Scene;

  // Camera settings
  moveSmoothTime = 0.1;
  rotationSmoothTime = 0.15;
  FOV = 30;
  centerMassK = 0.5;
  private _maxDistance = 500;
  currentMaxDistance = 500;
  private _minDist = 350;
  mapClippingPlane = 290;

  private _camOffset = new Vector3(0, -98.5, 562.6);
  private _defaultPosition = new Vector3(0, 155, -950);

  lookAtPlayerPos = true;
  lookAtPointOffset = Vector3.Zero();
  fixedOffset = Vector3.Zero();
  pauseOnMove = false;

  // State
  private _cameraState: ECameraState = ECameraState.STAY;
  private _player?: ICameraModel;
  private _enemy?: ICameraModel;

  private _calculatedPosition = Vector3.Zero();
  private _startCameraPosition = Vector3.Zero();
  private _startPosition?: Vector3;
  private _charsZ = 0;
  private _defOffsetFromChar = 0;
  private _dist = 0;
  private _zOffset = 0;
  private _middleX = 0;
  private _bordersActive = false;
  private _onlyPlayer = false;

  private _currVelocity = { x: 0, y: 0, z: 0 };
  private _xVelocity = { value: 0 };
  private _yVelocity = { value: 0 };
  private _zVelocity = { value: 0 };

  private _playerCenterMassY = 0;
  private _enemyCenterMassY = 0;

  private _blockCameraMovement = false;
  private _cameraMotion = false;

  // Vertical bounds (from CameraConfiguration)
  private _maxYPosition = 9999;
  private _minYPosition = 0;
  private _maxXPosition = 9999;
  private _minXPosition = -9999;
  private _startFollowYUp = 150;
  private _startFollowYBot = 50;
  private _upRotationBorder = 180;
  private _botRotationBorder = 20;
  private _leftHorizontalAngle = 30;
  private _rightHorizontalAngle = 30;
  private _botVerticalAngle = 10;
  private _upVerticalAngle = 30;

  // Move-to-target
  private _moveTimer = 0;
  private _moveTime = 0.5;
  private _startPos = Vector3.Zero();
  private _endPos = Vector3.Zero();
  private _startRot = Quaternion.Identity();
  private _endRotation = Quaternion.Identity();
  private _startMoveTime = 0;
  private _targetFollowNode?: TransformNode;
  private _onDone?: (() => void)[];

  // Callbacks
  private _onMoveDone?: () => void;

  constructor(
    node: TransformNode,
    camera: FreeCamera,
    config: CameraConfiguration,
    scene: Scene
  ) {
    BattleCamera._instance = this;
    this.node = node;
    this.camera = camera;
    this._scene = scene;
    this._readConfigData(config);

    scene.onBeforeRenderObservable.add(() => this._lateUpdate());
  }

  static createInstance(
    node: TransformNode,
    camera: FreeCamera,
    config: CameraConfiguration,
    scene: Scene
  ): BattleCamera {
    return new BattleCamera(node, camera, config, scene);
  }

  // ─── Initialize ─────────────────────────────────────────────────────────

  initialize(spawnPointPlayer: Vector3): void {
    this.fixedOffset = Vector3.Zero();
    this._cameraState = ECameraState.STAY;

    const aspect = this._scene.getEngine().getAspectRatio(this.camera);
    this._readConfigDataByAspect(aspect);

    this._fixForResolution(aspect);
    this._startCameraPosition.copyFrom(
      spawnPointPlayer.subtract(this._camOffset)
    );
    this._startCameraPosition.x = 0;
    this._charsZ = spawnPointPlayer.z;
    this._defOffsetFromChar = this._charsZ - this._startCameraPosition.z;

    if (this._minDist > this.currentMaxDistance) {
      const tmp = this._minDist;
      this._minDist = this.currentMaxDistance;
      this.currentMaxDistance = tmp;
    }

    this.moveToDefault(true);
  }

  disposePreviousLocation(): void {
    this._cameraState = ECameraState.STAY;
  }

  // ─── Model binding ───────────────────────────────────────────────────────

  /** Static wrapper — mirrors Unity's BattleCamera.SetModels(player, enemy). */
  static setModels(player: ICameraModel, enemy: ICameraModel): void {
    BattleCamera._instance.setModels(player, enemy);
  }

  setModels(player: ICameraModel, enemy: ICameraModel): void {
    this._player = player;
    this._enemy = enemy;
    if (this._cameraState !== ECameraState.STAY) {
      BattleCamera.moveToDojo(undefined, true);
    }
  }

  // ─── Camera state activation ─────────────────────────────────────────────

  static activateInventoryCamera(offsetFromChar: Vector3): void {
    const i = BattleCamera._instance;
    i._cameraState = ECameraState.INVENTORY;
    i._onlyPlayer = true;
    i.fixedOffset.x = offsetFromChar.x;
    i.fixedOffset.y = offsetFromChar.y;
    i.fixedOffset.z = 0;
    i.lookAtPlayerPos = false;
    i.camera.rotation = Vector3.Zero();
  }

  activateBattleCamera(instantly = false): void {
    this._cameraState = ECameraState.BATTLE;
    this._startPosition = undefined;
    this._onlyPlayer = false;
    this.fixedOffset = Vector3.Zero();
    this.lookAtPlayerPos = true;
    this._calculatedPosition = Vector3.Zero();
    if (instantly) {
      this._calculateBattleCameraPosition();
      this.camera.position.copyFrom(this._calculatedPosition);
    }
  }

  // ─── Movement helpers ────────────────────────────────────────────────────

  static move(
    targetPos: Vector3,
    onDone: (() => void) | undefined,
    instant: boolean
  ): void {
    BattleCamera._instance._onMoveDone = onDone;
    BattleCamera.moveToPoint(targetPos, instant);
  }

  static moveByOffset(
    offset: Vector3,
    onDone: (() => void) | undefined,
    instant: boolean
  ): void {
    BattleCamera.move(
      BattleCamera._instance.camera.position.add(offset),
      onDone,
      instant
    );
  }

  static moveToPoint(
    targetPos: Vector3,
    instant: boolean,
    callback?: () => void
  ): void {
    const i = BattleCamera._instance;
    if (callback) {
      const prev = i._onMoveDone;
      i._onMoveDone = prev ? () => { prev(); callback!(); } : callback;
    }
    i._endPos.copyFrom(targetPos);
    i._endRotation = Quaternion.Identity();
    if (instant) {
      i.camera.position.copyFrom(targetPos);
      i._cameraState = ECameraState.STAY;
      i._moveDone();
    } else {
      i._cameraState = ECameraState.MOVE_TO_POINT;
      i._startPos.copyFrom(i.camera.position);
      i._startRot.copyFrom(i.camera.rotationQuaternion ?? Quaternion.Identity());
      i._moveTimer = 0;
    }
  }

  static moveToDefault(instant: boolean): void {
    BattleCamera.moveToPoint(BattleCamera._instance._defaultPosition, instant);
  }

  static moveToDojo(onClosed: (() => void) | undefined, instant: boolean): void {
    const i = BattleCamera._instance;
    i._onMoveDone = onClosed;
    if (!i._player) {
      BattleCamera.moveToDefault(false);
      return;
    }
    i._onlyPlayer = false;
    const target = i._calculateCameraPos();
    BattleCamera.moveToPoint(target, instant);
    if (instant) {
      i._cameraState = ECameraState.BATTLE;
    }
  }

  static moveToObject(
    targetNode: TransformNode,
    offset: Vector3,
    onDone: (() => void) | undefined,
    instant: boolean
  ): void {
    const i = BattleCamera._instance;
    i._onMoveDone = onDone;
    i.fixedOffset.copyFrom(offset);
    i._targetFollowNode = targetNode;
    BattleCamera.moveToPoint(targetNode.absolutePosition.add(offset), instant);
    if (!instant) {
      i._cameraState = ECameraState.MOVE_TO_OBJECT;
    }
  }

  static moveToSpawnCentre(ignoreCallback: boolean): void {
    // Placeholder – SceneConfig.SpawnPointPlayer/Enemy referenced at runtime
  }

  static moveToStartBattlePosition(instant: boolean, callback?: () => void): void {
    const i = BattleCamera._instance;
    if (i._cameraState === ECameraState.BATTLE && i._startPosition) {
      BattleCamera.moveToPoint(i._startPosition, instant, callback);
    }
  }

  moveToDefault(instant: boolean): void {
    BattleCamera.moveToPoint(this._defaultPosition, instant);
  }

  roundEndTweenMotion(): void {
    this._cameraMotion = true;
    BattleCamera.moveToStartBattlePosition(false, () => {
      this._cameraMotion = false;
    });
  }

  roundEndTweenIsReady(): boolean {
    return !this._cameraMotion;
  }

  setCameraBlocked(enable: boolean): void {
    this._blockCameraMovement = enable;
  }

  // ─── Internal update ─────────────────────────────────────────────────────

  private _lateUpdate(): void {
    const dt = this._scene.getEngine().getDeltaTime() / 1000;

    switch (this._cameraState) {
      case ECameraState.BATTLE:
        if (!this._blockCameraMovement && this._player && this._enemy) {
          this._calculateBattleCameraPosition();
          this.camera.position.copyFrom(
            this._smoothDampV3(
              this.camera.position,
              this._calculatedPosition,
              dt,
              this.moveSmoothTime
            )
          );
        }
        break;

      case ECameraState.INVENTORY:
        if (this._player) {
          this._calculatedPosition.x = this._player.centerOfMassPosition.x;
          this._calculatedPosition.y = this._player.centerOfMassPosition.y;
          this._calculatedPosition.z = this.camera.position.z;
          this._calculatedPosition.addInPlace(this.fixedOffset);
          this.camera.position.copyFrom(
            this._smoothDampV3(
              this.camera.position,
              this._calculatedPosition,
              dt,
              0.1
            )
          );
        }
        break;

      case ECameraState.MOVE_TO_POINT:
        this._moveToTarget(dt);
        break;

      case ECameraState.MOVE_TO_OBJECT:
        if (this._targetFollowNode) {
          this._endPos.copyFrom(
            this._targetFollowNode.absolutePosition.add(this.fixedOffset)
          );
        }
        this._moveToTarget(dt);
        break;
    }
  }

  private _calculateBattleCameraPosition(): void {
    if (!this._player || !this._enemy) return;

    if (this._onlyPlayer) {
      this._calculatedPosition.x = this._player.modelBackPosX;
    } else {
      this._middleX =
        (this._enemy.modelBackPosX + this._player.modelBackPosX) / 2;
      this._calculatedPosition.x = this._middleX;

      if (this._bordersActive) {
        this._dist = Math.min(
          Math.max(
            Math.abs(this._enemy.modelBackPosX - this._player.modelBackPosX),
            this._minDist
          ),
          this.currentMaxDistance
        );
        if (this._dist < this.currentMaxDistance) {
          this._bordersActive = false;
        }
      }
    }

    this._playerCenterMassY = this._player.centerOfMassY;
    this._enemyCenterMassY = this._enemy.centerOfMassY;

    this._checkVerticalMovementDiapason();
    this._calculatedPosition.z = this.camera.position.z;

    if (!this._onlyPlayer) {
      if (!this._bordersActive) {
        this._dist = Math.min(
          Math.max(
            Math.abs(this._enemy.modelBackPosX - this._player.modelBackPosX),
            this._minDist
          ),
          this.currentMaxDistance
        );
        this._zOffset =
          (this._dist / this._minDist) * this._defOffsetFromChar;
        this._calculatedPosition.z = this._charsZ - this._zOffset;

        if (this._dist === this.currentMaxDistance) {
          this._bordersActive = true;
        }
      } else {
        this._calculatedPosition.z = this._charsZ - this._zOffset;
      }
    } else {
      this._calculatedPosition.z = this._startCameraPosition.z;
    }

    this._calculatedPosition.x = Math.min(
      Math.max(this._calculatedPosition.x, this._minXPosition),
      this._maxXPosition
    );

    this._calculatedPosition.addInPlace(this.fixedOffset);

    if (!this._startPosition) {
      this._startPosition = this._calculatedPosition.clone();
    }

    // Look-at rotation
    if (this.lookAtPlayerPos) {
      this.camera.target = new Vector3(
        this._calculatedPosition.x,
        this._calculatedPosition.y,
        this._charsZ
      ).add(this.lookAtPointOffset);
    }
  }

  private _checkVerticalMovementDiapason(): void {
    const highY = Math.max(this._playerCenterMassY, this._enemyCenterMassY);
    const lowY = Math.min(this._playerCenterMassY, this._enemyCenterMassY);

    if (
      this._playerCenterMassY >= this._startFollowYUp ||
      this._enemyCenterMassY >= this._startFollowYUp
    ) {
      this._calculatedPosition.y =
        this._startCameraPosition.y +
        (highY - this._startCameraPosition.y) * this.centerMassK;
    } else if (
      this._playerCenterMassY <= this._startFollowYBot ||
      this._enemyCenterMassY <= this._startFollowYBot
    ) {
      this._calculatedPosition.y =
        this._startCameraPosition.y +
        (lowY - this._startCameraPosition.y) * this.centerMassK;
    } else {
      this._calculatedPosition.y = this._startCameraPosition.y;
    }

    this._calculatedPosition.y = Math.min(
      Math.max(this._calculatedPosition.y, this._minYPosition),
      this._maxYPosition
    );
  }

  private _moveToTarget(dt: number): void {
    this._moveTimer = Math.min(this._moveTimer + dt, this._moveTime);
    const t = this._moveTime > 0 ? this._moveTimer / this._moveTime : 1;
    this.camera.position.copyFrom(
      Vector3.Lerp(this._startPos, this._endPos, t)
    );
    if (this._moveTimer >= this._moveTime) {
      this._cameraState = ECameraState.STAY;
      this._moveDone();
    }
  }

  private _moveDone(): void {
    if (this._onMoveDone) {
      this._onMoveDone();
      this._onMoveDone = undefined;
    }
  }

  private _calculateCameraPos(): Vector3 {
    if (!this._player || !this._enemy) return this._defaultPosition.clone();
    this._calculatedPosition.y = this._startCameraPosition.y;
    this._calculatedPosition.z = this._startCameraPosition.z;
    this._calculatedPosition.x =
      this._enemy.modelBackPosX -
      (this._enemy.modelBackPosX - this._player.modelBackPosX) / 2;
    this._dist = Math.min(
      Math.max(
        Math.abs(this._enemy.modelBackPosX - this._player.modelBackPosX),
        this._minDist
      ),
      this.currentMaxDistance
    );
    this._zOffset = (this._dist / this._minDist) * this._defOffsetFromChar;
    this._calculatedPosition.z = this._charsZ - this._zOffset;
    return this._calculatedPosition.clone();
  }

  private _smoothDampV3(
    current: Vector3,
    target: Vector3,
    dt: number,
    smoothTime: number
  ): Vector3 {
    return smoothDampVector3(current, target, this._currVelocity, smoothTime, dt);
  }

  // ─── Config helpers ───────────────────────────────────────────────────────

  private _readConfigData(config: CameraConfiguration): void {
    const s = config.current;
    if (!s) return;
    this.camera.maxZ = s.farClipPlane ?? 5000;
    this._maxYPosition = s.maxYPosition;
    this._minYPosition = s.minYPosition;
    this._maxXPosition = s.maxXPosition;
    this._minXPosition = s.minXPosition;
    this._botRotationBorder = s.startRotateYBot;
    this._upRotationBorder = s.startRotateYUp;
    this._startFollowYBot = s.startFollowYBot;
    this._startFollowYUp = s.startFollowYUp;
    this._leftHorizontalAngle = s.leftHorizontalAngle;
    this._rightHorizontalAngle = s.rightHorizontalAngle;
    this._botVerticalAngle = s.botVerticalAngle;
    this._upVerticalAngle = s.upVerticalAngle;
    this._camOffset.z = s.camZOffset;
    this.camera.fov = (this.FOV * Math.PI) / 180;
  }

  private _readConfigDataByAspect(aspect: number): void {
    // Aspect-specific z-offset handled here if needed per CameraSettings array.
  }

  private _fixForResolution(aspect: number): void {
    const defaultRatio = 16 / 9;
    const ratio = aspect / defaultRatio;
    this._minDist = BattleCamera.DEF_MIN_DIST * ratio;
    this.currentMaxDistance = this._maxDistance;
  }
}
