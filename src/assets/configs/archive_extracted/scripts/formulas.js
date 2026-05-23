//region utils
function makeKeyList(map) {
    var keyList = [];

    for (var key in map) {
        if (map.hasOwnProperty(key)) {
            keyList.push(key);
        }
    }

    return keyList;
}

function createIDMap(map) {
    var resultMap = {};

    for (var name in map) {
        if (map.hasOwnProperty(name)) {
            var value = map[name];

            if (resultMap.hasOwnProperty(value.ID)) {
                throw name.toString() + " And " + resultMap[value.ID].Name.toString() + " has the same ID: " + value.ID;
            }

            value.Name = name;
            resultMap[value.ID] = value;
        }
    }

    return resultMap;
}

function shuffle(arr) {
    for (var i = arr.length; i > 0; --i) {
        var index = Math.floor(Math.random() * i);

        var temp = arr[index];
        arr[index] = arr[i - 1];
        arr[i - 1] = temp;
    }

    return arr;
}
//endregion

//region ItemMerger
function getPower(stackLevel1, stackLevel2, rarity){
    var bar1 = getBar(stackLevel1, rarity);
    var bar2 = getBar(stackLevel2, rarity);
    var maxBar = stackLevel1 > stackLevel2 ? bar1 : bar2;
    var minBar = stackLevel1 < stackLevel2 ? bar1 : bar2;
    var cardsForStepBase = getRarityCardsForStepsBase(rarity);
    var cardsForStepInf = calcRarityCardsForStepInf(rarity);
    var stackPerStep = getRarityStackPerStep(rarity);
    var deltaSteps = Math.abs(getDeltaSteps(stackLevel1, stackLevel2, rarity));

    return maxBar + 1 / (cardsForStepBase * Math.pow(cardsForStepInf, deltaSteps - minBar));
}

function getBaseStackLevel(stackLevel, rarity){
    var stackPerStep = getRarityStackPerStep(rarity);

    return Math.floor(stackLevel / stackPerStep) * stackPerStep;
}

//Вычисляет разницу уровней между двумя шмотками
function getDeltaSteps(stackLevel1, stackLevel2, rarity){
    var stackPerStep = getRarityStackPerStep(rarity);

    return (getBaseStackLevel(stackLevel2, rarity) - getBaseStackLevel(stackLevel1, rarity)) / stackPerStep;
}

function getPowerSteps(power, rarity){
    var inflation = calcRarityCardsForStepInf(rarity);

    return Math.floor(Math.log((inflation - 1) * power + 1) / Math.log(inflation));
}

function calcBar(power, steps, rarity){
    var cardsForStepInf = calcRarityCardsForStepInf(rarity);
    var totalInf = Math.pow(cardsForStepInf, steps);

    return (power - (totalInf - 1) / (cardsForStepInf - 1)) / totalInf;
}

function mergeStackLevels(stackLevel1, stackLevel2, rarity, playerLevel) {
    var power = getPower(stackLevel1, stackLevel2, rarity);
    var steps = getPowerSteps(power, rarity);
    var bar = calcBar(power, steps, rarity);
    var stackPerStep = getRarityStackPerStep(rarity);

    return (steps + bar) * stackPerStep + getBaseStackLevel(Math.max(stackLevel1, stackLevel2), rarity);
}
//endregion

//region Interface
//Вычисляет количество уровней, полученных при стаке эквипа
function getLevelUpCount(stackLevel1, stackLevel2, rarity){
    return getDeltaSteps(stackLevel1, mergeStackLevels(stackLevel1, stackLevel2, rarity), rarity);
}

//Вычисляет значение барчика при мердже эквипа
function getLevelUpBar(stackLevel1, stackLevel2, rarity) {
    return getBar(mergeStackLevels(stackLevel1, stackLevel2, rarity), rarity);
}

//Вычисляет значения барчика эквипа
function getBar(stackLevel, rarity) {
    var stackPerStep = getRarityStackPerStep(rarity);
    var baseStackLevel = getBaseStackLevel(stackLevel, rarity);

    return (stackLevel - baseStackLevel) / stackPerStep;
}

//Вычисляет количество уровней, полученных при стаке перков
function getLevelUpCountPerks(stackLevel1, stackLevel2, rarity){
    var stackLevelMerge = mergeStackLevelsPerks(stackLevel1, stackLevel2, rarity);
    var stackPerStep = 5;

    return Math.floor(stackLevelMerge / stackPerStep) - Math.floor(stackLevel1 / stackPerStep);
}

//Вычисляет значение барчика при мердже перков
function getLevelUpBarPerks(stackLevel1, stackLevel2, rarity) {
    return getBarPerks(mergeStackLevelsPerks(stackLevel1, stackLevel2, rarity), rarity);
}

//Вычисляет значения барчика перка
function getBarPerks(stackLevel, rarity) {
    var stackPerStep = 5;
    var baseStackLevel = Math.floor(stackLevel / stackPerStep) * stackPerStep;

    return (stackLevel - baseStackLevel) / stackPerStep;
}
//endregion

function mergeStackLevelsPerks(stackLevel1, stackLevel2, rarity, playerLevel) {
    var cardsbase = getRarityCardsForStepBasePerks(rarity);
    var cardsInflation  = getRarityCardsForStepsInfPerks(rarity);
    var stackPerStep = 5;

    var perkLevel = Math.floor(stackLevel1 / stackPerStep);

    return stackLevel1 + stackPerStep / (cardsbase * Math.pow(cardsInflation, perkLevel));
}

//Расчет урона при ударе в бою
function calcStrikeDamage(baseDamage, attackAttribute, defenseAttribute) {
    return baseDamage * Math.pow(attributesInflation, ((attackAttribute - defenseAttribute) / stackPerLevel));
}

function compareItems(firstID, firstSL, secondID, secondSL, itemsMap){
    var firstRarity = itemsMap[firstID].Rarity;
    var secondRarity = itemsMap[secondID].Rarity;
    var firstModifSL = (getRarityStackLevelShift(firstRarity)) + firstSL;
    var secondModifSL = (getRarityStackLevelShift(secondRarity)) + secondSL;

    return firstModifSL < secondModifSL;
}

function calcFightDifficulty(WDP, UDP, MPP, BDP, HDP, WDE, UDE, MPE, BDE, HDE) {
    var WB = hitsFrequencyCross.WB;
    var WH = hitsFrequencyCross.WH;
    var UB = hitsFrequencyCross.UB;
    var UH = hitsFrequencyCross.UH;
    var MB = hitsFrequencyCross.MB;
    var MH = hitsFrequencyCross.MH;

    var inf = Math.pow(attributesInflation, 1 / stackPerLevel);
    var enemyPower = Math.pow(inf, WDE - BDP) * WB + Math.pow(inf, WDE - HDP) * WH + Math.pow(inf, UDE - BDP) * UB + Math.pow(inf, UDE - HDP) * UH + Math.pow(inf, MPE - BDP) * MB + Math.pow(inf, MPE - HDP) * MH;
    var playerPower = Math.pow(inf, WDP - BDE) * WB + Math.pow(inf, WDP - HDE) * WH + Math.pow(inf, UDP - BDE) * UB + Math.pow(inf, UDP - HDE) * UH + Math.pow(inf, MPP - BDE) * MB + Math.pow(inf, MPP - HDE) * MH;

    return enemyPower / playerPower;
}

function roundedCurrencies(coins, bonuses) {
    var result = {};

    result[CURRENCY.COIN] = Math.ceil(coins / 5) * 5;
    result[CURRENCY.BONUS] = Math.ceil(bonuses / 5) * 5;

    return result;
}

function currencies(coins, bonuses) {
    var result = {};

    result[CURRENCY.COIN] = coins;
    result[CURRENCY.BONUS] = bonuses;

    return result;
}