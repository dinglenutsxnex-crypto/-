export class UserDataController {

  static player = {
    Name: "PLAYER",
    Level: 1,

    Currency: {
      Coins: 9999,
      Gems: 999,
      ShadowEnergy: 100,
    },
  };

  static async create():
    Promise<void> {}

  static initPlayer():
    void {}

  static get isReady():
    boolean {
      return true;
    }

  static getPlayerModelInfo(): any {
    return {};
  }

  static getDefaultEnemyModelInfo(): any {
    return {};
  }
}
