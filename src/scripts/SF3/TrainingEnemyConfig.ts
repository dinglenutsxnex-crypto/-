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

// ─── Default training enemy ───────────────────────────────────────────────────
// Mirrors the default BrawlerEnemy the Unity server sends for the starter Dojo.
// Change gear here to reskin the training dummy.

export const TRAINING_ENEMY: ITrainingEnemyConfig = {
  alias:        "DUMMY",
  aiMode:       AiMode.Regular,
  warriorPower: 1.0,
  appearance: {
    head:      "head__01a",
    gender:    Gender.Male,
    hairColor: { colorId: 3, value: 0.05 },
    skinColor: { colorId: 1, value: 0.05 },
  },
  equipment: [
    { type: EquipmentType.Weapon, model: "wpn-fists"  },
    { type: EquipmentType.Armor,  model: "arm-base"   },
    { type: EquipmentType.Helmet, model: "hair__01_m" },
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
