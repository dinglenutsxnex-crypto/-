var boostersSettings = {
    CHAPTER1_RARE: {
        ItemPool: zonesItems[0],
        PerkPool: zonesPerks[0],
        Weights: [1000, 300, 75, 5],
        Quantities: [4, 1, 0, 0]
    },
    CHAPTER1_EPIC: {
        ItemPool: zonesItems[0],
        PerkPool: zonesPerks[0],
        Weights: [1000, 300, 75, 5],
        Quantities: [3, 1, 1, 0]
    },
    CHAPTER1_LEGENDARY: {
        ItemPool: zonesItems[0],
        PerkPool: zonesPerks[0],
        Weights: [1000, 300, 75, 5],
        Quantities: [2, 1, 1, 1]
    },
    CHAPTER2_RARE: {
        ItemPool: zonesItems[0],
        PerkPool: zonesPerks[0],
        Weights: [1000, 300, 75, 5],
        Quantities: [4, 1, 0, 0]
    },
    CHAPTER2_EPIC: {
        ItemPool: zonesItems[0],
        PerkPool: zonesPerks[0],
        Weights: [1000, 300, 75, 5],
        Quantities: [3, 1, 1, 0]
    },
    CHAPTER2_LEGENDARY: {
        ItemPool: zonesItems[0],
        PerkPool: zonesPerks[0],
        Weights: [1000, 300, 75, 5],
        Quantities: [2, 1, 1, 1]
    }
};

var BoosterModels = {
    CHAPTER1_RARE: {
        ID: 3,
        Rarity: RARITY.RARE,
        Zone: Zones.Zone1,
        Alias: "CHAPTER1_RARE",
        Image: "NewYear2017",
        Description: "",
        Loot: {
            And: populateBooster(
                zonesItems[0], zonesPerks[0],
                stackPerLevel * 2,
                1000, 300, 75, 5,
                4, 1
            )
        }
    },
    CHAPTER1_EPIC: {
        ID: 4,
        Rarity: RARITY.EPIC,
        Zone: Zones.Zone1,
        Alias: "CHAPTER1_EPIC",
        Image: "NewYear2017",
        Description: "",
        Loot: {
            And: populateBooster(
                zonesItems[0], zonesPerks[0],
                stackPerLevel * 2,
                1000, 300, 75, 5,
                3, 1, 1
            )
        }
    },
    CHAPTER1_LEGENDARY: {
        ID: 5,
        Rarity: RARITY.LEGENDARY,
        Zone: Zones.Zone1,
        Alias: "CHAPTER1_LEGENDARY",
        Image: "CHAPTER1_RARE",
        Description: "",
        Loot: {
            And: populateBooster(
                zonesItems[0], zonesPerks[0],
                stackPerLevel * 2,
                1000, 300, 75, 5,
                2, 1, 1, 1
            )
        }
    },
    CHAPTER2_RARE: {
        ID: 203,
        Rarity: RARITY.RARE,
        Zone: Zones.Zone2,
        Alias: "CHAPTER2_RARE",
        Description: "",
        Loot: {
            And: populateBooster(
                zonesItems[0], zonesPerks[0],
                stackPerLevel * 2,
                1000, 300, 75, 5,
                4, 1
            )
        }
    },
    CHAPTER2_EPIC: {
        ID: 204,
        Rarity: RARITY.EPIC,
        Zone: Zones.Zone2,
        Alias: "CHAPTER2_EPIC",
        Image: "CHAPTER1_EPIC",
        Description: "",
        Loot: {
            And: populateBooster(
                zonesItems[0], zonesPerks[0],
                stackPerLevel * 2,
                1000, 300, 75, 5,
                3, 1, 1
            )
        }
    },
    CHAPTER2_LEGENDARY: {
        ID: 205,
        Rarity: RARITY.LEGENDARY,
        Zone: Zones.Zone2,
        Alias: "CHAPTER2_LEGENDARY",
        Image: "CHAPTER1_LEGENDARY",
        Description: "",
        Loot: {
            And: populateBooster(
                zonesItems[0], zonesPerks[0],
                stackPerLevel * 2,
                1000, 300, 75, 5,
                2, 1, 1, 1
            )
        }
    },
    CHAPTER1_FIRST: {
        ID: 6,
        Rarity: RARITY.EPIC,
        Zone: Zones.Zone1,
        Alias: "CHAPTER1_EPIC",
        Image: "CHAPTER1_EPIC",
        Description: "",
        Loot: {
            And: [
                {And: createEquipments(2 * stackPerLevel, zonesItems[0][RARITY.COMMON], 3)},
                {And: createEquipments(2 * stackPerLevel, zonesItems[0][RARITY.RARE], 1)},
                {And: createEquipments(2 * stackPerLevel, zonesItems[0][RARITY.EPIC], 1)}
            ]
        }
    }
};

/**
 * Global map<Integer,BoosterModel> of all models, each model should have unique ID
 */

function createChoice(stackLevel, perksPool, itemsPool, weight) {
    var result = [];
    var oneDrop = [];
    var i;
    
    for (i = 0; i < itemsPool.length; ++i) {
        var model = itemsPool[i];
        var oneItem = {Equipments: {SL: Math.max(stackLevel, model.Level * stackPerLevel), Models: [itemsPool[i]]}};
        oneDrop.push(oneItem);
    }

    for (i = 0; i < perksPool.length; ++i) {
        var onePerk = {Perks: {SL: stackLevel, Models: [perksPool[i]]}};
        oneDrop.push(onePerk);
    }

    return {Or: oneDrop, Weight: weight};
}

function populateBooster (itemPool, perkPool, stackLevel, commonWeight, rareWeight, epicWeight, legendaryWeight, commonQuantity, rareQuantity,  epicQuantity, legendaryQuantity) {
    var loot = [];
    var i;
    var lootSettings = [
        createChoice(stackLevel, perkPool[RARITY.COMMON], itemPool[RARITY.COMMON] , commonWeight),
        createChoice(stackLevel, perkPool[RARITY.RARE], itemPool[RARITY.RARE], rareWeight),
        createChoice(stackLevel, perkPool[RARITY.EPIC], itemPool[RARITY.EPIC], epicWeight),
        createChoice(stackLevel, perkPool[RARITY.LEGENDARY], itemPool[RARITY.LEGENDARY], legendaryWeight)
    ];

    if(commonQuantity) {
        for (i = 0; i < commonQuantity; ++i) {
            loot.push({Or: lootSettings});
        }
    }

    if(rareQuantity) {
        for (i = 0; i < rareQuantity; ++i) {
            loot.push({Or: lootSettings.slice(1)});
        }
    }

    if(epicQuantity) {
        for (i = 0; i < epicQuantity; ++i) {
            loot.push({Or: lootSettings.slice(2)});
        }
    }

    if(legendaryQuantity) {
        for (i = 0; i < legendaryQuantity; ++i) {
            loot.push({Or: lootSettings.slice(3)});
        }
    }

    return loot;
}

var BoosterModelsMap = createIDMap(BoosterModels);

function getBoosterSize(boosterID) {
    var cardRarities = getBoosterCardsRarities(boosterID);
    var result = 0;

    for (var rarity in cardRarities) {
        if (cardRarities.hasOwnProperty(rarity)) {
            result += cardRarities[rarity];
        }
    }

    return result;
}

function getBoosterCardsRarities(boosterID) {
    var boosterSettings = boostersSettings[BoosterModelsMap[boosterID].Name];
    var result = {};

    for (var rarityName in RARITY) {
        if (RARITY.hasOwnProperty(rarityName)) {
            var rarity = RARITY[rarityName];

            if (rarity) {
                result[rarity] = boosterSettings.Quantities[rarity - 1];
            }
        }
    }

    return result;
}

function checkBoosterAvailability(chapterID, boosterID) {
    var zoneID = getChapterZoneID(chapterID);

    return zoneID == BoosterModelsMap[boosterID].Zone.ID;
}
