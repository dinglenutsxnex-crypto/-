using Nekki.Yaml;
using SF3.GameModels;
using SF3_Attributes;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerActionSetAttribute : TriggerAction
	{
		private readonly AttributeType _attribute;

		private readonly RpnValue<float> _attributeValue;

		private readonly RpnValue<float> _duration;

		private readonly bool _isFactor;

		public TriggerActionSetAttribute(Node yamlNode)
			: base(EActionType.SET_ATTRIBUTE, yamlNode)
		{
			string text = BaseMapping.GetText("Name").text;
			_isFactor = false;
			if (text.Contains("Factor_"))
			{
				_isFactor = true;
				text = text.Replace("Factor_", string.Empty);
			}
			_attribute = ComplianceUtils.GetAttributeTypeByName(text);
			Scalar text2 = BaseMapping.GetText("Value");
			Scalar text3 = BaseMapping.GetText("Frames");
			_attributeValue = text2.text;
			_duration = 0f;
			if (text3 != null)
			{
				_duration = text3.text;
			}
			Mathf.Clamp(_duration, 0f, float.MaxValue);
		}

		protected override void ApplyAction(object modelData)
		{
			if (_isFactor)
			{
				ModelsAttributesController.Instance.AddAttributeFactorModifier(((Model)modelData).id, _attribute, _attributeValue, _duration);
			}
		}
	}
}
