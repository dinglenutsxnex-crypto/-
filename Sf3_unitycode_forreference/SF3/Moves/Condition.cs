using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nekki;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Items;
using SF3.KeyPressInfo;
using SF3.UserData;
using UnityEngine;
using sf3DTO;

namespace SF3.Moves
{
	public class Condition : IConditionEqual
	{
		protected class FormulaContainer
		{
			private RpnParser.Formula _formula;

			protected static Dictionary<string, object> functionResultsStack = new Dictionary<string, object>();

			public FormulaContainer(RpnParser.Formula formula)
			{
				_formula = formula;
			}

			public static void ClearStack()
			{
				functionResultsStack.Clear();
			}

			public object Calculate()
			{
				return _formula.calculate();
			}
		}

		public const string precompilePrefix = "?CLC_";

		public const string functionPrefix = "?";

		public CompareOperationType compareOperation;

		protected FormulaContainer _formula1;

		protected FormulaContainer _formula2;

		protected object precompileValue;

		protected int _ignoredByAi;

		public const string Equal = "Equal";

		public const string Greater = "Greater";

		public const string GreaterEqual = "GreaterEqual";

		public const string Less = "Less";

		public const string LessEqual = "LessEqual";

		public static readonly string[] CompareOperationsAll = new string[5] { "Equal", "Greater", "GreaterEqual", "Less", "LessEqual" };

		public const string Or = "Or";

		public const string And = "And";

		public const string Not = "Not";

		public static readonly string[] LogicOperationsAll = new string[3] { "Or", "And", "Not" };

		public const string Value1 = "Value1";

		public const string Value2 = "Value2";

		public const string CurrentAnimation = "CurrentAnimation";

		public const string IgnoredByAi = "IgnoredByAI";

		private const int UnignoredByAiFlag = -1;

		public Condition()
		{
		}

		public Condition(string formula1Value, string formula2Value, CompareOperationType conditionCompareOperation, int ignoredByAi)
		{
			compareOperation = conditionCompareOperation;
			_formula1 = new FormulaContainer(new RpnParser.Formula(formula1Value));
			_formula2 = new FormulaContainer(new RpnParser.Formula(formula2Value));
			_ignoredByAi = ignoredByAi;
			precompileValue = null;
			if (formula1Value.Contains("?CLC_") || !formula1Value.Contains("?"))
			{
				precompileValue = _formula1.Calculate();
				_formula1 = _formula2;
				_formula2 = null;
			}
			else if (formula2Value.Contains("?CLC_") || !formula2Value.Contains("?"))
			{
				precompileValue = _formula2.Calculate();
				_formula2 = null;
			}
		}

		public static void ClearFunctionResults()
		{
			FormulaContainer.ClearStack();
		}

		public bool? IsEqual()
		{
			Model currentModel = TriggersVerification.currentVerificationData.currentModel;
			if (_ignoredByAi > 0 && currentModel != null && currentModel.IsAI)
			{
				return true;
			}
			object obj = _formula1.Calculate();
			if (obj == null)
			{
				return false;
			}
			if (_formula2 != null)
			{
				precompileValue = _formula2.Calculate();
			}
			if (obj is List<KeyData>)
			{
				if (!currentModel.IsAI)
				{
					return CompareKeyDataValue((List<KeyData>)obj, (List<KeyData>)precompileValue);
				}
				return false;
			}
			if (obj is string)
			{
				return CompareStringValue(obj.ToString(), precompileValue.ToString());
			}
			if (obj is List<IntervalAnimation>)
			{
				return CompareAnimationIntervalsValue((List<IntervalAnimation>)obj, precompileValue.ToString());
			}
			if (obj is List<string>)
			{
				return CompareStringListValue((List<string>)obj, precompileValue.ToString());
			}
			if (obj is string[])
			{
				return CompareStringArrayValue((string[])obj, precompileValue.ToString());
			}
			if (obj is WallConfig.EWallType)
			{
				return CompareWallTypes(obj.ToString(), precompileValue.ToString());
			}
			double result;
			if (!double.TryParse(obj.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
			{
				result = (obj.ToString().ToUpper().Equals("FALSE") ? 0.0 : Convert.ToDouble(obj));
			}
			double result2;
			if (!double.TryParse(precompileValue.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out result2))
			{
				result2 = (precompileValue.ToString().ToUpper().Equals("FALSE") ? 0.0 : Convert.ToDouble(precompileValue));
			}
			return CompareDigitValue(result, result2);
		}

		private bool CompareWallTypes(string compareWhat, string compareWith)
		{
			compareWhat = compareWhat.ToLower();
			compareWith = compareWith.ToLower();
			return compareWith == compareWhat;
		}

		private bool CompareStringListValue(List<string> compareWhat, string compareWith)
		{
			return compareWhat.Any((string whatValue) => whatValue.Equals(compareWith));
		}

		private bool CompareStringArrayValue(string[] compareWhat, string compareWith)
		{
			return compareWhat.Any((string whatValue) => whatValue.Equals(compareWith));
		}

		private bool CompareAnimationIntervalsValue(List<IntervalAnimation> compareWhat, string compareWith)
		{
			return compareWhat.Any((IntervalAnimation whatValue) => whatValue.name.Equals(compareWith));
		}

		private bool CompareDigitValue(double compareWhat, double compareWith)
		{
			if (compareOperation == CompareOperationType.EQUAL)
			{
				if (compareWhat == compareWith)
				{
					return true;
				}
			}
			else if (compareOperation == CompareOperationType.GREATER)
			{
				if (compareWhat > compareWith)
				{
					return true;
				}
			}
			else if (compareOperation == CompareOperationType.GREATER_EQUAL)
			{
				if (compareWhat >= compareWith)
				{
					return true;
				}
			}
			else if (compareOperation == CompareOperationType.LESS)
			{
				if (compareWhat < compareWith)
				{
					return true;
				}
			}
			else if (compareOperation == CompareOperationType.LESS_EQUAL && compareWhat <= compareWith)
			{
				return true;
			}
			return false;
		}

		private bool CompareStringValue(string compareWhat, string compareWith)
		{
			return compareWhat.Equals(compareWith);
		}

		private bool? CompareKeyDataValue(List<KeyData> compareWhat, List<KeyData> compareWith)
		{
			if (!BattleKeyManager.keyEventsEnabled)
			{
				return false;
			}
			foreach (KeyData condition in compareWith)
			{
				if (!compareWhat.Any((KeyData keyState) => keyState.IncludedInConditionData(condition)))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("Compare Operation: {0}; ", compareOperation));
			stringBuilder.Append(string.Format("Formula_1: {0} ;  Formula_2: {1}", _formula1, _formula2));
			return stringBuilder.ToString();
		}

		public virtual void PrintCondition()
		{
			Debug.Log("######### Condition #########");
			Debug.Log(ToString());
			Debug.Log("############################");
		}

		public static void Init()
		{
			Dictionary<string, RpnParser.ObjectDelegate> objectsDelegates = new Dictionary<string, RpnParser.ObjectDelegate>();
			Dictionary<string, RpnParser.ParameterDelegate> dictionary = new Dictionary<string, RpnParser.ParameterDelegate>();
			dictionary.Add("Player", GetPlayerState);
			dictionary.Add("Fight", GetFight);
			dictionary.Add("Module", GetModule);
			dictionary.Add("Coordinate", GetCoordinate);
			dictionary.Add("WallHit", GetWallHit);
			dictionary.Add("Random", GetRandom);
			dictionary.Add("GetVariable", GetVariable);
			dictionary.Add("Hit", GetHit);
			dictionary.Add("Gender", GetGender);
			dictionary.Add("ItemFloorHit", GetItemFloorHit);
			dictionary.Add("Strike", GetStrike);
			dictionary.Add("Material", GetMaterial);
			dictionary.Add("ArmorMaterial", GetArmorMaterial);
			dictionary.Add("WeaponMaterial", GetWeaponMaterial);
			dictionary.Add("Block", GetBlock);
			dictionary.Add("Trigger", GetCurrentTrigger);
			dictionary.Add("Direction", Direction);
			dictionary.Add("CLC_KeyPressConvert", GetCLC_KeyPress);
			dictionary.Add("KeyPress", GetKeyPress);
			dictionary.Add("CurrentAnimation", GetCurrentAnimation);
			dictionary.Add("CurrentInterval", GetCurrentIntervals);
			dictionary.Add("Health", GetHealth);
			dictionary.Add("InMirrorPose", GetModelMirrored);
			dictionary.Add("CurrentAnimationFrame", GetCurrentAnimationFrame);
			dictionary.Add("Impulse", GetImpulse);
			dictionary.Add("X", GetCoordinateX);
			dictionary.Add("Y", GetCoordinateY);
			dictionary.Add("CurrentStage", GetCurrentStage);
			dictionary.Add("Value", GetVariableValue);
			dictionary.Add("Existence", GetVariableExistence);
			dictionary.Add("Damage", GetDamage);
			dictionary.Add("BaseDamage", GetBaseDamage);
			dictionary.Add("Critical", GetCritical);
			dictionary.Add("Name", GetHitName);
			dictionary.Add("Capsule", GetHitCapsuleName);
			dictionary.Add("Type", GetType);
			dictionary.Add("ItemEquipped", GetItemEquipped);
			dictionary.Add("TagExists", GetTagExists);
			dictionary.Add("NeedsWeaponSwitch", GetNeedsWeaponSwitch);
			dictionary.Add("GetShadowCharge", GetShadowCharge);
			dictionary.Add("ShadowForm", GetShadowFormState);
			dictionary.Add("Location", Location);
			dictionary.Add("BattleType", GetBattleType);
			dictionary.Add("CurrentTab", CurrentTab);
			dictionary.Add("CurrentModule", CurrentModule);
			dictionary.Add("BlockName", GetBlockName);
			dictionary.Add("Event", GetTriggerEvent);
			dictionary.Add("GetPerkVariable", GetPerkVariable);
			dictionary.Add("Round", GetPlayerRound);
			dictionary.Add("RoundCounter", GetRoundCounter);
			dictionary.Add("Rule", GetRule);
			dictionary.Add("HasRule", HasRule);
			dictionary.Add("RuleVariable", RuleVariable);
			dictionary.Add("InMirrorAnimation", InMirrorAnimation);
			dictionary.Add("GetUserVariable", GetUserVariable);
			dictionary.Add("GetUserCurrency", GetUserCurrency);
			dictionary.Add("PrePurchase", PrePurchase);
			dictionary.Add("CurrentTime", GetCurrentTime);
			dictionary.Add("Join", Join);
			dictionary.Add("QuestEvent", QuestEvent);
			dictionary.Add("BattleID", GetCurrentBattleId);
			dictionary.Add("FightID", GetCurrentFightId);
			dictionary.Add("GetTimeStamp", GetCurrentServerTime);
			dictionary.Add("SelectedItemType", SelectedItemType);
			Dictionary<string, RpnParser.ParameterDelegate> parametersDelegates = dictionary;
			RpnParser.init(objectsDelegates, parametersDelegates);
		}

		private static object QuestEvent(List<object> objects)
		{
			string name = objects[0].ToString();
			string field = objects[1].ToString();
			return QuestController.Instance.GetEventArgument(name, field);
		}

		private static object Join(List<object> objects)
		{
			if (objects == null || objects.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object @object in objects)
			{
				stringBuilder.Append(@object.ToString());
			}
			return stringBuilder.ToString();
		}

		private static object GetRule(List<object> objects)
		{
			Model model = GetPlayerState(objects) as Model;
			string type = (string)objects[1];
			List<Rule> rulesByType = model.modelInfo.GetRulesByType(type);
			return (rulesByType == null || rulesByType.Count <= 0) ? null : rulesByType[0];
		}

		private static object HasRule(List<object> objects)
		{
			if (!IsCountArguments(objects, 1))
			{
				return 0;
			}
			Rule rule = (Rule)objects[0];
			return (rule != null) ? 1 : 0;
		}

		private static object RuleVariable(List<object> objects)
		{
			if (!IsCountArguments(objects, 3))
			{
				return 0;
			}
			string text = (string)objects[1];
			string text2 = (string)objects[2];
			Rule rule = GetRule(objects) as Rule;
			if (rule != null)
			{
				double? attributeNumberByType = rule.GetAttributeNumberByType(text2);
				if (attributeNumberByType.HasValue)
				{
					return attributeNumberByType.Value;
				}
			}
			Debug.LogError("RPN RuleVariable Name:" + text + " Variable:" + text2 + " not available");
			return 0;
		}

		private static bool IsCountArguments(List<object> args, int countargs)
		{
			if (args == null || args.Count != countargs)
			{
				Debug.LogError("RPN Argument Functions not available count:" + args.Count + " countargs:" + countargs);
				return false;
			}
			return true;
		}

		private static object PrePurchase(List<object> objects)
		{
			if (ShopTransaction.Current == null)
			{
				return 0;
			}
			switch (objects[0].ToString().ToUpper())
			{
			case "COIN":
				return GetCurrencyValueByType(CurrencyType.Coin);
			case "BONUS":
				return GetCurrencyValueByType(CurrencyType.Bonus);
			case "SHADOW":
				return GetCurrencyValueByType(CurrencyType.Shadow);
			case "ITEMID":
				return ShopTransaction.Current.ItemShop.item.id;
			default:
				return 0;
			}
		}

		private static long GetCurrencyValueByType(CurrencyType type)
		{
			return ShopTransaction.Current.ItemShop.GetCurrencyValue(type);
		}

		private static object GetUserCurrency(List<object> objects)
		{
			switch (objects[0].ToString().ToLower())
			{
			case "coin":
				return GetUserCurrencyValueByType(CurrencyType.Coin);
			case "bonus":
				return GetUserCurrencyValueByType(CurrencyType.Bonus);
			case "shadow":
				return GetUserCurrencyValueByType(CurrencyType.Shadow);
			default:
				return new GameVariables.LocalVariable((string)objects[0], 0);
			}
		}

		private static long GetUserCurrencyValueByType(CurrencyType type)
		{
			return UserManager.GetCurrency(type).Value;
		}

		private static object GetBattleType(List<object> objects)
		{
			return BattlesManager.currentBattleType.ToString();
		}

		private static object GetUserVariable(List<object> objects)
		{
			string globalVariable = UserManager.GetGlobalVariable((string)objects[0]);
			if (string.IsNullOrEmpty(globalVariable))
			{
				return new GameVariables.LocalVariable((string)objects[0], 0);
			}
			int result;
			if (int.TryParse(globalVariable, out result))
			{
				return new GameVariables.LocalVariable((string)objects[0], result);
			}
			float result2;
			if (float.TryParse(globalVariable, out result2))
			{
				return new GameVariables.LocalVariable((string)objects[0], result2);
			}
			return new GameVariables.LocalVariable((string)objects[0], globalVariable);
		}

		private static object GetPlayerRound(List<object> objects)
		{
			return BattleController.Instance.fightController.RoundController.PlayerWinCount + 1;
		}

		private static object GetRoundCounter(List<object> objects)
		{
			return BattleController.Instance.fightController.RoundController.CurrentRoundNumber;
		}

		public static object GetBlockName(List<object> objects)
		{
			object eventData = TriggersVerification.currentVerificationData.currentEventData.EventData;
			IntervalBlockable intervalBlockable = Model.hitResult.StrikeData.intervalAttack.intervalBlockable;
			if (eventData is StrikeData)
			{
				StrikeData strikeData = (StrikeData)eventData;
				if (strikeData.intervalAttack.intervalBlockable != null)
				{
					return strikeData.intervalAttack.intervalBlockable.names;
				}
			}
			else if (intervalBlockable != null)
			{
				return intervalBlockable.names;
			}
			return string.Empty;
		}

		public static object GetBlock(List<object> objects)
		{
			return null;
		}

		public static object GetCurrentTrigger(List<object> objects)
		{
			return TriggersVerification.currentVerificationData.currentTrigger;
		}

		public static object GetTriggerEvent(List<object> objects)
		{
			InfoTrigger infoTrigger = (InfoTrigger)objects[0];
			return TriggerEvent.GetEventNameByType(infoTrigger.calledByEvent);
		}

		public static object CurrentModule(List<object> objects)
		{
			return BaseModuleController.CurrentName;
		}

		public static object GetModule(List<object> objects)
		{
			return null;
		}

		public static object SelectedItemType(List<object> objects)
		{
			int num = -1;
			IntentModule intent = BaseModuleController.CurrentModule.Intent;
			if (intent is ShopIntentModule)
			{
				num = UserShopManager.Instance.shopConfiguration.selectedItem;
			}
			else if (intent is InventoryIntentModule)
			{
				num = InventoryManager.Instance.GetIdOfLastSelectedElementInReel();
			}
			if (num == -1)
			{
				Debug.Log("No item selected!");
				return string.Empty;
			}
			Equipment equipment = JS.Instance.Equipment[num];
			return equipment.GetEquipmentType().ToString();
		}

		public static object CurrentTab(List<object> objects)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IntentModule intent = BaseModuleController.CurrentModule.Intent;
			stringBuilder.Append(intent.TypeModule);
			if (intent is ShopIntentModule)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(((ShopIntentModule)intent).Category);
			}
			else if (intent is InventoryIntentModule)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(((InventoryIntentModule)intent).Category);
			}
			return stringBuilder.ToString();
		}

		public static object Location(List<object> objects)
		{
			return BattlesManager.currentBattle.GetLocation().locationName;
		}

		public static object GetShadowFormState(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.modelShadowForm.shadowFormActive ? 1 : 0;
		}

		public static object GetShadowCharge(List<object> objects)
		{
			Model model = (Model)GetPlayerState(objects);
			if (model != null)
			{
				return model.modelShadowForm.shadowEnergy;
			}
			return null;
		}

		public static object Direction(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.moveControl.GetYDirection();
		}

		public static object GetNeedsWeaponSwitch(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.IsNeedsMirroringModelObjectSwitch();
		}

		public static object GetRandom(List<object> objects)
		{
			return (objects.Count >= 2) ? NekkiMath.randomInt((int)(double)objects[0], (int)(double)objects[1] + 1) : NekkiMath.randomInt(0, 2);
		}

		public static object GetTagExists(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.modelInfo.GetEquippedTags();
		}

		public static object GetWallHit(List<object> objects)
		{
			return null;
		}

		public static object GetType(List<object> objects)
		{
			return TriggersVerification.currentVerificationData.currentEventData.EventData;
		}

		public static object GetHitName(List<object> objects)
		{
			return (Model.hitResult.StrikeData.intervalAttack == null) ? null : Model.hitResult.StrikeData.intervalAttack.hitTags;
		}

		public static object GetHitCapsuleName(List<object> objects)
		{
			return (Model.hitResult.StrikeData.collisionEdge != null) ? Model.hitResult.StrikeData.collisionEdge.name : string.Empty;
		}

		public static object GetItemEquipped(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.modelComponents.equippedItems;
		}

		public static object GetStrike(List<object> objects)
		{
			object eventData = TriggersVerification.currentVerificationData.currentEventData.EventData;
			if (eventData is HitResult)
			{
				return ((HitResult)eventData).StrikeData;
			}
			return (StrikeData)eventData;
		}

		public static object GetMaterial(List<object> objects)
		{
			object obj = objects[0];
			StrikeData strikeData;
			if (obj is StrikeData)
			{
				strikeData = (StrikeData)obj;
			}
			else
			{
				if (!(obj is HitResult))
				{
					Debug.LogError("Could not cast argument to StrikeData orHitResult ");
					return "none";
				}
				strikeData = ((HitResult)obj).StrikeData;
			}
			if (strikeData.intervalAttack == null || strikeData.intervalAttack.material == null)
			{
				return strikeData.attackingEdgeMat;
			}
			return strikeData.intervalAttack.material;
		}

		public static object GetArmorMaterial(List<object> objects)
		{
			object obj = objects[0];
			if (!(obj is HitResult))
			{
				Debug.LogError("Could not cast argument to HitResult");
				return "none";
			}
			return ((HitResult)obj).StrikeData.collisionEdgeMat;
		}

		public static object GetWeaponMaterial(List<object> objects)
		{
			object obj = objects[0];
			if (obj is HitResult)
			{
				return ((HitResult)obj).WeaponMaterialName;
			}
			if (obj is StrikeData)
			{
				StrikeData strikeData = (StrikeData)obj;
				return strikeData.attackingEdgeMat;
			}
			return "none";
		}

		public static object GetItemFloorHit(List<object> objects)
		{
			return TriggersVerification.currentVerificationData.currentEventData.EventData;
		}

		public static object GetHit(List<object> objects)
		{
			return Model.hitResult;
		}

		public static object GetDamage(List<object> objects)
		{
			return Model.hitResult.Damage;
		}

		public static object GetBaseDamage(List<object> objects)
		{
			return Model.hitResult.StrikeData.intervalAttack.damage.baseDamage;
		}

		public static object GetCritical(List<object> objects)
		{
			return Model.hitResult.StrikeData.criticalHit;
		}

		public static object GetImpulse(List<object> objects)
		{
			return (!(Model.hitResult.StrikeData.intervalAttack.impulse == Vector3.zero)) ? 1 : 0;
		}

		public static object GetFight(List<object> objects)
		{
			return null;
		}

		public static object GetCurrentStage(List<object> objects)
		{
			return EnumsCompliancer.GetEnumeratorName<FightController.EFightStage>((int)BattleController.Instance.fightController.FightStage);
		}

		public static object GetVariable(List<object> objects)
		{
			string text = (string)objects[0];
			string nameVal = objects[1].ToString();
			int num = 0;
			TriggersVerification.TriggerStack currentVerificationData = TriggersVerification.currentVerificationData;
			if (text.Equals("Me"))
			{
				num = currentVerificationData.currentModel.id;
			}
			else if (text.Equals("Enemy"))
			{
				num = currentVerificationData.currentModel.enemy.id;
			}
			else if (text.Equals("Global"))
			{
				num = -1;
			}
			else
			{
				if (!text.Equals("Parent"))
				{
					if (text.Equals("Child"))
					{
						Debug.LogWarning("GetVariable from child is not implemented!");
						return null;
					}
					Debug.LogError("GetVariable got invalid target argument: " + text);
					return null;
				}
				num = currentVerificationData.currentModel.parentModel.id;
			}
			return GameVariables.GetVariable(num, nameVal);
		}

		public static object GetVariableValue(List<object> objects)
		{
			if (objects[0] is GameVariables.LocalVariable)
			{
				GameVariables.LocalVariable localVariable = (GameVariables.LocalVariable)objects[0];
				if (localVariable != null)
				{
					return localVariable.value;
				}
			}
			return 0f;
		}

		public static object GetVariableExistence(List<object> objects)
		{
			GameVariables.LocalVariable localVariable = (GameVariables.LocalVariable)objects[0];
			return localVariable != null;
		}

		public static object GetPlayerState(List<object> objects)
		{
			Model model = TriggersVerification.currentVerificationData.currentModel ?? ModelsManager.Instance.CurrentUpdatedModel;
			switch ((string)objects[0])
			{
			case "Me":
				return model;
			case "Parent":
				return model.parentModel;
			case "Enemy":
				return model.enemy;
			case "Child":
				throw new NotImplementedException("GetPlayer child in conditions is not implemented!");
			default:
				return null;
			}
		}

		public static object GetCurrentAnimation(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.modelAnimation.animationNames;
		}

		public static object GetCurrentIntervals(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.animationIntervals;
		}

		public static object GetKeyPress(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			PlayerBattleKeyManager keyManager = modelState.keyManager;
			return (keyManager == null) ? null : keyManager.GetKeys();
		}

		public static object GetCLC_KeyPress(List<object> objects)
		{
			List<KeyData> list = new List<KeyData>();
			foreach (string @object in objects)
			{
				string text2 = @object.Trim();
				int num = text2.IndexOf('(');
				if (num == -1)
				{
					list.Add(new KeyData(text2));
					continue;
				}
				int num2 = text2.IndexOf(')');
				string battleCodeName = text2.Substring(0, num);
				string text3 = text2.Substring(num + 1, num2 - 1 - num);
				list.Add(new KeyData(battleCodeName, text3.Split('|')));
			}
			return list;
		}

		public static object GetHealth(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.health;
		}

		public static object GetModelMirrored(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.moveControl.GetRealMirrored();
		}

		public static object GetCurrentAnimationFrame(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.modelAnimation.animationKey;
		}

		public static object GetCoordinate(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			string distanceObjectName = objects[1] as string;
			string objectName = string.Empty;
			if (objects.Count > 2)
			{
				objectName = objects[2] as string;
			}
			return modelState.GetPositionOf(DistancePoint.GetDistanceObjectByName(distanceObjectName), objectName);
		}

		public static object GetCoordinateX(List<object> objects)
		{
			return ((Vector3)objects[0]).x;
		}

		public static object GetCoordinateY(List<object> objects)
		{
			return ((Vector3)objects[0]).y;
		}

		public static object GetPerkVariable(List<object> objects)
		{
			Model model = GetPlayerState(objects) as Model;
			if (null == model || objects == null)
			{
				return 0;
			}
			int perkId = int.Parse(objects[1].ToString(), CultureInfo.InvariantCulture);
			string attributeName = objects[2].ToString();
			return model.modelInfo.GetEquippedPerkAttributeValue(perkId, attributeName);
		}

		public static object GetGender(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.GetGender().ToString();
		}

		public static object InMirrorAnimation(List<object> objects)
		{
			Model modelState = GetModelState(objects);
			return modelState.moveControl.mirrored ? 1 : 0;
		}

		private static Model GetModelState(List<object> objects)
		{
			return (Model)objects[0];
		}

		private static object GetCurrentTime(List<object> objects)
		{
			return BattleInterface.Instance.TimeCount;
		}

		private static object GetCurrentFightId(List<object> objects)
		{
			return FightController.Instance.CurrentFight.battleID + "." + SF3Utils.GetFightIdtoUseInYaml(FightController.Instance.CurrentFight.fightID);
		}

		private static object GetCurrentBattleId(List<object> objects)
		{
			return FightController.Instance.CurrentFight.battleID.ToString();
		}

		private static object GetCurrentServerTime(List<object> objects)
		{
			return NetworkConnection.current.getCurrentServerDateTime().GetUnixTimeStampMilliseconds() / 1000;
		}

		public static List<IConditionEqual> Parse(Sequence nodeConditions)
		{
			List<IConditionEqual> list = new List<IConditionEqual>();
			for (int i = 0; i < nodeConditions.nodesInside.Count; i++)
			{
				IConditionEqual conditionEqual = Create((Mapping)nodeConditions.nodesInside[i]);
				if (conditionEqual != null)
				{
					list.Add(conditionEqual);
				}
			}
			return list;
		}

		private static IConditionEqual Create(Mapping nodeEntry)
		{
			IConditionEqual mainCondition = null;
			string key = nodeEntry.nodesInside[0].key;
			if (GetLogicOperationByName(key) != 0)
			{
				ParseConditionList(ref mainCondition, nodeEntry, key);
			}
			else
			{
				ParseCondition(ref mainCondition, nodeEntry);
			}
			return mainCondition;
		}

		private static void ParseConditionList(ref IConditionEqual result, Mapping nodeValue, string nodeKey)
		{
			List<IConditionEqual> list = new List<IConditionEqual>();
			for (int i = 0; i < ((Sequence)nodeValue.nodesInside[0]).nodesInside.Count; i++)
			{
				IConditionEqual conditionEqual = Create((Mapping)((Sequence)nodeValue.nodesInside[0]).nodesInside[i]);
				if (conditionEqual != null)
				{
					list.Add(conditionEqual);
				}
			}
			result = new ConditionList(nodeKey, list);
		}

		private static void ParseCondition(ref IConditionEqual mainCondition, Mapping nodeValue)
		{
			int ignoredByAi = -1;
			CompareOperationType compareOperationByName = GetCompareOperationByName(nodeValue.nodesInside[0].key);
			string text = ((Mapping)nodeValue.nodesInside[0]).GetText("Value1").text;
			text = text.Trim();
			string text2 = ((Mapping)nodeValue.nodesInside[0]).GetText("Value2").text;
			text2 = text2.Trim();
			if (text.Contains("CurrentAnimation"))
			{
				text2 = text2.ToLower();
			}
			else if (text2.Contains("CurrentAnimation"))
			{
				text = text.ToLower();
			}
			Scalar text3 = ((Mapping)nodeValue.nodesInside[0]).GetText("IgnoredByAI");
			if (text3 != null)
			{
				ignoredByAi = int.Parse(text3.text);
			}
			mainCondition = new Condition(text, text2, compareOperationByName, ignoredByAi);
		}

		public static CompareOperationType GetCompareOperationByName(string str)
		{
			CompareOperationType result = CompareOperationType.NONE;
			switch (str)
			{
			case "Equal":
				result = CompareOperationType.EQUAL;
				break;
			case "Greater":
				result = CompareOperationType.GREATER;
				break;
			case "GreaterEqual":
				result = CompareOperationType.GREATER_EQUAL;
				break;
			case "Less":
				result = CompareOperationType.LESS;
				break;
			case "LessEqual":
				result = CompareOperationType.LESS_EQUAL;
				break;
			default:
				Debug.LogError(string.Format("No compare operation with name [{0}] found", str));
				break;
			}
			return result;
		}

		public static LogicOperationType GetLogicOperationByName(string str)
		{
			LogicOperationType result = LogicOperationType.NONE;
			switch (str)
			{
			case "Or":
				result = LogicOperationType.OR;
				break;
			case "And":
				result = LogicOperationType.AND;
				break;
			case "Not":
				result = LogicOperationType.NOT;
				break;
			}
			return result;
		}

		public static bool IsCompareOperation(string key)
		{
			return CompareOperationsAll.Any((string operationName) => operationName.Equals(key));
		}

		public static bool IsLogicOperation(string key)
		{
			return LogicOperationsAll.Any((string operationName) => operationName.Equals(key));
		}
	}
}
