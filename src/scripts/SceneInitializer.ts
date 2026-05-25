/**
 * SceneInitializer.ts
 * Mirror of SF3/SceneInitializer.cs
 *
 * Orchestrates the full location-load sequence:
 *   1. Dispose previous location
 *   2. Load location prefab (GLB)
 *   3. Wait for SceneConfig
 *   4. Initialize all singleton systems in order
 *   5. Run InitBattle
 */

import { Scene, SceneLoader, AbstractMesh } from "@babylonjs/core";
import { BattleController } from "./BattleController";
import { BattleCamera } from "./BattleCamera";
import { EffectsManager } from "./EffectsManager";
import { IFightInfo } from "./FightController";

export interface ISceneInitializationObject {
  initialize(): void;
  disposePreviousLocation(): void;
}

export interface ISceneConfig {
  spawnPointPlayer: { x: number; y: number; z: number };
  spawnPointEnemy: { x: number; y: number; z: number };
  leftBorderX: number;
  rightBorderX: number;
  locationLeftBorder: number;
  locationRightBorder: number;
}

export class SceneInitializer {
  private _locationMeshes: AbstractMesh[] = [];
  private readonly _scene: Scene;

  constructor(scene: Scene) {
    this._scene = scene;
  }

  // ─── Initializer list (mirrors SceneInitializer.cs CreateInitializers) ────

  private _getInitObjects(): ISceneInitializationObject[] {
    return [
      BattleController.instance,
      BattleCamera.instance,
      EffectsManager.instance,
      // ModelsManager, BattleInterface – added when implemented
    ];
  }

  // ─── Public API ───────────────────────────────────────────────────────────

  /**
   * Load a new location and start the fight.
   * @param locationName Folder name under src/assets/models/locations/
   * @param fightInfo    Fight data for FightController
   * @param onReady      Called when the scene is live and the fight has started
   */
  async initializeNewLocationScene(
    locationName: string,
    fightInfo: IFightInfo,
    onReady?: () => void
  ): Promise<void> {
    this._disposePreviousLocationScene();

    // 1. Load location GLB – assets live under src/assets/models/locations/
    await this._loadLocationPrefab(locationName);

    // 2. Initialize all systems (mirrors "SingleTones" block in C#)
    for (const obj of this._getInitObjects()) {
      obj.initialize();
    }

    // 3. Start battle
    await BattleController.instance.initBattle(fightInfo);

    onReady?.();
  }

  // ─── Private helpers ──────────────────────────────────────────────────────

  private _disposePreviousLocationScene(): void {
    if (this._locationMeshes.length === 0) return;
    for (const obj of this._getInitObjects()) {
      obj.disposePreviousLocation();
    }
    for (const mesh of this._locationMeshes) {
      mesh.dispose();
    }
    this._locationMeshes = [];
  }

  private async _loadLocationPrefab(locationName: string): Promise<void> {
    const path = `assets/models/locations/${locationName}/`;
    const file = `${locationName}.glb`;

    try {
      const result = await SceneLoader.ImportMeshAsync(
        "",
        path,
        file,
        this._scene
      );
      this._locationMeshes = result.meshes;
      console.log(
        `[SceneInitializer] Loaded location "${locationName}" (${result.meshes.length} meshes)`
      );
    } catch (err) {
      console.warn(
        `[SceneInitializer] Could not load location "${locationName}": ${err}`
      );
    }
  }
}
