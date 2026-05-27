import { SceneManager } from "./core/SceneManager";
import { LoadScreen } from "./LoadScreen";
import { EnterPoint } from "./EnterPoint";

export class GameLoad {
  private _mgr: SceneManager;

  constructor(mgr: SceneManager) {
    this._mgr = mgr;
  }

  async start(): Promise<void> {
    const ep = new EnterPoint(this._mgr);
    await ep.init();
  }
}
