using System.Collections.Generic;
using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionUIElemets : TriggerAction
	{
		private List<string> ids;

		private bool show;

		public TriggerActionUIElemets(Node yamlNode)
			: base(EActionType.UI_ELEMENTS, yamlNode)
		{
			TryGetBool(out show, "Show", false, string.Empty);
			ids = new List<string>();
			Sequence sequence = BaseMapping.GetSequence("ids");
			if (sequence == null)
			{
				return;
			}
			foreach (Node item in sequence.nodesInside)
			{
				ids.Add(item.ToString());
			}
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			TutorialManager.Instance.SetVisibleComponents(ids.ToArray(), show);
		}
	}
}
