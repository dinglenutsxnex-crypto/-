/**
 * TrainingEnemyConfig.ts
 *
 * Mirrors Unity's BrawlerEnemy / Warrior data structures for the offline Dojo.
 * The Unity server would normally send enemy loadout; here we bake it in so
 * training always works without a network.
 *
 * To swap enemy drip, change the TRAINING_ENEMY constant below.
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
  alias:       string;
  aiMode:      AiMode;
  warriorPower: number;
  appearance:  ITrainingEnemyAppearance;
  equipment:   ITrainingEnemyEquipment[];
}

// ─── LegionDojo enemy: Gizmo ─────────────────────────────────────────────────
// Source: dojos.js  →  LegionDojo.Warriors = [ Warriors.Gizmo ]
//         battles.js Warriors.Gizmo:
//           Alias:       "CHAR_GIZMO"
//           Gender:      MALE
//           Appearance:  WarriorAppearences.Gizmo
//             Head:      Heads.HEAD_GIZMO  →  "head-gizmo"
//             HairColor: Colors.Hair_01 (colorId 1), value 0.05
//             SkinColor: Colors.Skin_01 (colorId 1), value 0.05
//           Equipments:
//             WPN_TWOHANDEDSWORD_01  (ID  4)  model "wpn__twohanded_sword_01_01"
//             HLM_FAKE               (ID 409) model "helm__fake"
//             ARM_STR_05             (ID 200) model "arm__str_05"

export const TRAINING_ENEMY: ITrainingEnemyConfig = {
  alias:        "CHAR_GIZMO",
  aiMode:       AiMode.Regular,
  warriorPower: 1.0,
  appearance: {
    head:      "head-gizmo",
    gender:    Gender.Male,
    hairColor: { colorId: 1, value: 0.05 },
    skinColor: { colorId: 1, value: 0.05 },
  },
  equipment: [
    { type: EquipmentType.Weapon, model: "wpn__twohanded_sword_01_01" },
    { type: EquipmentType.Armor,  model: "arm__str_05"                },
    { type: EquipmentType.Helmet, model: "helm__fake"                 },
  ],
};

// ─── Factory ──────────────────────────────────────────────────────────────────

/**
 * Build a ModelInfo from the training enemy config.
 * Called by ModelsManager / FightScene when spawning the dojo enemy.
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
