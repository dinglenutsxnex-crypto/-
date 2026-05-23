export enum AspectRatio { _4X3, _16X9 }

export interface CameraSettingsData {
  aspect: AspectRatio;
  minXPosition: number;
  maxXPosition: number;
  minYPosition: number;
  maxYPosition: number;
  startFollowYUp: number;
  startFollowYBot: number;
  startRotateYUp: number;
  startRotateYBot: number;
  farClipPlane: number;
  botVerticalAngle: number;
  upVerticalAngle: number;
  leftHorizontalAngle: number;
  rightHorizontalAngle: number;
  camZOffset: number;
}

export class CameraConfiguration {
  private _settings: Map<AspectRatio, CameraSettingsData> = new Map();
  private _current?: CameraSettingsData;

  get current(): CameraSettingsData | undefined { return this._current; }

  constructor(settings: CameraSettingsData[], aspectRatio: number) {
    for (const s of settings) {
      this._settings.set(s.aspect, s);
    }
    if (aspectRatio >= 1.5) {
      this._current = this._settings.get(AspectRatio._16X9);
    } else {
      this._current = this._settings.get(AspectRatio._4X3);
    }
    if (!this._current && settings.length > 0) {
      this._current = settings[0];
    }
  }

  static fromJSON(data: any): CameraConfiguration {
    const settings: CameraSettingsData[] = (data.settings ?? []).map((s: any) => ({
      aspect: s.aspect === 0 ? AspectRatio._4X3 : AspectRatio._16X9,
      minXPosition: s.minXPosition ?? -9999,
      maxXPosition: s.maxXPosition ?? 9999,
      minYPosition: s.minYPosition ?? 0,
      maxYPosition: s.maxYPosition ?? 400,
      startFollowYUp: s.startFollowYUp ?? 150,
      startFollowYBot: s.startFollowYBot ?? 50,
      startRotateYUp: s.startRotateYUp ?? 180,
      startRotateYBot: s.startRotateYBot ?? 20,
      farClipPlane: s.farClipPlane ?? 5000,
      botVerticalAngle: s.botVerticalAngle ?? 10,
      upVerticalAngle: s.upVerticalAngle ?? 30,
      leftHorizontalAngle: s.leftHorizontalAngle ?? 30,
      rightHorizontalAngle: s.rightHorizontalAngle ?? 30,
      camZOffset: s.camZOffset ?? 0,
    }));
    const screenRatio = typeof window !== "undefined" ? window.innerWidth / window.innerHeight : 1.77778;
    return new CameraConfiguration(settings, screenRatio);
  }
}
