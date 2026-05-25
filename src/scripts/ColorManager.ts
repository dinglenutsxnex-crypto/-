/**
 * ColorManager.ts
 * Mirror of ColorManager.cs
 *
 * Singleton that holds skin and hair colour presets loaded from config.
 */

import { ColorPreset } from "./ColorPreset";

export class ColorManager {
  private static _instance: ColorManager;
  static get instance(): ColorManager { return ColorManager._instance; }

  private _skinColors: ColorPreset[] = [];
  private _hairColors: ColorPreset[] = [];

  private constructor() {}

  static createInstance(): ColorManager {
    ColorManager._instance = new ColorManager();
    return ColorManager._instance;
  }

  /** Load colour presets from a config object (JSON from bundlesConfig or inline). */
  loadFromConfig(data: { skinColors?: any[]; hairColors?: any[] }): void {
    this._skinColors = (data.skinColors ?? []).map(ColorPreset.fromJSON);
    this._hairColors = (data.hairColors ?? []).map(ColorPreset.fromJSON);
  }

  getSkinColor(id: number): ColorPreset | undefined {
    return this._skinColors.find((x) => x.id === id);
  }

  getHairColor(id: number): ColorPreset | undefined {
    return this._hairColors.find((x) => x.id === id);
  }
}
