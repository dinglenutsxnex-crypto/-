/**
 * Created by Chibis on 28.10.16.
 */

var RoundRuleAttributes = {
    Description: {
        ID: 1
    },
    Name: {
        ID: 2
    },
    ApplyTo: {
        ID: 3
    },
    WeaponDamage: {
        ID: 4
    },
    UnarmedDamage: {
        ID: 5
    },
    BodyDefense: {
        ID: 6
    },
    HeadDefense: {
        ID: 7
    },
    Time: {
        ID: 8
    },
    ID: {
        ID: 9
    },
    Tag: {
        ID: 10
    },
    DarkFrames: {
        ID: 11
    },
    LightFrames: {
        ID: 12
    },
    TransitionFrames: {
        ID: 13
    },
    Duration: {
        ID: 14
    },
    Score: {
        ID: 15
    },
    Value: {
        ID: 16
    },
    RANGED_DAMAGE: {
        ID: 17
    },
    Gender: {
        ID: 18
    },
    Alias: {
        ID: 19
    }
};

var RRA = RoundRuleAttributes; // shortcut

var RoundRules = {
    ApplyFactor: {
        ID: 1,
        Attributes: attributes(
        )
    },
    Darkness: {
        ID: 2,
        Attributes: attributes(
            RRA.Description, "desc_Darkness",
            RRA.DarkFrames, 120,
            RRA.LightFrames, 180,
            RRA.TransitionFrames, 90
        )
    },
    HotGround: {
        ID: 3,
        Attributes: attributes(
            RRA.Description, "desc_HotGround",
            RRA.Time, 7
        )
    },
    GiveItem: {
        ID: 4,
        Attributes: attributes(
            RRA.ApplyTo, PLAYER,
            RRA.ID, 1000000
        )
    },
    GivePerk: {
        ID: 5,
        Attributes: attributes(
            RRA.ID, 3001
        )
    },
    GiveTag: {
        ID: 6,
        Attributes: attributes(
            RRA.Tag, "WEAPON"
        )
    },
    InvertedJoystick: {
        ID: 7,
        Attributes: attributes(
            RRA.Description, "desc_InvertedJoystick"
        )
    },
    ScoreFight: {
        ID: 8,
        Attributes: attributes(
            RRA.Description, "desc_ScoreFight",
            RRA.Score, 0
        )
    },
    WithoutHP: {
        ID: 9,
        Attributes: attributes(
        )
    },
    WithoutSF: {
        ID: 10,
        Attributes: attributes(
        )
    },
    WithoutTime: {
        ID: 11,
        Attributes: attributes(
        )
    },
    PerpetualSF: {
        ID: 13,
        Attributes: attributes(
            RRA.ApplyTo, ENEMY,
            RRA.Description, "desc_Perpetual_SF"
        )
    },
    Regeneration: {
        ID: 14,
        Attributes: attributes(
            RRA.ApplyTo, ENEMY,
            RRA.Description, "desc_Regeneration",
            RRA.Value, 0.0005
        )
    },
    Degeneration: {
        ID: 15,
        Attributes: attributes(
            RRA.ApplyTo, PLAYER,
            RRA.Description, "desc_Degeneration",
            RRA.Value, 0.0005
        )
    },
    SargeSF: {
        ID: 16,
        Attributes: attributes(
            RRA.ApplyTo, ENEMY
        )
    },
    TimeoutWin: {
        ID: 17,
        Attributes: attributes(
            RRA.Description, "desc_TimeoutWin"
        )
    },
    NoKicks: {
        ID: 18,
        Attributes: attributes(
            RRA.ApplyTo, PLAYER,
            RRA.Description, "desc_NoKicks"
        )
    },
    NoPunches: {
        ID: 19,
        Attributes: attributes(
            RRA.ApplyTo, PLAYER,
            RRA.Description, "desc_NoPunches"
        )
    },
    LifeSteal: {
        ID: 20,
        Attributes: attributes(
            RRA.ApplyTo, PLAYER,
            RRA.Value, 0.5,
            RRA.Description, "desc_LifeSteal"
        )
    },
    PerfectWin: {
        ID: 21,
        Attributes: attributes(
            RRA.ApplyTo, PLAYER,
            RRA.Description, "desc_PerfectWin"
        )
    },
    InvisibleWarrior: {
        ID: 22,
        Attributes: attributes(
            RRA.ApplyTo, ENEMY,
            RRA.Description, "desc_InvisibleWarrior"
        )
    },
    Shadowless: {
        ID: 23,
        Attributes: attributes(
            RRA.ApplyTo, ENEMY
        )
    },
    SetAlias: {
        ID: 24,
        Attributes: attributes(
            RRA.Alias, "CHAR_JUNE"
        )
    },
    SetGender: {
        ID: 25,
        Attributes: attributes(
            RRA.Gender, 1
        )
    },
    SetAppearance: {
        ID: 26,
        Attributes: attributes(
            RRA.ID, 10
        )
    }
};

/**
 * Global map<Integer,RoundRule> of all rules, each rule should have unique ID
 */
var RoundRulesMap = createIDMap(RoundRules);

function isInt(n) {
    return Number(n) === n && n % 1 === 0;
}

function attributes() {
    var attributes = {};

    for (var i = 0; i < arguments.length; i += 2) {
        attributes[arguments[i].ID] = arguments[i + 1];
    }

    return attributes;
}

function rule(idOrRule) {
    var id = isInt(idOrRule) ? idOrRule : idOrRule.ID;
    var attributes = {};
    for (var i = 1; i < arguments.length; i += 2) {
        attributes[arguments[i].ID] = arguments[i + 1];
    }
    var result = {};
    result.ID = id;
    result.Attributes = attributes;
    return result;
}