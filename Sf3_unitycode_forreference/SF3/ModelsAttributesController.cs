using System.Collections.Generic;
using System.Linq;
using SF3.GameModels;
using SF3_Attributes;

namespace SF3
{
	public class ModelsAttributesController
	{
		private static ModelsAttributesController _instance;

		private Dictionary<int, ModelAttributesModifier> _modelAttributesModifiers;

		public static ModelsAttributesController Instance
		{
			get
			{
				return _instance;
			}
		}

		public ModelsAttributesController()
		{
			_instance = this;
			_modelAttributesModifiers = new Dictionary<int, ModelAttributesModifier>();
		}

		public void Initialize()
		{
			for (int i = 0; i < _modelAttributesModifiers.Count; i++)
			{
				KeyValuePair<int, ModelAttributesModifier> keyValuePair = _modelAttributesModifiers.ElementAt(i);
				if (keyValuePair.Value == null)
				{
					_modelAttributesModifiers.Remove(keyValuePair.Key);
					i--;
				}
				else
				{
					keyValuePair.Value.Clear();
				}
			}
		}

		public void Update(float deltaTime)
		{
			foreach (KeyValuePair<int, ModelAttributesModifier> modelAttributesModifier in _modelAttributesModifiers)
			{
				modelAttributesModifier.Value.Update(deltaTime);
			}
		}

		public ModelAttributesModifier.ModifiedAttribute AddAttributeFactorModifier(int modelID, AttributeType attributeType, float value, float duration = float.MaxValue)
		{
			if (!_modelAttributesModifiers.ContainsKey(modelID))
			{
				_modelAttributesModifiers.Add(modelID, new ModelAttributesModifier());
			}
			return _modelAttributesModifiers[modelID].AddAttributeFactorModifier(attributeType, value, duration);
		}

		public void RemoveModelAttributeModifier(ModelAttributesModifier value)
		{
			for (int i = 0; i < _modelAttributesModifiers.Count; i++)
			{
				KeyValuePair<int, ModelAttributesModifier> keyValuePair = _modelAttributesModifiers.ElementAt(i);
				if (keyValuePair.Value == value)
				{
					_modelAttributesModifiers.Remove(keyValuePair.Key);
					break;
				}
			}
		}

		public ModelAttributesModifier GetModelAttributeModifier(int modelID)
		{
			if (!_modelAttributesModifiers.ContainsKey(modelID))
			{
				_modelAttributesModifiers.Add(modelID, new ModelAttributesModifier());
			}
			return _modelAttributesModifiers[modelID];
		}

		public List<ModelAttributesModifier.ModifiedAttribute> GetModelModifiedAttributes(int modelID)
		{
			List<ModelAttributesModifier.ModifiedAttribute> list = new List<ModelAttributesModifier.ModifiedAttribute>();
			if (_modelAttributesModifiers.ContainsKey(modelID))
			{
				list.AddRange(_modelAttributesModifiers[modelID].strikeModifiers);
				list.AddRange(_modelAttributesModifiers[modelID].hitModifiers);
			}
			return list;
		}
	}
}
