var attributesInflation = 4;
var stackPerLevel = 10;
var coinBase = 4;
var bonusBase = 1;
var moneyInflation = 1.6;
var experienceInflation = 1.6;
var baseBonusPrice = 0;

var hitsFrequency = {};
hitsFrequency[Attributes.WEAPON_DAMAGE] = 0.6;
hitsFrequency[Attributes.UNARMED_DAMAGE] = 0.2;
hitsFrequency[Attributes.MAGIC_POWER] = 0.2;
hitsFrequency[Attributes.BODY_DEFENSE] = 0.8;
hitsFrequency[Attributes.HEAD_DEFENSE] = 0.2;

var hitsFrequencyCross = {
    WB: hitsFrequency[Attributes.WEAPON_DAMAGE] * hitsFrequency[Attributes.BODY_DEFENSE],
    WH: hitsFrequency[Attributes.WEAPON_DAMAGE] * hitsFrequency[Attributes.HEAD_DEFENSE],
    UB: hitsFrequency[Attributes.UNARMED_DAMAGE] * hitsFrequency[Attributes.BODY_DEFENSE],
    UH: hitsFrequency[Attributes.UNARMED_DAMAGE] * hitsFrequency[Attributes.HEAD_DEFENSE],
    MB: hitsFrequency[Attributes.MAGIC_POWER],
    MH: 0
};

var minSL = -100000000;
var maxSL = +100000000;
var maxSurvivalCountWithoutRegeneration = 20;
var shopCooldown = 1000 * 60 * 60 * 8;
var battleMissionCooldownTime = 1000 * 60 * 60 * 4;
var battleMissionSlotCount = 3;

var PLAYER = "Player";
var ENEMY = "Enemy";
var ANY = "Any";

//region Offline commands constants

var compareOfflineStateAfterCommandExecution = true;

//endregion