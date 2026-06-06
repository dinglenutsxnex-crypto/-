using System.Collections.Generic;
using SF3_Attributes;

namespace SF3.GameModels
{
	public class ModelAttributesModifier
	{
		public class ModifiedAttribute
		{
			private bool _forcedRemove;

			private float _framesLeft;

			public AttributeType attribute { get; private set; }

			public float value { get; private set; }

			public bool isFactor { get; private set; }

			public ModifiedAttribute(AttributeType name, float value, bool isFactor, float framesLeft)
			{
				attribute = name;
				this.value = value;
				this.isFactor = isFactor;
				_framesLeft = framesLeft;
				_forcedRemove = false;
			}

			public bool Update(float deltaFrame)
			{
				_framesLeft -= deltaFrame;
				if (_framesLeft <= 0f || _forcedRemove)
				{
					return true;
				}
				return false;
			}

			public void ForcedRemove()
			{
				_forcedRemove = true;
			}

			public override string ToString()
			{
				return string.Concat("ModifiedAttribute [type:", attribute, " value:", value, "]");
			}
		}

		private List<ModifiedAttribute> _strikeModifiers;

		private List<ModifiedAttribute> _hitModifiers;

		public List<ModifiedAttribute> strikeModifiers
		{
			get
			{
				return _strikeModifiers;
			}
		}

		public List<ModifiedAttribute> hitModifiers
		{
			get
			{
				return _hitModifiers;
			}
		}

		public ModelAttributesModifier()
		{
			_strikeModifiers = new List<ModifiedAttribute>();
			_hitModifiers = new List<ModifiedAttribute>();
		}

		public ModifiedAttribute AddAttributeFactorModifier(AttributeType attributeType, float value, float duration)
		{
			ModifiedAttribute modifiedAttribute = new ModifiedAttribute(attributeType, value, true, duration);
			if (attributeType == AttributeType.CriticalChance)
			{
				_strikeModifiers.Add(modifiedAttribute);
			}
			else
			{
				_hitModifiers.Add(modifiedAttribute);
			}
			return modifiedAttribute;
		}

		public void Update(float deltaFrame)
		{
			for (int i = 0; i < _strikeModifiers.Count; i++)
			{
				if (_strikeModifiers[i].Update(deltaFrame))
				{
					_strikeModifiers.Remove(_strikeModifiers[i]);
					i--;
				}
			}
			for (int j = 0; j < _hitModifiers.Count; j++)
			{
				if (_hitModifiers[j].Update(deltaFrame))
				{
					_hitModifiers.Remove(_hitModifiers[j]);
					j--;
				}
			}
		}

		public void Clear()
		{
			_strikeModifiers.Clear();
			_hitModifiers.Clear();
		}
	}
}
