var baseFactionAttributeShift = 0.1 * stackPerLevel;
var baseFactionCritShift = 0.02;
var baseFactionCooldownShift = 2;

var baseModelAttributeShift = 0.1 * stackPerLevel;
var baseModelCritShift = 0.02;
var baseModelCooldownShift = 2;

var attributesWeights = calcAttributesEfficientValue();
var itemKoe = calcItemsKoes();

function calcItemsKoes() {
    var sum = 0;

    for (var attributeName in attributesWeights){
        if (attributesWeights.hasOwnProperty(attributeName)){
            sum += attributesWeights[attributeName];
        }
    }

    var result = {};

    result[ITEMTYPE.WEAPON] = attributesWeights[Attributes.WEAPON_DAMAGE] / sum;
    result[ITEMTYPE.ARMOR] = (attributesWeights[Attributes.UNARMED_DAMAGE] + attributesWeights[Attributes.BODY_DEFENSE]) / sum;
    result[ITEMTYPE.HELMET] = (attributesWeights[Attributes.HEAD_DEFENSE] + attributesWeights[Attributes.MAGIC_POWER]) / sum;

    for (var key in result){
        if (result.hasOwnProperty(key)){
            result[key] *= Object.keys(result).length;
        }
    }

    return result;
}

//Вычисляет значение для атрибутов предмета
function getAttributeBase(stackLevel, model) {
    var rarity = model.Rarity;
    var stackPerStep = getRarityStackPerStep(rarity);

    return Math.floor(stackLevel / stackPerStep) * stackPerStep;
}

//Возвращает начальное значение атрибутов на шмотке
function calcModelBaseTypeAttribute(model, attributeType, baseAttribute) {
    var result;

    switch (attributeType) {
        case Attributes.WEAPON_DAMAGE:
        case Attributes.RANGED_DAMAGE:
        case Attributes.UNARMED_DAMAGE:
        case Attributes.BODY_DEFENSE:
        case Attributes.HEAD_DEFENSE:
        case Attributes.MAGIC_POWER:
            result = baseAttribute;
            break;
        case Attributes.CRITICAL_CHANCE:
            result = 0.08;
            break;
        case Attributes.COOLDOWN:
            result = 10;
            break;
        default:
            throw "calcModelBaseTypeAttribute: unknown attributeType: " + attributeType;
    }

    return result;
}

//Возвращает фракционные сдвиги атрибутов
function getFactionItemTypeAttributesShifts(itemType, faction) {
    var result = {};

    switch (itemType) {
        case ITEMTYPE.WEAPON:
            switch (faction) {
                case FACTION.LEGION:
                    result[Attributes.WEAPON_DAMAGE] = 0;
                    result[Attributes.CRITICAL_CHANCE] = 0;
                    break;
                case FACTION.DYNASTY:
                    result[Attributes.WEAPON_DAMAGE] = baseFactionAttributeShift;
                    result[Attributes.CRITICAL_CHANCE] = -baseFactionCritShift;
                    break;
                case FACTION.HERALDS:
                    result[Attributes.WEAPON_DAMAGE] = -baseFactionAttributeShift;
                    result[Attributes.CRITICAL_CHANCE] = baseFactionCritShift;
                    break;
                default:
                    throw "getFactionItemTypeAttributesShifts: unknown faction: " + faction;
            }
            break;
        case ITEMTYPE.ARMOR:
            switch (faction) {
                case FACTION.LEGION:
                    result[Attributes.UNARMED_DAMAGE] = -baseFactionAttributeShift;
                    result[Attributes.BODY_DEFENSE] = baseFactionAttributeShift;
                    break;
                case FACTION.DYNASTY:
                    result[Attributes.UNARMED_DAMAGE] = baseFactionAttributeShift;
                    result[Attributes.BODY_DEFENSE] = -baseFactionAttributeShift;
                    break;
                case FACTION.HERALDS:
                    result[Attributes.UNARMED_DAMAGE] = -0;
                    result[Attributes.BODY_DEFENSE] = 0;
                    break;
                default:
                    throw "getFactionItemTypeAttributesShifts: unknown faction: " + faction;
            }
            break;
        case ITEMTYPE.HELMET:
            switch (faction) {
                case FACTION.LEGION:
                    result[Attributes.MAGIC_POWER] = -baseFactionAttributeShift;
                    result[Attributes.HEAD_DEFENSE] = baseFactionAttributeShift;
                    break;
                case FACTION.DYNASTY:
                    result[Attributes.MAGIC_POWER] = baseFactionAttributeShift;
                    result[Attributes.HEAD_DEFENSE] = -baseFactionAttributeShift;
                    break;
                case FACTION.HERALDS:
                    result[Attributes.MAGIC_POWER] = -0;
                    result[Attributes.HEAD_DEFENSE] = 0;
                    break;
                default:
                    throw "getFactionItemTypeAttributesShifts: unknown faction: " + faction;
            }
            break;
        case ITEMTYPE.RANGED:
            result[Attributes.RANGED_DAMAGE] = 0;
            result[Attributes.COOLDOWN] = 0;
            break;
        default:
            throw "getFactionItemTypeAttributesShifts: unknown itemType: " + itemType;
    }

    return result;
}

//Возвращает сдвиги атрибутов от редкости
function getRarityAttributesShifts(rarity, attributeType) {
    var shift = 0;

    switch (attributeType) {
        case Attributes.WEAPON_DAMAGE:
        case Attributes.RANGED_DAMAGE:
        case Attributes.UNARMED_DAMAGE:
        case Attributes.BODY_DEFENSE:
        case Attributes.HEAD_DEFENSE:
        case Attributes.MAGIC_POWER:
            shift = getRarityStackLevelShift(rarity);
            break;
        case Attributes.CRITICAL_CHANCE:
        case Attributes.COOLDOWN:
            shift = 0;
            break;
        default:
            throw "getRarityAttributesShifts: unknown attributeType: " + attributeType;
    }

    return shift;
}

//Вычисляет сдвиги для боевых атрибутов
function calcFightAttributeShift(attributeType) {
    var shift;

    switch (attributeType) {
        case Attributes.BODY_DEFENSE:
            shift = 1;
            break;
        case Attributes.WEAPON_DAMAGE:
        case Attributes.RANGED_DAMAGE:
        case Attributes.MAGIC_POWER:
        case Attributes.CRITICAL_CHANCE:
        case Attributes.COOLDOWN:
            shift = 1;
            break;
        case Attributes.UNARMED_DAMAGE:
            shift = 0.55;
            break;
        case Attributes.HEAD_DEFENSE:
            shift = 0.65;
            break;
        default:
            throw "calcFightAttributeShift: unknown attributeType: " + attributeType;

    }

    return (Math.log(shift) / Math.log(attributesInflation)) * stackPerLevel;
}

//Возвращает атрибуты на типе шмотки
function getAttributeTypes(itemType) {
    var result = {};

    switch (itemType) {
        case ITEMTYPE.WEAPON:
            result[Attributes.WEAPON_DAMAGE] = 0;
            result[Attributes.CRITICAL_CHANCE] = 0;
            break;
        case ITEMTYPE.ARMOR:
            result[Attributes.UNARMED_DAMAGE] = 0;
            result[Attributes.BODY_DEFENSE] = 0;
            break;
        case ITEMTYPE.HELMET:
            result[Attributes.HEAD_DEFENSE] = 0;
            result[Attributes.MAGIC_POWER] = 0;
            break;
        case ITEMTYPE.RANGED:
            result[Attributes.RANGED_DAMAGE] = 0;
            result[Attributes.COOLDOWN] = 0;
            break;
        default:
            throw "getAttributeTypes: unknown itemType: " + itemType;
    }

    return result;
}

//Вычисляет атрбуты после фракционных и редкостных сдвигов
function getShiftedAttributes(stackLevel, model) {
    var baseAttribute = getAttributeBase(stackLevel, model);
    var faction = model.Faction;
    var rarity = model.Rarity;
    var itemType = model.ItemType;
    var result = getAttributeTypes(model.ItemType);

    for (var attributeType in result) {
        if (result.hasOwnProperty(attributeType)) {
            result[attributeType] = calcModelBaseTypeAttribute(model, attributeType, baseAttribute);
            result[attributeType] += getFactionItemTypeAttributesShifts(itemType, faction)[attributeType];
            result[attributeType] += getRarityAttributesShifts(rarity, attributeType);
            result[attributeType] += getModelAttribute(model, attributeType);
        }
    }

    return result;
}

//Вычисляет атрбуты на карточке
function getAttributesVisible(stackLevel, modelID) {
    if (!ItemModelsMap.hasOwnProperty(modelID)) {
        throw "getAttributesBattle: unknown modelID: " + modelID;
    }

    var model = ItemModelsMap[modelID];
    var minLvL = 1;
    var minAttributeValue = 10;

    var maxLvL = 40;
    var maxAttributeValue = 5000;

    var ParNumb1 = (maxAttributeValue - (maxLvL * minAttributeValue / minLvL)) / (maxLvL *(maxLvL - minLvL));
    var ParNumb2 = ((minAttributeValue / minLvL) - ParNumb1 * minLvL);

    var result = getShiftedAttributes(stackLevel, model);

    for (var attributeType in result) {
        if (result.hasOwnProperty(attributeType)) {
            if(attributeType != Attributes.CRITICAL_CHANCE && attributeType != Attributes.COOLDOWN) {
                result[attributeType] = result[attributeType] / stackPerLevel;
                result[attributeType] = Math.floor(Math.pow(result[attributeType], 2) * ParNumb1 + result[attributeType] * ParNumb2);
            }
        }
    }

    return result;
}

//Вычисляет атрубуты в для боя
function getAttributesBattle(stackLevel, modelID) {
    if (!ItemModelsMap.hasOwnProperty(modelID)) {
        throw "getAttributesBattle: unknown modelID: " + modelID;
    }

    var model = ItemModelsMap[modelID];

    return getAttributesBattleLocal(stackLevel, model);
}

//TODO: Refactor JsFunction
function getAttributesBattleLocal(stackLevel, model) {
    var result = getShiftedAttributes(stackLevel, model);

    for (var attributeType in result) {
        if (result.hasOwnProperty(attributeType)) {
            result[attributeType] += calcFightAttributeShift(attributeType);
        }
    }

    return result;
}

function caclAttributeFactor(factor) {
    return Math.log(factor) * stackPerLevel / Math.log(attributesInflation);
}

function getModelAttribute(model, attributeType) {
    if (model.hasOwnProperty("Attributes")) {
        var modelAttributes = model.Attributes;

        if (modelAttributes.hasOwnProperty(attributeType)) {
            return modelAttributes[attributeType];
        }
    }

    return 0;
}

function getWarriorAttributes(warriorPower, modelIDs) {
    var result = {};

    for (var i = 0; i < modelIDs.length; ++i) {
        var modelID = modelIDs[i];

        if (!ItemModelsMap.hasOwnProperty(modelID)) {
            throw "getWarriorAttributes: unknown modelID: " + modelID;
        }

        var model = ItemModelsMap[modelID];
        var faction = model.Faction;
        var itemType = model.ItemType;
        var types = getAttributeTypes(model.ItemType);

        for (var attributeType in types) {
            if (types.hasOwnProperty(attributeType)) {
                result[attributeType] = calcModelBaseTypeAttribute(model, attributeType, warriorPower);
                result[attributeType] += getFactionItemTypeAttributesShifts(itemType, faction)[attributeType]
            }
        }
    }

    for (attributeType in result) {
        if (result.hasOwnProperty(attributeType)) {
            result[attributeType] += calcFightAttributeShift(attributeType);
        }
    }

    return result;
}

function calcAttributesEfficientValue() {
    var WB = hitsFrequencyCross.WB;
    var WH = hitsFrequencyCross.WH;
    var UB = hitsFrequencyCross.UB;
    var UH = hitsFrequencyCross.UH;
    var MB = hitsFrequencyCross.MB;
    var MH = hitsFrequencyCross.MH;

    var W = Math.pow(attributesInflation, calcFightAttributeShift(Attributes.WEAPON_DAMAGE) / stackPerLevel);
    var U = Math.pow(attributesInflation, calcFightAttributeShift(Attributes.UNARMED_DAMAGE) / stackPerLevel);
    var B = Math.pow(attributesInflation, calcFightAttributeShift(Attributes.BODY_DEFENSE) / stackPerLevel);
    var H = Math.pow(attributesInflation, calcFightAttributeShift(Attributes.HEAD_DEFENSE) / stackPerLevel);
    var M = Math.pow(attributesInflation, calcFightAttributeShift(Attributes.MAGIC_POWER) / stackPerLevel);

    var result = {};
    result[Attributes.WEAPON_DAMAGE] = W * (WB / B + WH / H);
    result[Attributes.UNARMED_DAMAGE] = U * (UB / B + UH / H);
    result[Attributes.MAGIC_POWER] = M * (MB / B + MH / H);
    result[Attributes.BODY_DEFENSE] = (WB * W + UB * U + MB * M) / B;
    result[Attributes.HEAD_DEFENSE] = (WH * W + UH * U + MH * M) / H;

    return result;
}