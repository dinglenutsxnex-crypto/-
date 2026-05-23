/*
//Test booster zone

var ConsoleDisplay = {
    Shop: true,
    NoMoney: true,
    EquipUpgr: true,
    BattlePass: true,
    LevelUp: true,
    Show: true
};

var PlayerContent = {
    expectation: 2,
    stop: false,
    Skill: 1,
    Level: 1,
    Experience: 50,
    Currency: {COIN: 160, BONUS: 0, SHADOW: 0},
    Perksmap: {},
    Itemsmap: {},
    Equipped: {}
};

var zonesPerksTest = zonesPerks;

function giveAndEquipPlayerWithStandartItems(Content){
    Content.Itemsmap = {5: {SL: 20, Counter: 0},401: {SL: 20, Counter: 0},201: {SL: 20, Counter: 0}};
    Content.Equipped = { WEAPON: { Rarity: 1, SL: 20, ID: 5 },HELMET: { Rarity: 1, SL: 20, ID: 401 }, ARMOR: { Rarity: 1, SL: 20, ID: 201 }}
}

giveAndEquipPlayerWithStandartItems(PlayerContent);

var ItemModelsTest = {
    //Weapons
    WPN_STR_C: {
        ID: 1,
        Level: 1,
        Model: "wpn__nunchaku_01_01",
        Alias: "wpn__nunchaku_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowTag: ["SHADOW_DASH"],
        Slots: 1
    },
    WPN_STR_R: {
        ID: 2,
        Level: 1,
        Model: "wpn__nunchaku_01_02",
        Alias: "wpn__nunchaku_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowTag: ["SHADOW_DASH"],
        Slots: 2
    },
    WPN_STR_E: {
        ID: 3,
        Level: 1,
        Model: "wpn__twohanded_sword_01_01",
        Alias: "wpn__twohanded_sword_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        Slots: 1,
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_STR_L: {
        ID: 4,
        Level: 1,
        Model: "wpn__twohanded_sword_01_02",
        Alias: "wpn__twohanded_sword_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Slots: 2,
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    ARM_STR_C: {
        ID: 201,
        Level: 1,
        Model: "arm__str_05",
        Alias: "arm__str_05",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        Slots: 2,
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_STR_R: {
        ID: 202,
        Level: 1,
        Model: "arm__str_04",
        Alias: "arm__str_04",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        Slots: 2,
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_STR_E: {
        ID: 203,
        Level: 1,
        Model: "arm__str_03",
        Alias: "arm__str_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        Slots: 2
    },
    ARM_STR_L: {
        ID: 204,
        Level: 1,
        Model: "arm__str_02",
        Alias: "arm__str_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        Slots: 2,
        Attributes: ModelsShifts.MoreBodyDefense
    },
    HLM_STR_C: {
        ID: 401,
        Level: 1,
        Model: "helm__str_01",
        Alias: "helm__str_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 1
    },
    HLM_STR_R: {
        ID: 402,
        Level: 1,
        Model: "helm__str_03",
        Alias: "helm__str_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_POWER_WILL"],
        ShadowMark: "SHADOW_POWER_WILL",
        Slots: 1,
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_STR_E: {
        ID: 403,
        Level: 1,
        Model: "helm__agl_01",
        Alias: "helm__agl_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_STR_L: {
        ID: 404,
        Level: 1,
        Model: "helm__agl_01",
        Alias: "helm__agl_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
/!*    RNG_STR_C: {
        ID: 601,
        Level: 1,
        Model: "helm__str_01",
        Alias: "helm__str_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 1
    },
    RNG_STR_R: {
        ID: 602,
        Level: 1,
        Model: "helm__str_03",
        Alias: "helm__str_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_POWER_WILL"],
        ShadowMark: "SHADOW_POWER_WILL",
        Slots: 1,
        Attributes: ModelsShifts.MoreHeadDefense
    },
    RNG_STR_E: {
        ID: 603,
        Level: 1,
        Model: "helm__agl_01",
        Alias: "helm__agl_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
    RNG_STR_L: {
        ID: 604,
        Level: 1,
        Model: "helm__agl_01",
        Alias: "helm__agl_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },*!/

    WPN_AGL_C: {
        ID: 5,
        Level: 1,
        Model: "wpn__twohanded_sword_01_03",
        Alias: "wpn__twohanded_sword_01_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Slots: 2,
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_AGL_R: {
        ID: 6,
        Level: 1,
        Model: "wpn__twohanded_sword_01_04",
        Alias: "wpn__twohanded_sword_01_04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Slots: 3,
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_AGL_E: {
        ID: 7,
        Level: 1,
        Model: "wpn__staff_01_01",
        Alias: "wpn__staff_01_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["STAFF", "BLUNT"],
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"],
        Slots: 1,
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_AGL_L: {
        ID: 8,
        Level: 1,
        Model: "wpn__staff_01_02",
        Alias: "wpn__staff_01_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["STAFF", "BLUNT"],
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"],
        ShadowMark: "SHADOW_STAFF_SPINNING_SLASH",
        Slots: 2,
        Attributes: ModelsShifts.MoreCriticalChance
    },
    ARM_AGL_C: {
        ID: 205,
        Level: 1,
        Model: "arm__agl_04",
        Alias: "arm__agl_04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_STASIS"],
        ShadowMark: "SHADOW_BLINK",
        Slots: 2
    },
    ARM_AGL_R: {
        ID: 206,
        Level: 1,
        Model: "arm__agl_05",
        Alias: "arm__agl_05",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_STASIS"],
        ShadowMark: "SHADOW_BLINK",
        Slots: 1,
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_AGL_E: {
        ID: 207,
        Level: 1,
        Model: "arm__str_07",
        Alias: "arm__str_07",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        Slots: 3,
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_AGL_L: {
        ID: 208,
        Level: 1,
        Model: "arm__agl_06",
        Alias: "arm__agl_06",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_STASIS"],
        ShadowMark: "SHADOW_BLINK",
        Slots: 3
    },
    HLM_AGL_C: {
        ID: 405,
        Level: 1,
        Model: "helm__agl_02",
        Alias: "helm__agl_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        Slots: 2
    },
    HLM_AGL_R: {
        ID: 406,
        Level: 1,
        Model: "helm__prc_01",
        Alias: "helm__prc_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_CONTROL"],
        Slots: 1,
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_AGL_E: {
        ID: 407,
        Level: 1,
        Model: "helm__prc_03",
        Alias: "helm__prc_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_EVAPORATION"],
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_AGL_L: {
        ID: 408,
        Level: 1,
        Model: "helm__str_09",
        Alias: "helm__str_09",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 2
    },
/!*    RNG_AGL_C: {
        ID: 605,
        Level: 1,
        Model: "helm__agl_02",
        Alias: "helm__agl_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        Slots: 2
    },
    RNG_AGL_R: {
        ID: 606,
        Level: 1,
        Model: "helm__prc_01",
        Alias: "helm__prc_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_CONTROL"],
        Slots: 1,
        Attributes: ModelsShifts.MoreHeadDefense
    },
    RNG_AGL_E: {
        ID: 607,
        Level: 1,
        Model: "helm__prc_03",
        Alias: "helm__prc_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_EVAPORATION"],
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
    RNG_AGL_L: {
        ID: 608,
        Level: 1,
        Model: "helm__str_09",
        Alias: "helm__str_09",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 2
    },*!/

    WPN_PRC_C: {
        ID: 9,
        Level: 1,
        Model: "wpn__axes_01_01",
        Alias: "wpn__axes_01_01",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        Slots: 1,
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_PRC_R: {
        ID: 10,
        Level: 1,
        Model: "wpn__axes_01_02",
        Alias: "wpn__axes_01_02",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        Slots: 2,
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_PRC_E: {
        ID: 11,
        Level: 1,
        Model: "wpn__axes_01_03",
        Alias: "wpn__axes_01_03",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        Slots: 2,
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_PRC_L: {
        ID: 12,
        Level: 1,
        Model: "wpn__sabers_01_01",
        Alias: "wpn__sabers_01_01",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SABERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        Slots: 1,
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    ARM_PRC_C: {
        ID: 209,
        Level: 1,
        Model: "arm__prc_02",
        Alias: "arm__prc_02",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "PRECISE"],
        ShadowTag: ["SHADOW_PUSH"],
        ShadowMark: "SHADOW_PUSH",
        Slots: 2,
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_PRC_R: {
        ID: 210,
        Level: 1,
        Model: "arm__prc_03",
        Alias: "arm__prc_03",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "PRECISE"],
        ShadowTag: ["SHADOW_SPURT"],
        ShadowMark: "SHADOW_PUSH",
        Slots: 2
    },
    ARM_PRC_E: {
        ID: 211,
        Level: 1,
        Model: "arm__agl_01",
        Alias: "arm__agl_01",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_BLINK"],
        ShadowMark: "SHADOW_BLINK",
        Slots: 2,
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_PRC_L: {
        ID: 212,
        Level: 1,
        Model: "arm__galen",
        Alias: "arm__galen",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_BLINK"],
        ShadowMark: "SHADOW_BLINK",
        Slots: 3,
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    HLM_PRC_C: {
        ID: 409,
        Level: 1,
        Model: "helm__str_08",
        Alias: "helm__str_08",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 1,
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_PRC_R: {
        ID: 410,
        Level: 1,
        Model: "helm__str_02",
        Alias: "helm__str_02",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_PRC_E: {
        ID: 411,
        Level: 1,
        Model: "helm__fake",
        Alias: "helm__fake",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        Slots: 0
    },
    HLM_PRC_L: {
        ID: 412,
        Level: 1,
        Model: "helm__str_10",
        Alias: "helm__str_10",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 2,
        Attributes: ModelsShifts.MoreHeadDefense
    }
/!*    RNG_PRC_C: {
        ID: 609,
        Level: 1,
        Model: "helm__str_08",
        Alias: "helm__str_08",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 1,
        Attributes: ModelsShifts.MoreHeadDefense
    },
    RNG_PRC_R: {
        ID: 610,
        Level: 1,
        Model: "helm__str_02",
        Alias: "helm__str_02",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 1,
        Attributes: ModelsShifts.MoreMagicPower
    },
    RNG_PRC_E: {
        ID: 611,
        Level: 1,
        Model: "helm__fake",
        Alias: "helm__fake",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        Slots: 0
    },
    RNG_PRC_L: {
        ID: 612,
        Level: 1,
        Model: "helm__str_10",
        Alias: "helm__str_10",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.RANGED,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        Slots: 2,
        Attributes: ModelsShifts.MoreHeadDefense
    }*!/
};

/!*var zonesPerksTest = {
    1: [],
    2: [
        PerkModels.SUPER_TWOHANDED_SWORD_1,
        PerkModels.SUPER_TWOHANDED_SWORD_2,
        PerkModels.SUPER_SWORDS_STRONG_1,
        PerkModels.SUPER_SWORDS_STRONG_2,
        PerkModels.SUPER_AXES_1,
        PerkModels.SUPER_SWORD_2,
        PerkModels.SUPER_TWOHANDED_SWORD_3,
        PerkModels.SUPER_SWORD_1,
        PerkModels.SUPER_SPEAR_1,
        PerkModels.SUPER_TWOHANDED_SWORD_4,
        PerkModels.SUPER_ARMOR_STRONG_1,
        PerkModels.SUPER_ARMOR_STRONG_2
    ],
    3: [
        PerkModels.SUPER_TWOHANDED_SWORD_1,
        PerkModels.SUPER_TWOHANDED_SWORD_2,
        PerkModels.SUPER_SWORDS_STRONG_1,
        PerkModels.SUPER_SWORDS_STRONG_2,
        PerkModels.SUPER_AXES_1,
        PerkModels.SUPER_SWORD_2,
        PerkModels.SUPER_TWOHANDED_SWORD_3,
        PerkModels.SUPER_SWORD_1,
        PerkModels.SUPER_SPEAR_1,
        PerkModels.SUPER_TWOHANDED_SWORD_4,
        PerkModels.SUPER_ARMOR_STRONG_1,
        PerkModels.SUPER_ARMOR_STRONG_2
    ],
    4: [
        PerkModels.SUPER_TWOHANDED_SWORD_1,
        PerkModels.SUPER_TWOHANDED_SWORD_2,
        PerkModels.SUPER_SWORDS_STRONG_1,
        PerkModels.SUPER_SWORDS_STRONG_2,
        PerkModels.SUPER_AXES_1,
        PerkModels.SUPER_SWORD_2,
        PerkModels.SUPER_TWOHANDED_SWORD_3,
        PerkModels.SUPER_SWORD_1,
        PerkModels.SUPER_SPEAR_1,
        PerkModels.SUPER_TWOHANDED_SWORD_4,
        PerkModels.SUPER_ARMOR_STRONG_1,
        PerkModels.SUPER_ARMOR_STRONG_2
    ]
};*!///Old zonesPerks

var ItemModelsMapTest = createIDMap(ItemModelsTest);

var zonesItemsTest = [
    createItemGroup(ItemModelsMapTest, ANY, ANY, ANY, 1, 3)
];

var ItemSettingRETest = [
    {
        ItemPool: zonesItemsTest[0][RARITY.RARE],
        Weight: 10
    },
    {
        ItemPool: zonesItemsTest[0][RARITY.EPIC],
        Weight: 1
    }
];

var itemsTest = createItemGroup(ItemModelsMapTest, ANY, ANY, ANY, ANY, ANY);

var playerItems = itemsTest[RARITY.COMMON].slice(0, 12)
    .concat(itemsTest[RARITY.RARE].slice(0, 12))
    .concat(itemsTest[RARITY.EPIC].slice(0, 12))
    .concat(itemsTest[RARITY.LEGENDARY].slice(0, 12));

/!*var boosterModelsTest = {
    CHAPTER1_RARE: {
        ID: 3,
        Rarity: RARITY.RARE,
        Zone: 1,
        Alias: "CHAPTER1_RARE",
        Description: "",
        Loot: {
            And: populateBooster(
                itemsTest, zonesPerksTest,
                10,
                1000, 300, 75, 5,
                4, 1
            )
        }
    },
    CHAPTER1_EPIC: {
        ID: 4,
        Rarity: RARITY.EPIC,
        Zone: 1,
        Alias: "CHAPTER1_EPIC",
        Description: "",
        Loot: {
            And: populateBooster(
                itemsTest, zonesPerksTest,
                10,
                1000, 300, 75, 5,
                3, 1, 1
            )
        }
    },
    CHAPTER1_LEGENDARY: {
        ID: 5,
        Rarity: RARITY.LEGENDARY,
        Zone: 1,
        Alias: "CHAPTER1_LEGENDARY",
        Description: "",
        Loot: {
            And: populateBooster(
                itemsTest, zonesPerksTest,
                10,
                1000, 300, 75, 5,
                2, 1, 1, 1
            )
        }
    }
};*!/ //Boosters

function applyItemN(ID, N) {
    for (var i = 0; i < N; ++i) {
        PlayerContent.Itemsmap[ID].SL = mergeStackLevels(10, PlayerContent.Itemsmap[ID].SL, ItemModelsMapTest[ID].Rarity, 0);
    }
}

function getMaxIDs(items, result) {
    for (var itemTypeName in ITEMTYPE) {
        if (ITEMTYPE.hasOwnProperty(itemTypeName)) {
            var itemType = ITEMTYPE[itemTypeName];
            var max = result[itemType].SL;
            var max_ID = result[itemType].ID;

            for (var ID in items) {
                if (items.hasOwnProperty(ID)) {
                    if (items[ID].SL > max && ItemModelsMapTest[ID].ItemType == itemType && ItemModelsMapTest[max_ID].ID !== ItemModelsMapTest[ID].ID) {
                         console.log('Change ' + itemTypeName + '\n' +
                             'Prev Item Rarity ' + ItemModelsMapTest[max_ID].Rarity.toString() + ' SL ' + max.toString() + ' - ID:' + ItemModelsMapTest[max_ID].ID + '\n' +
                             'New Item Rarity ' + ItemModelsMapTest[ID].Rarity.toString() + ' SL ' + items[ID].SL.toString()+ ' - ID:' + ItemModelsMapTest[ID].ID);
                         PlayerContent.Equipped[itemTypeName] = {Rarity: ItemModelsMapTest[ID].Rarity, SL: items[ID].SL, ID: ItemModelsMapTest[ID].ID};

                        max = items[ID].SL;
                        max_ID = ID;
                    }
                    else if (items[ID].SL > max && ItemModelsMapTest[ID].ItemType == itemType){
                        max = items[ID].SL;
                        max_ID = ID;
                        console.log('Upgraded ' + itemTypeName + ' to SL ' + items[ID].SL.toString()+ ' - ID:' + ItemModelsMapTest[ID].ID);
                        PlayerContent.Equipped[itemTypeName] = {Rarity: ItemModelsMapTest[ID].Rarity,SL: items[ID].SL, ID: ItemModelsMapTest[ID].ID};
                    }
                }
            }
        }

        result[itemType] = {ID: max_ID, SL: max};
    }

    console.log('----------------------------');
    return result;
}

function sumWeights(weightArray) {
    var result = 0;

    for (var objName in weightArray) {
        if (weightArray.hasOwnProperty(objName)) {
            var obj = weightArray[objName];

            if (!obj.hasOwnProperty("Weight")) {
                obj.Weight = 1;
            }

            result += obj.Weight;
        }
    }

    return result;
}

function makeChoice(weightArray) {
    var partSum = 0;
    var weightSum = sumWeights(weightArray);
    var randChoice = Math.random() * weightSum;

    for (var objName in weightArray) {
        if (weightArray.hasOwnProperty(objName)) {
            var obj = weightArray[objName];

            partSum += obj.Weight;

            if (partSum > randChoice) {
                return obj;
            }
        }
    }

    throw "makeChoise: can't make choise";
}

function getLootReward(loot) {
    var reward = loot;

    while (reward.hasOwnProperty("Or")) {
        reward = makeChoice(reward.Or);
    }

    if (reward.hasOwnProperty("Equipments")) {
        reward.Equipments.ID = reward.Equipments.Models[0].ID;
    }

    if (reward.hasOwnProperty("Perks")) {
        reward.Perks.ID = reward.Perks.Models[0].ID;
    }

    return reward;
}

function getBoosterReward(booster) {
    var reward = [];
    var loot = booster.Loot.And;

    for (var objName in loot) {
        if (loot.hasOwnProperty(objName)) {
            var obj = loot[objName];

            reward.push(getLootReward(obj));
        }
    }

    return reward;
}

function applyItem(itemObj) {
    if (itemObj.hasOwnProperty("Equipments")) {
        var item = itemObj.Equipments;
        var ID = item.ID;
        var stackLevel = PlayerContent.Itemsmap[ID].SL;

        PlayerContent.Itemsmap[ID].SL = mergeStackLevels(stackLevel, item.SL, ItemModelsMapTest[ID].Rarity, 0);
        ++PlayerContent.Itemsmap[ID].Counter;
    }
}

function applyBooster(booster) {
    var reward = getBoosterReward(booster);

    for (var i = 0; i < reward.length; ++i) {
        var item = reward[i];
        applyItem(item);
    }
}

function checkStackLevelEquip(stackLevel, rarity) {
    return checkStackLevelOr(stackLevel, rarity, ITEMTYPE.WEAPON) &&
        checkStackLevelOr(stackLevel, rarity, ITEMTYPE.ARMOR) &&
        checkStackLevelOr(stackLevel, rarity, ITEMTYPE.HELMET); /!*&&
        checkStackLevelOr(stackLevel, rarity, ITEMTYPE.RANGED); Убрал металки из проверки *!/
}

function checkStackLevelOr(stackLevel, rarity, itemType) {
    var eq = function(a, b) {return a == b;};

    for (var ID in PlayerContent.Itemsmap) {
        if (PlayerContent.Itemsmap.hasOwnProperty(ID)) {
            var item = ItemModelsMapTest[ID];

            if (PlayerContent.Itemsmap[ID].SL >= stackLevel &&
                checkObjProperty(item, "Rarity", rarity, eq) &&
                checkObjProperty(item, "ItemType", itemType, eq)) {
                return true;
            }
        }
    }

    return false;
}

function checkStackLevelAnd(stackLevel, rarity, itemType) {
    var eq = function(a, b) {return a == b;};

    for (var ID in PlayerContent.Itemsmap) {
        if (PlayerContent.Itemsmap.hasOwnProperty(ID)) {
            var item = ItemModelsMapTest[ID];

            if (PlayerContent.Itemsmap[ID].SL < stackLevel &&
                checkObjProperty(item, "Rarity", rarity, eq) &&
                checkObjProperty(item, "ItemType", itemType, eq)) {
                return false;
            }
        }
    }

    return true;
}

function countBoosters(stackLevel, booster, rarity, itemType) {
    var counter = 0;
    var result = {
        1: {SL: 0, ID: 1},
        2: {SL: 0, ID: 2},
        3: {SL: 0, ID: 3},
        4: {SL: 0, ID: 4},
        5: {SL: 0, ID: 0},
        6: {SL: 0, ID: 0}
    };

    while (!checkStackLevelOr(stackLevel, rarity, itemType)) {
        applyBooster(booster);
        ++counter;
        getMaxIDs(PlayerContent.Itemsmap, result);
    }

    result.Any = counter;

    while (!checkStackLevelEquip(stackLevel, rarity)) {
        applyBooster(booster);
        ++counter;
        getMaxIDs(PlayerContent.Itemsmap, result);
    }

    result.Equip = counter;

    while (!checkStackLevelAnd(stackLevel, rarity, itemType)) {
        applyBooster(booster);
        ++counter;
        getMaxIDs(PlayerContent.Itemsmap, result);
    }

    result.All = counter;

    return result;
}

function test(booster, rarity) {
    var N = 1;

    var result = {
        Any: 0,
        Equip: 0,
        All: 0
    };

    for (var j = 0; j < N; ++j) {
        for (var i = 0; i < playerItems.length; ++i) {
            var item = playerItems[i];

            PlayerContent.Itemsmap[item.ID] = {SL: 10, Counter: 0};
        }

        var counters = countBoosters(20, booster, rarity, ANY);

        for (var key in result) {
            if (result.hasOwnProperty(key)) {
                result[key] += counters[key] / N;
            }
        }
    }

    return result;
}

var result = {
    Rare: {
        //Common: test(boosterModelsTest.CHAPTER1_RARE, RARITY.COMMON)
        //Rare: test(boosterModelsTest.CHAPTER1_RARE, RARITY.RARE)
        //Epic: test(boosterModelsTest.CHAPTER1_RARE, RARITY.EPIC),
        //Legendary: test(boosterModelsTest.CHAPTER1_RARE, RARITY.LEGENDARY)
    }
    // Epic: {
    //     Common: test(boosterModelsTest.CHAPTER1_EPIC, RARITY.COMMON),
    //     Rare: test(boosterModelsTest.CHAPTER1_EPIC, RARITY.RARE)//,
    // //     Epic: test(boosterModelsTest.CHAPTER1_EPIC, RARITY.EPIC),
    // //     Legendary: test(boosterModelsTest.CHAPTER1_EPIC, RARITY.LEGENDARY)
    // },
    // Legendary: {
    //     Common: test(boosterModelsTest.CHAPTER1_LEGENDARY, RARITY.COMMON),
    //     Rare: test(boosterModelsTest.CHAPTER1_LEGENDARY, RARITY.RARE)//,
    // //     Epic: test(boosterModelsTest.CHAPTER1_LEGENDARY, RARITY.EPIC),
    // //     Legendary: test(boosterModelsTest.CHAPTER1_LEGENDARY, RARITY.LEGENDARY)
    // }
};

function getBattleReward(BattleNumber, playerContentFunc){
    var reward = BattlesTest[BattleNumber].Reward;
    var parsedReward = getObjLootReward(reward);
    CombineLootRewardWithPlayerContent(parsedReward, playerContentFunc);
    EquipBestItems(playerContentFunc);
    if(BattlesTest[BattleNumber].IsFinal == true){
        playerContentFunc.stop = true;
    }
}

function getObjLootReward(loot,RewardPool){
    if (RewardPool === undefined){
        RewardPool = {
            Experience: 0,
            Currency: {COIN: 0, BONUS: 0, SHADOW: 0},
            Perksmap: [],
            Itemsmap: []
        };
    }
    if (loot.hasOwnProperty("Currencies")){
        for (var currencyname in CURRENCY) {
            if (CURRENCY.hasOwnProperty(currencyname)) {
                if (loot.Currencies[CURRENCY[currencyname]]){
                    RewardPool.Currency[currencyname] += loot.Currencies[CURRENCY[currencyname]];
                }
                else RewardPool.Currency[currencyname] += 0;
            }
        }
    }
    if (loot.hasOwnProperty("Experience")){
        RewardPool.Experience += loot.Experience;
    }
    if (loot.hasOwnProperty("Perks")){
        var onePerk = {SL: (loot.Perks.SL), Counter: 0, ID: loot.Perks.Models[0].ID};
        RewardPool.Perksmap.push(onePerk)
    }
    if (loot.hasOwnProperty("Equipments")){
        var oneEquipment = {SL: (loot.Equipments.SL), Counter: 0, ID: loot.Equipments.Models[0].ID};
        RewardPool.Itemsmap.push(oneEquipment)
    }
    if (loot.hasOwnProperty("Or")){
        getObjLootReward(makeChoice(loot.Or), RewardPool);
    }
    if (loot.hasOwnProperty("And")){
        for (var i = 0; i < loot.And.length; ++i) {
            getObjLootReward(loot.And[i], RewardPool);
        }
    }
    return RewardPool;
}

function CombineLootRewardWithPlayerContent(AllRewards, playerContentFunc){
    var OldLvl = playerContentFunc.Level;
    var newExpInfo = newLevelCalculate(playerContentFunc.Level, playerContentFunc.Experience, AllRewards.Experience);
    playerContentFunc.Experience = newExpInfo.Experience;
    playerContentFunc.Level = newExpInfo.Level;
    var NewLvl = playerContentFunc.Level;
    if(NewLvl > OldLvl){
        if (ConsoleDisplay.LevelUp == true){
            console.log("LEVEL UP!! Level: " + NewLvl)
        }
    }
    for (var currencyname in CURRENCY) {
        if (CURRENCY.hasOwnProperty(currencyname)) {
            playerContentFunc.Currency[currencyname] += AllRewards.Currency[currencyname]
        }
    }
    for (var i = 0; i < AllRewards.Perksmap.length; i++){
        if (playerContentFunc.Perksmap[AllRewards.Perksmap[i].ID] === undefined){
            playerContentFunc.Perksmap[AllRewards.Perksmap[i].ID] = {SL: AllRewards.Perksmap[i].SL, Counter: 0};
        }
        else{
            var oldPerkSL = playerContentFunc.Perksmap[AllRewards.Perksmap[i].ID].SL;
            var newPerkSL = AllRewards.Perksmap[i].SL;
            var PerkRarity = PerkModelsMap[AllRewards.Perksmap[i].ID].Rarity; //Will change to testMap
            playerContentFunc.Perksmap[AllRewards.Perksmap[i].ID].SL = mergeStackLevelsPerks(oldPerkSL, newPerkSL, PerkRarity, playerContentFunc.Level);
            playerContentFunc.Perksmap[AllRewards.Perksmap[i].ID].Counter += 1;
        }
    }
    for (var u = 0; u < AllRewards.Itemsmap.length; u++){
        if (playerContentFunc.Itemsmap[AllRewards.Itemsmap[u].ID] === undefined){
            playerContentFunc.Itemsmap[AllRewards.Itemsmap[u].ID] = {SL: AllRewards.Itemsmap[u].SL, Counter: 0};
        }
        else{
            var oldItemSL = playerContentFunc.Itemsmap[AllRewards.Itemsmap[u].ID].SL;
            var newItemSL = AllRewards.Itemsmap[u].SL;
            var ItemRarity = ItemModelsMapTest[AllRewards.Itemsmap[u].ID].Rarity; //Will change to testMap
            playerContentFunc.Itemsmap[AllRewards.Itemsmap[u].ID].SL = mergeStackLevels(oldItemSL, newItemSL, ItemRarity, playerContentFunc.Level);
            playerContentFunc.Itemsmap[AllRewards.Itemsmap[u].ID].Counter += 1;
        }
    }
}

function EquipBestItems(playerContentFunc){
    for (var key in playerContentFunc.Itemsmap){
        if(playerContentFunc.Itemsmap.hasOwnProperty(key)){
            if (ItemModelsMapTest[key].Level <= playerContentFunc.Level){
                var itemType = ItemModelsMapTest[key].ItemType;
                for (var itemtypename in ITEMTYPE){
                    if(ITEMTYPE.hasOwnProperty(itemtypename)){
                        if(itemType === ITEMTYPE[itemtypename]){
                            var itemTypeName = itemtypename;
                        }
                    }
                }
                if((playerContentFunc.Equipped[itemTypeName] === undefined) || compareItems(playerContentFunc.Equipped[itemTypeName].ID,playerContentFunc.Equipped[itemTypeName].SL,key,playerContentFunc.Itemsmap[key].SL, ItemModelsMapTest)){
                    if(playerContentFunc.Equipped[itemTypeName].ID == key){
                        if (ConsoleDisplay.EquipUpgr == true){
                            console.log("Upgraded " + itemTypeName + "!!!")
                        }
                    }
                    else{
                        if (ConsoleDisplay.EquipUpgr == true){
                            console.log("New " + itemTypeName + "!!!")
                        }
                    }
                    playerContentFunc.Equipped[itemTypeName] = {Rarity: ItemModelsMapTest[key].Rarity,SL: playerContentFunc.Itemsmap[key].SL, ID: ItemModelsMapTest[key].ID};
                    if(ConsoleDisplay.Show == true){
                        console.log(playerContentFunc.Equipped)
                    }
                }
            }
        }
    }
}

//console.log(compareItems(201,20,202,20,ItemModelsMapTest));
/!*getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);
getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);
getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);
getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);
getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);
getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);
getLootAfterBattle(Battles.beta_BootCamp_MQ_FirstExam,1);*!/

//console.log(JSON.stringify(createWeightEquipment(baseItemRatio, 1, ItemSettingRE)));
//console.log(PlayerContent);
//console.log((Battles.beta_BootCamp_MQ_FirstExam.Fights[3].Rewards[2].And[2].And[0]));
//console.log(getMaxIDs(PlayerContent.Itemsmap,{1: {SL: 0, ID: 1},2: {SL: 0, ID: 2},3: {SL: 0, ID: 3},4: {SL: 0, ID: 4},5: {SL: 0, ID: 0},6: {SL: 0, ID: 0}} ));

//Test battle zone
BattlesTest = {
    1: {
        ID: 1,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 0).WarriorPower,
            EquippedItems: {
                HELMET: 412,
                ARMOR: 214,
                WEAPON: 22
            }
        },
        Reward: {
            Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 0 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 0 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]
        }


    },
    2: {
        ID: 2,
        ParentID: 1,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 3).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 1 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 1 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    3: {
        ID: 3,
        ParentID: 2,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 6).WarriorPower,
            EquippedItems: {
                HELMET: 400,
                ARMOR: 202,
                WEAPON: 50
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 2 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 2 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    4: {
        ID: 4,
        ParentID: 3,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 9).WarriorPower,
            EquippedItems: {
                HELMET: 401,
                ARMOR: 203,
                WEAPON: 47
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 5 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 5 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    5: {
        ID: 5,
        ParentID: 4,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 12).WarriorPower,
            EquippedItems: {
                HELMET: 406,
                ARMOR: 202,
                WEAPON: 55
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 8 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 8 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    6: {
        ID: 6,
        ParentID: 5,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 15).WarriorPower,
            EquippedItems: {
                HELMET: 411,
                ARMOR: 201,
                WEAPON: 44
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 11 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 11 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    7: {
        ID: 7,
        ParentID: 6,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 18).WarriorPower,
            EquippedItems: {
                HELMET: 406,
                ARMOR: 210,
                WEAPON: 29
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 14 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 14 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio - itemRatioShift + 14 * stackPerLevel / levelPrecision, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio - itemRatioShift + 14 * stackPerLevel / levelPrecision, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    8: {
        ID: 8,
        ParentID: 7,
        IsFinal: true,
        Type: BattleType.MAIN,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 21).WarriorPower,
            EquippedItems: {
                HELMET: 407,
                ARMOR: 200,
                WEAPON: 6
            }
        },
        Reward: {Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 17 / levelPrecision), 0),
            Experience: baseMainExp * Math.pow(experienceInflation, 17 / levelPrecision),
            And: [
                {And: createEquipments(baseItemRatio - itemRatioShift + 17 * stackPerLevel / levelPrecision, zonesItemsTest[0][RARITY.COMMON], 1)},
                {And: createWeightEquipment(baseItemRatio - itemRatioShift + 17 * stackPerLevel / levelPrecision, 1, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    9: {
        ID: 9,
        ParentID: 1,
        Type: BattleType.SIDE,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 3).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 1 / levelPrecision), 0),
            Experience: baseSideExp * Math.pow(experienceInflation, 1 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    10: {
        ID: 10,
        ParentID: 2,
        Type: BattleType.SIDE,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 6).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 2 / levelPrecision), 0),
            Experience: baseSideExp * Math.pow(experienceInflation, 2 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    11: {
        ID: 11,
        ParentID: 3,
        Type: BattleType.SIDE,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 9).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 5 / levelPrecision), 0),
            Experience: baseSideExp * Math.pow(experienceInflation, 5 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    12: {
        ID: 12,
        ParentID: 4,
        Type: BattleType.SIDE,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 12).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 8 / levelPrecision), 0),
            Experience: baseSideExp * Math.pow(experienceInflation, 8 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    13: {
        ID: 13,
        ParentID: 5,
        Type: BattleType.SIDE,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 15).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 11 / levelPrecision), 0),
            Experience: baseSideExp * Math.pow(experienceInflation, 11 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    14: {
        ID: 14,
        ParentID: 6,
        Type: BattleType.SIDE,
        Warrior: {
            WarriorPower: makeWP(baseWarriorPower, 18).WarriorPower,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 14 / levelPrecision), 0),
            Experience: baseSideExp * Math.pow(experienceInflation, 14 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRETest)},
                {And: createPerks(0, zonesPerksTest[0][RARITY.RARE], 1)}
            ]}
    },
    17: {
        ID: 17,
        Type: BattleType.MISSION,
        Warrior: {
            WarriorAmplifier: -8,
            WarriorPower: 0,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {
            Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), 0),
            Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)}
            ]
        }
    },
    18: {
        ID: 18,
        Type: BattleType.MISSION,
        Warrior: {
            WarriorAmplifier: -3,
            WarriorPower: 0,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {
            Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), 0),
            Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)}
            ]
        }
    },
    19: {
        ID: 19,
        Type: BattleType.MISSION,
        Warrior: {
            WarriorAmplifier: 2,
            WarriorPower: 0,
            EquippedItems: {
                HELMET: 408,
                ARMOR: 214,
                WEAPON: 28
            }
        },
        Reward: {
            Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), 0),
            Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
            And: [
                {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRETest)}
            ]
        }
    }
};

BattlesTestMap = createIDMap(BattlesTest);

AvailableBattles = [];
getFirstAvailableBattles(BattlesTestMap, AvailableBattles );

makeGraph(BattlesTestMap);

function makeGraph(battlesMap) {
    var battleID;

    for (battleID in battlesMap) {
        if (battlesMap.hasOwnProperty(battleID)) {
            var battle = battlesMap[battleID];

            if (!battle.hasOwnProperty("ChildsID")) {
                battle.ChildsID = [];
            }
        }
    }

    for (battleID in battlesMap) {
        if (battlesMap.hasOwnProperty(battleID)) {
            battle = battlesMap[battleID];

            if (battle.hasOwnProperty("ParentID")) {
                var parentBattle = battlesMap[battle.ParentID];

                parentBattle.ChildsID.push(battle.ID);
            }
        }
    }
}

function getFirstAvailableBattles(battlesMap, availableBattles) {
    for (var battleID in battlesMap) {
        if (battlesMap.hasOwnProperty(battleID)) {
            var battle = battlesMap[battleID];

            if (!battle.hasOwnProperty("ParentID") && battle.Type != BattleType.MISSION) {
                availableBattles.push(battle.ID);
            }
        }
    }
    renewAllMissions(availableBattles, battlesMap);
}

function completeBattle(battleID, availableBattles, battlesMap) {
    var battleIndex = availableBattles.indexOf(battleID);

    if (battleIndex == -1) {
        console.log(PlayerContent);
        throw "No battle: " + battleID + " in available battles";
    }

    var battle = battlesMap[availableBattles[battleIndex]];
    availableBattles.splice(battleIndex, 1);
    for (var i = 0; i < battle.ChildsID.length; ++i) {
        availableBattles.push(battle.ChildsID[i]);
    }
}

function chooseBattle(player, availableBattles, battlesMap) {
    var battle = chooseBattleWithType(player, availableBattles, battlesMap, BattleType.MAIN);
    if (battle == null) {
        battle = chooseBattleWithType(player, availableBattles, battlesMap, BattleType.SIDE);
        if (battle == null){
            battle = chooseBattleWithType(player, availableBattles, battlesMap, BattleType.MISSION);
            if (battle == null){
                return null
            }
        }
    }

    return battle;
}

function chooseBattleWithType (player, availableBattles, battlesMap, battleType) {
    for (var i = 0; i < availableBattles.length; ++i) {
        var availableBattle = battlesMap[availableBattles[i]].ID;
        if (canWinBattle(player, availableBattle, battlesMap, player.Skill) && battlesMap[availableBattle].Type === battleType){
            return availableBattles[i];
        }
    }
    return null;
}

function fightFirst(player, avBattle, battleMap){
    var battleChosen = chooseBattle(player,  avBattle, battleMap);
    if (battleChosen != null){
        completeBattle(battleChosen, avBattle, battleMap);
        if (ConsoleDisplay.BattlePass == true){
            console.log("Passed battle " + battleChosen);
            console.log("");
            console.log("*");
        }
        getBattleReward(battleChosen, player);
    }
    else{
        return null;
    }
}

function findMain(avBattle, battleMap){
    for (var index in avBattle){
        if (avBattle.hasOwnProperty(index)){
            if(battleMap[avBattle[index]].Type == BattleType.MAIN){
                return battleMap[avBattle[index]].ID
            }
        }
    }
}

function getMainParams(avBattle, battleMap){

}

function setMissionParams(){

}

function renewAllMissions(avBattle, battleMap){
    for(var battleID in battleMap){
        if(battleMap.hasOwnProperty(battleID)){
            var battle = battleMap[battleID];

            if(battleMap[battleID].Type == BattleType.MISSION){
                var existingBattle = avBattle.indexOf(battle.ID);

                if(existingBattle == -1){
                    var mainID = findMain(avBattle, battleMap);
                    avBattle.push(battleMap[battleID].ID);
                    battleMap[battleID].Warrior.WarriorPower = makeWP(battleMap[mainID].Warrior.WarriorPower, battleMap[battleID].Warrior.WarriorAmplifier).WarriorPower
                }
            }
        }
    }
}

function goShopping(player, avBattle, battleMap){
    var currentShop;
    if (currentShop == undefined){
        currentShop = getRefreshedShop(player.Level)
    }

    var logObj = {log: ""};

    var chosenItem = findStrongestAvalibleItem(currentShop, player, logObj);

    if(ConsoleDisplay.Shop) {
        if(logObj.log != "") {

        }
    }

    for (var itemTypeName in ITEMTYPE) {
        if (ITEMTYPE.hasOwnProperty(itemTypeName)) {
            if(ItemModelsMapTest[chosenItem.ID].ItemType == ITEMTYPE[itemTypeName]){
                var chosenItemTypeName = itemTypeName;
            }
        }
    }
    var coin = chosenItem.Price;
    player.Currency.COIN -= coin;
    player.Itemsmap[player.Equipped[chosenItemTypeName].ID].SL = priceAndSL.SL;
    if (ConsoleDisplay.Shop){
        console.log("Bought " + chosenItemTypeName + " for " + priceAndSL.Price[CURRENCY.COIN]+ " ----------------------------------------- SHOP");
    }

    else{
        if (ConsoleDisplay.NoMoney){
            console.log("NO MONEY SHOP -------------------------------------------------------------------------------------------- MISSION RENEW");
        }
        renewAllMissions(avBattle, battleMap);
    }
}

function FightAll(player, avBattle, battleMap){
    while (player.stop == false) {
        if (chooseBattle(player,  avBattle, battleMap) != null){
            fightFirst(player, avBattle, battleMap);
        }
        else{
            goShopping(player, avBattle, battleMap);
            EquipBestItems(player);
        }
    }
}

function canWinBattle(player, battleID, battleMap) {
    var battle = battleMap[battleID];
    var EIDarray = [];

    for(var key in battle.Warrior.EquippedItems){
        if (battle.Warrior.EquippedItems.hasOwnProperty(key)){
            EIDarray.push(battle.Warrior.EquippedItems[key]);
        }
    }

    var PHA = getAttributesBattleLocal(player.Equipped.HELMET.SL, ItemModelsMapTest[player.Equipped.HELMET.ID]);
    var PBA = getAttributesBattleLocal(player.Equipped.ARMOR.SL, ItemModelsMapTest[player.Equipped.ARMOR.ID]);
    var PWA = getAttributesBattleLocal(player.Equipped.WEAPON.SL, ItemModelsMapTest[player.Equipped.WEAPON.ID]);
    var AEA = getWarriorAttributes(battle.Warrior.WarriorPower, EIDarray);

    var WDP = PWA[Attributes.WEAPON_DAMAGE];
    var UDP = PBA[Attributes.UNARMED_DAMAGE];
    var MPP = PHA[Attributes.MAGIC_POWER];
    var BDP = PBA[Attributes.BODY_DEFENSE];
    var HDP = PHA[Attributes.HEAD_DEFENSE];
    var WDE = AEA[Attributes.WEAPON_DAMAGE];
    var UDE = AEA[Attributes.UNARMED_DAMAGE];
    var MPE = AEA[Attributes.MAGIC_POWER];
    var BDE = AEA[Attributes.BODY_DEFENSE];
    var HDE = AEA[Attributes.HEAD_DEFENSE];

    var Difficulty = calcFightDifficulty(WDP, UDP, MPP, BDP, HDP, WDE, UDE, MPE, BDE, HDE);

    return Difficulty < player.Skill

}

function calcItemPower(player, itemID, itemSL) {
    var EIDarray = [400, 202, 50];

    var PHA = getAttributesBattleLocal(player.Equipped.HELMET.SL, ItemModelsMapTest[player.Equipped.HELMET.ID]);
    var PBA = getAttributesBattleLocal(player.Equipped.ARMOR.SL, ItemModelsMapTest[player.Equipped.ARMOR.ID]);
    var PWA = getAttributesBattleLocal(player.Equipped.WEAPON.SL, ItemModelsMapTest[player.Equipped.WEAPON.ID]);
    var AEA = getWarriorAttributes(player.Level * 10, EIDarray);

    var WDP = PWA[Attributes.WEAPON_DAMAGE];
    var UDP = PBA[Attributes.UNARMED_DAMAGE];
    var MPP = PHA[Attributes.MAGIC_POWER];
    var BDP = PBA[Attributes.BODY_DEFENSE];
    var HDP = PHA[Attributes.HEAD_DEFENSE];
    var WDE = AEA[Attributes.WEAPON_DAMAGE];
    var UDE = AEA[Attributes.UNARMED_DAMAGE];
    var MPE = AEA[Attributes.MAGIC_POWER];
    var BDE = AEA[Attributes.BODY_DEFENSE];
    var HDE = AEA[Attributes.HEAD_DEFENSE];

    var DiffWithoutItem = calcFightDifficulty(WDP, UDP, MPP, BDP, HDP, WDE, UDE, MPE, BDE, HDE);

    if(ItemModelsMapTest[itemID].ItemType == ITEMTYPE.HELMET){
        PHA = getAttributesBattleLocal(itemSL, ItemModelsMapTest[itemID]);
    }
    if(ItemModelsMapTest[itemID].ItemType == ITEMTYPE.ARMOR){
        PBA = getAttributesBattleLocal(itemSL, ItemModelsMapTest[itemID]);
    }
    if(ItemModelsMapTest[itemID].ItemType == ITEMTYPE.WEAPON){
        PWA = getAttributesBattleLocal(itemSL, ItemModelsMapTest[itemID]);
    }

    WDP = PWA[Attributes.WEAPON_DAMAGE];
    UDP = PBA[Attributes.UNARMED_DAMAGE];
    MPP = PHA[Attributes.MAGIC_POWER];
    BDP = PBA[Attributes.BODY_DEFENSE];
    HDP = PHA[Attributes.HEAD_DEFENSE];

    var DiffWithItem = calcFightDifficulty(WDP, UDP, MPP, BDP, HDP, WDE, UDE, MPE, BDE, HDE);
    return DiffWithoutItem / DiffWithItem
}

function gatherStats(ammountOfGames, player, avBattle, battleMap) {
    var stats = {
        1:{
            weapon: 0,
            helmet: 0,
            armor: 0
        },
        2:{
            weapon: 0,
            helmet: 0,
            armor: 0
        },
        3:{
            weapon: 0,
            helmet: 0,
            armor: 0
        },
        4:{
            weapon: 0,
            helmet: 0,
            armor: 0
        }
    };

    for (var i = 0; i < ammountOfGames; i++){
        FightAll(player, avBattle, battleMap);

        for (var j = 1; j < 6; j++){
            if (player.Equipped.WEAPON.Rarity == j){
                stats[j].weapon += 1;
            }
        }

        for (var b = 1; b < 6; b++){
            if (player.Equipped.HELMET.Rarity == b){
                stats[b].helmet += 1;
            }
        }

        for (var c = 1; c < 6; c++){
            if (player.Equipped.ARMOR.Rarity == c){
                stats[c].armor += 1;
            }
        }

        player = {
            stop: false,
            Skill: 0.2,
            Level: 2,
            Experience: 0,
            Currency: {COIN: 160, BONUS: 0, SHADOW: 0},
            Perksmap: {},
            Itemsmap: {},
            Equipped: {}
        };

        giveAndEquipPlayerWithStandartItems(player);
        AvailableBattles.length = 0;
        AvailableBattles.push(1, 17, 18, 19)
    }
    var helmets = stats[1].helmet + stats[2].helmet + stats[3].helmet + stats[4].helmet;
    var armors = stats[1].armor + stats[2].armor + stats[3].armor + stats[4].armor;
    var weapons = stats[1].weapon + stats[2].weapon + stats[3].weapon + stats[4].weapon;
    var finalStats = {
        helmet: {C: String(stats[1].helmet / helmets), R: String(stats[2].helmet / helmets), E: stats[3].helmet / helmets, L: stats[4].helmet / helmets},
        armor: {C: stats[1].armor / armors, R: stats[2].armor / armors, E: stats[3].armor / armors, L: stats[4].armor / armors},
        weapon: {C: stats[1].weapon / weapons, R: stats[2].weapon / weapons, E: stats[3].weapon / weapons, L: stats[4].weapon / weapons}
    };
    console.log(finalStats);
}

//fightFirst(PlayerContent, AvailableBattles, BattlesTestMap);

FightAll(PlayerContent, AvailableBattles, BattlesTestMap);
console.log(PlayerContent.Itemsmap);
console.log(findStrongestAvalibleItem(getRefreshedShop(PlayerContent.Level), PlayerContent, 2));

//gatherStats(22, PlayerContent, AvailableBattles, BattlesTestMap);
//console.log(calcRarityItemCoinMultiplier(3) * baseCoinPrice);

//console.log(calcRarityItemCoinMultiplier(3) * baseCoinPrice * Math.pow(moneyInflation, 2));
//console.log(calcRarityItemBonusBasePrice(3) * baseBonusPrice);


//cards stack counter
var N = 100000;
var NCount = 30;
var buckets = 12;

var stats = {};

for (var i = 0; i < N; ++i) {
    var result = [];

    for (var k = 0; k < buckets; ++k) {
        result.push(0);
    }

    for (var count = 0; count < NCount; ++count) {
        var bucket = Math.floor(Math.random() * buckets);

        ++result[bucket];
    }

    var maximum = 0;

    for (k = 0; k < result.length; ++k) {
        maximum = Math.max(maximum, result[k]);
    }

    if (!stats[maximum]) {
        stats[maximum] = 1;
    } else  {
        ++stats[maximum];
    }
}

var text = "";

for (var num in stats) {
    if(stats.hasOwnProperty(num)) {
        stats[num] /= N;
        text += "(" + num + ";" + stats[num] + "), ";
    }
}

function getRefreshedShop(level){
    var maxshoplevel;

    if (level < 3){
        maxshoplevel = level;
    }
    else{
        maxshoplevel = 3;
    }

    var LEG = createShopGroup(createItemGroup(ItemModelsTest, FACTION.LEGION, ANY, RARITY.LEGENDARY, 1, maxshoplevel));
    var EPI = createShopGroup(createItemGroup(ItemModelsTest, FACTION.LEGION, ANY, RARITY.EPIC, 1, maxshoplevel));
    var RAR = createShopGroup(createItemGroup(ItemModelsTest, FACTION.LEGION, ANY, RARITY.RARE, 1, maxshoplevel));

    var item1 = makeChoice(LEG);

    var item2 = makeChoice(EPI);
    var ItemIndex = EPI.indexOf(item2);
    EPI.splice(ItemIndex, 1);
    var item3 = makeChoice(EPI);
    ItemIndex = EPI.indexOf(item3);
    EPI.splice(ItemIndex, 1);
    var item4 = makeChoice(EPI);

    var item5 = makeChoice(RAR);
    ItemIndex = RAR.indexOf(item5);
    RAR.splice(ItemIndex, 1);
    var item6 = makeChoice(RAR);

    console.log([item1[1].ID, item2[1].ID, item3[1].ID, item4[1].ID, item5[1].ID, item6[1].ID]);
    return [item1[1].ID, item2[1].ID, item3[1].ID, item4[1].ID, item5[1].ID, item6[1].ID];
}//Returns ID array

function getPriceAvailebleItems(shop, player){
    var result = [];

    for (var obj in shop) {
        if (shop.hasOwnProperty(obj)) {
            if ((obj.Price != 0) && obj.Price <= player.Currency.COIN){
                result.push(obj)
            }
        }
    }
}

function findSatisfyingItems(availableItems, player){
    var result = [];

    for (var obj in availableItems) {
        if (availableItems.hasOwnProperty(obj)) {
            if (obj.Profit > player.expectation){
                result.push(obj)
            }
        }
    }
}

function findSatisfyingItem(statisfyingItems){
    var best;
    for (var obj in statisfyingItems) {
        if (statisfyingItems.hasOwnProperty(obj)) {
            if ((best == undefined) || best < obj.Profit){
                best = obj.Profit
            }
        }
    }
}

function findStrongestAvalibleItem(shopIDs, player, logObj) {

    var availableItems = getPriceAvailebleItems(shopIDs, player);

    if (availableItems.length == 0) {
        logObj.log = "no_money";
        return null
    }

    var statisfyingItems = findSatisfyingItems(availableItems, player);

    if (statisfyingItems.length == 0) {
        logObj.log = "nothing_good";
        return null
    }

    return findSatisfyingItem(statisfyingItems);

}

function makeNiceObj(shopIDs, player){
/!*var NiceObj = {Type: [], SLcurr: [], SLupgr: [], Price: [], Power: [], Profit: []};*!/
var NiceArr = [];
    /!*for (var i = 0 ; i < shopIDs.length ; i++){

        if (player.Itemsmap[shopIDs[i]] != undefined){
            NiceObj.SLcurr.push(player.Itemsmap[shopIDs[i]].SL)
        }

        else{
            NiceObj.SLcurr.push(0)
        }
        NiceObj.Type.push(ItemModelsMapTest[shopIDs[i]].ItemType);
        NiceObj.Price.push(calcShopItemParametersLocal(player.Level, ItemModelsMapTest[shopIDs[i]], NiceObj.SLcurr[i]).Price[CURRENCY.COIN]);
        NiceObj.SLupgr.push(calcShopItemParametersLocal(player.Level, ItemModelsMapTest[shopIDs[i]], NiceObj.SLcurr[i]).SL);
        NiceObj.Power.push(calcItemPower(player, shopIDs[i], NiceObj.SLupgr[i]));

        if(NiceObj.Price[i] != 0){
            NiceObj.Profit.push(NiceObj.Power[i] / NiceObj.Price[i] * 1000);

        }

        else{
            NiceObj.Profit.push(-1);
        }
    }*!/
    for (var i = 0 ; i < shopIDs.length ; i++){

        if (player.Itemsmap[shopIDs[i]] != undefined){
            NiceArr[i].SLcurr = (player.Itemsmap[shopIDs[i]].SL)
        }

        else{
            NiceArr[i].SLcurr = (0)
        }
        NiceArr[i].ID = shopIDs[i];
        NiceArr[i].Type = (ItemModelsMapTest[shopIDs[i]].ItemType);
        NiceArr[i].Price = (calcShopItemParametersLocal(player.Level, ItemModelsMapTest[shopIDs[i]], NiceArr[i].SLcurr).Price[CURRENCY.COIN]);
        NiceArr[i].SLupgr = (calcShopItemParametersLocal(player.Level, ItemModelsMapTest[shopIDs[i]], NiceArr[i].SLcurr).SL);
        NiceArr[i].Power = (calcItemPower(player, shopIDs[i], NiceArr[i].SLupgr));

        if(NiceArr.Price[i] != 0){
            NiceArr[i].Profit = (NiceArr[i].Power / NiceArr[i].Price * 1000);

        }

        else{
            NiceArr[i].Profit = -1;
        }
    }
    return NiceArr
}
//console.log(getRefreshedShop(1));
*/
