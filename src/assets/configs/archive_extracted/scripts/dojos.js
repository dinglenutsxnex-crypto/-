var Dojos = {
    LegionDojo: {
        ID: 1,
        Type: BattleType.DOJO,
        Alias: "LegionDojo",
        Location: Locations.Dojo_Legion,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.Gizmo
                ]
            }
        ]
    },
    DynastyDojo: {
        ID: 2,
        Type: BattleType.DOJO,
        Alias: "DynastyDojo",
        Location: Locations.Camp,
        Fights: 1,
        Warriors: [
            {
                WarriorBlocks: [
                    Warriors.June
                ]
            }
        ]
    }
};

var DojosMap = createIDMap(Dojos);