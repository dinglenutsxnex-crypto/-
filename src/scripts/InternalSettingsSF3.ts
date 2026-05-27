interface InternalSettingsData {
  ExternalPaths: Record<string, string>;
  LocalSettings: { Debug: boolean; BundlesPath: string; HandleLogWarnings: boolean; NoImageTexture: string };
  ServerSettings: Record<string, unknown>;
}

export class InternalSettingsSF3 {
  private static _data: InternalSettingsData | null = null;

  static async init(): Promise<void> {
    const res = await fetch("assets/configs/internalSettings.json");
    this._data = await res.json();
  }

  static get isDebug(): boolean {
    return this._data?.LocalSettings?.Debug ?? false;
  }

  static getPath(key: string): string {
    return this._data?.ExternalPaths?.[key] ?? "";
  }
}
