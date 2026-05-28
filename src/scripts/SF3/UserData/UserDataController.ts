import { Gender } from "../../sf3DTO/Gender";
import { ModelInfo, IColorPreset } from "../GameModels/ModelInfo";
import { EquipmentType } from "../Items/EquipmentType";

interface IUserEquipment {
  ID: number;
  Model?: string;
  Equipped?: number;
  Default?: number;
  Type?: string;
  Hidden?: number;
}

interface IUserAppearance {
  Head: string;
  HairColor: {
    Color: number;
    Value: number;
  };
  SkinColor: {
    Color: number;
    Value: number;
  };
}

interface UserData {
  User: {
    Name: string;
    Gender: string;
    Level: number;
    Experience: number;
    LevelExperience: number;
    CurrentDojo: number;
    Currency: { Bonus: number; Coin: number; Shadow: number };
    Appearance: IUserAppearance;
    Equipments: IUserEquipment[];
    Tags: string[];
    GlobalVariables: unknown;
  };
}

export class UserDataController {
  private static _data: UserData | null = null;

  static async create(): Promise<void> {
    const res = await fetch("assets/configs/gamesettings/user_default.json");
    const json = await res.json();

    this._data = json as UserData;
  }

  static initPlayer(): void {
    if (!this._data) {
      console.warn("[UserDataController] initPlayer called before create()");
      return;
    }

    const u = this._data.User;

    console.log(
      `[UserDataController] player: ${u.Name} lv${u.Level} (${u.Gender})`
    );
  }

  static get player(): UserData["User"] | null {
    return this._data?.User ?? null;
  }

  static get isReady(): boolean {
    return this._data !== null;
  }

  static getPlayerModelInfo(): ModelInfo {
    if (!this._data) {
      console.warn(
        "[UserDataController] getPlayerModelInfo() before data ready"
      );

      return ModelInfo.createPlayer();
    }

    const user = this._data.User;

    const modelInfo = new ModelInfo();

    modelInfo.alias = user.Name || "PLAYER";
    modelInfo.isPlayer = true;
    modelInfo.isControl = true;

    modelInfo.gender =
      user.Gender === "Female" ? Gender.Female : Gender.Male;

    modelInfo.head = user.Appearance?.Head || "head__01a";

    modelInfo.tags = user.Tags || [];

    modelInfo.hairColor = this.buildColorPreset(
      user.Appearance?.HairColor
    );

    modelInfo.skinColor = this.buildColorPreset(
      user.Appearance?.SkinColor
    );

    this.applyEquipment(modelInfo, user.Equipments || []);

    return modelInfo;
  }

  static getDefaultEnemyModelInfo(): ModelInfo {
    const enemy = new ModelInfo();

    enemy.alias = "ENEMY";
    enemy.isPlayer = false;
    enemy.isControl = false;
    enemy.gender = Gender.Male;

    enemy.head = "head__01a";

    enemy.setEquipment(EquipmentType.Armor, "arm-base");
    enemy.setEquipment(EquipmentType.Helmet, "hair-01");
    enemy.setEquipment(EquipmentType.Weapon, "wpn-fists");

    return enemy;
  }

  private static applyEquipment(
    modelInfo: ModelInfo,
    equipments: IUserEquipment[]
  ): void {
    for (const item of equipments) {
      if (!item.Model || !item.Type) {
        continue;
      }

      if (!item.Equipped && !item.Default) {
        continue;
      }

      const equipmentType = this.parseEquipmentType(item.Type);

      if (equipmentType === null) {
        continue;
      }

      modelInfo.setEquipment(equipmentType, item.Model);
    }
  }

  private static parseEquipmentType(
    type: string
  ): EquipmentType | null {
    switch (type) {
      case "Weapon":
        return EquipmentType.Weapon;

      case "Armor":
        return EquipmentType.Armor;

      case "Helmet":
        return EquipmentType.Helmet;

      case "Ranged":
        return EquipmentType.Ranged;

      case "Magic":
        return EquipmentType.Magic;

      default:
        return null;
    }
  }

  private static buildColorPreset(color: any): IColorPreset {
    return {
      colorId: color?.Color ?? 0,
      value: color?.Value ?? 0,
    };
  }
}
