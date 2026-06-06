using Nekki.Yaml;
using SF3.Items;

namespace SF3.Moves
{
	public class TriggerEventEquip : TriggerEvent
	{
		private readonly int _itemId;

		public TriggerEventEquip(Mapping eventMap)
			: base(ETriggerEvents.EVENT_EQUIP, eventMap)
		{
			if (eventMap != null)
			{
				YamlUtils.TryGetInt(out _itemId, eventMap, "ItemID", -1);
			}
		}

		protected override bool Equal()
		{
			BaseItem baseItem = (BaseItem)arguments[0];
			return _itemId == -1 || baseItem.id == _itemId;
		}
	}
}
