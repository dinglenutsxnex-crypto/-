var Zones = {
    Zone1: {ID: 1, Value: 1},
    Zone2: {ID: 2, Value: 2}
};

var ChapterModels = {
    Chapter1: {ID: 100, Zone: Zones.Zone1},
    Chapter1_1: {ID: 110, Zone: Zones.Zone1},
    Chapter1_2: {ID: 120, Zone: Zones.Zone1}
};

var ZonesMap = createIDMap(Zones);

var ChapterModelsMap = createIDMap(ChapterModels);

function getChapterZoneID(chapterID) {
    if (ChapterModelsMap[chapterID].hasOwnProperty("Zone")) {
        return ChapterModelsMap[chapterID].Zone.ID;
    }

    throw "No zone binding to chapterID: chapterID"
}
