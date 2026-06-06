using Nekki.Yaml;
using SF3.GameModels;
using SF3.Items;

namespace SF3.Moves
{
	public class TriggerActionDisarm : TriggerAction
	{
		private readonly bool _instantFreeze;

		private readonly float? _force;

		public TriggerActionDisarm(Node yamlNode)
			: base(EActionType.DISARM, yamlNode)
		{
			float outResult;
			if (TryGetFloat(out outResult, "Force", 0f, string.Empty, null, false))
			{
				_force = outResult;
			}
			else
			{
				_force = null;
			}
			TryGetBool(out _instantFreeze, "InstantFreeze", false, string.Empty, null, false);
			if (_instantFreeze && _force.HasValue)
			{
				Messenger.Error("Disarm could not have force value if instant freeze applied!", this);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			Model model = (Model)modelData;
			if (model.modelComponents.DropItems(model.modelShadowForm.shadowFormActive, _force, _instantFreeze))
			{
				model.modelInfo.Drop(model.modelComponents.droppedItemsNew);
				model.modelMoves.SwitchToDisarm(model.isPlayer);
				BattleInterface.Instance.ClearShadowPerkSlot(model.isPlayer ? 1 : 2, EquipmentType.Weapon);
				model.modelAnimation.UpdateBonesData();
			}
		}
	}
}
