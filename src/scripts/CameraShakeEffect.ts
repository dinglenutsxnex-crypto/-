/**
 * CameraShakeEffect.ts
 * Mirror of SF3/Effects/CameraShakeEffect.cs
 *
 * Sinusoidal camera shake with configurable amplitude, period, and duration
 * expressed in frames (BabylonJS version uses wall-clock seconds instead).
 */

import { Vector3 } from "@babylonjs/core";

interface ShakeData {
  duration: number;   // seconds
  amplitude: Vector3;
  period: Vector3;
}

export class CameraShakeEffect {
  private _active = false;
  private _elapsed = 0;
  private _data: ShakeData = {
    duration: 0,
    amplitude: Vector3.Zero(),
    period: Vector3.Zero(),
  };

  /** Current positional offset to apply to the camera each frame. */
  shift: Vector3 = Vector3.Zero();

  initialize(): void {
    this.shift = Vector3.Zero();
    this._active = false;
    this._elapsed = 0;
  }

  /** Called each frame by CameraMotionEffects. dt in seconds. */
  update(dt: number): void {
    if (!this._active) return;

    this._elapsed += dt;
    if (this._elapsed >= this._data.duration) {
      this.stop();
      return;
    }
    this._shake();
  }

  stop(): void {
    this._active = false;
    this.shift = Vector3.Zero();
  }

  startShake(duration: number, amplitude: Vector3, period: Vector3): void {
    if (duration === 0) return;
    this._elapsed = 0;
    this._data = { duration, amplitude, period };
    this._active = true;
  }

  private _shake(): void {
    const { duration, amplitude, period } = this._data;
    const t = this._elapsed;
    const remaining = duration - t;
    const decay = remaining / duration;

    const x =
      period.x !== 0
        ? Math.sin((t * 2 * Math.PI) / period.x) * amplitude.x * decay
        : 0;
    const y =
      period.y !== 0
        ? Math.sin((t * 2 * Math.PI) / period.y) * amplitude.y * decay
        : 0;
    const z =
      period.z !== 0
        ? Math.sin((t * 2 * Math.PI) / period.z) * amplitude.z * decay
        : 0;

    this.shift = new Vector3(x, y, z);
  }
}
