var MoreWeaponDamage = {};
MoreWeaponDamage[Attributes.WEAPON_DAMAGE] = baseModelAttributeShift;
MoreWeaponDamage[Attributes.CRITICAL_CHANCE] = -baseModelCritShift;

var MoreCriticalChance = {};
MoreCriticalChance[Attributes.WEAPON_DAMAGE] = -baseModelAttributeShift;
MoreCriticalChance[Attributes.CRITICAL_CHANCE] = baseModelCritShift;

var MoreUnarmedDamage = {};
MoreUnarmedDamage[Attributes.BODY_DEFENSE] = -baseModelAttributeShift;
MoreUnarmedDamage[Attributes.UNARMED_DAMAGE] = baseModelAttributeShift;

var MoreBodyDefense = {};
MoreBodyDefense[Attributes.BODY_DEFENSE] = baseModelAttributeShift;
MoreBodyDefense[Attributes.UNARMED_DAMAGE] = -baseModelAttributeShift;

var MoreMagicPower = {};
MoreMagicPower[Attributes.MAGIC_POWER] = baseModelAttributeShift;
MoreMagicPower[Attributes.HEAD_DEFENSE] = -baseModelAttributeShift;

var MoreHeadDefense = {};
MoreHeadDefense[Attributes.MAGIC_POWER] = -baseModelAttributeShift;
MoreHeadDefense[Attributes.HEAD_DEFENSE] = baseModelAttributeShift;

var ModelsShifts = {
    MoreWeaponDamage: MoreWeaponDamage,
    MoreCriticalChance: MoreCriticalChance,
    MoreUnarmedDamage: MoreUnarmedDamage,
    MoreBodyDefense: MoreBodyDefense,
    MoreMagicPower: MoreMagicPower,
    MoreHeadDefense: MoreHeadDefense
};
//noinspection JSUnusedGlobalSymbols
var ItemModels = {
    //Weapons
    WPN_NUNCHAKU_01: {
        NoShop: 1,
        ID: 2,
        Level: 5,
        Model: "wpn__nunchaku_01_01",
        Image: "wpn__nunchaku_01_01",
        Alias: "wpn__nunchaku_01_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        EnemyShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_NUNCHAKU_02: {
        NoShop: 1,
        ID: 3,
        Level: 11,
        Model: "wpn__nunchaku_01_02",
        Image: "wpn__nunchaku_01_02",
        Alias: "wpn__nunchaku_01_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_TWOHANDEDSWORD_01: {
        NoShop: 1,
        ID: 4,
        Level: 22,
        Model: "wpn__twohanded_sword_01_01",
        Image: "wpn__twohanded_sword_01_01",
        Alias: "wpn__twohanded_sword_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_TWOHANDEDSWORD_02: {
        ID: 5,
        Level: 3,
        Model: "wpn__twohanded_sword_01_02",
        Image: "wpn__twohanded_sword_01_02",
        Alias: "wpn__twohanded_sword_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_TWOHANDEDSWORD_03: {
        ID: 30,
        Level: 2,
        Model: "wpn__twohanded_sword_01_03",
        Image: "wpn__twohanded_sword_01_03",
        Alias: "wpn__twohanded_sword_01_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_TWOHANDEDSWORD_04: {
        NoShop: 1,
        ID: 6,
        Level: 25,
        Model: "wpn__twohanded_sword_01_04",
        Image: "wpn__twohanded_sword_01_04",
        Alias: "wpn__twohanded_sword_01_04",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_SWORD", "SLASHING"],
        ShadowTag: ["SHADOW_TWOHANDED_SWORD_SLASH"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_STAFF_04: {
        NoShop: 1,
        ID: 8,
        Level: 11,
        Model: "wpn__staff_01_01",
        Image: "wpn__staff_01_01",
        Alias: "wpn__staff_01_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["STAFF", "BLUNT"],
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"],
        EnemyShadowMark: "SHADOW_STAFF_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_STAFF_03: {
        NoShop: 1,
        ID: 9,
        Level: 11,
        Model: "wpn__staff_01_02",
        Image: "wpn__staff_01_02",
        Alias: "wpn__staff_01_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["STAFF", "BLUNT"],
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"],
        ShadowMark: "SHADOW_STAFF_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_STAFF_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_AXES_01: {
        NoShop: 1,
        ID: 12,
        Level: 19,
        Model: "wpn__axes_01_01",
        Image: "wpn__axes_01_01",
        Alias: "wpn__axes_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        EnemyShadowMark: "SHADOW_AXES_THROW",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_AXES_02: {
        ID: 13,
        Level: 1,
        Model: "wpn__axes_01_02",
        Image: "wpn__axes_01_02",
        Alias: "wpn__axes_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_AXES_THROW",
        EnemyShadowMark: "SHADOW_AXES_THROW",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_AXES_03: {
        NoShop: 1,
        ID: 14,
        Level: 19,
        Model: "wpn__axes_01_03",
        Image: "wpn__axes_01_03",
        Alias: "wpn__axes_01_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_AXES_THROW",
        EnemyShadowMark: "SHADOW_AXES_THROW",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_SABERS_01_01: {
        NoShop: 1,
        ID: 15,
        Level: 11,
        Model: "wpn__sabers_01_01",
        Image: "wpn__sabers_01_01",
        Alias: "wpn__sabers_01_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SABERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        EnemyShadowMark: "SHADOW_SABERS_BLINK",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_SABERS_01_02: {
        NoShop: 1,
        ID: 16,
        Level: 4,
        Model: "wpn__sabers_01_02",
        Image: "wpn__sabers_01_02",
        Alias: "wpn__sabers_01_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SABERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SABERS_BLINK",
        EnemyShadowMark: "SHADOW_SABERS_BLINK",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_SABERS_01_03: {
        NoShop: 1,
        ID: 17,
        Level: 15,
        Model: "wpn__sabers_01_03",
        Image: "wpn__sabers_01_03",
        Alias: "wpn__sabers_01_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SABERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SABERS_BLINK",
        EnemyShadowMark: "SHADOW_SABERS_BLINK",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_SABERS_01_04: {
        NoShop: 1,
        ID: 18,
        Level: 5,
        Model: "wpn__sabers_01_04",
        Image: "wpn__sabers_01_04",
        Alias: "wpn__sabers_01_04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SABERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SABERS_BLINK",
        EnemyShadowMark: "SHADOW_SABERS_BLINK",
        Attributes: ModelsShifts.MoreWeaponDamage
     },
    WPN_GUANDAO_01_01: {
        NoShop: 1,
        ID: 24,
        Level: 15,
        Model: "wpn__guandao_01_01",
        Image: "wpn__guandao_01_01",
        Alias: "wpn__guandao_01_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["GUANDAO", "SLASHING"],
        EnemyShadowMark: "SHADOW_GUANDAO_SPINNING_SLASH_1",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_GUANDAO_01_02: {
        NoShop: 1,
        ID: 25,
        Level: 6,
        Model: "wpn__guandao_01_02",
        Image: "wpn__guandao_01_02",
        Alias: "wpn__guandao_01_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["GUANDAO", "SLASHING"],
        ShadowMark: "SHADOW_GUANDAO_SPINNING_SLASH_1",
        EnemyShadowMark: "SHADOW_GUANDAO_SPINNING_SLASH_1",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_GUANDAO_01_03: {
        NoShop: 1,
        ID: 26,
        Level: 18,
        Model: "wpn__guandao_01_03",
        Image: "wpn__guandao_01_03",
        Alias: "wpn__guandao_01_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["GUANDAO", "SLASHING"],
        ShadowMark: "SHADOW_GUANDAO_SPINNING_SLASH_1",
        EnemyShadowMark: "SHADOW_GUANDAO_SPINNING_SLASH_1",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_ONEHANDED_SWORD_01_01: {
        ID: 20,
        Level: 2,
        Model: "wpn__onehanded_sword_01_01",
        Image: "wpn__onehanded_sword_01_01",
        Alias: "wpn__onehanded_sword_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORD_LONELY", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        EnemyShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_ONEHANDED_SWORD_01_02: {
        NoShop: 1,
        ID: 21,
        Level: 13,
        Model: "wpn__onehanded_sword_01_02",
        Image: "wpn__onehanded_sword_01_02",
        Alias: "wpn__onehanded_sword_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORD_LONELY", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_ONEHANDED_SWORD_01_03: {
        NoShop: 1,
        ID: 22,
        Level: 28,
        Model: "wpn__onehanded_sword_01_03",
        Image: "wpn__onehanded_sword_01_03",
        Alias: "wpn__onehanded_sword_01_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORD_LONELY", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_ONEHANDED_SWORD_01_04: {
        NoShop: 1,
        ID: 23,
        Level: 25,
        Model: "wpn__onehanded_sword_01_04",
        Image: "wpn__onehanded_sword_01_04",
        Alias: "wpn__onehanded_sword_01_04",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORD_LONELY", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_DUAL_SWORDS_01_01: {
        ID: 27,
        Level: 1,
        Model: "wpn__dual_swords_01_01",
        Image: "wpn__dual_swords_01_01",
        Alias: "wpn__dual_swords_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORDS", "SLASHING"],
        EnemyShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_DUAL_SWORDS_01_02: {
        NoShop: 1,
        ID: 28,
        Level: 19,
        Model: "wpn__dual_swords_01_02",
        Image: "wpn__dual_swords_01_02",
        Alias: "wpn__dual_swords_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORDS", "SLASHING"],
        ShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        EnemyShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_DUAL_SWORDS_01_03: {
        NoShop: 1,
        ID: 29,
        Level: 19,
        Model: "wpn__dual_swords_01_03",
        Image: "wpn__dual_swords_01_03",
        Alias: "wpn__dual_swords_01_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORDS", "SLASHING"],
        ShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        EnemyShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_NUNCHAKU_03: {
        NoShop: 1,
        ID: 35,
        Level: 21,
        Model: "wpn__nunchaku_01_03",
        Image: "wpn__nunchaku_01_03",
        Alias: "wpn__nunchaku_01_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_NUNCHAKU_04: {
        NoShop: 1,
        ID: 36,
        Level: 4,
        Model: "wpn__nunchaku_01_04",
        Image: "wpn__nunchaku_01_04",
        Alias: "wpn__nunchaku_01_04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_NUNCHAKU_05: {
        NoShop: 1,
        ID: 37,
        Level: 15,
        Model: "wpn__nunchaku_01_05",
        Image: "wpn__nunchaku_01_05",
        Alias: "wpn__nunchaku_01_05",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_NUNCHAKU_06: {
        NoShop: 1,
        ID: 38,
        Level: 18,
        Model: "wpn__nunchaku_01_06",
        Image: "wpn__nunchaku_01_06",
        Alias: "wpn__nunchaku_01_06",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["NUNCHAKU", "BLUNT"],
        ShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_NUNCHAKU_SPINNING_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_AXES_02_01: {
        NoShop: 1,
        ID: 43,
        Level: 25,
        Model: "wpn__axes_02_01",
        Image: "wpn__axes_02_01",
        Alias: "wpn__axes_02_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        EnemyShadowMark: "SHADOW_AXES_THROW",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_AXES_02_02: {
        NoShop: 1,
        ID: 44,
        Level: 16,
        Model: "wpn__axes_02_02",
        Image: "wpn__axes_02_02",
        Alias: "wpn__axes_02_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_AXES_THROW",
        EnemyShadowMark: "SHADOW_AXES_THROW",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_AXES_02_03: {
        ID: 45,
        Level: 3,
        Model: "wpn__axes_02_03",
        Image: "wpn__axes_02_03",
        Alias: "wpn__axes_02_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["AXES", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_AXES_THROW",
        EnemyShadowMark: "SHADOW_AXES_THROW",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_SPEAR_01_01: {
        ID: 46,
        Level: 3,
        Model: "wpn__spear_01_01",
        Image: "wpn__spear_01_01",
        Alias: "wpn__spear_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SPEAR", "SLASHING"],
        EnemyShadowMark: "SHADOW_SPEAR_SLASH",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_SPEAR_01_02: {
        ID: 47,
        Level: 2,
        Model: "wpn__spear_01_02",
        Image: "wpn__spear_01_02",
        Alias: "wpn__spear_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SPEAR", "SLASHING"],
        ShadowMark: "SHADOW_SPEAR_SLASH",
        EnemyShadowMark: "SHADOW_SPEAR_SLASH",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_SPEAR_01_03: {
        NoShop: 1,
        ID: 48,
        Level: 22,
        Model: "wpn__spear_01_03",
        Image: "wpn__spear_01_03",
        Alias: "wpn__spear_01_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SPEAR", "SLASHING"],
        ShadowMark: "SHADOW_SPEAR_SLASH",
        EnemyShadowMark: "SHADOW_SPEAR_SLASH",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_SPEAR_01_04: {
        NoShop: 1,
        ID: 49,
        Level: 16,
        Model: "wpn__spear_01_04",
        Image: "wpn__spear_01_04",
        Alias: "wpn__spear_01_04",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SPEAR", "SLASHING"],
        ShadowMark: "SHADOW_SPEAR_SLASH",
        EnemyShadowMark: "SHADOW_SPEAR_SLASH",
        ShadowTag: ["SHADOW_STAFF_SPINNING_SLASH"]
    },
    WPN_HAMMERS_01_01: {
        NoShop: 1,
        ID: 50,
        Level: 25,
        Model: "wpn__hammers_01_01",
        Image: "wpn__hammers_01_01",
        Alias: "wpn__hammers_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HAMMERS", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_HAMMERS_THROW",
        EnemyShadowMark: "SHADOW_HAMMERS_THROW",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_HAMMERS_01_02: {
        ID: 51,
        Level: 3,
        Model: "wpn__hammers_01_02",
        Image: "wpn__hammers_01_02",
        Alias: "wpn__hammers_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HAMMERS", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_HAMMERS_THROW",
        EnemyShadowMark: "SHADOW_HAMMERS_THROW",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_HAMMERS_02_01: {
        NoShop: 1,
        ID: 52,
        Level: 13,
        Model: "wpn__hammers_02_01",
        Image: "wpn__hammers_02_01",
        Alias: "wpn__hammers_02_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HAMMERS", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        EnemyShadowMark: "SHADOW_HAMMERS_THROW",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_HAMMERS_02_02: {
        ID: 53,
        Level: 1,
        Model: "wpn__hammers_02_02",
        Image: "wpn__hammers_02_02",
        Alias: "wpn__hammers_02_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HAMMERS", "AXES_HAMMERS", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_HAMMERS_THROW",
        EnemyShadowMark: "SHADOW_HAMMERS_THROW",
        Attributes: ModelsShifts.MoreWeaponDamage
    },
    WPN_ONEHANDED_SWORD_02_01: {
        NoShop: 1,
        ID: 54,
        Level: 22,
        Model: "wpn__onehanded_sword_02_01",
        Image: "wpn__onehanded_sword_02_01",
        Alias: "wpn__onehanded_sword_02_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORD_LONELY", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_ONEHANDED_SWORD_02_02: {
        ID: 55,
        Level: 1,
        Model: "wpn__onehanded_sword_02_02",
        Image: "wpn__onehanded_sword_02_02",
        Alias: "wpn__onehanded_sword_02_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORD_LONELY", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"],
        ShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        EnemyShadowMark: "SHADOW_SWORD_LONELY_SPINNING_SLASH",
        Attributes: ModelsShifts.MoreCriticalChance
    },
    WPN_DUAL_SWORDS_02_01: {
        ID: 56,
        Level: 2,
        Model: "wpn__dual_swords_02_01",
        Image: "wpn__dual_swords_02_01",
        Alias: "wpn__dual_swords_02_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["SWORDS", "SLASHING"],
        ShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        EnemyShadowMark: "SHADOW_DUAL_SWORDS_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_TWOHANDEDHAMMER_01_01: {
        NoShop: 1,
        ID: 60,
        Level: 10,
        Model: "wpn__twohanded_hammer_01_01",
        Image: "wpn__twohanded_hammer_01_01",
        Alias: "wpn__twohanded_hammer_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_HAMMER", "SLASHING"],
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_TWOHANDEDHAMMER_01_02: {
        NoShop: 1,
        ID: 61,
        Level: 13,
        Model: "wpn__twohanded_hammer_01_02",
        Image: "wpn__twohanded_hammer_01_02",
        Alias: "wpn__twohanded_hammer_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_HAMMER", "SLASHING"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_TWOHANDEDHAMMER_02_01: {
        NoShop: 1,
        ID: 62,
        Level: 10,
        Model: "wpn__twohanded_hammer_02_01",
        Image: "wpn__twohanded_hammer_02_01",
        Alias: "wpn__twohanded_hammer_02_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_HAMMER", "SLASHING"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_TWOHANDEDHAMMER_02_02: {
        NoShop: 1,
        ID: 63,
        Level: 13,
        Model: "wpn__twohanded_hammer_02_02",
        Image: "wpn__twohanded_hammer_02_02",
        Alias: "wpn__twohanded_hammer_02_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["TWOHANDED_HAMMER", "SLASHING"],
        ShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        EnemyShadowMark: "SHADOW_TWOHANDED_SWORD_SLASH",
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_HALBERD_01_01: {
        NoShop: 1,
        ID: 64,
        Level: 16,
        Model: "wpn__halberd_01_01",
        Image: "wpn__halberd_01_01",
        Alias: "wpn__halberd_01_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HALBERD", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_HALBERD_01_02: {
        NoShop: 1,
        ID: 65,
        Level: 22,
        Model: "wpn__halberd_01_02",
        Image: "wpn__halberd_01_02",
        Alias: "wpn__halberd_01_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HALBERD", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"]
    },
    WPN_HALBERD_01_03: {
        NoShop: 1,
        ID: 66,
        Level: 16,
        Model: "wpn__halberd_01_03",
        Image: "wpn__halberd_01_03",
        Alias: "wpn__halberd_01_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.WEAPON,
        Tags: ["HALBERD", "SLASHING"],
        ShadowTag: ["SHADOW_DASH"]
    },
    //Armors
    ARM_STR_05: {
        ID: 200,
        Level: 2,
        Model: "arm__str_05",
        Image: "arm__str_05",
        Alias: "arm__str_05",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        EnemyShadowMark: "SHADOW_THUD",
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_STR_04: {
        ID: 201,
        Level: 3,
        Model: "arm__str_04",
        Image: "arm__str_04",
        Alias: "arm__str_04",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_STRONG_ARMOR",
        EnemyShadowMark: "SHADOW_STRONG_ARMOR",
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_STR_03: {
        ID: 202,
        Level: 1,
        Model: "arm__str_03",
        Image: "arm__str_03",
        Alias: "arm__str_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        EnemyShadowMark: "SHADOW_STRONG_ARMOR",
        ShadowMark: "SHADOW_STRONG_ARMOR"
    },
    ARM_STR_02: {
        ID: 203,
        Level: 2,
        Model: "arm__str_02",
        Image: "arm__str_02",
        Alias: "arm__str_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_CARAPACE"],
        EnemyShadowMark: "SHADOW_STRONG_ARMOR",
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_AGL_01: {
        NoShop: 1,
        ID: 206,
        Level: 5,
        Model: "arm__agl_01",
        Image: "arm__agl_01",
        Alias: "arm__agl_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_BLINK"],
        ShadowMark: "SHADOW_BLINK",
        EnemyShadowMark: "SHADOW_BLINK",
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_GALEN: {
        NoShop: 1,
        ID: 207,
        Level: 5,
        Model: "arm__galen",
        Image: "arm__galen",
        Alias: "arm__galen",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_BLINK"],
        ShadowMark: "SHADOW_BLINK",
        EnemyShadowMark: "SHADOW_BLINK",
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_AGL_04: {
        NoShop: 1,
        ID: 208,
        Level: 4,
        Model: "arm__agl_04",
        Image: "arm__agl_04",
        Alias: "arm__agl_04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_MINE"],
        EnemyShadowMark: "SHADOW_BLINK",
        ShadowMark: "SHADOW_BLINK"
    },
    ARM_AGL_05: {
        NoShop: 1,
        ID: 209,
        Level: 5,
        Model: "arm__agl_05",
        Image: "arm__agl_05",
        Alias: "arm__agl_05",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_MINE"],
        EnemyShadowMark: "SHADOW_BLINK",
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_STR_07: {
        ID: 210,
        Level: 1,
        Model: "arm__str_07",
        Image: "arm__str_07",
        Alias: "arm__str_07",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_CARAPACE",
        EnemyShadowMark: "SHADOW_CARAPACE",
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_AGL_06: {
        NoShop: 1,
        ID: 211,
        Level: 4,
        Model: "arm__agl_06",
        Image: "arm__agl_06",
        Alias: "arm__agl_06",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_MINE"],
        EnemyShadowMark: "SHADOW_BLINK",
        ShadowMark: "SHADOW_BLINK"
    },
    ARM_STR_10: {
        ID: 212,
        Level: 3,
        Model: "arm__str_10",
        Image: "arm__str_10",
        Alias: "arm__str_10",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_THUD",
        EnemyShadowMark: "SHADOW_CARAPACE",
        Attributes: ModelsShifts.MoreBodyDefense
    },
    ARM_AGL_09: {
        NoShop: 1,
        ID: 213,
        Level: 6,
        Model: "arm__agl_09",
        Image: "arm__agl_09",
        Alias: "arm__agl_09",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_THUD"],
        EnemyShadowMark: "SHADOW_BLINK",
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_STR_12: {
        ID: 214,
        Level: 1,
        Model: "arm__str_12",
        Image: "arm__str_12",
        Alias: "arm__str_12",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG"],
        EnemyShadowMark: "SHADOW_THUD",
        ShadowTag: ["SHADOW_THUD"]
    },
    ARM_JUNE: {
        ID: 215,
        Level: 3,
        Model: "arm__june",
        Image: "arm__june",
        Alias: "arm__june",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_THUD"],
        NoShop: 1,
        EnemyShadowMark: "SHADOW_BLINK",
        ShadowMark: "SHADOW_BLINK",
        Attributes: ModelsShifts.MoreUnarmedDamage
    },
    ARM_STR_13: {
        ID: 216,
        Level: 1,
        Model: "arm__str_13",
        Image: "arm__str_13",
        Alias: "arm__str_13",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        EnemyShadowMark: "SHADOW_CARAPACE",
        ShadowMark: "SHADOW_CARAPACE"
    },
    ARM_STR_14: {
        NoShop: 1,
        ID: 217,
        Level: 3,
        Model: "arm__str_14",
        Image: "arm__str_14",
        Alias: "arm__str_14",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        EnemyShadowMark: "SHADOW_THUD",
        ShadowTag: ["SHADOW_THUD"]
    },
    ARM_STR_15: {
        ID: 218,
        Level: 3,
        Model: "arm__str_15",
        Image: "arm__str_15", //placeholder
        Alias: "arm__str_15",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        EnemyShadowMark: "SHADOW_CARAPACE",
        ShadowTag: ["SHADOW_THUD"],
        ShadowMark: "SHADOW_CARAPACE"
    },
    ARM_AGL_08: {
        NoShop: 1,
        ID: 219,
        Level: 4,
        Model: "arm__agl_05",
        Image: "arm__agl_05", //placeholder
        Alias: "arm__agl_05",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        EnemyShadowMark: "SHADOW_BLINK",
        ShadowTag: ["SHADOW_THUD"]
    },
    ARM_STR_11: {
        NoShop: 1,
        ID: 220,
        Level: 10,
        Model: "arm__str_15",
        Image: "arm__str_15", //placeholder
        Alias: "arm__str_15",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "STRONG", "ARMOR_METAL"],
        ShadowTag: ["SHADOW_THUD"],
        EnemyShadowMark: "SHADOW_THUD",
        ShadowMark: "SHADOW_THUD"
    },
    ARM_AGL_10: {
        NoShop: 1,
        ID: 221,
        Level: 4,
        Model: "arm__agl_01",
        Image: "arm__agl_01", //placeholder
        Alias: "arm__agl_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.ARMOR,
        Tags: ["ARMOR", "AGILE"],
        ShadowTag: ["SHADOW_THUD"],
        EnemyShadowMark: "SHADOW_BLINK",
        ShadowMark: "SHADOW_BLINK"
    },

    //Helmets
    HLM_TUTOR_01: {
        ID: 4010,
        Level: 1,
        Model: "helm__fake",
        Image: "helm__fake",
        Alias: "helm__fake",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["SKELETON", "TUTORIAL", "TUTORIAL_MOVEMENT_COMBOS"],
        NoShop: 1
    },
    HLM_STUDENT: {
        NoShop: 1,
        ID: 4011,
        Level: 4,
        Model: "helm__student",
        Image: "helm__student",
        Alias: "helm__student",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        EnemyShadowMark: "SHADOW_MINE",
        Tags: ["HELM"]
    },
    HLM_GUARDSMAN: {
        ID: 4012,
        Level: 3,
        Alias: "helm__guardsman",
        Model: "helm__guardsman",
        Image: "helm__guardsman",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        EnemyShadowMark: "SHADOW_MINE",
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_JUNE: {
        ID: 4013,
        Level: 3,
        Alias: "hair-june",
        Model: "hair-june",
        Image: "hair-june",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_01: {
        ID: 4014,
        Level: 1,
        Alias: "hair-01",
        Model: "hair-01",
        Image: "hair-01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_02: {
        ID: 4015,
        Level: 1,
        Alias: "hair__02",
        Model: "hair__02",
        Image: "hair__02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_03: {
        ID: 4016,
        Level: 1,
        Alias: "hair__03",
        Model: "hair__03",
        Image: "hair__03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_04: {
        ID: 4017,
        Level: 1,
        Alias: "hair__04",
        Model: "hair__04",
        Image: "hair__04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_05: {
        ID: 4018,
        Level: 1,
        Alias: "hair__05",
        Model: "hair__05",
        Image: "hair__05",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HAIR_ITU: {
        ID: 4019,
        Level: 1,
        Alias: "hair__Itu",
        Model: "hair__Itu",
        Image: "hair__Itu",
        Faction: FACTION.HERALDS,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"]
    },
    //
    HLM_STR_01: {
        ID: 400,
        Level: 1,
        Model: "helm__str_01",
        Image: "helm__str_01",
        Alias: "helm__str_01",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        EnemyShadowMark: "SHADOW_MIND_THROW",
        ShadowMark: "SHADOW_MIND_THROW"
    },
    HLM_STR_03: {
        ID: 401,
        Level: 2,
        Model: "helm__str_03",
        Image: "helm__str_03",
        Alias: "helm__str_03",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_POWER_WILL"],
        EnemyShadowMark: "SHADOW_UPPERCUT",
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_AGL_01: {
        NoShop: 1,
        ID: 402,
        Level: 5,
        Model: "helm__agl_01",
        Image: "helm__agl_01",
        Alias: "helm__agl_01",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        ShadowMark: "SHADOW_MINE",
        EnemyShadowMark: "SHADOW_MINE",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_AGL_02: {
        NoShop: 1,
        ID: 403,
        Level: 4,
        Model: "helm__agl_02",
        Image: "helm__agl_02",
        Alias: "helm__agl_02",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MINE"],
        EnemyShadowMark: "SHADOW_MINE",
        ShadowMark: "SHADOW_MINE"
    },
    HLM_STR_09: {
        ID: 406,
        Level: 1,
        Model: "helm__str_09",
        Image: "helm__str_09",
        Alias: "helm__str_09",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        EnemyShadowMark: "SHADOW_MIND_THROW",
        ShadowMark: "SHADOW_MIND_THROW"
    },
    HLM_STR_08: {
        ID: 407,
        Level: 2,
        Model: "helm__str_08",
        Image: "helm__str_08",
        Alias: "helm__str_08",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_UPPERCUT",
        EnemyShadowMark: "SHADOW_UPPERCUT",
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_STR_02: {
        ID: 408,
        Level: 3,
        Model: "helm__str_02",
        Image: "helm__str_02",
        Alias: "helm__str_02",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        EnemyShadowMark: "SHADOW_MIND_THROW",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_FAKE: {
        ID: 409,
        Level: 1,
        Model: "helm__fake",
        Image: "helm__fake",
        Alias: "helm__fake",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        NoShop: 1
    },
    HLM_STR_10: {
        ID: 410,
        Level: 3,
        Model: "helm__str_10",
        Image: "helm__str_10",
        Alias: "helm__str_10",
        Faction: FACTION.LEGION,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        EnemyShadowMark: "SHADOW_MIND_THROW",
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_STR_11: {
        ID: 411,
        Level: 3,
        Model: "helm__str_11",
        Image: "helm__str_11",
        Alias: "helm__str_11",
        Faction: FACTION.LEGION,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_UPPERCUT",
        EnemyShadowMark: "SHADOW_UPPERCUT",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_STR_12: {
        ID: 412,
        Level: 1,
        Model: "helm__str_12",
        Image: "helm__str_12",
        Alias: "helm__str_12",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        EnemyShadowMark: "SHADOW_UPPERCUT",
        ShadowTag: ["SHADOW_MIND_THROW"]
    },
    HLM_AGL_04: {
        NoShop: 1,
        ID: 413,
        Level: 4,
        Model: "helm__agl_04",
        Image: "helm__agl_04",
        Alias: "helm__agl_04",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MINE",
        EnemyShadowMark: "SHADOW_MINE",
        Attributes: ModelsShifts.MoreHeadDefense
    },
    HLM_AGL_03: {
        NoShop: 1,
        ID: 414,
        Level: 5,
        Model: "helm__agl_03",
        Image: "helm__agl_03",
        Alias: "helm__agl_03",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.LEGENDARY,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MINE",
        EnemyShadowMark: "SHADOW_MINE",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_AGL_05: {
        NoShop: 1,
        ID: 415,
        Level: 6,
        Model: "helm__agl_05",
        Image: "helm__agl_05",
        Alias: "helm__agl_05",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        EnemyShadowMark: "SHADOW_MINE",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_STR_14: {
        ID: 416,
        Level: 1,
        Model: "helm__str_14",
        Image: "helm__str_14",
        Alias: "helm__str_14",
        Faction: FACTION.LEGION,
        Rarity: RARITY.EPIC,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MIND_THROW",
        EnemyShadowMark: "SHADOW_MIND_THROW",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_STR_15: {
        ID: 417,
        Level: 3,
        Model: "helm__str_15",
        Image: "helm__str_15",
        Alias: "helm__str_15",
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        EnemyShadowMark: "SHADOW_UPPERCUT",
        Attributes: ModelsShifts.MoreMagicPower
    },
    HLM_AGL_10: {
        NoShop: 1,
        ID: 418,
        Level: 4,
        Model: "helm__agl_10",
        Image: "helm__agl_10",
        Alias: "helm__agl_10",
        Faction: FACTION.DYNASTY,
        Rarity: RARITY.RARE,
        ItemType: ITEMTYPE.HELMET,
        Tags: ["HELM"],
        ShadowTag: ["SHADOW_MIND_THROW"],
        ShadowMark: "SHADOW_MINE",
        EnemyShadowMark: "SHADOW_MINE",
        Attributes: ModelsShifts.MoreMagicPower
    },
    //Ranged
    //Default
    DefaultWeapon: {
        ID: 1000000,
        Level: 1,
        Alias: "default",
        Model: "wpn-fists",
        Image: "wpn-fists",
        Default: 1,
        ItemType: ITEMTYPE.WEAPON,
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        Tags: ["WEAPON", "FISTS"],
        Hidden: 1,
        NoShop: 1
    },
    DefaultWeaponShock: {
        ID: 1000001,
        Level: 1,
        Alias: "default",
        Model: "wpn-fists",
        Image: "wpn-fists",
        Default: 1,
        ItemType: ITEMTYPE.WEAPON,
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        Tags: ["WEAPON", "FISTS"],
        Hidden: 1,
        NoShop: 1
    },
    DefaultArmor: {
        ID: 1000002,
        Level: 1,
        Alias: "default",
        Model: "arm__base",
        Image: "arm__base",
        Default: 1,
        ItemType: ITEMTYPE.ARMOR,
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        Tags: ["ARMOR", "STRONG"],
        Hidden: 1,
        NoShop: 1
    },
    DefaultHelm: {
        ID: 1000003,
        Level: 1,
        Alias: "default",
        Model: "hair-01",
        Image: "hair-01",
        Default: 1,
        ItemType: ITEMTYPE.HELMET,
        Faction: FACTION.LEGION,
        Rarity: RARITY.COMMON,
        Tags: ["HELM"],
        Hidden: 1,
        NoShop: 1
    }
};

/**
 * Global map<Integer,ItemModel> of all models, each model should have unique ID
 */
var ItemModelsMap = createIDMap(ItemModels);

var zonesItems = [
    createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, ANY, 1, 3)
];

var zonesItemsLvl2 = [
    createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, ANY, 1, 2)
];

function calcItemPerkSlotsByModelId(modelId) {
    var model = ItemModelsMap[modelId];

    if (model.hasOwnProperty("Slots")) {
        return model["Slots"];
    } else {
        return getRarityTypePerkSlots(model);
    }
}

//region Item settings
function getRarityCardsForStepBasePerks(rarity){
    switch (rarity) {
        case RARITY.COMMON:
            return 5;
        case RARITY.RARE:
            return 5;
        case RARITY.EPIC:
            return 5;
        case RARITY.LEGENDARY:
            return 5;
        default:
            throw "getRarityCardsForStepBasePerks: unknown Rarity: " + rarity;
    }
}

function getRarityCardsForStepsInfPerks(rarity) {
    switch (rarity) {
        case RARITY.COMMON:
            return 2;
        case RARITY.RARE:
            return 2;
        case RARITY.EPIC:
            return 2;
        case RARITY.LEGENDARY:
            return 2;
        default:
            throw "getRarityCardsForSteps: unknown Rarity: " + rarity;
    }
}

function getRarityStepsPerLevel(rarity) {
    switch (rarity) {
        case RARITY.COMMON:
            return 5;
        case RARITY.RARE:
            return 4;
        case RARITY.EPIC:
            return 2;
        case RARITY.LEGENDARY:
            return 1;
        default:
            throw "getRarityStepsPerLevel: unknown Rarity: " + rarity;
    }
}

function getRarityStackPerStep(rarity){
    return stackPerLevel / getRarityStepsPerLevel(rarity);
}

function getRarityCardsForStepsBase(rarity) {
    switch (rarity) {
        case RARITY.COMMON:
            return 2;
        case RARITY.RARE:
            return 1;
        case RARITY.EPIC:
            return 1;
        case RARITY.LEGENDARY:
            return 1;
        default:
            throw "getRarityCardsForStepsBase: unknown Rarity: " + rarity;
    }
}

function getRarityCardsInflation(rarity){
    switch (rarity) {
        case RARITY.COMMON:
            return 4;
        case RARITY.RARE:
            return 3;
        case RARITY.EPIC:
            return 2.5;
        case RARITY.LEGENDARY:
            return 2;
        default:
            throw "getRarityCardsInflation: unknown Rarity: " + rarity;
    }
}

function calcRarityCardsForStepInf(rarity) {
    var cardInflation = getRarityCardsInflation(rarity);
    var stepsInLevel = getRarityStepsPerLevel(rarity);

    return Math.pow(cardInflation, 1 / stepsInLevel);
}

function getRarityTypePerkSlots(model) {
    var rarity = model.Rarity;
    var itemType = model.ItemType;

    switch (itemType) {
        case ITEMTYPE.WEAPON:
        case ITEMTYPE.ARMOR:
            switch (rarity) {
                case RARITY.COMMON:
                    return 1;
                case RARITY.RARE:
                    return 2;
                case RARITY.EPIC:
                case RARITY.LEGENDARY:
                    return 3;
                default:
                    throw "getRarityTypePerkSlots: unknown Rarity: " + rarity;
            }
        case ITEMTYPE.HELMET:
        case ITEMTYPE.RANGED:
            switch (rarity) {
                case RARITY.COMMON:
                    return 0;
                case RARITY.RARE:
                    return 1;
                case RARITY.EPIC:
                case RARITY.LEGENDARY:
                    return 2;
                default:
                    throw "getRarityTypePerkSlots: unknown Rarity: " + rarity;
            }
        default:
            throw "getRarityTypePerkSlots: unknown Type: " + itemType;
    }
}

function getRarityStackLevelShift(rarity) {
    switch (rarity) {
        case RARITY.COMMON:
            return 0;
        case RARITY.RARE:
            return 0.2 * stackPerLevel ;
        case RARITY.EPIC:
            return 0.6 * stackPerLevel;
        case RARITY.LEGENDARY:
            return 1 * stackPerLevel;
        default:
            throw "getRarityStackLevelShift: unknown Rarity: " + rarity;
    }
}
//endregion

function createItemGroup(itemPool, faction, itemType, rarity, minLevel, maxLevel) {
    var result = {};

    for (var rarityName in RARITY) {
        if (RARITY.hasOwnProperty(rarityName)) {
            result[RARITY[rarityName]] = [];
        }
    }

    for (var itemID in itemPool) {
        if (itemPool.hasOwnProperty(itemID)) {
            var item = itemPool[itemID];

            if (item.NoShop == 1) {
                continue;
            }

            if (checkItemRestrictions(item, faction, itemType, rarity, minLevel, maxLevel)) {
                result[item.Rarity].push(item);
            }
        }
    }

    return result;
}

function checkItemRestrictions(item, faction, itemType, rarity, minLevel, maxLevel) {
    var eq = function(a, b) {return a == b;};
    var ge = function(a, b) {return a >= b;};
    var le = function(a, b) {return a <= b;};

    return checkObjProperty(item, "Faction", faction, eq) &&
        checkObjProperty(item, "ItemType", itemType, eq) &&
        checkObjProperty(item, "Rarity", rarity, eq) &&
        checkObjProperty(item, "Level", minLevel, ge) &&
        checkObjProperty(item, "Level", maxLevel, le);
}

function checkObjProperty(obj, property, value, comparator) {
    return (value == ANY || comparator(obj[property], value));
}
