/**
 * ColorPreset.ts
 * Mirror of ColorPreset.cs
 */

import { Color3 } from "@babylonjs/core";

export class ColorPreset {
  constructor(
    public readonly id: number,
    public readonly color: Color3
  ) {}

  static fromJSON(data: any): ColorPreset {
    return new ColorPreset(
      data.id ?? data.ID ?? 0,
      new Color3(data.r ?? 1, data.g ?? 1, data.b ?? 1)
    );
  }
}
