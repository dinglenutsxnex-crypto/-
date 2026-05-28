export class UserDataController {

  static player = {
    coins: 9999,
    gems: 999,
    level: 1,
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
