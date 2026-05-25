/**
 * ColorChanger.ts
 * Mirror of SF3/ColorChanger.cs
 *
 * Animates a material colour property over time using an animation curve
 * (eased lerp). Attaches to any BabylonJS Material via a named property.
 */

import { Color3, Material, StandardMaterial, PBRMaterial } from "@babylonjs/core";

type EaseFn = (t: number) => number;

export class ColorChanger {
  fromColor: Color3 = Color3.White();
  toColor: Color3 = Color3.Black();
  onlyAlpha = false;
  forSeconds = 1; // Unity "forFrames" translated to seconds at 60 fps → /60
  shaderColorField = "";
  playOnEnable = true;

  private _material?: StandardMaterial | PBRMaterial;
  private _active = false;
  private _elapsed = 0;
  private _easeFn: EaseFn = (t) => t; // linear by default

  setMaterial(mat: StandardMaterial | PBRMaterial): void {
    this._material = mat;
  }

  setEase(fn: EaseFn): void {
    this._easeFn = fn;
  }

  play(): void {
    if (!this._material) return;
    this._elapsed = 0;
    this._active = true;
  }

  stop(): void {
    this._active = false;
  }

  /** Call from scene.onBeforeRenderObservable. dt in seconds. */
  update(dt: number): void {
    if (!this._active || !this._material) return;

    this._elapsed += dt;
    const t = Math.min(this._elapsed / Math.max(this.forSeconds, 0.001), 1);
    const eased = this._easeFn(t);

    const color = Color3.Lerp(this.fromColor, this.toColor, eased);

    if (this._material instanceof StandardMaterial) {
      this._material.diffuseColor = color;
    } else if (this._material instanceof PBRMaterial) {
      this._material.albedoColor = color;
    }

    if (t >= 1) {
      this._active = false;
    }
  }
}
