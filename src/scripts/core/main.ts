import { Engine, Scene } from "@babylonjs/core";
import "@babylonjs/loaders";
import { SceneManager } from "./SceneManager";
import { LoadScreen } from "../LoadScreen";
import { GameLoad } from "../GameLoad";
import "../../ui/styles/global.css";

async function main(): Promise<void> {
  const canvas = document.getElementById("render-canvas") as unknown as HTMLCanvasElement;
  if (!canvas) { console.error("Canvas not found"); return; }

  const mgr = new SceneManager(canvas);

  LoadScreen.mount();

  const gameLoad = new GameLoad(mgr);
  await gameLoad.start();

  mgr.start();
}

main().catch(console.error);
