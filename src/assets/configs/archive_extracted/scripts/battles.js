var levelPrecision = 20;

var baseWarriorPower = 20;
var baseMainExp = 15;
var baseMainCoins = 20 * coinBase;
var baseSideExp = 10;
var baseSideCoins = 17.5 * coinBase;
var baseMissionExp = 15;
var baseMissionCoins = 17.5 * coinBase;
var baseSurviveExp = 0;
var baseSurviveCoins = 7.5 * coinBase;
var baseSurviveRatio = 20;
var baseItemRatio = 20;

var missionsEasyRatio = 0.5;
var missionsMediumRatio = 0.75;
var missionsHardRatio = 1.15;

var easyBonusReward = 0;
var normalBonusReward = 0;
var hardBonusReward = 0;

var Icons = {
    daily: "daily",
    periodic: "periodic",
    survival: "survival",
    side: "side",
    story: "story",
    boss: "boss"
};

var Locations = {
    Dojo_Legion: {
        X: 344,
        Y: 560,
        LocationName: "dojo_legion",
        Music: "Chapter 1/4_dojo_legion_fight"
    },
    Dojo_Dojo_Legion: {
        X: 344,
        Y: 560,
        LocationName: "dojo_legion",
        Music: "Chapter 1/1_dojo_legion"
    },
    dojo_dynasty: {
        X: 1536,
        Y: 994,
        LocationName: "dojo_dynasty",
        Music: "fight_dojo_dynasty"
    },
    Bamboo: {
        X: 1205,
        Y: 690,
        LocationName: "bamboo",
        Music: "Chapter 1/3_bamboo_forest"
    },
    Camp: {
        X: 670,
        Y: 470,
        LocationName: "camp",
        Music: "Chapter 1/5_legion_camp_fight"
    },
    Camp_Training: {
        X: 670,
        Y: 470,
        LocationName: "camp_training",
        Music: "Chapter 1/2_legion_camp_training"
    },
    Camp_Night: {
        X: 670,
        Y: 470,
        LocationName: "camp_night",
        Music: "Chapter 1/8_legion_camp_night"
    },
    Camp_Boss: {
        X: 670,
        Y: 470,
        LocationName: "camp",
        Music: "Chapter 1/7_boss"
    },
    Gorge: {
        X: 1450,
        Y: 390,
        LocationName: "cave",
        Music: "Chapter 1/6_bridge"
    },
    Temple: {
        X: 1096,
        Y: 1004,
        LocationName: "temple",
        Music: "theme_one_more_breath"
    },
    Brawler: {
        X: 1196,
        Y: 1004,
        LocationName: "bamboo",
        Music: "theme_one_more_breath"
    },
    Dojo_Legion_test: {
        X: 1000,
        Y: 800,
        LocationName: "dojo_legion",
        Music: "theme_one_more_breath"
    },
    Camp_Night_test: {
        X: 1100,
        Y: 800,
        LocationName: "camp_night",
        Music: "theme_one_more_breath"
    },
    Arena_test: {
        X: 1200,
        Y: 800,
        LocationName: "arena",
        Music: "theme_one_more_breath"
    },
    Temple_test: {
        X: 1300,
        Y: 800,
        LocationName: "temple",
        Music: "theme_one_more_breath"
    },
    Daily_Arena: {
        X: 1500,
        Y: 750,
        LocationName: "temple",
        Music: "theme_one_more_breath"
    }
};

var Super_Crutch_All_Tags = [PerkModels.SUPER_TWOHANDED_SWORD_2.Leveling.SUPER_TWOHANDED_SWORD_2_1, PerkModels.SUPER_SWORDS_STRONG_3.Leveling.SUPER_SWORDS_STRONG_3_1, PerkModels.SUPER_AXES_1.Leveling.SUPER_AXES_1_1, PerkModels.SUPER_GUANDAO_2.Leveling.SUPER_GUANDAO_2_1, PerkModels.SUPER_STAFF_1.Leveling.SUPER_STAFF_1_1, PerkModels.SUPER_SWORD_1.Leveling.SUPER_SWORD_1_1, PerkModels.SUPER_SWORDS_AGILE_1.Leveling.SUPER_SWORDS_AGILE_1_1, PerkModels.SUPER_NUNCHAKU_1.Leveling.SUPER_NUNCHAKU_1_1, PerkModels.SUPER_SPEAR_2.Leveling.SUPER_SPEAR_2_1, PerkModels.SUPER_HAMMERS_2.Leveling.SUPER_HAMMERS_2_1, PerkModels.SUPER_ARMOR_STRONG_3.Leveling.SUPER_ARMOR_STRONG_3_1, PerkModels.SUPER_ARMOR_AGILE_2.Leveling.SUPER_ARMOR_AGILE_2_1];

var Super_Crutch_All_Tags_2 = [PerkModels.SUPER_TWOHANDED_SWORD_3.Leveling.SUPER_TWOHANDED_SWORD_3_1, PerkModels.SUPER_SWORDS_STRONG_2.Leveling.SUPER_SWORDS_STRONG_2_1, PerkModels.SUPER_AXES_2.Leveling.SUPER_AXES_2_1, PerkModels.SUPER_GUANDAO_1.Leveling.SUPER_GUANDAO_1_1, PerkModels.SUPER_STAFF_2.Leveling.SUPER_STAFF_2_1, PerkModels.SUPER_SWORD_2.Leveling.SUPER_SWORD_2_1, PerkModels.SUPER_SWORDS_AGILE_2.Leveling.SUPER_SWORDS_AGILE_2_1, PerkModels.SUPER_NUNCHAKU_2.Leveling.SUPER_NUNCHAKU_2_1, PerkModels.SUPER_SPEAR_3.Leveling.SUPER_SPEAR_3_1, PerkModels.SUPER_HAMMERS_1.Leveling.SUPER_HAMMERS_1_1, PerkModels.SUPER_ARMOR_STRONG_2.Leveling.SUPER_ARMOR_STRONG_2_1, PerkModels.SUPER_ARMOR_AGILE_1.Leveling.SUPER_ARMOR_AGILE_1_1];

var Warriors = {
    //MainQuestChapter1
    Hammerhead: {
        Alias: "CHAR_HAMMERHEAD",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_4a,
        Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_03, ItemModels.HLM_STR_12, ItemModels.ARM_STR_12],
        Perks: [PerkModels.PERK_STONE_SKIN.Leveling.PERK_STONE_SKIN_1, PerkModels.SUPER_SWORD_1.Leveling.SUPER_SWORD_1_2]
    },
    Outcast: {
        Alias: "CHAR_OUTCAST",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_5a,
        Equipments: [ItemModels.DefaultWeapon, ItemModels.HLM_STR_15, ItemModels.ARM_STR_14],
        Perks: [PerkModels.PERK_STEEL_FOOT.Leveling.PERK_STEEL_FOOT_5, PerkModels.SUPER_ARMOR_STRONG_3.Leveling.SUPER_ARMOR_STRONG_3_1]
    },
    Maul: {
        Alias: "CHAR_MAUL",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_5a,
        Equipments: [ItemModels.WPN_HAMMERS_02_01, ItemModels.HLM_STR_14, ItemModels.ARM_STR_13],
        Perks: [PerkModels.PERK_BERSERK.Leveling.PERK_BERSERK_1, PerkModels.SUPER_HAMMERS_1.Leveling.SUPER_HAMMERS_1_2]
    },
    Spade: {
        Alias: "CHAR_SPADE",
        Gender: GENDER.FEMALE,
        Appearance: WarriorAppearences.WA_1a,
        Equipments: [ItemModels.WPN_SPEAR_01_02, ItemModels.HLM_STR_03, ItemModels.ARM_STR_02],
        Perks: [PerkModels.PERK_IRON_GRIP.Leveling.PERK_IRON_GRIP_5, PerkModels.SUPER_SPEAR_2.Leveling.SUPER_SPEAR_2_1]
    },
    Avalanche: {
        Alias: "CHAR_AVALANCHE",
        Gender: GENDER.FEMALE,
        Appearance: WarriorAppearences.WA_2a,
        Equipments: [ItemModels.WPN_ONEHANDED_SWORD_02_02, ItemModels.HLM_STR_09, ItemModels.ARM_STR_03],
        Perks: [PerkModels.PERK_STEEL_FOOT.Leveling.PERK_STEEL_FOOT_1, PerkModels.SUPER_SWORD_1.Leveling.SUPER_SWORD_1_1]
    },
    Rascal: {
        Alias: "CHAR_RASCAL",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Scout,
        Equipments: [ItemModels.WPN_AXES_02_02, ItemModels.HLM_STR_11, ItemModels.ARM_STR_04],
        Perks: [PerkModels.PERK_ROCK.Leveling.PERK_ROCK_2, PerkModels.SUPER_AXES_1.Leveling.SUPER_AXES_1_1]
    },
    Boulder: {
        Alias: "CHAR_BOULDER",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_5a,
        Equipments: [ItemModels.WPN_DUAL_SWORDS_01_03, ItemModels.HLM_STR_02, ItemModels.ARM_STR_15],
        Perks: [PerkModels.PERK_STONE_SKIN.Leveling.PERK_STONE_SKIN_2, PerkModels.SUPER_SWORDS_STRONG_2.Leveling.SUPER_SWORDS_STRONG_2_2]
    },
    Greta_1: {
        Alias: "CHAR_GRETA",
        Gender: GENDER.FEMALE,
        Appearance: WarriorAppearences.WA_1a,
        Equipments: [ItemModels.WPN_TWOHANDEDSWORD_04, ItemModels.HLM_STR_08, ItemModels.ARM_STR_05],
        Perks: [PerkModels.PERK_REVENGE.Leveling.PERK_REVENGE_1, PerkModels.SUPER_TWOHANDED_SWORD_2.Leveling.SUPER_TWOHANDED_SWORD_2_1, PerkModels.SUPER_ARMOR_STRONG_1.Leveling.SUPER_ARMOR_STRONG_1_1]
    },
    June: {
        Alias: "CHAR_JUNE",
        Gender: GENDER.FEMALE,
        Appearance: WarriorAppearences.June,
        Equipments: [ItemModels.WPN_NUNCHAKU_03, ItemModels.HAIR_JUNE, ItemModels.ARM_JUNE],
        Perks: [PerkModels.SUPER_NUNCHAKU_1.Leveling.SUPER_NUNCHAKU_1_1, PerkModels.SUPER_ARMOR_AGILE_1.Leveling.SUPER_ARMOR_AGILE_1_1, PerkModels.PERK_CORNERED_CAT.Leveling.PERK_CORNERED_CAT_5]
    },
    Fort_Commander: {
        Alias: "CHAR_FORT_COMMANDER",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_3a,
        Equipments: [ItemModels.WPN_SPEAR_01_03, ItemModels.HAIR_01, ItemModels.ARM_AGL_05],
        Perks: [PerkModels.SUPER_SPEAR_2.Leveling.SUPER_SPEAR_2_2, PerkModels.PERK_SPURT.Leveling.PERK_SPURT_1]
    },
    Gizmo: {
        Alias: "CHAR_GIZMO",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Gizmo,
        Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01, ItemModels.HLM_FAKE, ItemModels.ARM_STR_05],
        Perks: [PerkModels.PERK_BREACHER.Leveling.PERK_BREACHER_2, PerkModels.PERK_STEEL_FOOT.Leveling.PERK_STEEL_FOOT_3, PerkModels.PERK_PREDATOR.Leveling.PERK_PREDATOR_3, PerkModels.SUPER_TWOHANDED_SWORD_1.Leveling.SUPER_TWOHANDED_SWORD_1_3]
    },
    Gizmo_Fists: {
        Alias: "CHAR_GIZMO",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Gizmo,
        Equipments: [ItemModels.DefaultWeapon, ItemModels.HLM_FAKE, ItemModels.ARM_STR_05],
        Perks: [PerkModels.PERK_BREACHER.Leveling.PERK_BREACHER_2, PerkModels.PERK_STEEL_FOOT.Leveling.PERK_STEEL_FOOT_3, PerkModels.PERK_PREDATOR.Leveling.PERK_PREDATOR_3]
    },
    Sarge: {
        Alias: "CHAR_SARGE",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Sarge,
        Equipments: [ItemModels.WPN_HAMMERS_01_02, ItemModels.HLM_FAKE, ItemModels.ARM_STR_07],
        Perks: [PerkModels.SUPER_HAMMERS_1.Leveling.SUPER_HAMMERS_1_2, PerkModels.PERK_BASHER.Leveling.PERK_BASHER_4, PerkModels.PERK_ROCK.Leveling.PERK_ROCK_5]
    },
    Ran_Di: {
        Alias: "CHAR_RANDI",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Ran_Di,
        Equipments: [ItemModels.WPN_NUNCHAKU_05, ItemModels.HLM_GUARDSMAN, ItemModels.ARM_AGL_06],
        Perks: [PerkModels.SUPER_NUNCHAKU_1.Leveling.SUPER_NUNCHAKU_1_1, PerkModels.PERK_FLURRY.Leveling.PERK_FLURRY_2, PerkModels.PERK_CORNERED_CAT.Leveling.PERK_CORNERED_CAT_3]
    },
    Guardsman: {
        Alias: "CHAR_GUARDSMAN",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_3a,
        Equipments: [ItemModels.WPN_GUANDAO_01_02, ItemModels.HLM_AGL_02, ItemModels.ARM_AGL_06],
        Perks: [PerkModels.SUPER_GUANDAO_1.Leveling.SUPER_GUANDAO_1_4, PerkModels.PERK_PARRY.Leveling.PERK_PARRY_3]
    },
    Mysterious_opponent: {
        Alias: "CHAR_MYSTERIOUS_OPPONENT",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.WA_3a,
        Equipments: [ItemModels.WPN_SABERS_01_03, ItemModels.HLM_FAKE, ItemModels.ARM_AGL_05],
        Perks: [PerkModels.SUPER_SWORDS_AGILE_2.Leveling.SUPER_SWORDS_AGILE_2_1]
    },
//
    Greta_2: {
        Alias: "CHAR_TRESPASSER",
        Gender: GENDER.FEMALE,
        Appearance: WarriorAppearences.WA_1a,
        Equipments: [ItemModels.WPN_AXES_02_03, ItemModels.HLM_STR_08, ItemModels.ARM_STR_05],
        Perks: [PerkModels.PERK_REVENGE.Leveling.PERK_REVENGE_1, PerkModels.SUPER_AXES_1.Leveling.SUPER_AXES_1_1, PerkModels.SUPER_ARMOR_STRONG_1.Leveling.SUPER_ARMOR_STRONG_1_1]
    },
    Legion_scout_1: {
        Alias: "CHAR_LEGION_SCOUT",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Scout,
        Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01, ItemModels.HLM_STR_01, ItemModels.ARM_STR_04],
        Perks: [PerkModels.PERK_BREACHER.Leveling.PERK_BREACHER_1, PerkModels.SUPER_SWORD_1.Leveling.SUPER_SWORD_1_1]
    },
    Legion_scout_2: {
        Alias: "CHAR_LEGION_SCOUT",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Scout,
        Equipments: [ItemModels.WPN_AXES_02_01, ItemModels.HLM_STR_03, ItemModels.ARM_STR_04],
        Perks: [PerkModels.PERK_BASHER.Leveling.PERK_BASHER_2, PerkModels.SUPER_AXES_1.Leveling.SUPER_AXES_1_1]
    },
    Galen: {
        Alias: "CHAR_Galen",
        Gender: GENDER.MALE,
        Appearance: WarriorAppearences.Galen,
        Equipments: [ItemModels.WPN_STAFF_03, ItemModels.HLM_FAKE, ItemModels.ARM_GALEN],
        Perks: [PerkModels.SUPER_STAFF_1.Leveling.SUPER_STAFF_1_1]
    }
};

var WarriorBlockPresets = {
    Wpn_Str_LowTier: [
        {Equipments: [ItemModels.DefaultWeapon]},
        {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
        {Equipments: [ItemModels.WPN_AXES_01]},
        {Equipments: [ItemModels.WPN_AXES_02_01]},
        {Equipments: [ItemModels.WPN_SPEAR_01_01]},
        {Equipments: [ItemModels.WPN_HAMMERS_01_01]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
        {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
        {Equipments: [ItemModels.WPN_AXES_02]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_04]},
        {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_02]}
    ],
    Wpn_Str_MedTier: [
        {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
        {Equipments: [ItemModels.WPN_AXES_01]},
        {Equipments: [ItemModels.WPN_AXES_02_01]},
        {Equipments: [ItemModels.WPN_SPEAR_01_01]},
        {Equipments: [ItemModels.WPN_SPEAR_01_02]},
        {Equipments: [ItemModels.WPN_HAMMERS_01_01]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_02]},
        {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
        {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_02]},
        {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_03]},
        {Equipments: [ItemModels.WPN_AXES_02]},
        {Equipments: [ItemModels.WPN_AXES_03]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_04]},
        {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_02]},
        {Equipments: [ItemModels.WPN_AXES_02_02]},
        {Equipments: [ItemModels.WPN_AXES_02_03]},
        {Equipments: [ItemModels.WPN_SPEAR_01_03]},
        {Equipments: [ItemModels.WPN_HAMMERS_01_02]}
    ],
    Wpn_Str_HighTier: [
        {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_04]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_03]},
        {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_03]},
        {Equipments: [ItemModels.WPN_SPEAR_01_04]},
        {Equipments: [ItemModels.WPN_HAMMERS_02_01]},
        {Equipments: [ItemModels.WPN_HAMMERS_02_02]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_02_01]},
        {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_02_02]},
        {Equipments: [ItemModels.WPN_DUAL_SWORDS_02_01]}
    ],
    Wpn_Agl_LowTier: [
        {Equipments: [ItemModels.DefaultWeapon]},
        {Equipments: [ItemModels.WPN_STAFF_04]},
        {Equipments: [ItemModels.WPN_SABERS_01_01]},
        {Equipments: [ItemModels.WPN_SABERS_01_03]},
        {Equipments: [ItemModels.WPN_GUANDAO_01_01]}
    ],
    Wpn_Agl_LowTier_NoFists: [
        {Equipments: [ItemModels.WPN_STAFF_04]},
        {Equipments: [ItemModels.WPN_SABERS_01_01]},
        {Equipments: [ItemModels.WPN_SABERS_01_03]},
        {Equipments: [ItemModels.WPN_GUANDAO_01_01]}
    ],
    Wpn_Agl_HighTier: [
        {Equipments: [ItemModels.WPN_SABERS_01_04]},
        {Equipments: [ItemModels.WPN_GUANDAO_01_03]},
        {Equipments: [ItemModels.WPN_STAFF_04]}
    ],
    Arm_Str_LowTier: [
        {Equipments: [ItemModels.ARM_STR_03]},
        {Equipments: [ItemModels.ARM_STR_02]},
        {Equipments: [ItemModels.ARM_STR_12]},
        {Equipments: [ItemModels.ARM_STR_04]},
        {Equipments: [ItemModels.ARM_STR_05]}
    ],
    Arm_Str_MedTier: [
        {Equipments: [ItemModels.ARM_STR_03]},
        {Equipments: [ItemModels.ARM_STR_02]},
        {Equipments: [ItemModels.ARM_STR_12]},
        {Equipments: [ItemModels.ARM_STR_13]},
        {Equipments: [ItemModels.ARM_STR_14]},
        {Equipments: [ItemModels.ARM_STR_04]},
        {Equipments: [ItemModels.ARM_STR_05]}
    ],
    Arm_Str_HighTier: [
        {Equipments: [ItemModels.ARM_STR_07]},
        {Equipments: [ItemModels.ARM_STR_15]}
    ],
    Arm_Agl_LowTier: [
        {Equipments: [ItemModels.ARM_AGL_05]},
        {Equipments: [ItemModels.ARM_AGL_08]},
        {Equipments: [ItemModels.ARM_AGL_09]}
    ],
    Arm_Agl_HighTier: [
        {Equipments: [ItemModels.ARM_AGL_01]},
        {Equipments: [ItemModels.ARM_AGL_04]}
    ],
    Hlm_Str_LowTier: [
        {Equipments: [ItemModels.HLM_STR_03]},
        {Equipments: [ItemModels.HLM_STR_08]},
        {Equipments: [ItemModels.HLM_STR_11]},
        {Equipments: [ItemModels.HLM_STR_12]},
        {Equipments: [ItemModels.HLM_STR_15]}
    ],
    Hlm_Str_MedTier: [
        {Equipments: [ItemModels.HLM_STR_01]},
        {Equipments: [ItemModels.HLM_STR_03]},
        {Equipments: [ItemModels.HLM_STR_08]},
        {Equipments: [ItemModels.HLM_STR_11]},
        {Equipments: [ItemModels.HLM_STR_12]},
        {Equipments: [ItemModels.HLM_STR_14]},
        {Equipments: [ItemModels.HLM_STR_15]}
    ],
    Hlm_Str_HighTier: [
        {Equipments: [ItemModels.HLM_STR_09]},
        {Equipments: [ItemModels.HLM_STR_02]}
    ],
//    Hlm_Str_HighTier: [
//        {Equipments: [ItemModels.HLM_STR_10]}
//    ],
    Hlm_Agl_LowTier: [
        {Equipments: [ItemModels.HLM_STUDENT]},
        {Equipments: [ItemModels.HLM_AGL_05]}
    ],
    Hlm_Agl_HighTier: [
        {Equipments: [ItemModels.HLM_AGL_04]},
        {Equipments: [ItemModels.HLM_AGL_03]}
    ],
    Arm_Train_Like_Liquidator: [
        {Equipments: [ItemModels.ARM_STR_03]},
        {Equipments: [ItemModels.ARM_STR_02]},
        {Equipments: [ItemModels.ARM_STR_12]},
        {Equipments: [ItemModels.ARM_STR_14]}
    ],
    Hlm_Train_Like_Liquidator: [
        {Equipments: [ItemModels.HLM_STR_01]},
        {Equipments: [ItemModels.HLM_STR_03]},
        {Equipments: [ItemModels.HLM_STR_12]},
        {Equipments: [ItemModels.HLM_STR_15]}
    ],
    RandomHair: [
        {Equipments: [ItemModels.HAIR_01]},
        {Equipments: [ItemModels.HAIR_02]},
        {Equipments: [ItemModels.HAIR_03]},
        {
            Equipments: [ItemModels.HAIR_04],
            Gender: GENDER.FEMALE
        },
        {
            Equipments: [ItemModels.HAIR_05],
            Gender: GENDER.FEMALE
        }
    ]
};

var ItemSettingRE = [
    {
        ItemPool: zonesItems[0][RARITY.RARE],
        Weight: 10
    },
    {
        ItemPool: zonesItems[0][RARITY.EPIC],
        Weight: 1
    }
];

var ItemSettingRELvl2 = [
    {
        ItemPool: zonesItemsLvl2[0][RARITY.RARE],
        Weight: 9
    },
    {
        ItemPool: zonesItemsLvl2[0][RARITY.EPIC],
        Weight: 1
    },
    {
        ItemPool: zonesItems[0][RARITY.RARE],
        Weight: 10
    },
    {
        ItemPool: zonesItems[0][RARITY.EPIC],
        Weight: 1
    }
]; //For first 4-8 main battles

var ItemSettingCLvl2 = [
    {
        ItemPool: zonesItemsLvl2[0][RARITY.COMMON],
        Weight: 9
    },
    {
        ItemPool: zonesItems[0][RARITY.COMMON],
        Weight: 10
    }
];

var ItemSettingRLvl2 = [
    {
        ItemPool: zonesItemsLvl2[0][RARITY.RARE],
        Weight: 9
    },
    {
        ItemPool: zonesItems[0][RARITY.RARE],
        Weight: 10
    }
];

var ItemSettingRELvl = [
    {
        ItemPool: zonesItemsLvl2[0][RARITY.RARE],
        Weight: 10
    },
    {
        ItemPool: zonesItemsLvl2[0][RARITY.EPIC],
        Weight: 1
    }
]; //For first 1 main battles

var PerkSettingsRE = [
    {
        ItemPool: zonesPerks[0][RARITY.RARE],
        Weight: 10
    },
    {
        ItemPool: zonesPerks[0][RARITY.EPIC],
        Weight: 10
    },
    {
        ItemPool: zonesPerks[0][RARITY.LEGENDARY],
        Weight: 0
    }
];

var PerkSettingsRL = [
    {
        ItemPool: zonesPerks[0][RARITY.RARE],
        Weight: 10
    },
    {
        ItemPool: zonesPerks[0][RARITY.LEGENDARY],
        Weight: 10
    }
];

var Battles = {
    // MAIN QUEST
    beta_BootCamp_MQ_FirstExam: {
        ID: 40,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1],
        Alias: "beta_BootCamp_MQ_FirstExam",
        Icon: Icons.story,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: [
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Hammerhead
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, -2)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 0 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 0 / levelPrecision),
                        And: [
                            {And: createEquipments(baseItemRatio, zonesItemsLvl2[0][RARITY.COMMON], 1)},
                            {And: createEquipments(baseItemRatio, zonesItemsLvl2[0][RARITY.RARE], 1)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Outcast
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 1)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 1 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 1 / levelPrecision),
                        And: [
                            {And: createEquipments(baseItemRatio, zonesItemsLvl2[0][RARITY.COMMON], 1)},
                            {And: createEquipments(baseItemRatio, zonesItemsLvl2[0][RARITY.RARE], 1)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Spade
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 4)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 4 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 4 / levelPrecision),
                        And: [
                            {And: createEquipments(baseItemRatio, zonesItemsLvl2[0][RARITY.COMMON], 1)},
                            {And: createEquipments(baseItemRatio, zonesItemsLvl2[0][RARITY.RARE], 1)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE]}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Maul
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 7)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 7 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 7 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingCLvl2)},
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Avalanche
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 10)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 10 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 10 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingCLvl2)},
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Rascal
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 14)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 14 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 14 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingCLvl2)},
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Boulder
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 18)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 18 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 18 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingCLvl2)},
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            },
            {
                RoundsToWin: 3,
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Greta_1
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 20),
                            makeWP(baseWarriorPower, 22),
                            makeWP(baseWarriorPower, 24)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.BodyDefense, 1.3, RRA.HeadDefense, 1.3, RRA.ApplyTo, ENEMY)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.BodyDefense, 1.3, RRA.HeadDefense, 1.3, RRA.ApplyTo, PLAYER)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 22 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 22 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingCLvl2)},
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            }
        ]
    },
    beta_LegionDojo_MQ_NoRestOnLaurels: {
        ID: 50,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1_1],
        Completion: [FightResult.WIN, FightResult.LOSS, FightResult.SURRENDER],
        Parent: function() {return {BattleId: Battles.beta_BootCamp_MQ_FirstExam.ID}},
        Alias: "beta_LegionDojo_MQ_NoRestOnLaurels",
        Icon: Icons.story,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.COMMON},
            {Rarity: RARITY.RARE},
            {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.June
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 21)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 21 / levelPrecision), 0),
                Experience: baseMainExp * Math.pow(experienceInflation, 21 / levelPrecision),
                And: [
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Gorge_MQ_GameChanger: {
        ID: 60,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID}},
        Alias: "beta_Gorge_MQ_GameChanger",
        Icon: Icons.story,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: [
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Guardsman
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 24)
                        ]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 24 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 24 / levelPrecision),
                        And: [
                            {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                            {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.COMMON},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Ran_Di
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 27)
                        ]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 27 / levelPrecision), 0),
                        Experience: baseMainExp * Math.pow(experienceInflation, 27 / levelPrecision),
                        And: [
                            {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                            {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            }
        ]
    },
    beta_Gorge_MQ_SargesSecretCache: {
        ID: 70,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_Gorge_MQ_GameChanger.ID}},
        Alias: "beta_Gorge_MQ_SargesSecretCache",
        Icon: Icons.story,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.COMMON},
            {Rarity: RARITY.RARE},
            {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Mysterious_opponent
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 30)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.InvisibleWarrior)]
            },
            {
                RuleBlocks: [rule(RoundRules.Shadowless)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 30 / levelPrecision), 0),
                Experience: baseMainExp * Math.pow(experienceInflation, 30 / levelPrecision),
                And: [
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Bamboo_MQ_FinalResolution: {
        ID: 80,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_Gorge_MQ_SargesSecretCache.ID}},
        Alias: "beta_Bamboo_MQ_FinalResolution",
        Icon: Icons.story,
        Location: Locations.Bamboo,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.COMMON},
            {Rarity: RARITY.RARE},
            {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Fort_Commander
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 34)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 34 / levelPrecision), 0),
                Experience: baseMainExp * Math.pow(experienceInflation, 34 / levelPrecision),
                And: [
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_LegionDojo_MQ_SpillTheBeans: {
        ID: 90,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_Bamboo_MQ_FinalResolution.ID}},
        Alias: "beta_LegionDojo_MQ_SpillTheBeans",
        Icon: Icons.story,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.COMMON},
            {Rarity: RARITY.RARE},
            {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Gizmo,
                    Warriors.Gizmo_Fists
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 38)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.GiveTag, RRA.Tag, "BETA_LEGIONDOJO_MQ_SPILLTHEBEANS", RRA.ApplyTo, ENEMY)]
            },
            {
                RuleBlocks: [rule(RoundRules.GiveTag, RRA.Tag, "SUPER_GIZMO_THROW_1", RRA.ApplyTo, ENEMY)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 38 / levelPrecision), 0),
                Experience: baseMainExp * Math.pow(experienceInflation, 38 / levelPrecision),
                And: [
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_BootCamp_MQ_YouWereASonToMe: {
        ID: 100,
        Type: BattleType.MAIN,
        Chapters: [ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_LegionDojo_MQ_SpillTheBeans.ID}},
        Alias: "beta_BootCamp_MQ_YouWereASonToMe",
        Icon: Icons.boss,
        Location: Locations.Camp_Boss,
        RoundsToWin: 3,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.COMMON},
            {Rarity: RARITY.RARE},
            {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Sarge
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 40),
                    makeWP(baseWarriorPower, 42),
                    makeWP(baseWarriorPower, 44)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.WithoutSF), rule(RoundRules.SargeSF), rule(RoundRules.SargeSF)]
            },
            {
                RuleBlocks: [rule(RoundRules.WithoutSF), rule(RoundRules.GiveTag, RRA.Tag, "BETA_BAMBOO_MQ_YOUWEREASONTOME_BOSS", RRA.ApplyTo, ENEMY), rule(RoundRules.GiveTag, RRA.Tag, "BETA_BAMBOO_MQ_YOUWEREASONTOME_BOSS", RRA.ApplyTo, ENEMY)]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.BodyDefense, 1.3, RRA.HeadDefense, 1.3, RRA.ApplyTo, ENEMY)]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.BodyDefense, 1.3, RRA.HeadDefense, 1.3, RRA.ApplyTo, PLAYER)]
            }
        ],
        Rewards: [
            {},
            {},
            {},
            {
                Currencies: roundedCurrencies(baseMainCoins * Math.pow(moneyInflation, 38 / levelPrecision), 0),
                Experience: baseMainExp * Math.pow(experienceInflation, 38 / levelPrecision),
                And: [
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.COMMON], 1)},
                    {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    // SIDE QUEST
    beta_LegionDojo_SQ_BloodyRelic: {
        ID: 41,
        Type: BattleType.SIDE,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1, ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_BootCamp_MQ_FirstExam.ID, FightWins: 1}},
        Alias: "beta_LegionDojo_SQ_BloodyRelic",
        Icon: Icons.side,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {ItemType: ITEMTYPE.WEAPON, Rarity: RARITY.EPIC}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Greta_2
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 4)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Shadowless)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 4 / levelPrecision), 0),
                Experience: baseSideExp * Math.pow(experienceInflation, 4 / levelPrecision),
                Equipments: {Models: [ItemModels.WPN_TWOHANDEDSWORD_03], SL: baseItemRatio},
                And: [
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Gorge_SQ_ScoutsLastTrial: {
        ID: 42,
        Type: BattleType.SIDE,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1, ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_BootCamp_MQ_FirstExam.ID, FightWins: 3}},
        Alias: "beta_Gorge_SQ_ScoutsLastTrial",
        Icon: Icons.side,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: [
            {
                RewardIcon: [
                    {Rarity: RARITY.RARE},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Legion_scout_1
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 11)
                        ]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 11 / levelPrecision), 0),
                        Experience: baseSideExp * Math.pow(experienceInflation, 11 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.RARE},
                    {Rarity: RARITY.RARE},
                    {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            Warriors.Legion_scout_2
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 19)
                        ]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 19 / levelPrecision), 0),
                        Experience: baseSideExp * Math.pow(experienceInflation, 19 / levelPrecision),
                        Equipments: {Models: [ItemModels.ARM_STR_04], SL: baseItemRatio + stackPerLevel},
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRLvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            }
        ]
    },
    beta_CampNight_SQ_TrainLikeLiquidator: {
        ID: 43,
        Type: BattleType.SIDE,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1, ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_BootCamp_MQ_FirstExam.ID, FightWins: 6}},
        Alias: "beta_CampNight_SQ_TrainLikeLiquidator",
        Icon: Icons.side,
        Location: Locations.Camp_Night,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {ItemType: ITEMTYPE.WEAPON, Rarity: RARITY.LEGENDARY}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_SHADOW_SOLDIER",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_1b},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b},
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Str_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Train_Like_Liquidator
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Train_Like_Liquidator
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 27)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.PerpetualSF, RRA.Description, "desc_Perpetual_SF")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 27 / levelPrecision), 0),
                Experience: baseSideExp * Math.pow(experienceInflation, 27 / levelPrecision),
                Equipments: {Models: [ItemModels.WPN_DUAL_SWORDS_02_01], SL: baseItemRatio + stackPerLevel},
                And: [
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Bamboo_SQ_Negotiations: {
        ID: 81,
        Type: BattleType.SIDE,
        Chapters: [ChapterModels.Chapter1_2],
        Parent: function() {return {BattleId: Battles.beta_Bamboo_MQ_FinalResolution.ID, FightWins: 1}},
        Alias: "beta_Bamboo_SQ_Negotiations",
        Icon: Icons.side,
        Location: Locations.Bamboo,
        RoundsToWin: 1,
        RoundTime: 45,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {ItemType: ITEMTYPE.PERK, Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_CONSPIRATOR",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                WarriorBlocks: [
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.WPN_STAFF_03]},
                    {Equipments: [ItemModels.WPN_SABERS_01_04]}
                ]
            },
            {
                WarriorBlocks: [
                    {Equipments: [ItemModels.ARM_AGL_04]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_AGL_01]},
                    {Equipments: [ItemModels.HLM_AGL_04]}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 36)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.TimeoutWin, RRA.Description, "desc_TimeoutWin")]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.ApplyTo, PLAYER, RRA.WeaponDamage, 0)]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.ApplyTo, PLAYER, RRA.UnarmedDamage, 0)]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.ApplyTo, PLAYER, RRA.RANGED_DAMAGE, 0)]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: roundedCurrencies(baseSideCoins * Math.pow(moneyInflation, 36 / levelPrecision), 0),
                Experience: baseSideExp * Math.pow(experienceInflation, 36 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 2, ItemSettingRLvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    // SURVIVAL
    beta_Bamboo_Survival_DieEvenHarder: {
        ID: 10000,
        Type: BattleType.SURVIVAL,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        Completion: [FightResult.WIN, FightResult.LOSS, FightResult.SURRENDER],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 1)
                && battleIsNotOpen(battles, Battles.beta_Gorge_MQ_GameChanger.ID);
        },
        Alias: "beta_Bamboo_Survival_DieEvenHarder",
        Icon: Icons.survival,
        Location: Locations.Bamboo,
        RoundsToWin: 6,
        RoundsToLose: 1,
        RoundTime: 99,
        Cooldown: 15 * 60 * 1000,
        Fights: 1,
        HPRecovery: 0.1,
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_PARTISAN",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.DefaultWeapon]},
                    {Equipments: [ItemModels.WPN_STAFF_04]},
                    {Equipments: [ItemModels.WPN_SABERS_01_01]},
                    {Equipments: [ItemModels.WPN_SABERS_01_02]},
                    {Equipments: [ItemModels.WPN_GUANDAO_01_01]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.ARM_AGL_01]},
                    {Equipments: [ItemModels.ARM_AGL_08]},
                    {Equipments: [ItemModels.ARM_AGL_05]},
                    {Equipments: [ItemModels.ARM_AGL_09]},
                    {Equipments: [ItemModels.ARM_AGL_10]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_AGL_01]},
                    {Equipments: [ItemModels.HLM_AGL_03]},
                    {Equipments: [ItemModels.HLM_AGL_05]},
                    {Equipments: [ItemModels.HAIR_01]}
                ]
            },
            {
                WarriorBlocks: makeSurviveParams(baseWarriorPower, -levelPrecision / 2, 4, 6, 1)
            }
        ],
        Rewards: [
            {},
            {
                Currencies: roundedCurrencies(baseSurviveCoins * 2, 0),
                And: createEquipments(baseSurviveRatio, zonesItemsLvl2[0][RARITY.COMMON], 1)
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * 2, 0),
                And: createEquipments(baseSurviveRatio, zonesItemsLvl2[0][RARITY.COMMON], 2)
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * 2.5, 0),
                And: createEquipments(baseSurviveRatio, zonesItemsLvl2[0][RARITY.COMMON], 3)
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * 3, 0),
                And: [
                    {And: createEquipments(baseSurviveRatio, zonesItemsLvl2[0][RARITY.COMMON], 2)},
                    {And: createWeightEquipment(baseSurviveRatio, 1, ItemSettingRELvl2)}
                ]
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * 3.5, 0),
                And: [
                    {And: createEquipments(baseSurviveRatio, zonesItemsLvl2[0][RARITY.COMMON], 1)},
                    {And: createWeightEquipment(baseSurviveRatio, 2, ItemSettingRELvl2)}
                ]
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * 4, 0),
                And: createWeightEquipment(baseSurviveRatio, 3, ItemSettingRELvl2)
            }
        ]
    },
    beta_Gorge_Survival_TradeRoute: {
        ID: 10010,
        Type: BattleType.SURVIVAL,
        Chapters: [ChapterModels.Chapter1_2],
        Completion: [FightResult.WIN, FightResult.LOSS, FightResult.SURRENDER],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && battleIsOpenOrCompleted(battles, Battles.beta_Gorge_MQ_GameChanger.ID);
        },
        Alias: "beta_Gorge_Survival_TradeRoute",
        Icon: Icons.survival,
        Location: Locations.Gorge,
        RoundsToWin: 6,
        RoundsToLose: 1,
        RoundTime: 99,
        Cooldown: 15 * 60 * 1000,
        Fights: 1,
        HPRecovery: 0.1,
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_BORDER_PATROL",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_1b},
                    {Appearance: WarriorAppearences.WA_4a},
                    {Appearance: WarriorAppearences.WA_5a},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.DefaultWeapon]},
                    {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_02]},
                    {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
                    {Equipments: [ItemModels.WPN_AXES_01]},
                    {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_03]},
                    {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_02]},
                    {Equipments: [ItemModels.WPN_AXES_02]},
                    {Equipments: [ItemModels.WPN_AXES_03]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_02]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_03]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_02]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_03]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_01]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_02]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_03]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.ARM_STR_04]},
                    {Equipments: [ItemModels.ARM_STR_03]},
                    {Equipments: [ItemModels.ARM_STR_02]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_STR_01]},
                    {Equipments: [ItemModels.HLM_STR_02]},
                    {Equipments: [ItemModels.HLM_STR_03]},
                    {Equipments: [ItemModels.HAIR_02]},
                    {Equipments: [ItemModels.HAIR_03]}
                ]
            },
            {
                WarriorBlocks: makeSurviveParams(baseWarriorPower, levelPrecision - levelPrecision / 2, 4, 6, 1)
            }
        ],
        Rewards: [
            {},
            {
                Currencies: roundedCurrencies(baseSurviveCoins * moneyInflation * 2, 0),
                And: createEquipments(baseSurviveRatio + 10, zonesItems[0][RARITY.COMMON], 1)
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * moneyInflation * 2, 0),
                And: createEquipments(baseSurviveRatio + 10, zonesItems[0][RARITY.COMMON], 2)
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * moneyInflation * 2.5, 0),
                And: createEquipments(baseSurviveRatio + 10, zonesItems[0][RARITY.COMMON], 3)
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * moneyInflation * 3, 0),
                And: [
                    {And: createEquipments(baseSurviveRatio + 10, zonesItems[0][RARITY.COMMON], 2)},
                    {And: createWeightEquipment(baseSurviveRatio + 10, 1, ItemSettingRE)}
                ]
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * moneyInflation * 3.5, 0),
                And: [
                    {And: createEquipments(baseSurviveRatio + 10, zonesItems[0][RARITY.COMMON], 1)},
                    {And: createWeightEquipment(baseSurviveRatio + 10, 2, ItemSettingRE)}
                ]
            },
            {
                Currencies: roundedCurrencies(baseSurviveCoins * moneyInflation * 4, 0),
                And: createWeightEquipment(baseSurviveRatio + 10, 3, ItemSettingRE)
            }
        ]
    },
    // DAILY
//    beta_BootCamp_Daily: {
//        ID: 20000,
//        Type: BattleType.DAILY,
//        GenerationConditions: function (level, battles) {
//            return minLevel(level, 1)
//                && battleIsOpenOrCompleted(battles, Battles.beta_BootCamp_MQ_FirstExam.ID)
//                && battleIsNotOpen(battles, Battles.beta_Bamboo_MQ_FinalResolution.ID);
//        },
//        Schedule: {
//            Origin: "2016-01-01T03:00:00+03",
//            Period: 60 * 1000
//        },
//        Alias: "beta_Daily",
//        Icon: Icons.survival,
//        Location: Locations.Camp,
//        RoundsToWin: 2,
//        RoundTime: 99,
//        Fights: 1,
//        Warriors: [
//            {
//                WarriorBlocks: [
//                    {
//                        Alias: "CHAR_OAK",
//                        Gender: GENDER.FEMALE,
//                        Appearance: WarriorAppearences.WA_1,
//                        WarriorPower: baseWarriorPower + stackPerLevel * (21 / levelPrecision),
//                        Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_02, ItemModels.HLM_STR_09, ItemModels.ARM_STR_03]
//                    }
//                ]
//            },
//            {
//                WarriorBlocks: [
//                    {WarriorPower: baseWarriorPower + stackPerLevel * (10 / levelPrecision) + survivePowerShift}
//                ]
//            }
//        ],
//        Rewards: [
//            {},
//            {},
//            {
//                Currencies: roundedCurrencies(baseSurviveCoins * Math.pow(moneyInflation, 10 / levelPrecision), 0),
//                Experience: baseSurviveExp * Math.pow(experienceInflation, 10 / levelPrecision),
//                And: createEquipments(baseSurviveRatio, zonesItems[0][RARITY.COMMON], 3)
//            }
//        ]
//    },
    // MISSIONS
    // a
    beta_Mission_YouOweMeOne: {
        ID: 30001,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_YouOweMeOne",
        Icon: Icons.periodic,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: [
            {
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_LEGIONARY",
                                Perks: Super_Crutch_All_Tags
                            }
                        ]
                    },
                    {
                        RandType: RandType.RAND,
                        WarriorBlocks: [
                            {Gender: GENDER.FEMALE},
                            {Gender: GENDER.MALE}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_1a},
                            {Appearance: WarriorAppearences.WA_1b},
                            {Appearance: WarriorAppearences.WA_5a}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
                            {Equipments: [ItemModels.WPN_AXES_01]},
                            {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                            {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                            {Equipments: [ItemModels.WPN_SPEAR_01_01]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_12]},
                            {Equipments: [ItemModels.ARM_STR_02]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HLM_STR_11]},
                            {Equipments: [ItemModels.HLM_STR_03]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 10)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), normalBonusReward),
                        Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision)
                    }
                ]

            },
            {
                RewardIcon: [
                    {Rarity: RARITY.RARE},
                    {Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_LEGIONARY",
                                Perks: Super_Crutch_All_Tags_2
                            }
                        ]
                    },
                    {
                        RandType: RandType.RAND,
                        WarriorBlocks: [
                            {Gender: GENDER.FEMALE},
                            {Gender: GENDER.MALE}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_1a},
                            {Appearance: WarriorAppearences.WA_1b},
                            {Appearance: WarriorAppearences.WA_5a}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
                            {Equipments: [ItemModels.WPN_AXES_01]},
                            {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                            {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                            {Equipments: [ItemModels.WPN_SPEAR_01_01]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_12]},
                            {Equipments: [ItemModels.ARM_STR_02]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HLM_STR_11]},
                            {Equipments: [ItemModels.HLM_STR_03]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 10)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), normalBonusReward),
                        Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRELvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            }
        ]
    },
    beta_Mission_TheLegionsJustice: {
        ID: 30002,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID,3)
        },
        Alias: "beta_Mission_TheLegionsJustice",
        Icon: Icons.periodic,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_DELINQUENT",
                        Gender: GENDER.MALE,
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_1b},
                    {Appearance: WarriorAppearences.WA_4a},
                    {Appearance: WarriorAppearences.WA_5a}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
                    {Equipments: [ItemModels.WPN_AXES_01]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_01]}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.ARM_STR_12]},
                    {Equipments: [ItemModels.ARM_STR_02]},
                    {Equipments: [ItemModels.ARM_STR_03]},
                    {Equipments: [ItemModels.ARM_STR_14]}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_STR_14]},
                    {Equipments: [ItemModels.HLM_STR_12]},
                    {Equipments: [ItemModels.HLM_STR_03]},
                    {Equipments: [ItemModels.HLM_STR_01]}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 19)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 19 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 19 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 3, ItemSettingRELvl2)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_FullContact: {
        ID: 30003,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_FullContact",
        Icon: Icons.periodic,
        Location: Locations.Camp_Night,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: [
            {
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_YOUNG_LEGIONARY",
                                Perks: Super_Crutch_All_Tags_2,
                                Equipments: [ItemModels.DefaultWeapon]
                            }
                        ]
                    },
                    {
                        RandType: RandType.RAND,
                        WarriorBlocks: [
                            {Gender: GENDER.FEMALE},
                            {Gender: GENDER.MALE}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_2a},
                            {Appearance: WarriorAppearences.WA_3b}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_12]},
                            {Equipments: [ItemModels.DefaultArmor]},
                            {Equipments: [ItemModels.ARM_STR_14]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HAIR_02]},
                            {Equipments: [ItemModels.HAIR_03]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 0)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless, RRA.ApplyTo, PLAYER)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 1000000, RRA.ApplyTo, ENEMY)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 19 / levelPrecision), hardBonusReward),
                        Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 19 / levelPrecision)
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.RARE},
                    {Rarity: RARITY.RARE},
                    {Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_YOUNG_LEGIONARY",
                                Equipments: [ItemModels.DefaultWeapon],
                                Perks: Super_Crutch_All_Tags
                            }
                        ]
                    },
                    {
                        RandType: RandType.RAND,
                        WarriorBlocks: [
                            {Gender: GENDER.FEMALE},
                            {Gender: GENDER.MALE}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_2a},
                            {Appearance: WarriorAppearences.WA_3b}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_12]},
                            {Equipments: [ItemModels.DefaultArmor]},
                            {Equipments: [ItemModels.ARM_STR_14]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HAIR_02]},
                            {Equipments: [ItemModels.HAIR_03]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 19)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless, RRA.ApplyTo, PLAYER)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.Shadowless)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 1000000, RRA.ApplyTo, ENEMY, RRA.Description, "desc_Unarmed")]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 19 / levelPrecision), hardBonusReward),
                        Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 19 / levelPrecision),
                        And: [
                            {And: createWeightEquipment(baseItemRatio, 3, ItemSettingRELvl2)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            }
        ]
    },
    beta_Mission_FreshBlood: {
        ID: 30004,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_FreshBlood",
        Icon: Icons.periodic,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: [
            {
                RewardIcon: [
                    {Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_LEGIONARY",
                                Gender: GENDER.MALE,
                                Equipments: [ItemModels.DefaultWeapon],
                                Perks: Super_Crutch_All_Tags_2
                            }
                        ]
                    },
                    {
                        RandType: RandType.ONCE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_2a},
                            {Appearance: WarriorAppearences.WA_2b}
                        ]
                    },
                    {
                        RandType: RandType.ONCE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_12]},
                            {Equipments: [ItemModels.ARM_STR_02]},
                            {Equipments: [ItemModels.ARM_STR_03]},
                            {Equipments: [ItemModels.ARM_STR_14]}
                        ]
                    },
                    {
                        RandType: RandType.ONCE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HLM_STR_14]},
                            {Equipments: [ItemModels.HLM_STR_12]},
                            {Equipments: [ItemModels.HLM_STR_03]},
                            {Equipments: [ItemModels.HLM_STR_01]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 1)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.WithoutSF)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 1000000, RRA.ApplyTo, ENEMY)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.NoPunches, RRA.ApplyTo, PLAYER, RRA.Description, "desc_NoPunches")]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveTag, RRA.Tag, "LOCK_NO_PUNCHES", RRA.ApplyTo, ENEMY)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 1 / levelPrecision), easyBonusReward),
                        Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 1 / levelPrecision)
                    }
                ]
            },
            {
                RewardIcon: [
                    {Rarity: RARITY.RARE}
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_LEGIONARY",
                                Gender: GENDER.MALE,
                                Equipments: [ItemModels.DefaultWeapon],
                                Perks: Super_Crutch_All_Tags_2
                            }
                        ]
                    },
                    {
                        RandType: RandType.ONCE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_3a},
                            {Appearance: WarriorAppearences.WA_3b}
                        ]
                    },
                    {
                        RandType: RandType.ONCE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_12]},
                            {Equipments: [ItemModels.ARM_STR_02]},
                            {Equipments: [ItemModels.ARM_STR_03]},
                            {Equipments: [ItemModels.ARM_STR_14]}
                        ]
                    },
                    {
                        RandType: RandType.ONCE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HLM_STR_14]},
                            {Equipments: [ItemModels.HLM_STR_12]},
                            {Equipments: [ItemModels.HLM_STR_03]},
                            {Equipments: [ItemModels.HLM_STR_01]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(baseWarriorPower, 1)
                        ]
                    }
                ],
                Rules: [
                    {
                        RuleBlocks: [rule(RoundRules.WithoutSF)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 1000000, RRA.ApplyTo, ENEMY)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.NoPunches, RRA.ApplyTo, PLAYER, RRA.Description, "desc_NoPunches")]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveTag, RRA.Tag, "LOCK_NO_PUNCHES", RRA.ApplyTo, ENEMY)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 1 / levelPrecision), easyBonusReward),
                        Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 1 / levelPrecision),
                        Or: [
                            {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRELvl2)},
                            {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                            {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                        ]
                    }
                ]
            }
        ]
    },
    beta_Mission_Marauders: {
        ID: 30005,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_Marauders",
        Icon: Icons.periodic,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_LOOTER",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 1)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 1 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 1 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_Nightwatch: {
        ID: 30006,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_Nightwatch",
        Icon: Icons.periodic,
        Location: Locations.Camp_Night,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_INVADER",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_LowTier_NoFists
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 19)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Degeneration, RRA.Description, "desc_Degeneration")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 19 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 19 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 3, ItemSettingRELvl2)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_HitNCount: {
        ID: 30007,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_HitNCount",
        Icon: Icons.periodic,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 1,
        RoundTime: 40,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Gizmo
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 10)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.ScoreFight, RRA.Description, "desc_ScoreFight")]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), normalBonusReward),
                Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_Blackouts: {
        ID: 30008,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_Blackouts",
        Icon: Icons.periodic,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_SHADOW_SOLDIER",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_1b},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b},
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
                    {Equipments: [ItemModels.WPN_AXES_01]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_01]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.ARM_STR_12]},
                    {Equipments: [ItemModels.ARM_STR_02]},
                    {Equipments: [ItemModels.ARM_STR_14]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_STR_14]},
                    {Equipments: [ItemModels.HLM_STR_12]},
                    {Equipments: [ItemModels.HLM_STR_03]}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 10)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Darkness, RRA.Description, "desc_Darkness")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), normalBonusReward),
                Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_BlastFromThePast: {
        ID: 30021,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 2)
        },
        Alias: "beta_Mission_BlastFromThePast",
        Icon: Icons.periodic,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_CULTIST",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b},
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_HighTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 1)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Degeneration, RRA.Description, "desc_Degeneration")]
            },
            {
                RuleBlocks: [rule(RoundRules.LifeSteal, RRA.Description, "desc_LifeSteal")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 1 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 1 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    // b
    beta_Mission_MaintainSecrecy: {
        ID: 30009,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_MaintainSecrecy",
        Icon: Icons.periodic,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 45,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_SARGES_ALLY",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_HighTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_HighTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_HighTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 1)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.TimeoutWin, RRA.Description, "desc_TimeoutWin")]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.ApplyTo, PLAYER, RRA.WeaponDamage, 0)]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.ApplyTo, PLAYER, RRA.UnarmedDamage, 0)]
            },
            {
                RuleBlocks: [rule(RoundRules.ApplyFactor, RRA.ApplyTo, PLAYER, RRA.RANGED_DAMAGE, 0)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 1 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 1 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_RecklessRecruit: {
        ID: 30010,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_RecklessRecruit",
        Icon: Icons.periodic,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 1,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_TURNCOAT",
                        Equipments: [ItemModels.ARM_STR_05],
                        Gender: GENDER.MALE,
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_4a},
                    {Appearance: WarriorAppearences.WA_5a}
                ]
            },

            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.WPN_AXES_01]},
                    {Equipments: [ItemModels.WPN_AXES_02]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                    {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_02]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                    {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_02]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_01]},
                    {Equipments: [ItemModels.WPN_SPEAR_01_02]}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_STR_11]},
                    {Equipments: [ItemModels.HLM_STR_12]},
                    {Equipments: [ItemModels.HLM_STR_03]},
                    {Equipments: [ItemModels.HLM_STR_09]}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 10)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Shadowless)]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), normalBonusReward),
                Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_AllTomorrowsParties: {
        ID: 30011,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_AllTomorrowsParties",
        Icon: Icons.periodic,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_LEGIONARY",
                        Equipments: [ItemModels.DefaultWeapon, ItemModels.DefaultArmor, ItemModels.HLM_FAKE],
                        Gender: GENDER.MALE,
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_4a},
                    {Appearance: WarriorAppearences.WA_5a}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 10)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Shadowless)]
            },
            {
                RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 1000000, RRA.ApplyTo, ENEMY, RRA.Description, "desc_Unarmed")]
            },
            {
                RuleBlocks: [rule(RoundRules.GiveItem)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 10 / levelPrecision), normalBonusReward),
                Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 10 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 2, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_AWOL: {
        ID: 30012,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_AWOL",
        Icon: Icons.periodic,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_SHADOW_SOLDIER",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Str_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Str_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Str_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 1)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Darkness, RRA.Description, "desc_Darkness")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 1 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 1 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 1, ItemSettingRELvl2)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_Runaway: {
        ID: 30013,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_Runaway",
        Icon: Icons.periodic,
        Location: Locations.Camp_Night,
        RoundsToWin: 1,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_RUNAWAY",
                        Gender: GENDER.MALE,
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_LowTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 19)
                ]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 19 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 19 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 3, ItemSettingRELvl2)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_GoodReputation: {
        ID: 30014,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1, ChapterModels.Chapter1_1],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_BootCamp_MQ_FirstExam.ID, 3)
        },
        Alias: "beta_Mission_GoodReputation",
        Icon: Icons.periodic,
        Location: Locations.Camp_Night,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_TROUBLEMAKER",
                        Gender: GENDER.MALE,
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_1b},
                    {Appearance: WarriorAppearences.WA_4a},
                    {Appearance: WarriorAppearences.WA_5a}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Str_MedTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Str_MedTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Str_MedTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 19)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Shadowless)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 19 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 19 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio, 3, ItemSettingRELvl2)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
//    // c
    beta_Mission_StartWithTheBasics: {
        ID: 30015,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_StartWithTheBasics",
        Icon: Icons.periodic,
        Location: Locations.Dojo_Legion,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.June
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 30)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [RoundRules.NoPunches]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 30 / levelPrecision), normalBonusReward),
                Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 30 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 2, ItemSettingRE)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    /*    beta_Mission_FaceYourselfFaceYourFears: {
     ID: 30016,
     Type: BattleType.MISSION,
     Chapters: [ChapterModels.Chapter1_2],
     GenerationConditions: function (level, battles) {
     return minLevel(level, 1)
     && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
     },
     Alias: "beta_Mission_FaceYourselfFaceYourFears",
     Icon: Icons.periodic,
     Location: Locations.Dojo_Legion,
     RoundsToWin: 2,
     RoundTime: 99,
     Fights: 1,
     RewardIcon: [
     {Rarity: RARITY.RARE},
     {Rarity: RARITY.RARE}
     ],
     Warriors: [
     {
     WarriorBlocks: [
     Warriors.June
     ]
     },
     {
     WarriorBlocks: [
     {
     Alias: "CHAR_DOPPELGANGER"
     }
     ]
     },
     {
     WarriorBlocks: [
     makeWP(baseWarriorPower, 30)
     ]
     }
     ],
     Rewards: [
     {},
     {},
     {
     Currencies: roundedCurrencies(missionsMediumRatio * baseMissionCoins * Math.pow(moneyInflation, 30 / levelPrecision), 0),
     Experience: missionsMediumRatio * baseMissionExp * Math.pow(experienceInflation, 30 / levelPrecision),
     And: [
     {Or: [
     {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.RARE], 2), Weight: 10},
     {And: createEquipments(baseItemRatio + stackPerLevel, zonesItems[0][RARITY.EPIC], 2), Weight: 1}
     ]},
     {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
     ]
     }
     ]
     }, */
    beta_Mission_KnowYourEnemy: {
        ID: 30017,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_KnowYourEnemy",
        Icon: Icons.periodic,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_GRETAS_SOLDIER",
                        Gender: GENDER.MALE,
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_4a},
                    {Appearance: WarriorAppearences.WA_5a}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Str_LowTier
            },
            {
                WarriorBlocks: [{Equipments: [ItemModels.ARM_STR_05]}]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_STR_11]},
                    {Equipments: [ItemModels.HLM_STR_12]},
                    {Equipments: [ItemModels.HLM_STR_03]},
                    {Equipments: [ItemModels.HLM_STR_09]}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 39)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Shadowless)]
            },
            {
                RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 1000000, RRA.ApplyTo, ENEMY)]
            },
            {
                RuleBlocks: [rule(RoundRules.GiveItem)]
            },
            {
                RuleBlocks: [rule(RoundRules.NoPunches, RRA.Description, "desc_NoPunches")]
            },
            {
                RuleBlocks: [rule(RoundRules.NoPunches, RRA.ApplyTo, ENEMY)]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 39 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 39 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 3, ItemSettingRE)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_Fractured: {
        ID: 30018,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_Fractured",
        Icon: Icons.periodic,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_DEFECTOR",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_1a},
                    {Appearance: WarriorAppearences.WA_1b},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b},
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Str_MedTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Arm_Str_MedTier
            },
            {
                RandType: RandType.ONCE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Str_MedTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 21)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 21 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 21 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 1, ItemSettingRE)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_AGiftFromOldFriends: {
        ID: 30019,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_AGiftFromOldFriends",
        Icon: Icons.periodic,
        Location: Locations.Bamboo,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_HERBALIST",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 21)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Degeneration, RRA.Description, "desc_Degeneration")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 21 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 21 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 1, ItemSettingRE)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_PrideAndPrejudice: {
        ID: 30020,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_PrideAndPrejudice",
        Icon: Icons.periodic,
        Location: Locations.Gorge,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_SURVIVALIST",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 21)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Regeneration, RRA.Description, "desc_Regeneration")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsEasyRatio * baseMissionCoins * Math.pow(moneyInflation, 21 / levelPrecision), easyBonusReward),
                Experience: missionsEasyRatio * baseMissionExp * Math.pow(experienceInflation, 21 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 1, ItemSettingRE)},
                    {And: createPerks(0, zonesPerks[0][RARITY.RARE], 1)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_RiotOuttaNowhere: {
        ID: 30022,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_RiotOuttaNowhere",
        Icon: Icons.periodic,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_REBEL",
                        Perks: Super_Crutch_All_Tags
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b},
                    {Appearance: WarriorAppearences.WA_2a},
                    {Appearance: WarriorAppearences.WA_2b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Wpn_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Arm_Agl_LowTier
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: WarriorBlockPresets.Hlm_Agl_LowTier
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 39)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 39 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 39 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 3, ItemSettingRE)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },
    beta_Mission_StarvingForAFight: {
        ID: 30023,
        Type: BattleType.MISSION,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Mission_StarvingForAFight",
        Icon: Icons.periodic,
        Location: Locations.Camp,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        RewardIcon: [
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE},
            {Rarity: RARITY.RARE}
        ],
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_ROBBER",
                        Perks: Super_Crutch_All_Tags_2
                    }
                ]
            },
            {
                RandType: RandType.RAND,
                WarriorBlocks: [
                    {Gender: GENDER.FEMALE},
                    {Gender: GENDER.MALE}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Appearance: WarriorAppearences.WA_3a},
                    {Appearance: WarriorAppearences.WA_3b}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.WPN_GUANDAO_01_01]},
                    {Equipments: [ItemModels.WPN_GUANDAO_01_02]},
                    {Equipments: [ItemModels.WPN_GUANDAO_01_03]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.ARM_AGL_06]},
                    {Equipments: [ItemModels.ARM_AGL_10]}
                ]
            },
            {
                RandType: RandType.SHUFFLE,
                WarriorBlocks: [
                    {Equipments: [ItemModels.HLM_AGL_02]},
                    {Equipments: [ItemModels.HLM_AGL_10]}
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 39)
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [rule(RoundRules.Degeneration, RRA.Description, "desc_Degeneration")]
            }
        ],
        Rewards: [
            {},
            {},
            {
                Currencies: roundedCurrencies(missionsHardRatio * baseMissionCoins * Math.pow(moneyInflation, 39 / levelPrecision), hardBonusReward),
                Experience: missionsHardRatio * baseMissionExp * Math.pow(experienceInflation, 39 / levelPrecision),
                And: [
                    {And: createWeightEquipment(baseItemRatio + stackPerLevel, 3, ItemSettingRE)},
                    {Boosters: [BoosterModels.CHAPTER1_RARE], Probability: 0.05}
                ]
            }
        ]
    },

    //LOCAL
    dojo_Legion: {
        ID: 1,
        Hidden: 1,
        Type: BattleType.DOJO,
        Completion: [],
        Alias: "dojo_Legion",
        Icon: Icons.daily,
        Location: Locations.Dojo_Dojo_Legion,
        RoundsToWin: 1,
        RoundTime: 99999,
        Replayable: 1,
        Fights: [
            {
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_GIZMO",
                                AiMode: AiMode.NONE_MODE,
                                Gender: GENDER.MALE,
                                Appearance: WarriorAppearences.Gizmo,
                                Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01, ItemModels.HLM_FAKE, ItemModels.ARM_STR_05]
                            }
                        ]
                    }
                ]
            },
            {
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_JUNE",
                                AiMode: AiMode.NONE_MODE,
                                Gender: GENDER.FEMALE,
                                Appearance: WarriorAppearences.June,
                                Equipments: [ItemModels.WPN_NUNCHAKU_03, ItemModels.HAIR_JUNE, ItemModels.ARM_JUNE]
                            }
                        ]
                    }
                ]
            }
        ]
    },
    beta_LegionDojo_MQ_Tutorial_Controls: {
        ID: 10,
        Hidden: 1,
        Type: BattleType.LOCAL,
        Alias: "beta_LegionDojo_MQ_Tutorial_Controls",
        Icon: Icons.story,
        Location: Locations.Camp_Training,
        RoundsToWin: 1,
        RoundTime: 99,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_GIZMO",
                        Gender: GENDER.MALE,
                        Appearance: WarriorAppearences.Gizmo,
                        AiMode: AiMode.NONE_MODE,
                        Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01, ItemModels.HLM_FAKE, ItemModels.ARM_STR_05],
                        Tags: ["SKELETON", "TUTORIAL_MOVEMENT_COMBOS"],
                        WarriorPower: 17.5
                    }
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [
                    rule(RoundRules.WithoutTime)
                ]
            },
            {
                RuleBlocks: [
                    rule(RoundRules.WithoutSF)
                ]
            },
            {
                RuleBlocks: [
                    rule(RoundRules.ApplyFactor, RRA.WeaponDamage, 1.4, RRA.UnarmedDamage, 1.4)
                ]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: {
                    1: 75
                },
                Experience: 10
            }
        ]
    },
    beta_BootCamp_MQ_Tutorial_FreeFight: {
        ID: 20,
        Hidden: 1,
        Type: BattleType.LOCAL,
        Alias: "beta_BootCamp_MQ_Tutorial_FreeFight",
        Icon: Icons.story,
        Location: Locations.Camp_Night,
        RoundsToWin: 1,
        RoundTime: 99,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_GIZMO",
                        Gender: GENDER.MALE,
                        Appearance: WarriorAppearences.Gizmo,
                        Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01, ItemModels.HLM_FAKE, ItemModels.ARM_STR_05],
                        Tags: ["SKELETON", "TUTORIAL_CHEAT", "TUTORIAL_BLOCK"],
                        WarriorPower: 17.5
                    }
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [
                    rule(RoundRules.WithoutTime)
                ]
            },
            {
                RuleBlocks: [
                    rule(RoundRules.WithoutSF)
                ]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: {
                    1: 75
                },
                Experience: 15
            }
        ]
    },
    beta_Bamboo_MQ_Tutorial_ShadowForm: {
        ID: 30,
        Hidden: 0,
        Type: BattleType.LOCAL,
        Alias: "beta_Bamboo_MQ_Tutorial_ShadowForm",
        Icon: Icons.story,
        Location: Locations.Bamboo,
        RoundsToWin: 1,
        RoundTime: 99,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    {
                        Alias: "CHAR_PARTISAN",
                        Gender: GENDER.MALE,
                        Appearance: WarriorAppearences.Partisan,
                        Equipments: [ItemModels.WPN_STAFF_04, ItemModels.HLM_AGL_01, ItemModels.ARM_AGL_01],
                        Tags: ["SKELETON", "TUTORIAL_SF", "SHADOW_STAFF_SPINNING_SLASH", "SHADOW_BLINK"],
                        WarriorPower: 17.5
                    }
                ]
            }
        ],
        Rules: [
            {
                RuleBlocks: [
                    rule(RoundRules.WithoutTime)
                ]
            }
        ],
        Rewards: [
            {},
            {
                Currencies: {
                    1: 60
                },
                Experience: 25,
                Perks: {SL: 10, Models: [PerkModels.SUPER_SWORDS_STRONG_1]}
            }
        ]
    },
    chapter1_brawler1_test: {
        ID: 500,
        Hidden: 1,
        Type: BattleType.BRAWLER,
        Completion: [],
        Chapters: [ChapterModels.Chapter1],
        Alias: "chapter1_Brawler",
        Icon: Icons.boss,
        Location: Locations.Brawler,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.June
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 20)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {}
        ]
    },
    chapter1_1_brawler1_test: {
        ID: 501,
        Hidden: 1,
        Type: BattleType.BRAWLER,
        Completion: [],
        Chapters: [ChapterModels.Chapter1_1, ChapterModels.Chapter1_2],
        Alias: "chapter2_Brawler",
        Icon: Icons.daily,
        Location: Locations.Camp_Night,
        RoundsToWin: 2,
        RoundTime: 99,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.June
                ]
            },
            {
                WarriorBlocks: [
                    makeWP(baseWarriorPower, 20)
                ]
            }
        ],
        Rewards: [
            {},
            {},
            {}
        ]
    },
    beta_Daily_Bonus_Farm: {
        ID: 88892,
        Type: BattleType.DAILY,
        Chapters: [ChapterModels.Chapter1_2],
        GenerationConditions: function (level, battles) {
            return minLevel(level, 1)
                && wonFights(battles, Battles.beta_LegionDojo_MQ_NoRestOnLaurels.ID, 1)
        },
        Alias: "beta_Daily_Bonus_Farm",
        Icon: Icons.daily,
        Location: Locations.Bamboo,
        RoundsToWin: 2,
        RoundTime: 99,
        Schedule: {
            Origin: "2016-01-01T03:00:00+03",
            Period: 24 * 60 * 60 * 1000
        },
        Fights: [
            {
                RewardIcon: [
                ],
                Warriors: [
                    {
                        WarriorBlocks: [
                            {
                                Alias: "CHAR_GRETAS_GUARD",
                                Perks: Super_Crutch_All_Tags_2
                            }
                        ]
                    },
                    {
                        RandType: RandType.RAND,
                        WarriorBlocks: [
                            {Gender: GENDER.FEMALE},
                            {Gender: GENDER.MALE}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Appearance: WarriorAppearences.WA_1a},
                            {Appearance: WarriorAppearences.WA_1b}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.ARM_STR_05]},
                            {Equipments: [ItemModels.ARM_STR_04]},
                            {Equipments: [ItemModels.ARM_STR_03]},
                            {Equipments: [ItemModels.ARM_STR_02]},
                            {Equipments: [ItemModels.ARM_STR_15]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.HLM_STR_01]},
                            {Equipments: [ItemModels.HLM_STR_03]},
                            {Equipments: [ItemModels.HLM_STR_08]},
                            {Equipments: [ItemModels.HLM_STR_02]},
                            {Equipments: [ItemModels.HLM_STR_11]}
                        ]
                    },
                    {
                        RandType: RandType.SHUFFLE,
                        WarriorBlocks: [
                            {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_02]},
                            {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_01]},
                            {Equipments: [ItemModels.WPN_AXES_01]},
                            {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_03]},
                            {Equipments: [ItemModels.WPN_TWOHANDEDSWORD_02]},
                            {Equipments: [ItemModels.WPN_AXES_02]},
                            {Equipments: [ItemModels.WPN_AXES_03]},
                            {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_01]},
                            {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_02]},
                            {Equipments: [ItemModels.WPN_ONEHANDED_SWORD_01_03]},
                            {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_01]},
                            {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_02]},
                            {Equipments: [ItemModels.WPN_DUAL_SWORDS_01_03]},
                            {Equipments: [ItemModels.WPN_SPEAR_01_01]},
                            {Equipments: [ItemModels.WPN_SPEAR_01_02]},
                            {Equipments: [ItemModels.WPN_SPEAR_01_03]}
                        ]
                    },
                    {
                        WarriorBlocks: [
                            makeWP(0, 0)
                        ]
                    }
                ],
                Rules: [
                    {
                        RandType: RandType.SHUFFLE,
                        RuleBlocks: [
                            rule(RoundRules.GiveItem, RRA.ID, 2, RRA.ApplyTo, PLAYER),
                            rule(RoundRules.GiveItem, RRA.ID, 8, RRA.ApplyTo, PLAYER),
                            rule(RoundRules.GiveItem, RRA.ID, 15, RRA.ApplyTo, PLAYER),
                            rule(RoundRules.GiveItem, RRA.ID, 24, RRA.ApplyTo, PLAYER)
                        ]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 215, RRA.ApplyTo, PLAYER)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.GiveItem, RRA.ID, 4013, RRA.ApplyTo, PLAYER)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.SetAlias, RRA.Alias, "CHAR_JUNE")]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.SetGender, RRA.Gender, 2)]
                    },
                    {
                        RuleBlocks: [rule(RoundRules.SetAppearance, RRA.ID, 10)]
                    }
                ],
                Rewards: [
                    {},
                    {},
                    {
                        Currencies: roundedCurrencies(150, 80),
                        Experience: 0
                    }
                ]
            }
        ]
    }
};

var BattlesMap = createIDMap(Battles);

for (var battleName in BattlesMap) {
    if(BattlesMap.hasOwnProperty(battleName)){
        var battle = BattlesMap[battleName];

        if(battle.hasOwnProperty("Parent")){
            battle.Parent = battle.Parent();
            var parent = battle.Parent;

            if(!(parent.hasOwnProperty("FightWins"))) {
                parent.FightWins = getFightCount(BattlesMap[parent.BattleId]);
            }
        }
    }
}

for (var locationName in Locations) {
    if(Locations.hasOwnProperty(locationName)) {
        var location = Locations[locationName];

        if(!location.hasOwnProperty("Group")) {
            location.Group = "X" + location.X.toString() + "Y" + location.Y.toString();
        }
    }
}

function chapterRange(firstChapter, secondChapter) {
    if (firstChapter.ID > secondChapter.ID) {
        var tempChapter = firstChapter;
        firstChapter = secondChapter;
        secondChapter = tempChapter;
    }

    var firstID = firstChapter.ID;
    var secondID = secondChapter.ID;
    var result = [];

    for (var chapterID in ChapterModelsMap) {
        if (ChapterModelsMap.hasOwnProperty(chapterID)) {
            if (chapterID >= firstID && chapterID <= secondID) {
                result.push(ChapterModelsMap[chapterID]);
            }
        }

    }

    return result;
}

function createEquipments(stackLevel, drop, quantity) {
    var result = [];
    var oneDrop = [];
    var oneEquipment;

    for (var i = 0; i < drop.length; i++) {
        var model = drop[i];
        var sl = Math.max(stackLevel, model.Level * stackPerLevel);

        oneEquipment = {Equipments: {SL: sl, Models: [model]}};
        oneDrop.push(oneEquipment);
    }

    for (i = 0; i < quantity; i++) {
        result.push({Or: oneDrop});
    }

    return result;
}

function oneItemPool(stackLevel, weightItemSettings, FuncName) { //Тут создаю пул всех предметов/перков различной рарности с весом.
    var ItemPool = [];

    for(var i = 0; i < weightItemSettings.length; i++){
        var OneKindOfRarityPool = (FuncName(stackLevel, weightItemSettings[i]["ItemPool"], 1))[0];
        OneKindOfRarityPool.Weight = weightItemSettings[i]["Weight"];
        ItemPool.push(OneKindOfRarityPool);
    }

    return ItemPool;
}

function createWeightEquipment(stackLevel, quantity, weightEquipmentSettings){
    var result = [];
    var AllRarityPoolWithWeight = oneItemPool(stackLevel, weightEquipmentSettings, createEquipments); //Тут выбор одной штуки из всех штук с весами.

    for (var i = 0; i < quantity; i++){
        result.push({Or: (AllRarityPoolWithWeight)}); //Тут пушу в результат ону штуку столько, сколько надо раз.
    }

    return result;
}

function createWeightPerks(stackLevel, quantity, weightPerkSettings){
    var result = [];
    var AllRarityPoolWithWeight = oneItemPool(stackLevel, weightPerkSettings, createPerks); //Тут выбор одного перка из всех перков с весами.
    for (var i = 0; i < quantity; i++){
        result.push({Or: (AllRarityPoolWithWeight)}); //Тут пушу в результат ону штуку столько, сколько надо раз.
    }
    return result;
}

function createPerks(stackLevel, drop, quantity) {
    var result = [];
    var oneDrop = [];
    var onePerk;

    for (var i = 0; i < drop.length; i++) {
        onePerk = {Perks: {SL: stackLevel, Models: [drop[i]]}};
        oneDrop.push(onePerk);
    }

    for (i = 0; i < quantity; i++) {
        result.push({Or: oneDrop});
    }

    return result;
}

function makeSurviveParams(basePower, from, shift, rounds, progression) {
    if(rounds == 1) {
        throw "survive must have more than one round";
    }

    var result = [makeWP(basePower, from)];

    for(var i = 1; i < rounds; ++i) {
        result.push(makeWP(basePower, from + i * (shift) + i * (i - 1) * progression / 2));
    }

    return result;
}

function makeWP(basePower, amplifier) {
    return {WarriorPower: basePower + stackPerLevel * amplifier / levelPrecision};
}

function getFightCount(battle) {
    if(!battle.hasOwnProperty("Fights")) {
        throw "incorrect battle with ID: " + battle.ID;
    }

    var fights = battle.Fights;
    if (typeof fights === "number") {
        return fights;
    } else {
        return fights.length;
    }
}

function minLevel(cur, min) {
    return cur >= min;
}

function wonFights(battles, battleId, fightCount) {
    checkBattleType(battleId, BattleType.MAIN);
    return battles !== null && battles[battleId] !== null && battles[battleId] >= fightCount
}

function battleIsOpenOrCompleted(battles, battleId) {
    checkBattleType(battleId, BattleType.MAIN);
    return battles !== null && battles[battleId] !== null;
}

function battleIsCompleted(battles, battleId) {
    return wonFights(battles, battleId, getFightCount(BattlesMap[battleId]));
}

function battleIsOpen(battles, battleId) {
    return battleIsOpenOrCompleted(battles, battleId) && !battleIsCompleted(battles, battleId);
}

function battleIsNotOpen(battles, battleId) {
    checkBattleType(battleId, BattleType.MAIN);
    return battles === null || battles[battleId] === null;
}

function checkBattleType(battleId, type) {
    if (BattlesMap[battleId].Type !== type) {
        throw "battle with ID=" + battleId + " has invalid type"
    }
}

function checkBattleModelGenerationCondition(id, level, battles) {
    var battle = BattlesMap[id];
    return ("GenerationConditions" in battle) ? battle.GenerationConditions(level, battles) : true;
}
