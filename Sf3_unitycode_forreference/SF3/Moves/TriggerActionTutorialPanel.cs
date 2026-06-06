using System.Collections.Generic;
using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionTutorialPanel : TriggerAction
	{
		protected string message = string.Empty;

		protected List<string> replace;

		public TriggerActionTutorialPanel(EActionType type, Node node)
			: base(type, node)
		{
			Setup(node);
		}

		protected void Setup(Node yamlNode)
		{
			TryGetString(out message, "Message", string.Empty, string.Empty);
			replace = new List<string>();
			Sequence sequence = BaseMapping.GetSequence("Replace");
			if (sequence == null)
			{
				return;
			}
			foreach (Node item in sequence.nodesInside)
			{
				replace.Add(item.ToString());
			}
		}

		protected string[] GetRPNArray()
		{
			string[] array = new string[replace.Count];
			for (int i = 0; i < replace.Count; i++)
			{
				RpnParser.Formula formula = new RpnParser.Formula(replace[i]);
				array[i] = formula.calculate().ToString();
			}
			return array;
		}
	}
}
