var ShopBoosterModels = [
    {ID: 1, Booster: BoosterModels.CHAPTER1_RARE, Price: currencies(400, 75)},
    {ID: 2, Booster: BoosterModels.CHAPTER1_EPIC, Price: currencies(1000, 150)},
    {ID: 3, Booster: BoosterModels.CHAPTER1_LEGENDARY, Price: currencies(0, 250)},
    {ID: 201, Booster: BoosterModels.CHAPTER2_RARE, Price: currencies(400, 75)},
    {ID: 202, Booster: BoosterModels.CHAPTER2_EPIC, Price: currencies(1000, 150)},
    {ID: 203, Booster: BoosterModels.CHAPTER2_LEGENDARY, Price: currencies(0, 250)}
];

function getAvailableBoosters(chapterID) {
    var result = [];

    for (var shopBoosterID in ShopBoosterModels) {
        if (ShopBoosterModels.hasOwnProperty(shopBoosterID)) {
            var shopBooster = ShopBoosterModels[shopBoosterID];
            var boosterID = shopBooster.Booster.ID;

            if (checkBoosterAvailability(chapterID, boosterID)) {
                result.push(shopBooster.ID);
            }
        }
    }

    return result;
}
