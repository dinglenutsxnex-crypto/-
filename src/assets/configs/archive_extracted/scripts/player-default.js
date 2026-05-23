PlayerDefaults = {
    Currencies: currencies(40 * coinBase, 200),
    Experience: 50,
    Level: 1,
    Equipments: [
        {Model: ItemModels.DefaultWeapon, SL: 0.0, Equipped: false},
        {Model: ItemModels.DefaultArmor, SL: 20.0, Equipped: false},
        {Model: ItemModels.DefaultHelm, SL: 20.0, Equipped: false},
        {Model: ItemModels.ARM_STR_12, SL: 20.0, Equipped: true},
        {
            Model: ItemModels.WPN_DUAL_SWORDS_01_01,
            SL: 20.0,
            Equipped: true,
            Perks: [{Slot: 0, Perk: PerkModels.SUPER_SWORDS_STRONG_1}]
        },
        {Model: ItemModels.HLM_STR_01, SL: 20.0, Equipped: true}
    ],
    Perks: [
        {Model: PerkModels.SUPER_SWORDS_STRONG_1, SL: 0.0}
    ],
    Permissions: Permission.NONE,

    //Client
    CurrentDojo: 1.1,
    Gender: GENDER.MALE,
    Appearance: WarriorAppearences.WA_1a,
    Tutorial: "Complete",
    Player: 1,
    Rating: 1000,
    AiMode: AiMode.NONE_MODE,
    Controller: 1,
    Tags: [
        "SKELETON",
        "HEAD",
        "PLAYER"
    ],
    MapScale: 0.7,
    GlobalVariables: {
        Server: [],
        Local: [
            {
                SelectedBattle: 40
            },
            {
                FirstRun: 1
            } ,
            {
                STATE_tutorial: 99
            }
        ]
    }
};
