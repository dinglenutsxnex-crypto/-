using System;
using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionSetCharacterShader : TriggerAction
	{
		private string materialName;

		private bool toggle;

		private float transitionTime = -1f;

		public TriggerActionSetCharacterShader(Node yamlNode)
			: base(EActionType.SET_SHADER_EFFECT, yamlNode)
		{
			Mapping mapping = (Mapping)((Mapping)yamlNode).nodesInside[0];
			Scalar text = mapping.GetText("Name");
			if (text != null)
			{
				materialName = text.text;
			}
			text = mapping.GetText("Toggle");
			if (text != null)
			{
				toggle = text.text.Equals("On");
			}
			text = mapping.GetText("Transition");
			if (text != null)
			{
				transitionTime = Convert.ToSingle(text.text);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			Model model = ((base.targetType != EPlayerType.This) ? ModelsManager.Instance.Enemy : ModelsManager.Instance.Player);
			model.OverrideMaterial(materialName, toggle, transitionTime);
		}
	}
}
