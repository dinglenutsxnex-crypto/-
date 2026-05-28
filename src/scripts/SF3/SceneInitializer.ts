import {
  Vector3,
  FreeCamera,
  HemisphericLight,
  DirectionalLight,
  Scene,
  TransformNode,
  Color4,
  Color3,
} from "@babylonjs/core";

import "@babylonjs/loaders";

import { SceneManager } from "../core/SceneManager";
import { ModelsManager } from "./ModelsManager";
import { UserDataController } from "./UserData/UserDataController";

export async function initializeScene(
  mgr: SceneManager
): Promise<void> {

  const scene = mgr.scene;

  setupCamera(scene);
  setupLights(scene);

  // IMPORTANT:
  // this file is the REAL entrypoint currently.
  // Old hardcoded mesh spawning below
  // was overriding the whole battle system.

  if (!UserDataController.isReady) {
    await UserDataController.create();
    UserDataController.initPlayer();
  }

  const modelsManager =
    new ModelsManager(scene);

  await modelsManager.loadModels();

  const playerInfo =
    UserDataController.getPlayerModelInfo();

  const enemyInfo =
    UserDataController.getDefaultEnemyModelInfo();

  await modelsManager.createBattleModels(
    playerInfo,
    enemyInfo
  );

  // Correct SF3-style positions
  if (modelsManager.player?.root) {

    modelsManager.player.root.position =
      new Vector3(-180, 0, 0);

    modelsManager.player.root.rotation.y =
      Math.PI / 2;
  }

  if (modelsManager.enemy?.root) {

    modelsManager.enemy.root.position =
      new Vector3(180, 0, 0);

    modelsManager.enemy.root.rotation.y =
      -Math.PI / 2;
  }

  // Camera follow logic
  const cam =
    scene.activeCamera as FreeCamera;

  const cameraRoot =
    new TransformNode(
      "camera_root",
      scene
    );

  cam.parent = cameraRoot;

  scene.onBeforeRenderObservable.add(() => {

    const player =
      modelsManager.player?.root;

    const enemy =
      modelsManager.enemy?.root;

    if (!player || !enemy) {
      return;
    }

    const midpoint =
      player.position
        .add(enemy.position)
        .scale(0.5);

    const distance =
      Vector3.Distance(
        player.position,
        enemy.position
      );

    cameraRoot.position.x =
      midpoint.x;

    cameraRoot.position.y =
      110 + distance * 0.03;

    cam.position.z =
      -620 - distance * 0.4;

    cam.setTarget(
      new Vector3(
        midpoint.x,
        110,
        0
      )
    );
  });

  console.log(
    "[SceneInitializer] Battle scene initialized"
  );
}

function setupCamera(
  scene: Scene
): void {

  scene.clearColor =
    new Color4(0.32, 0.32, 0.32, 1);

  scene.ambientColor =
    new Color3(0.7, 0.7, 0.7);

  const cam =
    new FreeCamera(
      "cam",
      new Vector3(0, 110, -620),
      scene
    );

  cam.setTarget(
    new Vector3(0, 110, 0)
  );

  cam.minZ = 10;
  cam.maxZ = 5000;

  cam.fov = 30 * Math.PI / 180;

  scene.activeCamera = cam;
}

function setupLights(
  scene: Scene
): void {

  const hemi =
    new HemisphericLight(
      "hemi",
      new Vector3(0, 1, 0),
      scene
    );

  hemi.intensity = 0.9;

  const dir =
    new DirectionalLight(
      "dir",
      new Vector3(-0.5, -1, -0.3),
      scene
    );

  dir.position.set(
    300,
    500,
    300
  );

  dir.intensity = 1.4;
}
