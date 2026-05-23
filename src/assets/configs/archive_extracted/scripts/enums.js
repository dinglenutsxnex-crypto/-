var CURRENCY = {
    UNKNOWN_CURRENCY_TYPE: 0,
    COIN: 1,
    BONUS: 2,
    SHADOW: 3
};

var FACTION = {
    UNKNOWN_FACTION: 0,
    LEGION: 1,
    HERALDS: 2,
    DYNASTY: 3
};

var RARITY = {
    UNKNOWN_RARITY: 0,
    COMMON: 1,
    RARE: 2,
    EPIC: 3,
    LEGENDARY: 4
};

var ITEMTYPE = {
    UNKNOWN_ITEM_TYPE: 0,
    HELMET: 1,
    ARMOR: 2,
    WEAPON: 3,
    RANGED: 4,
    MAGIC: 5,
    PERK: 6
};

var GENDER = {
    UNKNOWN_GENDER: 0,
    MALE: 1,
    FEMALE: 2
};

var PERKTYPE = {
    UNKNOWN_PERK_TYPE: 0,
    PERK: 1,
    ENCHANTMENT: 2,
    MOVE: 3
};

var RandType = {
    UNKWNOWN_RAND_TYPE: 0,
    NONE: 1,
    SHUFFLE: 2,
    RAND: 3,
    ONCE: 4
};

var AiMode = {
    UNKNOWN_AI_MODE: 0,
    REGULAR_MODE: 1,
    NONE_MODE: 2,
    SENSEI_MODE: 3,
    TUTORIAL_MODE: 4
};

var Attributes = {
    WEAPON_DAMAGE: "WeaponDamage",
    RANGED_DAMAGE: "RangedDamage",
    UNARMED_DAMAGE: "UnarmedDamage",
    BODY_DEFENSE: "BodyDefense",
    HEAD_DEFENSE: "HeadDefense",
    CRITICAL_CHANCE: "CriticalChance",
    MAGIC_POWER: "MagicPower",
    COOLDOWN: "Cooldown"
};

var BattleType = {
    UNKNOWN_BATTLE_TYPE: 0,
    MAIN: 1,
    SIDE: 2,
    SURVIVAL: 3,
    DAILY: 4,
    MISSION: 5,

    BRAWLER: 11,

    TEST: 100,
    LOCAL: 200,
    DOJO: 300
};

var Permission = {
    NONE: 0x00000000,
    CHEATER: 0x40000000
};

var FightResult = {
    UNKNOWN_FIGHT_RESULT: 0,
    WIN: 1,
    LOSS: 2,
    SURRENDER: 3
};