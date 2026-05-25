// Ported from Assembly-CSharp/sf3DTO/FightResult.cs
// Original used Google.Protobuf [OriginalName] attributes for wire names —
// those are preserved here as string constants for any serialization needs.

export enum FightResult {
  UnknownFightResult = 0,   // proto: "UNKNOWN_FIGHT_RESULT"
  Win                = 1,   // proto: "WIN"
  Loss               = 2,   // proto: "LOSS"
  Surrender          = 3,   // proto: "SURRENDER"
}

/** Wire names for serialization (mirrors proto [OriginalName]). */
export const FightResultWireName: Record<FightResult, string> = {
  [FightResult.UnknownFightResult]: "UNKNOWN_FIGHT_RESULT",
  [FightResult.Win]:                "WIN",
  [FightResult.Loss]:               "LOSS",
  [FightResult.Surrender]:          "SURRENDER",
};
