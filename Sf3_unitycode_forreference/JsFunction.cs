using System.Collections.Generic;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;

public class JsFunction
{
	public static Dictionary<string, JsValue> CalculateNewLevel(long level, long experience, long value)
	{
		return JS.CallFunction("newLevelCalculate", level, experience, value).AsDictionary();
	}

	public static float CalculateAttributeFactor(float factorValue)
	{
		return JS.CallFunction("caclAttributeFactor", factorValue).AsFloat();
	}

	public static Dictionary<string, JsValue> CalculateShopitemParameters(int level, int itemId, double stackLevel)
	{
		return JS.CallFunction("calcShopItemParameters", level, itemId, stackLevel).AsDictionary();
	}

	public static Dictionary<string, JsValue> GetWarriorAttributes(double stacklevel, int[] modelIds)
	{
		return JS.CallFunction("getWarriorAttributes", stacklevel, modelIds).AsDictionary();
	}

	public static long CalculateRewardMultiplier(long baseReward, int multiplierId, int amount, int roundsPlayed, bool silent)
	{
		return JS.CallFunction("calculateMultiplierReward", baseReward, multiplierId, amount, roundsPlayed, silent).AsLong();
	}

	public static double MergeStackLevels(double stackLevel, double mergeWithStackLevel, int rarity, int level)
	{
		return JS.CallFunction("mergeStackLevels", stackLevel, mergeWithStackLevel, rarity, level).AsNumber();
	}

	public static int GetLevelUpCount(double stackLevel, double newItemStackLevel, int rarity)
	{
		return JS.CallFunction("getLevelUpCount", stackLevel, newItemStackLevel, rarity).AsInteger();
	}

	public static float GetBar(double stackLevel, int rarity)
	{
		return JS.CallFunction("getBar", stackLevel, rarity).AsFloat();
	}

	public static float GetLevelUpBar(double stackLevel, double newItemStackLevel, int rarity)
	{
		return JS.CallFunction("getLevelUpBar", stackLevel, newItemStackLevel, rarity).AsFloat();
	}

	public static JsValue GetAttributesBattle(double stackLevel, int id)
	{
		return JS.CallFunction("getAttributesBattle", stackLevel, id);
	}

	public static JsValue GetAttributesVisible(double stackLevel, int id)
	{
		return JS.CallFunction("getAttributesVisible", stackLevel, id);
	}

	public static float CalculateStrikeDamage(float baseDamage, float attackAttribute, float defenseAttribute)
	{
		return JS.CallFunction("calcStrikeDamage", baseDamage, attackAttribute, defenseAttribute).AsFloat();
	}

	public static int calcItemPerkSlotsByModelId(int id)
	{
		return JS.CallFunction("calcItemPerkSlotsByModelId", id).AsInteger();
	}

	public static double MergeStackLevelsPerks(double stackLevel, double mergeWithStackLevel, int rarity, int level)
	{
		return JS.CallFunction("mergeStackLevelsPerks", stackLevel, mergeWithStackLevel, rarity, level).AsNumber();
	}

	public static int GetLevelUpCountPerks(double stackLevel, double newItemStackLevel, int rarity)
	{
		return JS.CallFunction("getLevelUpCountPerks", stackLevel, newItemStackLevel, rarity).AsInteger();
	}

	public static float GetBarPerks(double stackLevel, int rarity)
	{
		return JS.CallFunction("getBarPerks", stackLevel, rarity).AsFloat();
	}

	public static float GetLevelUpBarPerks(double stackLevel, double newItemStackLevel, int rarity)
	{
		return JS.CallFunction("getLevelUpBarPerks", stackLevel, newItemStackLevel, rarity).AsFloat();
	}

	public static ObjectInstance GetPerkLeveling(int id, double stackLevel)
	{
		return JS.CallFunction("getPerkLeveling", id, stackLevel).AsObject();
	}

	public static Dictionary<string, JsValue> GetAttributesVisibleNextLevel(double stackLevel, int modelId)
	{
		return JS.CallFunction("getAttributesVisibleNextLevel", stackLevel, modelId).AsDictionary();
	}

	public static int CalculateRealExperience(int level, long experience)
	{
		return JS.CallFunction("calcRealExp", level, experience).AsInteger();
	}

	public static Dictionary<string, JsValue> CalculateLevelAndExperience(int experience)
	{
		return JS.CallFunction("calcLevelAndExp", experience).AsDictionary();
	}

	public static Dictionary<string, JsValue> GetPerkTypeRestrictions(int id)
	{
		return JS.CallFunction("getPerkTypeRestrictions", id).AsDictionary();
	}

	public static float CalculateFightDifficulty(params object[] content)
	{
		return JS.CallFunction("calcFightDifficulty", content).AsFloat();
	}

	public static float GetBaseDamageModifier(int perkId, double stackLevel)
	{
		return JS.CallFunction("getSuperMoveMultiplier", perkId, stackLevel).AsFloat();
	}

	public static float GetBoosterSize(int boosterId)
	{
		return JS.CallFunction("getBoosterSize", boosterId).AsFloat();
	}

	public static Dictionary<string, JsValue> GetBoosterCardsRarities(int boosterId)
	{
		return JS.CallFunction("getBoosterCardsRarities", boosterId).AsDictionary();
	}

	public static ArrayInstance GetAvailableBoosters(int chapterID)
	{
		return JS.CallFunction("getAvailableBoosters", chapterID).AsArray();
	}
}
