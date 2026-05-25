/**
 * EffectsManager.ts
 * Mirror of SF3/Effects/EffectsManager.cs
 *
 * Coordinates all in-fight effects:
 *   – GameTimeEffects  → slowmotion / freeze-frame
 *   – AnimationEffects → particle / sprite effects (VFX)
 *   – CameraMotionEffects → camera shake + dolly zoom
 */

import { Scene, Vector3 } from "@babylonjs/core";
import { CameraShakeEffect } from "./CameraShakeEffect";

// ─── SlowMotion (mirrors SF3/Effects/SlowmotionEffect.cs + GameTimeEffects) ──

class GameTimeEffects {
  slowMotionActive = false;
  freezeFrameActive = false;
  private _timeScale = 1;

  initialize(): void {
    this.slowMotionActive = false;
    this.freezeFrameActive = false;
    this._timeScale = 1;
  }

  update(_dt: number): void {
    // Time-scale logic applied to scene tick; extended in SlowmotionEffect.
  }

  get timeScale(): number { return this._timeScale; }
  set timeScale(v: number) { this._timeScale = v; }

  stopAll(): void {
    this.slowMotionActive = false;
    this.freezeFrameActive = false;
    this._timeScale = 1;
  }
}

// ─── CameraMotionEffects ──────────────────────────────────────────────────────

class CameraMotionEffects {
  private _shake = new CameraShakeEffect();

  initialize(): void {
    this._shake.initialize();
  }

  update(dt: number): void {
    this._shake.update(dt);
  }

  shake(duration: number, amplitude: Vector3, period: Vector3): void {
    this._shake.startShake(duration, amplitude, period);
  }

  get shakeShift(): Vector3 { return this._shake.shift; }

  stopAll(): void {
    this._shake.stop();
  }
}

// ─── EffectsManager ──────────────────────────────────────────────────────────

export class EffectsManager {
  private static _instance: EffectsManager;
  static get instance(): EffectsManager { return EffectsManager._instance; }

  private readonly _scene: Scene;
  private _enabled = false;

  private _gameTime = new GameTimeEffects();
  private _cameraMotion = new CameraMotionEffects();

  get slowMotionActive(): boolean { return this._gameTime.slowMotionActive; }
  get freezeFrameActive(): boolean { return this._gameTime.freezeFrameActive; }
  get timeScale(): number { return this._gameTime.timeScale; }

  private constructor(scene: Scene) {
    this._scene = scene;
    scene.onBeforeRenderObservable.add(() => {
      if (!this._enabled) return;
      const dt = scene.getEngine().getDeltaTime() / 1000;
      this._gameTime.update(dt);
      this._cameraMotion.update(dt);
    });
  }

  static createInstance(scene: Scene): EffectsManager {
    EffectsManager._instance = new EffectsManager(scene);
    return EffectsManager._instance;
  }

  initialize(): void {
    this._gameTime.initialize();
    this._cameraMotion.initialize();
    this._enabled = false;
  }

  disposePreviousLocation(): void {
    // no persistent state to clear
  }

  effectsEnabling(enable: boolean): void {
    this._enabled = enable;
    if (!enable) {
      this.resetAll();
    }
  }

  static resetAll(): void {
    const i = EffectsManager._instance;
    i._gameTime.stopAll();
    i._cameraMotion.stopAll();
  }

  static shakeCamera(duration: number, amplitude: Vector3, period: Vector3): void {
    EffectsManager._instance._cameraMotion.shake(duration, amplitude, period);
  }

  /** Returns the current camera shake offset (applied by BattleCamera). */
  get cameraShakeShift(): Vector3 {
    return this._cameraMotion.shakeShift;
  }

  resetAll(): void {
    this._gameTime.stopAll();
    this._cameraMotion.stopAll();
  }
}
