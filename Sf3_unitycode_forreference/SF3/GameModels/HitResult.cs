using System.Runtime.InteropServices;
using SF3.Items;
using SF3.Moves;
using SF3_Attributes;
using UnityEngine;

namespace SF3.GameModels
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct HitResult
	{
		public const string DefaultCollisionMaterialName = "none";

		public string WeaponMaterialName { get; private set; }

		public Vector3 Impulse { get; private set; }

		public StrikeData StrikeData { get; private set; }

		public Vector3 CapsuleAttackDirection { get; private set; }

		public bool CanDealDamage { get; set; }

		public float Damage { get; private set; }

		public float DamageTaken
		{
			get
			{
				return (!CanDealDamage) ? 0f : Damage;
			}
		}

		public void SetStrikeData(StrikeData strikeDataVal)
		{
			CanDealDamage = true;
			StrikeData = strikeDataVal;
			WeaponMaterialName = "none";
		}

		public float CalculateDamageCanBeDoneToEnemy(IntervalAttack attackInterval, Model me)
		{
			RecalculateAttributes(me, me.enemy);
			float baseDamage = attackInterval.damage.baseDamage;
			float finallyAttribute = GetFinallyAttribute(me.modelInfo, attackInterval.damage.attackAttribute);
			float finallyAttribute2 = GetFinallyAttribute(me.enemy.modelInfo, (attackInterval.damage.defenseAttribute == AttributeType.None) ? AttributeType.BodyDefense : attackInterval.damage.defenseAttribute);
			return GetDamageCalculated(baseDamage, finallyAttribute, finallyAttribute2, me);
		}

		public void CalculateHitOnMe(Model me)
		{
			ModelInfo modelInfo = me.modelInfo;
			ModelInfo modelInfo2 = StrikeData.attackingModel.modelInfo;
			Impulse = StrikeData.intervalAttack.impulse;
			Impulse = new Vector3(Impulse.x * (float)StrikeData.direction, Impulse.y, Impulse.z);
			float num = ((StrikeData.intervalAttack.damage.defenseAttribute == AttributeType.None) ? GetFinallyAttribute(modelInfo, (StrikeData.collisionEdge != null) ? StrikeData.collisionEdge.defense : AttributeType.BodyDefense) : GetFinallyAttribute(modelInfo, StrikeData.intervalAttack.damage.defenseAttribute));
			float baseDamage = StrikeData.intervalAttack.damage.baseDamage;
			float finallyAttribute = GetFinallyAttribute(modelInfo2, StrikeData.intervalAttack.damage.attackAttribute);
			Damage = GetDamageCalculated(baseDamage, finallyAttribute, num, StrikeData.attackingModel);
			if (StrikeData.attackEdge != null)
			{
				CapsuleAttackDirection = StrikeData.GetCapsuleMovingDirection();
			}
			if (me.GetWeapon().modelMaterials.modelMaterials.Length > 0)
			{
				WeaponMaterialName = me.GetWeapon().modelMaterials.modelMaterials[0].materialName;
			}
			else
			{
				WeaponMaterialName = "none";
			}
			GameVariables.LocalVariable variable = GameVariables.GetVariable(me.id, "BLOCK");
			GameVariables.LocalVariable variable2 = GameVariables.GetVariable(me.id, "CRITICAL");
			bool block = variable != null && (double)variable.value > 0.0;
			bool crit = variable2 != null && (double)variable2.value > 0.0;
			BattleLog.Hit(me.enemy.modelInfo.alias, me.modelInfo.alias, baseDamage, finallyAttribute, StrikeData.intervalAttack.damage.attackAttribute.ToString(), num, (StrikeData.collisionEdge != null) ? StrikeData.collisionEdge.defense.ToString() : "BodyDefense", Damage, block, crit, me.animationInfo.animation.name, me.enemy.animationInfo.animation.name);
		}

		public float GetFinallyAttribute(ModelInfo info, AttributeType type)
		{
			return info.attributes.GetFinallyAttribute(type);
		}

		private float GetDamageCalculated(float baseDamage, float attributeAttack, float attributeDefense, Model attackingModel)
		{
			IPerk equippedPerkByAnimationGroup = attackingModel.modelInfo.GetEquippedPerkByAnimationGroup(attackingModel.AnimationCurrent.groupNames);
			if (equippedPerkByAnimationGroup != null)
			{
				baseDamage *= JsFunction.GetBaseDamageModifier(equippedPerkByAnimationGroup.GetId(), equippedPerkByAnimationGroup.GetStackLevel());
			}
			return JsFunction.CalculateStrikeDamage(baseDamage, attributeAttack, attributeDefense);
		}

		private void RecalculateAttributes(params Model[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i].modelInfo.attributes.CalculateFinallyAttributes();
				data[i].modelInfo.attributes.ApplyStrikeModifiers();
				data[i].modelInfo.attributes.ApplyHitModifiers();
			}
		}
	}
}
