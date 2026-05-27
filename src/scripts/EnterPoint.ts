import { SceneManager } from "./core/SceneManager";
import { LoadScreen } from "./LoadScreen";
import { InternalSettingsSF3 } from "./InternalSettingsSF3";
import { UserDataController } from "./SF3/UserData/UserDataController";
import { EnterPointScene } from "./ui/EnterPointScene";

export class EnterPoint {
  private _mgr: SceneManager;

  constructor(mgr: SceneManager) {
    this._mgr = mgr;
  }

  async init(): Promise<void> {
    await this._step("InternalSettings", () => InternalSettingsSF3.init());
    await this._step("UserData", () => UserDataController.create());
    await this._step("UserData.initPlayer", () => UserDataController.initPlayer());
    await this._step("Scene", () => this._loadEnterPointScene());
    LoadScreen.hide();
    console.log("[EnterPoint] SESSION_START");
  }

  private async _step(name: string, fn: () => void | Promise<void>): Promise<void> {
    try {
      const t = performance.now();
      await fn();
      console.log(`[Boot] ${name} (${(performance.now() - t).toFixed(1)}ms)`);
    } catch (e) {
      console.error(`[Boot] ${name} failed`, e);
    }
  }

  private async _loadEnterPointScene(): Promise<void> {
    const config = await this._mgr.loadSceneJSON("assets/scenes/enterPoint.scene.json");
    this._mgr.applyConfig(config);
    const epScene = new EnterPointScene(this._mgr, config);
    epScene.mount();
  }
}
