var AnalyticsEventType = {
    // gameplay events
    BUY_BOOSTER: 1,
    BUY_ITEM: 2,
    CURRENCY_CHANGE: 3,
    EQUIP_ACTION: 4,
    FIGHT_END: 5,
    LEVEL_UP: 6,
    LOGIN: 7,
    LOGOUT: 8,
    OPEN_BOOSTER: 9,
    PERK_ACTION: 10,
    REFRESH_BATTLES: 11,
    SHOP_REFRESH: 12,
    CHAPTER_CHANGED: 13,

    // client events
    UI_EVENT: 100,
    CLIENT_CRASH: 101,
    CLIENT_ERROR: 102,
    CLIENT_FEEDBACK: 103,
	SCREEN_CHANGED: 104,
    CLIENT_FIGHT_END: 105,
    APPLICATION_OPEN: 106,

    // cheats
    CHEAT_ADD_EXPERIENCE: 200,
    CHEAT_ADD_ITEM: 201,
    CHEAT_ADD_PERK: 202,
    CHEAT_RESET_PERKS: 203,
    CHEAT_SET_PLAYER: 204,
    CHEAT_SET_ALL_ITEMS: 205,

    // tutorial
    TUTORIAL_STEP: 300
};

var LogLevel = {
    DISABLED: {
        ID: 1,
        Events: []
    },
    MINIMUM: {
        ID: 2,
        Events: [
            AnalyticsEventType.CHEAT_ADD_EXPERIENCE,
            AnalyticsEventType.CHEAT_ADD_ITEM,
            AnalyticsEventType.CHEAT_ADD_PERK,
            AnalyticsEventType.CHEAT_RESET_PERKS,
            AnalyticsEventType.CHEAT_SET_PLAYER,
            AnalyticsEventType.CHEAT_SET_ALL_ITEMS,

            AnalyticsEventType.LOGIN,
            AnalyticsEventType.LOGOUT,
            AnalyticsEventType.CLIENT_FEEDBACK
        ]
    },
    FULL: {
        ID: 3,
        Events: createFullEventArray()
    }
};

function createFullEventArray() {
    var result = [];
    var i = 0;
    for (var item in AnalyticsEventType) {
        result[i] = AnalyticsEventType[item];
        i++;
    }

    return result;
}

function calcLogLevel() {
    return LogLevel.FULL.ID;
}

var LogLevelMap = createIDMap(LogLevel);

