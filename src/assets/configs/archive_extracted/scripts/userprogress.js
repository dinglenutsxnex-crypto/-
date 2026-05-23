/*var UserProgress = [
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000,
    50,
    250,
    500,
    1000,
    2000
];*/
var UserProgress = [];

FillLvLprogress(100, 200, 225, 30);

function FillLvLprogress(startingExp1, startingExp2, startingExp3, AmountOfLevels) {
    UserProgress.push(startingExp1);
    UserProgress.push(startingExp2);
    UserProgress.push(startingExp3);

    for(var i = 3; i < (AmountOfLevels); i++) {
        UserProgress.push(Math.floor(UserProgress[i-1] * experienceInflation))
    }
}

function calcRealExp(level, exp) {
    var result = exp;

    for(var i = 0; i < Math.min(level - 1, UserProgress.length); ++i) {
        result += UserProgress[i];
    }

    return result;
}

function calcLevelAndExp(exp) {
    var curExp = exp;
    var level = 1;

    while(level < UserProgress.length && curExp >= UserProgress[level - 1]) {
        curExp -= UserProgress[level - 1];
        ++level;
    }

    return {
        Level: level,
        Experience: curExp,
        LevelExperience: UserProgress[level - 1]
    };
}

function newLevelCalculate(level, exp, newExp){
    var newParams = calcLevelAndExp(calcRealExp(level, exp) + newExp);

    return {
        Level: newParams.Level,
        Experience: newParams.Experience,
        LevelExperience: UserProgress[newParams.Level - 1]
    };
}

function calcExpForLevelUp(oldLevel, newLevel, curExp) {
    var newParams = newLevelCalculate(oldLevel, curExp, 0);
    oldLevel = newParams.Level;
    curExp = newParams.Experience;
    
    if (newLevel <= oldLevel) {
        return 0;
    }

    var result = UserProgress[oldLevel - 1] - curExp;

    for(var i = oldLevel; i + 1 < newLevel; ++i) {
        result += UserProgress[i];
    }

    return result;
}