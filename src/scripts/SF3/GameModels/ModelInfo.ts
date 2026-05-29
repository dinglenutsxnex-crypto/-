import { Gender } from "../../sf3DTO/Gender";
import { EquipmentType } from "../Items/EquipmentType";

export enum AiMode {
  Unknown = 0,
  Regular = 1,
  None    = 2,
  Sensei  = 3,
  Dojo    = 4,
}

export interface IColorPreset {
  colorId: number;
  value:   number;
}

export interface IEquipmentSlot {
  type:  EquipmentType;
  model: string;
}

export class ModelInfo {
  alias:        string = "";
  gender:       Gender = Gender.Male;
  head:         string = "head__01a";
  skeleton:     string = "default";
  skinColor:    IColorPreset = { colorId: 1, value: 0.05 };
  hairColor:    IColorPreset = { colorId: 0, value: 0.05 };
  isControl:    boolean = false;
  isPlayer:     boolean = false;
  aiMode:       AiMode = AiMode.Regular;
  warriorPower: number = 0;
  maxLife:      number = 1;
  tags:         string[] = [];

  private _equipment: Map<EquipmentType, string> = new Map();

  setEquipment(type: EquipmentType, model: string): void {
    this._equipment.set(type, model);
  }

  getEquippedModel(type: EquipmentType): string | undefined {
    return this._equipment.get(type);
  }

  getEquipmentSlots(): IEquipmentSlot[] {
    const slots: IEquipmentSlot[] = [];
    this._equipment.forEach((model, type) => slots.push({ type, model }));
    return slots;
  }

  static createPlayer(overrides?: Partial<ModelInfo>): ModelInfo {
    const info = new ModelInfo();
    info.alias     = "PLAYER";
    info.isPlayer  = true;
    info.isControl = true;
    info.aiMode    = AiMode.None;
    info.setEquipment(EquipmentType.Armor,   "arm__base");
    info.setEquipment(EquipmentType.Helmet,  "hair-01");
    if (overrides) Object.assign(info, overrides);
    return info;
  }

  static createEnemy(overrides?: Partial<ModelInfo>): ModelInfo {
    const info = new ModelInfo();
    info.alias     = "ENEMY";
    info.isPlayer  = false;
    info.isControl = false;
    info.aiMode    = AiMode.Regular;
    info.setEquipment(EquipmentType.Armor,   "arm__base");
    info.setEquipment(EquipmentType.Helmet,  "hair-01");
    if (overrides) Object.assign(info, overrides);
    return info;
  }
}
