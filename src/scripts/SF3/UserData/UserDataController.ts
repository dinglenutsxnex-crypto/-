import { Gender }        from "../../sf3DTO/Gender";
import { ModelInfo }     from "../GameModels/ModelInfo";
import { EquipmentType } from "../Items/EquipmentType";

// ─── Raw JSON shapes ──────────────────────────────────────────────────────────

interface RawEquipment {
  ID:          number;
  Model?:      string;
  Type?:       string;   // "Weapon" | "Armor" | "Helmet" | "Ranged" | "Magic"
  Equipped?:   number;   // 1 = equipped
  Default?:    number;
  Hidden?:     number;
  Tags?:       string[];
}

interface RawColor {
  Color: number;
  Value: number;
}

interface RawAppearance {
  Head:       string;
  HairColor?: RawColor;
  SkinColor?: RawColor;
}

interface RawUser {
  Name:             string;
  Gender:           string;   // "Male" | "Female"
  Level:            number;
  Experience:       number;
  LevelExperience:  number;
  CurrentDojo:      number;
  Currency:         { Bonus: number; Coin: number; Shadow: number };
  Appearance:       RawAppearance;
  Tags:             string[];
  Equipments?:      RawEquipment[];
  GlobalVariables:  unknown;
}

interface UserData {
  User: RawUser;
}

// ─── Helpers ──────────────────────────────────────────────────────────────────

const TYPE_MAP: Record<string, EquipmentType> = {
  Weapon: EquipmentType.Weapon,
  Armor:  EquipmentType.Armor,
  Helmet: EquipmentType.Helmet,
  Ranged: EquipmentType.Ranged,
  Magic:  EquipmentType.Magic,
};

function parseGender(raw: string): Gender {
  return raw.toLowerCase() === "female" ? Gender.Female : Gender.Male;
}

// Pull the model name of the first equipped item for a given equipment slot.
function getEquippedModel(
  equipments: RawEquipment[],
  type: EquipmentType,
): string | undefined {
  return equipments.find(
    e => TYPE_MAP[e.Type ?? ""] === type && e.Equipped === 1 && e.Model,
  )?.Model;
}

// ─── Controller ───────────────────────────────────────────────────────────────

export class UserDataController {
  private static _data: UserData | null = null;

  // ── Lifecycle ──────────────────────────────────────────────────────────────

  static async create(): Promise<void> {
    const res  = await fetch("assets/configs/gamesettings/user_default.json");
    const json = await res.json();
    this._data = json as UserData;
  }

  static initPlayer(): void {
    if (!this._data) {
      console.warn("[UserDataController] initPlayer called before create()");
      return;
    }
    const u = this._data.User;
    console.log(`[UserDataController] player: ${u.Name} lv${u.Level} (${u.Gender})`);
  }

  // ── Getters ────────────────────────────────────────────────────────────────

  static get player(): RawUser | null {
    return this._data?.User ?? null;
  }

  static get isReady(): boolean {
    return this._data !== null;
  }

  // ── ModelInfo factories ───────────────────────────────────────────────────

  /**
   * Build a ModelInfo for the local player from user_default.json.
   * Falls back to sensible defaults for any missing field.
   */
  static getPlayerModelInfo(): ModelInfo {
    const u    = this._data?.User;
    const info = new ModelInfo();

    info.isPlayer  = true;
    info.isControl = true;
    info.alias     = u?.Name ?? "PLAYER";
    info.gender    = parseGender(u?.Gender ?? "Male");
    info.head      = u?.Appearance?.Head ?? "head__01a";

    const equips = u?.Equipments ?? [];

    const armor  = getEquippedModel(equips, EquipmentType.Armor)  ?? "arm__base";
    const helmet = getEquippedModel(equips, EquipmentType.Helmet) ?? "hair-01";
    const weapon = getEquippedModel(equips, EquipmentType.Weapon) ?? "wpn-fists";

    info.setEquipment(EquipmentType.Armor,  armor);
    info.setEquipment(EquipmentType.Helmet, helmet);
    info.setEquipment(EquipmentType.Weapon, weapon);

    console.log(
      `[UserDataController] playerModelInfo — head:${info.head} armor:${armor} helmet:${helmet}`,
    );
    return info;
  }

  /**
   * Default enemy ModelInfo used when there is no server fight data.
   * Mirrors the simplest BrawlerEnemy the Unity backend would send for dojo.
   */
  static getDefaultEnemyModelInfo(): ModelInfo {
    const info = new ModelInfo();

    info.isPlayer  = false;
    info.isControl = false;
    info.alias     = "ENEMY";
    info.gender    = Gender.Male;
    info.head      = "head__01a";

    info.setEquipment(EquipmentType.Armor,  "arm__base");
    info.setEquipment(EquipmentType.Helmet, "hair-01");
    info.setEquipment(EquipmentType.Weapon, "wpn-fists");

    console.log("[UserDataController] defaultEnemyModelInfo created");
    return info;
  }
}
