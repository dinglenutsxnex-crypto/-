using System;
using System.Collections.Generic;
using System.Globalization;
using Nekki.Yaml;
using SF3_Attributes;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class IntervalAttack : IntervalAnimation
	{
		public class Damage
		{
			public float baseDamage { get; private set; }

			public float weaponDamage { get; private set; }

			public AttributeType attackAttribute { get; private set; }

			public AttributeType defenseAttribute { get; private set; }

			public Damage()
			{
				baseDamage = 0f;
				weaponDamage = 0f;
			}

			public void SetBaseDamage(float value)
			{
				baseDamage = value;
			}

			public void SetWeaponDamage(float value)
			{
				weaponDamage = value;
			}

			public void SetAttackAttributes(AttributeType value)
			{
				attackAttribute = value;
			}

			public void SetDefenseAttributes(AttributeType value)
			{
				defenseAttribute = value;
			}
		}

		public Vector3 impulse { get; private set; }

		public string[] attackingParts { get; private set; }

		public string[] hitTags { get; private set; }

		public IntervalBlockable intervalBlockable { get; private set; }

		public IntervalDodgeable intervalDodgeable { get; private set; }

		public IntervalSlowmotion intervalSlowmotion { get; private set; }

		public bool isCollision
		{
			get
			{
				return attackingParts.Length > 0;
			}
		}

		public Damage damage { get; private set; }

		public string material { get; private set; }

		public IntervalAttack()
			: base(EIntervalsType.INTERVAL_ATTACK)
		{
			attackingParts = new string[0];
			hitTags = new string[0];
			damage = new Damage();
			intervalBlockable = null;
			intervalDodgeable = null;
			intervalSlowmotion = null;
			material = null;
		}

		public override List<IntervalAnimation> Parse(Mapping intervalNode)
		{
			base.Parse(intervalNode);
			List<IntervalAnimation> list = new List<IntervalAnimation>();
			Scalar text = intervalNode.GetText("Material");
			if (text != null)
			{
				material = text.text;
			}
			Sequence sequence = intervalNode.GetSequence("AttackingParts");
			if (sequence != null)
			{
				string[] array = new string[sequence.nodesInside.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ((Scalar)sequence.nodesInside[i]).text;
				}
				attackingParts = array;
			}
			Mapping mapping = intervalNode.GetMapping("Impulse");
			if (mapping != null)
			{
				Vector3 zero = Vector3.zero;
				text = mapping.GetText("X");
				if (text != null)
				{
					zero.x = float.Parse(text.text, CultureInfo.InvariantCulture);
				}
				text = mapping.GetText("Y");
				if (text != null)
				{
					zero.y = float.Parse(text.text, CultureInfo.InvariantCulture);
				}
				text = mapping.GetText("Z");
				if (text != null)
				{
					zero.z = float.Parse(text.text, CultureInfo.InvariantCulture);
				}
				SetImpulse(zero);
			}
			sequence = intervalNode.GetSequence("HitTags");
			if (sequence != null)
			{
				string[] array2 = new string[sequence.nodesInside.Count];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = ((Scalar)sequence.nodesInside[j]).text;
				}
				hitTags = array2;
			}
			mapping = intervalNode.GetMapping("Damage");
			if (mapping != null)
			{
				text = mapping.GetText("BaseDamage");
				if (text != null)
				{
					damage.SetBaseDamage(float.Parse(text.text, CultureInfo.InvariantCulture));
				}
				text = mapping.GetText("WeaponDamage");
				if (text != null)
				{
					damage.SetWeaponDamage(float.Parse(text.text, CultureInfo.InvariantCulture));
				}
				text = mapping.GetText("Attack");
				if (text != null)
				{
					damage.SetAttackAttributes(ComplianceUtils.GetAttributeTypeByName(text.text));
				}
				text = mapping.GetText("Defense");
				if (text != null)
				{
					damage.SetDefenseAttributes(ComplianceUtils.GetAttributeTypeByName(text.text));
				}
			}
			else
			{
				Debug.LogWarning("Attacking interval havnt mapping Damage!!!");
			}
			mapping = intervalNode.GetMapping("Dodge");
			if (mapping != null)
			{
				intervalDodgeable = new IntervalDodgeable(this);
				list.AddRange(intervalDodgeable.Parse(mapping));
			}
			mapping = intervalNode.GetMapping("Block");
			if (mapping != null)
			{
				intervalBlockable = new IntervalBlockable(this);
				list.AddRange(intervalBlockable.Parse(mapping));
			}
			mapping = intervalNode.GetMapping("Slowmo");
			if (mapping != null)
			{
				intervalSlowmotion = new IntervalSlowmotion(this);
				list.AddRange(intervalSlowmotion.Parse(mapping));
			}
			list.Add(this);
			return list;
		}

		public void SetImpulse(Vector3 value)
		{
			impulse = value;
		}
	}
}
