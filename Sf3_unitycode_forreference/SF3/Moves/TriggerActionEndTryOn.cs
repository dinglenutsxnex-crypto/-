using Nekki.Yaml;
using SF3.Items;

namespace SF3.Moves
{
	public class TriggerActionEndTryOn : TriggerAction
	{
		public TriggerActionEndTryOn(Node yamlNode)
			: base(EActionType.END_TRY_ON, yamlNode)
		{
		}

		protected override void ApplyAction(object modelData)
		{
			ShopManager.Instance.EndTryOnItem();
		}
	}
}
