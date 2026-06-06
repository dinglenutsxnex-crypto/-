using Nekki.Yaml;
using SF3.GameModels;
using SF3.Items;

namespace SF3.Moves
{
	public class TriggerActionCreateModel : TriggerAction
	{
		private readonly ModelInfo _modelInfo;

		private readonly EquipmentType _overrideFromParent;

		public TriggerActionCreateModel(Node yamlNode)
			: base(EActionType.CREATE_MODEL, yamlNode)
		{
			Mapping outResult;
			if (TryGetMapping(out outResult, "ModelInfo", string.Empty, this))
			{
				_modelInfo = new ModelInfo(outResult);
			}
			_overrideFromParent = EquipmentType.None;
			string outResult2;
			if (TryGetString(out outResult2, "OverrideFromParent", string.Empty, string.Empty, null, false))
			{
				SF3Utils.TryParseEnum(out _overrideFromParent, outResult2, EquipmentType.None);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			Model model = (Model)modelData;
			ModelInfo modelInfo = (ModelInfo)_modelInfo.Clone();
			modelInfo.PreInit();
			if (_overrideFromParent != 0)
			{
				Equipment equippedForType = model.GetEquippedForType(_overrideFromParent);
				if (equippedForType != null)
				{
					Equipment equipment = (Equipment)equippedForType.Clone();
					equipment.ClearTags();
					equipment.ClearAttributes();
					modelInfo.AddAndEquipSingleInstance(equipment, false);
				}
			}
			Model model2 = ModelsManager.Instance.CreateModel(modelInfo, model.id);
			model2.Initialize(model);
			model.childModels.Add(model2);
			if (model.modelShadowForm.shadowFormActive)
			{
				model2.ActivateShadowForm(true);
			}
		}
	}
}
