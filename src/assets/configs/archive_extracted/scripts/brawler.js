var maxBrawlerRoundDuration = 99 * 1000;
var brawlerRoundsToWin = 2;

function getBrawlerFightReward (playerLevel, playerRating, enemyRating, fightResult) {
    return {
        Currencies: roundedCurrencies(10, 3)
    };
}

function getBrawlerBattleID(chapterID) {
    if (chapterID == ChapterModels.Chapter1["ID"]) {
        return 500;
    } else {
        return 501;
    }
}