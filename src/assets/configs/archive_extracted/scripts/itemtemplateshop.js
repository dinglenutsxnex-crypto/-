var baseCoinPrice = 250 * coinBase;
var baseBonusPrice = 250 * bonusBase;

var TemplateShopWeapons = [
    {
        Quantity: 0,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level < 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.WEAPON, RARITY.LEGENDARY, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.WEAPON, RARITY.LEGENDARY, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 0,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level < 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.WEAPON, RARITY.EPIC, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.WEAPON, RARITY.EPIC, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 0,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level < 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.WEAPON, RARITY.RARE, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.WEAPON, RARITY.RARE, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }
        ]
    }
];
var TemplateShopArmors = [
    {
        Quantity: 0,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level < 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.ARMOR, RARITY.LEGENDARY, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.ARMOR, RARITY.LEGENDARY, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 0,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level < 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.ARMOR, RARITY.EPIC, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.ARMOR, RARITY.EPIC, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 0,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level < 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.ARMOR, RARITY.RARE, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ITEMTYPE.ARMOR, RARITY.RARE, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }
        ]
    }
];
var TemplateShopHelms = [
    {
        Quantity: 1,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level == 1;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.LEGENDARY, 1, 1)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level == 2;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.LEGENDARY, 1, 2)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.LEGENDARY, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }


        ]
    },
    {
        Quantity: 3,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level == 1;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.EPIC, 1, 1)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level == 2;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.EPIC, 1, 2)),
                Amplifiers: [CommonAmplifiers]
            },

            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.EPIC, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }

        ]
    },
    {
        Quantity: 2,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level == 1;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.RARE, 1, 1)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level == 2;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.RARE, 1, 2)),
                Amplifiers: [CommonAmplifiers]
            },
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return level >= 3;
                },
                Models: createShopGroup(createItemGroup(ItemModelsMap, FACTION.LEGION, ANY, RARITY.RARE, 1, 3)),
                Amplifiers: [CommonAmplifiers]
            }

        ]
    }
];
var TemplateShopRanged = [
    {
        Quantity: 2,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return true;
                },
                Models: [
                    {1: ItemModels.WPN_TWOHANDEDSWORD_01},
                    {1: ItemModels.WPN_AXES_01},
                    {1: ItemModels.WPN_ONEHANDED_SWORD_01_01},
                    {1: ItemModels.WPN_DUAL_SWORDS_01_01},
                    {1: ItemModels.WPN_AXES_02_01},
                    {1: ItemModels.WPN_SPEAR_01_01},
                    {1: ItemModels.WPN_HAMMERS_01_01},
                    {1: ItemModels.ARM_STR_12},
                    {1: ItemModels.HLM_STR_12}
                ],
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 2,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return true;
                },
                Models: [
                    {1: ItemModels.WPN_TWOHANDEDSWORD_02},
                    {1: ItemModels.WPN_TWOHANDEDSWORD_03},
                    {1: ItemModels.WPN_AXES_02},
                    {1: ItemModels.WPN_AXES_03},
                    {1: ItemModels.WPN_ONEHANDED_SWORD_01_02},
                    {1: ItemModels.WPN_ONEHANDED_SWORD_01_04},
                    {1: ItemModels.WPN_DUAL_SWORDS_01_02},
                    {1: ItemModels.WPN_AXES_02_02},
                    {1: ItemModels.WPN_AXES_02_03},
                    {1: ItemModels.WPN_SPEAR_01_02},
                    {1: ItemModels.WPN_SPEAR_01_03},
                    {1: ItemModels.WPN_HAMMERS_01_02},
                    {1: ItemModels.ARM_STR_05},
                    {1: ItemModels.ARM_STR_04},
                    {1: ItemModels.ARM_STR_03},
                    {1: ItemModels.ARM_STR_02},
                    {1: ItemModels.HLM_STR_01},
                    {1: ItemModels.HLM_STR_03},
                    {1: ItemModels.HLM_STR_08},
                    {1: ItemModels.HLM_STR_02},
                    {1: ItemModels.HLM_STR_11}
                ],
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 1,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return true;
                },
                Models: [
                    {1: ItemModels.WPN_TWOHANDEDSWORD_04},
                    {1: ItemModels.WPN_ONEHANDED_SWORD_01_03},
                    {1: ItemModels.WPN_DUAL_SWORDS_01_03},
                    {1: ItemModels.WPN_SPEAR_01_04},
                    {1: ItemModels.WPN_HAMMERS_02_01},
                    {1: ItemModels.ARM_STR_07},
                    {1: ItemModels.HLM_STR_09}
                ],
                Amplifiers: [CommonAmplifiers]
            }
        ]
    },
    {
        Quantity: 1,
        Blocks: [
            {
                /** @return {boolean} */
                Restriction: function (level) {
                    return true;
                },
                Models: [
                    {1: ItemModels.WPN_DUAL_SWORDS_02_01},
                    {1: ItemModels.WPN_HAMMERS_02_02},
                    {1: ItemModels.WPN_ONEHANDED_SWORD_02_01},
                    {1: ItemModels.WPN_ONEHANDED_SWORD_02_02},
                    {1: ItemModels.ARM_STR_10},
                    {1: ItemModels.HLM_STR_10}
                ],
                Amplifiers: [CommonAmplifiers]
            }
        ]
    }
];
/**
 * List of all shop templates
 */
var ItemTemplateShopList = [TemplateShopHelms];

/** @return {boolean} */
function checkItemTemplateShopRestriction(templateListIndex, templateShopIndex, templateBlockIndex, level) {
    var templateShop = ItemTemplateShopList[templateListIndex];
    var template = templateShop[templateShopIndex];
    var block = template.Blocks[templateBlockIndex];

    return block.Restriction(level);
}

/**
 * Model price calculator
 * @param modelId
 * @param stackLevel
 * @param playerLevel
 * @param purchaseCount
 * @returns {*}
 */
function calcItemPriceByModelId(modelId, stackLevel, playerLevel, purchaseCount) {
    var price;

    if (modelId in ItemModelsMap) {
        var model = ItemModelsMap[modelId];
        if (model.hasOwnProperty("Price"))
            price = model.Price(stackLevel, playerLevel, purchaseCount);
        else
            price = calcUpgradeItemPrice(playerLevel, modelId, stackLevel);
    }

    return roundedCurrencies(price[CURRENCY.COIN], price[CURRENCY.BONUS]);
}

function calcCardsForLevel(rarity) {
    var cardInflation = getRarityCardsInflation(rarity);
    var cardForStepInf = calcRarityCardsForStepInf(rarity);
    var cardsForStep = getRarityCardsForStepsBase(rarity);

    return cardsForStep * (cardInflation - 1) / (cardForStepInf - 1);
}

function calcBaseItemCoinPrice(model) {
    var rarity = model.Rarity;
    var itemType = model.ItemType;

    return baseCoinPrice * calcTypeItemCoinMultiplier(itemType) * calcRarityItemCoinMultiplier(rarity);
}

function calcTypeItemCoinMultiplier(itemType) {
    var modiff = 1;

    switch (itemType) {
        case ITEMTYPE.WEAPON:
            return modiff *= itemKoe[ITEMTYPE.WEAPON];
        case ITEMTYPE.ARMOR:
            return modiff *= itemKoe[ITEMTYPE.ARMOR];
        case ITEMTYPE.HELMET:
            return modiff *= itemKoe[ITEMTYPE.HELMET];
        case ITEMTYPE.RANGED:
            return modiff *= itemKoe[ITEMTYPE.WEAPON];
        default:
            throw "calcTypeItemCoinMultiplier: unknown ItemType: " + itemType;
    }
}

function calcRarityItemCoinMultiplier(rarity) {
    var modiff = 1 / calcCardsForLevel(rarity);

    switch (rarity) {
        case RARITY.COMMON:
        case RARITY.RARE:
            return modiff *= 1;
        case RARITY.EPIC:
            return modiff *= 1;
        case RARITY.LEGENDARY:
            return modiff *= 0;
        default:
            throw "calcRarityItemCoinMultiplier: unknown rarity: " + rarity;
    }
}

function calcBaseItemBonusPrice(model) {
    var rarity = model.Rarity;
    var itemType = model.ItemType;

    return baseBonusPrice * calcRarityItemBonusMultiplier(rarity) * calcTypeItemBonusMultiplier(itemType);
}

function calcRarityItemBonusMultiplier(rarity){
    var modiff = 1 / calcCardsForLevel(rarity);

    switch (rarity) {
        case RARITY.COMMON:
            return modiff *= 1;
        case RARITY.RARE:
            return modiff *= 1;
        case RARITY.EPIC:
            return modiff *= 1.1;
        case RARITY.LEGENDARY:
            return modiff *= 1.2;
        default:
            throw "calcRarityItemBonusMultiplier: unknown rarity: " + rarity;
    }
}

function calcTypeItemBonusMultiplier(itemType) {
    var modiff = 1;

    switch (itemType) {
        case ITEMTYPE.WEAPON:
            return modiff *= itemKoe[ITEMTYPE.WEAPON];
        case ITEMTYPE.ARMOR:
            return modiff *= itemKoe[ITEMTYPE.ARMOR];
        case ITEMTYPE.HELMET:
            return modiff *= itemKoe[ITEMTYPE.HELMET];
        case ITEMTYPE.RANGED:
            return modiff *= itemKoe[ITEMTYPE.WEAPON];
        default:
            throw "calcTypeItemBonusMultiplier: unknown ItemType: " + itemType;
    }
}

function calcInflationMultiplier(stackLevel) {
    var itemLevel = Math.floor(stackLevel / stackPerLevel);

    return Math.pow(moneyInflation, itemLevel);
}

/**
 * Default model price calculator
 * @param model
 * @param stackLevel
 * @param playerLevel
 * @param purchaseCount
 * @returns {{}}
 */
function calcDefaultItemPrice(model, stackLevel, playerLevel, purchaseCount) {
    var coinPrice = calcBaseItemCoinPrice(model);
    var bonusPrice = calcBaseItemBonusPrice(model);

    coinPrice *= calcInflationMultiplier(stackLevel);

    var result = {};

    result[CURRENCY.COIN] = Math.floor(coinPrice);
    result[CURRENCY.BONUS] = Math.floor(bonusPrice);

    return result;
}

function calcUpgradeStackLevel(playerLevel, model, stackLevel) {
    var rarity = model.Rarity;
    var stackPerStep = getRarityStackPerStep(rarity);

    return Math.max((Math.floor(stackLevel / stackPerStep) + 1) * stackPerStep, playerLevel * stackPerLevel, model.Level * stackPerLevel);
}

function calcItemUpgradeMultiplier(playerLevel, model, stackLevel) {
    var rarity = model.Rarity;
    var stackPerStep = getRarityStackPerStep(rarity);
    var stepsPerLevel = getRarityStepsPerLevel(rarity);
    var cardInflation = getRarityCardsInflation(rarity);
    var itemLevel = Math.floor(stackLevel / stackPerLevel);

    var targetStackLevel = calcUpgradeStackLevel(playerLevel, model, stackLevel);
    var diff = (targetStackLevel - stackLevel) / stackPerStep;
    var itemStep = Math.floor((stackLevel % stackPerLevel) / stackPerStep) / stepsPerLevel;
    var multiplier = Math.pow(cardInflation, itemStep  + itemLevel - playerLevel) * diff;


    if (itemLevel < playerLevel) {
        if (diff < stepsPerLevel) {
            multiplier = Math.min(1, multiplier);
        } else {
            multiplier = 1;
        }
    }

    return multiplier;
}

function calcBuyItemPriceOld(playerLevel, model) {
    var itemLevel = Math.max(playerLevel, model.Level);

     return calcDefaultItemPrice(model, itemLevel * stackPerLevel, playerLevel, 0);
}

function calcBuyItemPrice(playerLevel, model) {
    var rarity = model.Rarity;
    var itemType = model.ItemType;
    var shift = getRarityStackLevelShift(rarity) / stackPerLevel;
    var itemLevel = Math.max(playerLevel, model.Level);

    var priceKoe = Math.pow(shift, 2) * (1 - shift / 3) * itemKoe[itemType];
    var coinPrice = baseCoinPrice * priceKoe;
    var bonusPrice = baseBonusPrice * priceKoe;

    coinPrice *= calcInflationMultiplier(itemLevel * stackPerLevel);

    if (rarity == RARITY.LEGENDARY) {
        coinPrice = 0;
    }

    var result = {};

    result[CURRENCY.COIN] = Math.floor(coinPrice);
    result[CURRENCY.BONUS] = Math.floor(bonusPrice);

    return result;
}

function calcUpgradeItemPrice(playerLevel, model, stackLevel) {
    if (stackLevel == 0) {
        return calcBuyItemPrice(playerLevel, model);
    }

    var itemPrice = calcDefaultItemPrice(model, playerLevel * stackPerLevel, playerLevel, 0);
    var multiplier = calcItemUpgradeMultiplier(playerLevel, model, stackLevel);

    itemPrice[CURRENCY.COIN] *= multiplier;
    itemPrice[CURRENCY.BONUS] *= multiplier;

    return itemPrice;
}

function calcShopItemParameters(playerLevel, modelID, stackLevel) {
    if (!ItemModelsMap.hasOwnProperty(modelID)) {
        throw "calcShopItemParameters: unknown modelID: " + modelID;
    }

    var model = ItemModelsMap[modelID];
    playerLevel = Math.max(playerLevel, 2);

    return calcShopItemParametersLocal(playerLevel, model, stackLevel)
}

function calcShopItemParametersLocal(playerLevel, model, stackLevel) {
    var currencies = calcUpgradeItemPrice(playerLevel, model, stackLevel);
    currencies = roundedCurrencies(currencies[CURRENCY.COIN], currencies[CURRENCY.BONUS]);

    return {
        Price: currencies,
        SL: calcUpgradeStackLevel(playerLevel, model, stackLevel)
    };
}

function calcStackLevel(playerLevel, amplifierValue, modelId) {
    var model = ItemModelsMap[modelId];
    var rarity = model.Rarity;
    var stepsInLevel = getRarityStepsPerLevel(rarity);
    //TODO Sort shop and return correct formula
    return stackPerLevel * (/*playerLevel - */1 + amplifierValue / stepsInLevel);
}

function createShopGroup(items) {
    var result = [];

    for (var factionName in items) {
        if (items.hasOwnProperty(factionName)) {
            var faction = items[factionName];

            for (var i = 0; i < faction.length; ++i) {
                result.push({1: faction[i]});
            }
        }
    }

    return result;
}

