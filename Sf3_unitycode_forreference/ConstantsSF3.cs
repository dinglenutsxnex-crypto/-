using System.Collections.Generic;
using SF3_Attributes;
using UnityEngine;

public class ConstantsSF3 : MonoBehaviour
{
	public enum QuestEvents
	{
		OnPostPurchase = 0,
		OnPrePurchase = 1,
		OnPreSceneChange = 2,
		OnPostSceneChange = 3,
		OnFightEnter = 4,
		OnFightEnd = 5,
		OnStartSession = 6,
		OnEquip = 7,
		OnUnEquip = 8,
		OnShopRefresh = 9,
		OnLevelUp = 10,
		OnTimerEnd = 11
	}

	public enum ELocationSceneModule
	{
		None = 0,
		Preloader = 1,
		Fight = 2,
		Shop = 3,
		Inventory = 4,
		PerkScreen = 5,
		Map = 6,
		DojoInterface = 7,
		CharacterCreation = 8,
		BoosterpacksScreen = 9
	}

	public const int TARGET_FRAME_RATE = 60;

	public const float DEFAULT_ASPECT = 1.33f;

	public const float stackPerLevel = 10f;

	public const float inflationKoe = 4f;

	public const float LowerTransparencyLimit = 0.001f;

	public const float UpperTransparencyLimit = 0.999f;

	public static Dictionary<AttributeType, float> AttributeShifts = new Dictionary<AttributeType, float>
	{
		{
			AttributeType.WeaponDamage,
			1f
		},
		{
			AttributeType.RangedDamage,
			1f
		},
		{
			AttributeType.UnarmedDamage,
			0.5f
		},
		{
			AttributeType.BodyDefense,
			1f
		},
		{
			AttributeType.HeadDefense,
			0.65f
		},
		{
			AttributeType.CriticalChance,
			1f
		},
		{
			AttributeType.MagicPower,
			1f
		}
	};

	public static float GetCurrentAspect
	{
		get
		{
			return (float)Screen.width / (float)Screen.height / 1.33f;
		}
	}
}
