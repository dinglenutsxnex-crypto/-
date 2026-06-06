using System.Collections.Generic;
using Nekki.Yaml;
using SF3.KeyPressInfo;

namespace SF3.Moves
{
	public class TriggerEventKeyPressed : TriggerEvent
	{
		private const string DEFAULT_CODE = "Any";

		private const string DEFAULT_STATE = "Any";

		private readonly KeyData _keyData;

		public TriggerEventKeyPressed(Mapping eventMap)
			: base(ETriggerEvents.EVENT_KEY_PRESSED, eventMap)
		{
			if (eventMap == null)
			{
				_keyData = new KeyData("Any", "Any");
				return;
			}
			Sequence sequence = eventMap.GetSequence("States");
			Scalar text = eventMap.GetText("Key");
			List<string> list = new List<string>();
			if (sequence == null)
			{
				list.Add("Any");
			}
			else
			{
				foreach (Scalar item in sequence.nodesInside)
				{
					list.Add(item.text);
				}
			}
			_keyData = new KeyData((text != null) ? text.text : "Any", list);
		}

		public override bool Equal(BattleEventArgs args)
		{
			return base.Equal(args) && ((KeyData)args.EventData).IncludedInConditionData(_keyData);
		}
	}
}
