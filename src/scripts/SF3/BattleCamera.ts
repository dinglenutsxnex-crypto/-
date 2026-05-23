import { Vector3, Quaternion, FreeCamera, Scalar } from "@babylonjs/core";
import { CameraConfiguration, CameraSettingsData, AspectRatio } from "../CameraConfiguration";

/* ─────────────────────────────────────────────
   Unity Mathf.SmoothDamp port
   ───────────────────────────────────────────── */
function smoothDamp(
  current: number, target: number, velocity: { value: number },
  smoothTime: number, maxSpeed = Infinity, dt = 1 / 60,
): number {
  smoothTime = Math.max(0.0001, smoothTime);
  const omega = 2 / smoothTime;
  const x = omega * dt;
  const exp = 1 / (1 + x + 0.48 * x * x + 0.235 * x * x * x);
  const change = current - target;
  const maxChange = maxSpeed * smoothTime;
  const clamped = Scalar.Clamp(change, -maxChange, maxChange);
  const temp = (velocity.value + omega * clamped) * dt;
  velocity.value = (velocity.value - omega * temp) * exp;
  return target + (clamped + temp) * exp;
}

function smoothDampAngle(
  current: number, target: number, velocity: { value: number },
  smoothTime: number, maxSpeed = Infinity, dt = 1 / 60,
): number {
  let delta = target - current;
  if (delta > 180) delta -= 360;
  if (delta < -180) delta += 360;
  return smoothDamp(current, current + delta, velocity, smoothTime, maxSpeed, dt);
}

function vec3SmoothDamp(
  current: Vector3, target: Vector3, v: { x: number; y: number; z: number },
  smoothTime: number, dt = 1 / 60,
): Vector3 {
  return new Vector3(
    smoothDamp(current.x, target.x, { value: v.x }, smoothTime, Infinity, dt),
    smoothDamp(current.y, target.y, { value: v.y }, smoothTime, Infinity, dt),
    smoothDamp(current.z, target.z, { value: v.z }, smoothTime, Infinity, dt),
  );
}

interface Keyframe { time: number; value: number; inTangent: number; outTangent: number; }
function evalCurve(kf: Keyframe[], t: number): number {
  if (kf.length === 0 || t <= kf[0].time) return kf[0]?.value ?? 0;
  if (t >= kf[kf.length - 1].time) return kf[kf.length - 1].value;
  for (let i = 0; i < kf.length - 1; i++) {
    const a = kf[i], b = kf[i + 1];
    if (t >= a.time && t <= b.time) {
      const u = (t - a.time) / (b.time - a.time);
      const u2 = u * u, u3 = u2 * u;
      return (2 * u3 - 3 * u2 + 1) * a.value
        + (u3 - 2 * u2 + u) * (b.time - a.time) * a.outTangent
        + (-2 * u3 + 3 * u2) * b.value
        + (u3 - u2) * (b.time - a.time) * b.inTangent;
    }
  }
  return kf[kf.length - 1].value;
}

/* ─────────────────────────────────────────────
   Dummy providers for tracking
   ───────────────────────────────────────────── */
export interface IModelTracker {
  modelBackPos: number;
  centerOfMassY: number;
}

export enum ECameraState { BATTLE, INVENTORY, MOVE_TO_POINT, MOVE_TO_OBJECT, STAY }

/* ─────────────────────────────────────────────
   BattleCamera
   ───────────────────────────────────────────── */
export class BattleCamera {
  private static readonly DEF_MIN_DIST = 350;

  private _cam: FreeCamera;
  private _state = ECameraState.STAY;
  private _player?: IModelTracker;
  private _enemy?: IModelTracker;

  /* config */
  private _moveSmoothTime = 0.15;
  private _rotationSmoothTime = 0.15;
  private _centerMassK = 0.5;
  private _minDist = 350;
  private _currentMaxDistance = 500;
  private _camOffset = new Vector3(0, -98.5, 562.6);

  /* computed per frame */
  private _startCamPos = Vector3.Zero();
  private _charsZ = 0;
  private _defOffsetFromChar = 0;
  private _zOffset = 0;
  private _yzOffset = 0;
  private _dist = 0;
  private _fixedOffset = Vector3.Zero();
  private _lookAtPlayerPos = false;
  private _onlyPlayer = false;
  private _bordersActive = false;

  private _calc = Vector3.Zero();
  private _cv = { x: 0, y: 0, z: 0 };
  private _xv = { value: 0 };
  private _yv = { value: 0 };
  private _zv = { value: 0 };

  private _pCMY = 0;
  private _eCMY = 0;
  private _midX = 0;

  private _yzMoveCurve: Keyframe[] = [
    { time: 0, value: 0, inTangent: 0, outTangent: 1 },
    { time: 1, value: 1, inTangent: 1, outTangent: 0 },
  ];

  /* MOVE_TO state */
  private _moveTimer = 0;
  private _moveTime = 1;
  private _moveStart = Vector3.Zero();
  private _moveEnd = Vector3.Zero();
  private _moveStartRot = Quaternion.Identity();
  private _moveEndRot = Quaternion.Identity();
  private _moveToTargetCurve: Keyframe[] = [
    { time: 0, value: 0, inTangent: 0, outTangent: 1 },
    { time: 1, value: 1, inTangent: 1, outTangent: 0 },
  ];

  constructor(cam: FreeCamera, config?: CameraConfiguration) {
    this._cam = cam;
    this._cam.mode = FreeCamera.PERSPECTIVE_CAMERA;

    if (config?.current) {
      const c = config.current;
      this._cam.maxZ = c.farClipPlane;
      this._setConfig(c);
    }
  }

  /* ── Public API ────────────────────────── */

  track(player: IModelTracker, enemy: IModelTracker): void {
    this._player = player;
    this._enemy = enemy;
  }

  applyConfig(config: CameraConfiguration): void {
    if (!config.current) return;
    const c = config.current;
    this._cam.maxZ = c.farClipPlane;
    this._setConfig(c);
  }

  setSpawnPoint(spawn: Vector3): void {
    this._startCamPos = spawn.subtract(this._camOffset);
    this._startCamPos.x = 0;
    this._charsZ = spawn.z;
    this._defOffsetFromChar = this._charsZ - this._startCamPos.z;
    const ratio = window.innerWidth / window.innerHeight / 1.77778;
    this._minDist = BattleCamera.DEF_MIN_DIST * ratio;
    this._currentMaxDistance = 500;
    if (this._minDist > this._currentMaxDistance) {
      const tmp = this._minDist;
      this._minDist = this._currentMaxDistance;
      this._currentMaxDistance = tmp;
    }
    this._cam.position.copyFrom(this._startCamPos);
  }

  setFOV(fovDeg: number): void { this._cam.fov = fovDeg * Math.PI / 180; }
  setClipping(near: number, far: number): void { this._cam.minZ = near; this._cam.maxZ = far; }
  setLookAtPlayerPos(v: boolean): void { this._lookAtPlayerPos = v; }
  setOnlyPlayer(v: boolean): void { this._onlyPlayer = v; }
  setFixedOffset(v: Vector3): void { this._fixedOffset.copyFrom(v); }
  setMoveSmoothTime(t: number): void { this._moveSmoothTime = t; }
  setRotationSmoothTime(t: number): void { this._rotationSmoothTime = t; }
  setCenterMassK(k: number): void { this._centerMassK = k; }
  setMinMaxDistance(min: number, max: number): void { this._minDist = min; this._currentMaxDistance = max; }

  get state(): ECameraState { return this._state; }

  activateBattle(instantly = false): void {
    this._state = ECameraState.BATTLE;
    this._onlyPlayer = false;
    this._fixedOffset.setAll(0);
    this._lookAtPlayerPos = true;
    this._calc.setAll(0);
    if (instantly) {
      this._calcBattle();
      this._cam.position.copyFrom(this._calc);
    }
  }

  activateInventory(offset: Vector3): void {
    this._state = ECameraState.INVENTORY;
    this._onlyPlayer = true;
    this._lookAtPlayerPos = false;
    this._fixedOffset.x = offset.x; this._fixedOffset.y = offset.y; this._fixedOffset.z = 0;
  }

  moveTo(
    startPos: Vector3, endPos: Vector3,
    startRot: Quaternion, endRot: Quaternion,
    moveTime: number,
  ): void {
    this._state = ECameraState.MOVE_TO_POINT;
    this._moveStart.copyFrom(startPos);
    this._moveEnd.copyFrom(endPos);
    this._moveStartRot.copyFrom(startRot);
    this._moveEndRot.copyFrom(endRot);
    this._moveTime = moveTime;
    this._moveTimer = 0;
  }

  moveToObject(target: Vector3, endRot: Quaternion, moveTime: number): void {
    this._state = ECameraState.MOVE_TO_OBJECT;
    this._moveStart.copyFrom(this._cam.position);
    this._moveEnd.copyFrom(target.add(this._fixedOffset));
    this._moveStartRot.copyFrom(this._cam.rotationQuaternion ?? Quaternion.Identity());
    this._moveEndRot.copyFrom(endRot);
    this._moveTime = moveTime;
    this._moveTimer = 0;
  }

  /* ── Per-frame ─────────────────────────── */

  update(dt: number): void {
    switch (this._state) {
      case ECameraState.BATTLE:
        this._calcBattle();
        this._cam.position.copyFrom(vec3SmoothDamp(this._cam.position, this._calc, this._cv, this._moveSmoothTime, dt));
        break;
      case ECameraState.INVENTORY:
        this._calcInventory();
        this._cam.position.copyFrom(vec3SmoothDamp(this._cam.position, this._calc, this._cv, 0.1, dt));
        break;
      case ECameraState.MOVE_TO_POINT:
      case ECameraState.MOVE_TO_OBJECT:
        this._moveToTarget(dt);
        break;
    }
  }

  /* ── Private ───────────────────────────── */

  private _setConfig(c: CameraSettingsData): void {
    this._startFollowYUp = c.startFollowYUp;
    this._startFollowYBot = c.startFollowYBot;
    this._upRotationBorder = c.startRotateYUp;
    this._botRotationBorder = c.startRotateYBot;
    this._minYPosition = c.minYPosition;
    this._maxYPosition = c.maxYPosition;
    this._minXPosition = c.minXPosition;
    this._maxXPosition = c.maxXPosition;
  }

  private _startFollowYUp = 150;
  private _startFollowYBot = 50;
  private _upRotationBorder = 180;
  private _botRotationBorder = 20;
  private _maxYPosition = 400;
  private _minYPosition = 0;
  private _maxXPosition = 9999;
  private _minXPosition = -9999;

  private _calcInventory(): void {
    if (!this._player) return;
    this._calc.x = this._player.modelBackPos;
    this._calc.y = this._player.centerOfMassY;
    this._calc.z = this._cam.position.z;
    this._calc.addInPlace(this._fixedOffset);
  }

  private _calcBattle(): void {
    if (!this._player || !this._enemy) return;

    if (this._onlyPlayer) {
      this._calc.x = this._player.modelBackPos;
    } else {
      this._midX = (this._enemy.modelBackPos + this._player.modelBackPos) / 2;
      this._calc.x = this._midX;
      if (this._bordersActive) {
        this._dist = Scalar.Clamp(
          Math.abs(this._enemy.modelBackPos - this._player.modelBackPos),
          this._minDist, this._currentMaxDistance,
        );
        if (this._dist < this._currentMaxDistance) this._bordersActive = false;
        else this._calc.x = 0;
      }
    }

    this._pCMY = this._player.centerOfMassY;
    this._eCMY = this._enemy.centerOfMassY;

    const distRatio = (this._dist - this._minDist) / (this._currentMaxDistance - this._minDist);
    this._yzOffset = Scalar.Lerp(0, 0, evalCurve(this._yzMoveCurve, distRatio));

    this._checkVertRot();
    this._calc.z = this._cam.position.z;

    if (this._lookAtPlayerPos) {
      const lookTarget = new Vector3(this._calc.x, 0, this._charsZ);
      const camPos = new Vector3(this._cam.position.x, 0, 0);
      const dir = lookTarget.subtract(camPos);
      if (dir.length() > 1) {
        const q = Quaternion.FromLookDirectionRH(dir, Vector3.Up());
        const e = q.toEulerAngles();
        const rx = smoothDampAngle(this._cam.rotation.x, e.x * 180 / Math.PI, this._xv, this._rotationSmoothTime);
        const ry = smoothDampAngle(this._cam.rotation.y, e.y * 180 / Math.PI, this._yv, this._rotationSmoothTime);
        this._cam.rotation.x = rx * Math.PI / 180;
        this._cam.rotation.y = ry * Math.PI / 180;
      }
    }

    this._calc.x = Scalar.Clamp(this._calc.x, this._minXPosition, this._maxXPosition);
    this._checkVertMove();

    this._dist = Scalar.Clamp(
      Math.abs(this._enemy.modelBackPos - this._player.modelBackPos),
      this._minDist, this._currentMaxDistance,
    );
    this._zOffset = this._dist / this._minDist * this._defOffsetFromChar;
    this._calc.z = this._charsZ - this._zOffset;
    if (this._dist >= this._currentMaxDistance) this._bordersActive = true;
    this._calc.y += this._yzOffset;
    this._calc.addInPlace(this._fixedOffset);
  }

  private _checkVertRot(): void {
    const maxY = this._pCMY > this._eCMY ? this._pCMY : this._eCMY;
    const minY = this._pCMY < this._eCMY ? this._pCMY : this._eCMY;
    if (this._pCMY >= this._upRotationBorder || this._eCMY >= this._upRotationBorder) {
      this._calc.y = this._startCamPos.y + (maxY - this._startCamPos.y) * this._centerMassK;
    } else if (this._pCMY <= this._botRotationBorder || this._eCMY <= this._botRotationBorder) {
      this._calc.y = this._startCamPos.y + (minY - this._startCamPos.y) * this._centerMassK;
    } else {
      this._calc.y = this._startCamPos.y;
    }
  }

  private _checkVertMove(): void {
    const maxY = this._pCMY > this._eCMY ? this._pCMY : this._eCMY;
    const minY = this._pCMY < this._eCMY ? this._pCMY : this._eCMY;
    if (this._pCMY >= this._startFollowYUp || this._eCMY >= this._startFollowYUp) {
      this._calc.y = this._startCamPos.y + (maxY - this._startCamPos.y) * this._centerMassK;
    } else if (this._pCMY <= this._startFollowYBot || this._eCMY <= this._startFollowYBot) {
      this._calc.y = this._startCamPos.y + (minY - this._startCamPos.y) * this._centerMassK;
    } else {
      this._calc.y = this._startCamPos.y;
    }
    this._calc.y = Scalar.Clamp(this._calc.y, this._minYPosition, this._maxYPosition);
  }

  private _moveToTarget(dt: number): void {
    this._moveTimer += dt;
    const t = evalCurve(this._moveToTargetCurve, this._moveTimer / this._moveTime);
    this._cam.position = Vector3.Lerp(this._moveStart, this._moveEnd, t);
    this._cam.rotationQuaternion = Quaternion.Slerp(this._moveStartRot, this._moveEndRot, t);
    if (this._moveTimer >= this._moveTime) this._state = ECameraState.STAY;
  }
}
