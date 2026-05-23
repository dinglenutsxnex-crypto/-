var ABTests = {
    ShopPrices: {
        HigherShopPrices: {
            Tag: "higher-shop-prices",
            Part: 0
        },
        LowerShopPrices: {
            Tag: "lower-shop-prices",
            Part: 0
        }
    }
};

checkTestsSum(ABTests);
var ABTestsMap = makeTestsTagMap(ABTests);
createTestsPath(ABTestsMap);

function chooseABTag(playerABTag) {
    if (playerABTag != null) {
        return playerABTag
    }

    var partSum = 0;
    var result;
    var randChoice = Math.random();
    var test = ABTests.ShopPrices;

    for (var groupName in test) {
        if (test.hasOwnProperty(groupName)) {
            var group = test[groupName];

            partSum += group.Part;

            if (partSum > randChoice) {
                return group.Tag;
            }
        }
    }

    return null;
}

function getPathByABTag(tag) {
    if (ABTestsMap.hasOwnProperty(tag)) {
        return ABTestsMap[tag].Path;
    } else {
        throw "No group with tag " + tag;
    }
}

function checkTestsSum(abTests) {
    for (var testName in abTests) {
        if (abTests.hasOwnProperty(testName)) {
            var test = abTests[testName];
            var partSum = 0;

            for (var groupName in test) {
                if (test.hasOwnProperty(groupName)) {
                    var group = test[groupName];

                    partSum += group.Part;
                }
            }

            if (partSum >= 1) {
                throw "Parts sum more than 1 in test " + testName;
            }
        }
    }
}

function makeTestsTagMap(abTests) {
    var testsTagMap = {};

    for (var testName in abTests) {
        if (abTests.hasOwnProperty(testName)) {
            var test = abTests[testName];

            for (var groupName in test) {
                if (test.hasOwnProperty(groupName)) {
                    var group = test[groupName];

                    testsTagMap[group.Tag] = group;
                }
            }
        }
    }

    return testsTagMap;
}

function createTestsPath(abTestsMap) {
    for (var tag in abTestsMap) {
        if (abTestsMap.hasOwnProperty(tag)) {
            var group = abTestsMap[tag];

            group.Path = "scripts/" + group.Tag;
        }
    }
}

function getABTagList() {
    var keyList = [];

    for (var key in ABTestsMap) {
        if (ABTestsMap.hasOwnProperty(key)) {
            keyList.push(key);
        }
    }

    return keyList;
}