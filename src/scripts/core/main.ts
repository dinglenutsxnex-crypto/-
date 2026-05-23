import { SceneManager } from "./SceneManager";
import { initializeScene } from "../SF3/SceneInitializer";

async function main(): Promise<void> {
  const canvas = document.getElementById("render-canvas") as unknown as HTMLCanvasElement;
  if (!canvas) { console.error("Canvas not found"); return; }
  const mgr = new SceneManager(canvas);
  await initializeScene(mgr);
  mgr.start();
}

main().catch(console.error);
