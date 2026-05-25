import { Engine, Scene } from "@babylonjs/core";
import "@babylonjs/loaders";
import { FightScene } from "../battle/FightScene";

async function main(): Promise<void> {
  const canvas = document.getElementById("render-canvas") as unknown as HTMLCanvasElement;
  if (!canvas) { console.error("Canvas not found"); return; }

  const engine = new Engine(canvas, true, { preserveDrawingBuffer: false, stencil: true });
  const scene = new Scene(engine);

  const fightScene = new FightScene(scene);
  await fightScene.initialize("dojo_Legion", {
    battleID: "default",
    fightID: "default",
    roundsToWin: 2,
    roundsToLose: 3,
  });

  engine.runRenderLoop(() => scene.render());
  window.addEventListener("resize", () => engine.resize());
}

main().catch(console.error);
