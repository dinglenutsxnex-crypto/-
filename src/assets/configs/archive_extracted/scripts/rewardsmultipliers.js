var RewardsMultipliers = {
    Reward_Bonus_HeadHit: {
        ID: 1,
        Multiplier: 0.002,
        LimitPerRound: 50,
        LimitPerFight: function(roundCount) {
            return calcFightLimit(roundCount, RewardsMultipliers.Reward_Bonus_HeadHit.LimitPerRound)
        }
    },
    Reward_Bonus_ShadowAbilities: {
        ID: 2,
        Multiplier: 0.01,
        LimitPerRound: 5,
        LimitPerFight: function(roundCount) {
            return calcFightLimit(roundCount, RewardsMultipliers.Reward_Bonus_ShadowAbilities.LimitPerRound)
        }
    },
    Reward_Bonus_Critical: {
        ID: 3,
        Multiplier: 0.005,
        LimitPerRound: 50,
        LimitPerFight: function(roundCount) {
            return calcFightLimit(roundCount, RewardsMultipliers.Reward_Bonus_Critical.LimitPerRound)
        }
    },
    Reward_Bonus_FirstStrike: {
        ID: 4,
        Multiplier: 0.01,
        LimitPerRound: 1,
        LimitPerFight: function(roundCount) {
            return calcFightLimit(roundCount, RewardsMultipliers.Reward_Bonus_FirstStrike.LimitPerRound);
        }
    },
    Reward_Bonus_Combo: {
        ID: 5,
        Multiplier: 0.01,
        LimitPerRound: 50,
        LimitPerFight: function(roundCount) {
            return RewardsMultipliers.Reward_Bonus_Combo.LimitPerRound;
        }
    }
};

/**
 *
 * @param baseReward
 * @param rewardMultiplierID
 * @param amount
 * @param roundCount
 * @param silent for client always true
 * @returns {number}
 */
function calculateMultiplierReward(baseReward, rewardMultiplierID, amount, roundCount, silent){

    if (!RewardMultipliersMap.hasOwnProperty(rewardMultiplierID)) {
        throw "Multiplier with ID "+rewardMultiplierID+" not found";
    }

    var multiplierObj = RewardMultipliersMap[rewardMultiplierID];
    var realLimit = multiplierObj.LimitPerFight(roundCount);
    if (realLimit < amount) {
        if (silent) {
            amount = realLimit;
        } else {
            throw "Multiplier with id:"+rewardMultiplierID+", limit reached. amount:"+amount+", roundCount:"+roundCount+"";
        }
    }

    return Math.ceil(baseReward *  multiplierObj.Multiplier * amount);
}

function calcFightLimit(roundCount, limit) {
    return limit * roundCount;
}

function getRewardMultiplierLimit(ID) {
    return RewardMultipliersMap[ID].LimitPerRound;
}

/**
 * Global map<Integer, RewardMultiplierModel> of all models, each model should have unique ID
 */
var RewardMultipliersMap = createIDMap(RewardsMultipliers);
