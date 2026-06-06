using System.Collections.Generic;
using Jint.Native;
using Nekki.Yaml;
using SF3;
using SF3.GameModels;
using SF3.Items;

public class GiveItemRule : FightRule
{
	public EPlayerType ApplyTo { get; private set; }

	public Equipment Equipment { get; private set; }

	public IPerk Perk { get; private set; }

	public GiveItemRule(Mapping rule)
		: base(rule)
	{
		base.startOnStage = FightController.EFightStage.RoundStart;
		base.stopOnStage = FightController.EFightStage.RoundEnd;
		ApplyTo = Model.GetPlayerTypeByName(YamlUtils.GetText(rule, "ApplyTo", "Me"));
		Mapping mapping = rule.GetMapping("Equipment");
		if (mapping != null)
		{
			Equipment = Equipment.Create(mapping);
		}
		Mapping mapping2 = rule.GetMapping("Perk");
		if (mapping2 != null)
		{
			Perk = SF3.Items.Perk.Create(mapping2);
		}
	}

	public GiveItemRule(Dictionary<string, JsValue> rule)
		: base(rule)
	{
	}

	public override void StartRule()
	{
		base.StartRule();
		GiveItem();
	}

	public override void StopRule()
	{
		base.StopRule();
		TakeAwayItem();
	}

	private void GiveItem()
	{
		switch (ApplyTo)
		{
		case EPlayerType.This:
			GiveItemTo(ModelsManager.Instance.Player);
			break;
		case EPlayerType.Enemy:
			GiveItemTo(ModelsManager.Instance.Enemy);
			break;
		}
	}

	private void TakeAwayItem()
	{
		switch (ApplyTo)
		{
		case EPlayerType.This:
			TakeItemFrom(ModelsManager.Instance.Player);
			break;
		case EPlayerType.Enemy:
			TakeItemFrom(ModelsManager.Instance.Enemy);
			break;
		}
	}

	private void TakeItemFrom(Model model)
	{
		GiveItemTo(model, false);
	}

	private void GiveItemTo(Model model, bool toEquip = true)
	{
		if (toEquip)
		{
			if (Equipment != null)
			{
				model.EquipItemNotExisted(Equipment, false);
			}
			if (Perk != null)
			{
				model.EquipItemNotExisted(Perk, false);
			}
		}
		else
		{
			if (Equipment != null)
			{
				model.UnEquipItemNotExisted(Equipment, false);
			}
			if (Perk != null)
			{
				model.UnEquipItemNotExisted(Perk, false);
			}
		}
	}
}
