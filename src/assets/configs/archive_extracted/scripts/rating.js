var ratingWidth = 400;
var ratingSpeed = 0.01;
var speedKoe = calcSpeedKoe(ratingWidth, ratingSpeed);

function calcSpeedKoe (width, speed) {
    return 2 * width * Math.log((0.5 + speed) / (0.5 - speed));
}

function calcWinProbability(r1, r2, width) {
    return 1 / (1 + Math.exp((r2 - r1) / width));
}

function calcPlayerDeltaRating (playerRating, enemyRating, fightResult) {
    var win = fightResult == FightResult.WIN;
    var probability = calcWinProbability(playerRating, enemyRating, ratingWidth);

    return speedKoe * (win - probability);
}
