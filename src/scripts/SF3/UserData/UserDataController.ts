import { Gender } from "../../sf3DTO/Gender";
import { ModelInfo } from "../GameModels/ModelInfo";
import { EquipmentType } from "../Items/EquipmentType";

interface IUserEquipment {
  Model?: string;
  Equipped?: number;
  Type?: string;
}

interface UserData {
  User: {
    Name: string;
    Gender: string;
    Appearance: {
      Head: string;
    };
    Equipments: IUserEquipment[];
  };
}

export class UserDataController {

  private static _data:
    UserData | null = null;

  static async create():
    Promise<void> {

    const res =
      await fetch(
        "assets/configs/gamesettings/user_default.json"
      );

    this._data =
      await res.json();
  }

  static initPlayer():
    void {

    console.log(
      "[UserDataController] initialized"
    );
  }

  static get isReady():
    boolean {

    return this._data !== null;
  }

  static getPlayerModelInfo():
    ModelInfo {

    const model =
      new ModelInfo();

    const user =
      this._data?.User;

    model.alias =
      user?.Name ?? "PLAYER";

    model.gender =
      user?.Gender === "Female"
        ? Gender.Female
        : Gender.Male;

    model.head =
      user?.Appearance?.Head
      ?? "head__01a";

    for (const eq of user?.Equipments ?? []) {

      if (
        !eq.Model ||
        !eq.Equipped
      ) {
        continue;
      }

      switch (eq.Type) {

        case "Armor":

          model.setEquipment(
            EquipmentType.Armor,
            eq.Model
          );

          break;

        case "Helmet":

          model.setEquipment(
            EquipmentType.Helmet,
            eq.Model
          );

          break;
      }
    }

    return model;
  }

  static getDefaultEnemyModelInfo():
    ModelInfo {

    const enemy =
      new ModelInfo();

    enemy.alias =
      "ENEMY";

    enemy.gender =
      Gender.Male;

    enemy.head =
      "head__01a";

    enemy.setEquipment(
      EquipmentType.Armor,
      "arm__base"
    );

    enemy.setEquipment(
      EquipmentType.Helmet,
      "hair-01"
    );

    return enemy;
  }
}
