/**
 * TrainingEnemyConfig.ts
 *
 * Mirrors Unity's BrawlerEnemy / Warrior data structures for the offline Dojo.
 * The Unity server would normally send enemy loadout; here we bake it in so
 * training always works without a network.
 *
 * Source of truth: battles.js → Warriors.Gizmo + warrior-appearences.js → Gizmo
 *
 * Warriors.Gizmo = {
 *   Alias:       "CHAR_GIZMO",
 *   Gender:      GENDER.MALE,
 *   Appearance:  WarriorAppearences.Gizmo,          // Head: "head-gizmo", HairColor: Hair_01/0.05, SkinColor: Skin_01/0.05
 *   Equipments:  [WPN_TWOHANDEDSWORD_01,             // model: "wpn__twohanded_sword_01_01"
 *                 HLM_FAKE,                           // model: "helm__fake"  (no visible helm — Gizmo's own hair)
 *                 ARM_STR_05],                        // model: "arm__str_05"
 * }
 *
 * Dojo sequence (from FightController.cs → DojoRound()):
 *   1. Models spawn at SceneConfig.SpawnPointPlayer / SpawnPointEnemy
 *   2. ThrowBirth() fires EVENT_BIRTH → plays birth animation
 *      Gizmo birth anim: gizmo_stance.bytes  (walk-in / stance-enter, plays once)
 *      Gizmo idle  anim: gizmo_stance_idle.bytes (loops during fight)
 *   3. Camera snaps instantly (InitBattleCamera instant=true) — no cinematic pan
 *   4. Load screen hides
 *   5. Immediately RoundFightStart — NO "ROUND 1", NO "FIGHT!" banner, NO timer
 *      Players can fight straight away.
 */

import { Gender }        from "../sf3DTO/Gender";
import { AiMode, ModelInfo } from "./GameModels/ModelInfo";
import { EquipmentType } from "./Items/EquipmentType";

// ─── Enemy appearance / gear config ──────────────────────────────────────────

export interface ITrainingEnemyAppearance {
  head:      string;
  gender:    Gender;
  hairColor: { colorId: number; value: number };
  skinColor: { colorId: number; value: number };
}

export interface ITrainingEnemyEquipment {
  type:  EquipmentType;
  model: string;
}

export interface ITrainingEnemyConfig {
  alias:        string;
  aiMode:       AiMode;
  warriorPower: number;
  appearance:   ITrainingEnemyAppearance;
  equipment:    ITrainingEnemyEquipment[];
}

// ─── Gizmo — LegionDojo opponent (dojos.js: Dojos.LegionDojo → Warriors.Gizmo) ──

export const TRAINING_ENEMY: ITrainingEnemyConfig = {
  alias:        "CHAR_GIZMO",
  aiMode:       AiMode.DojoMode,   // ModelAi.cs: DojoMode → TacticsBehaviorRegular
  warriorPower: 1.0,
  appearance: {
    head:      "head-gizmo",        // Heads.HEAD_GIZMO → head: "head-gizmo"
    gender:    Gender.Male,
    hairColor: { colorId: 1, value: 0.05 },  // Colors.Hair_01
    skinColor: { colorId: 1, value: 0.05 },  // Colors.Skin_01
  },
  equipment: [
    // WPN_TWOHANDEDSWORD_01 → Model: "wpn__twohanded_sword_01_01"
    { type: EquipmentType.Weapon, model: "wpn__twohanded_sword_01_01" },
    // HLM_FAKE → Model: "helm__fake"  (invisible slot — Gizmo has no helmet)
    { type: EquipmentType.Helmet, model: "helm__fake"               },
    // ARM_STR_05 → Model: "arm__str_05"  (Legion heavy armor)
    { type: EquipmentType.Armor,  model: "arm__str_05"              },
  ],
};

// ─── Factory ──────────────────────────────────────────────────────────────────

/**
 * Build a ModelInfo from the training enemy config.
 * Called by ModelsManager when spawning the dojo enemy.
 *
 * Birth animation sequence (mirrors Unity ThrowBirth → EVENT_BIRTH):
 *   gizmo_stance.bytes      — plays once on spawn (stance-enter / walk-in)
 *   gizmo_stance_idle.bytes — loops during fight
 *
 * Enemy is auto-mirrored (Model.cs: forceMirrored=true for non-player)
 * so Gizmo naturally faces left toward the player.
 */
export function buildTrainingEnemyModelInfo(
  cfg: ITrainingEnemyConfig = TRAINING_ENEMY,
): ModelInfo {
  const info = new ModelInfo();

  info.alias        = cfg.alias;
  info.isPlayer     = false;
  info.isControl    = false;
  info.aiMode       = cfg.aiMode;
  info.warriorPower = cfg.warriorPower;
  info.gender       = cfg.appearance.gender;
  info.head         = cfg.appearance.head;
  info.hairColor    = cfg.appearance.hairColor;
  info.skinColor    = cfg.appearance.skinColor;

  for (const eq of cfg.equipment) {
    info.setEquipment(eq.type, eq.model);
  }

  console.log(
    `[TrainingEnemyConfig] built enemy "${cfg.alias}" — ` +
    `head:${info.head} ` +
    cfg.equipment.map(e => `${EquipmentType[e.type]}:${e.model}`).join(" "),
  );

  return info;
}
