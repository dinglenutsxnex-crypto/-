import { EquipmentType } from "./EquipmentType";

export class Equipment {
  readonly id: number;
  model: string;
  private _equipped = false;
  private _hidden = false;
  private _default = false;
  private _type: EquipmentType;
  private _level = 1;
  private _tags: string[] = [];
  private _stackLevel = 1;

  constructor(id: number, model: string, type: EquipmentType) {
    this.id = id;
    this.model = model;
    this._type = type;
  }

  static create(id: number): Equipment {
    const def = ITEM_MODEL_MAP[id];
    if (!def) throw new Error(`No item model for ID ${id}`);
    return new Equipment(id, def.Model, def.ItemType);
  }

  static getDefaultEquipment(type: EquipmentType): Equipment | null {
    const def = EQUIPMENT_DEFAULTS[Number(type)];
    if (!def) return null;
    return Equipment.create(def.ID);
  }

  getEquipmentType(): EquipmentType { return this._type; }
  isEquipped(): boolean { return this._equipped; }
  isHidden(): boolean { return this._hidden; }
  isDefault(): boolean { return this._default; }
  get level(): number { return this._level; }
  get tags(): string[] { return [...this._tags]; }
  getStackLevel(): number { return this._stackLevel; }

  setEquipped(v: boolean) { this._equipped = v; }
  setModel(m: string) { this.model = m; }

  fillData(data: Record<string, unknown>): void {
    if (typeof data.Model === "string") this.model = data.Model;
    if (typeof data.Level === "number") this._level = data.Level;
    if (typeof data.StackLevel === "number") this._stackLevel = data.StackLevel;
    if (data.Tags instanceof Array) this._tags = data.Tags.map(String);
    if (data.Equipped === 1 || data.Equipped === "1") this._equipped = true;
    if (data.Default === 1 || data.Default === "1") this._default = true;
    if (data.Hidden === 1 || data.Hidden === "1") this._hidden = true;
    if (typeof data.Type === "string") this._type = EquipmentType[data.Type as keyof typeof EquipmentType] ?? this._type;
  }
}

const ITEM_MODEL_MAP: Record<number, { Model: string; ItemType: EquipmentType }> = {
  1000000: { Model: "wpn-fists", ItemType: EquipmentType.Weapon },
  1000001: { Model: "wpn-fists", ItemType: EquipmentType.Weapon },
  1000002: { Model: "arm__base", ItemType: EquipmentType.Armor },
  1000003: { Model: "hair-01", ItemType: EquipmentType.Helmet },
};

const EQUIPMENT_DEFAULTS: Record<number, { ID: number }> = {
  [EquipmentType.Weapon]: { ID: 1000001 },
  [EquipmentType.Armor]: { ID: 1000002 },
  [EquipmentType.Helmet]: { ID: 1000003 },
};
