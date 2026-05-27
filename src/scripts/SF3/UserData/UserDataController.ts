interface UserData {
  User: {
    Name: string;
    Gender: string;
    Level: number;
    Experience: number;
    LevelExperience: number;
    CurrentDojo: number;
    Currency: { Bonus: number; Coin: number; Shadow: number };
    Appearance: { Head: string; HairColor: unknown; SkinColor: unknown };
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
    console.log(`[UserDataController] player: ${u.Name} lv${u.Level} (${u.Gender})`);
  }

  static get player(): UserData["User"] | null {
    return this._data?.User ?? null;
  }

  static get isReady(): boolean {
    return this._data !== null;
  }
}
