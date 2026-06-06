using System.Collections.Generic;
using Jint.Native;
using Nekki;
using Nekki.Yaml;
using SF3;
using SF3.GameModels;
using SF3.Items;

public class RandomEquipmentRule : FightRule
{
	private Equipment _itemGivenEnemy;

	private Equipment _itemGivenMe;

	public EPlayerType ApplyTo { get; private set; }

	public List<Equipment> Equipment { get; private set; }

	public RandomEquipmentRule(Mapping rule)
		: base(rule)
	{
		base.startOnStage = FightController.EFightStage.RoundStart;
		base.stopOnStage = FightController.EFightStage.RoundEnd;
		ApplyTo = Model.GetPlayerTypeByName(YamlUtils.GetText(rule, "ApplyTo", "Me"));
		Equipment = new List<Equipment>();
		Sequence sequence = rule.GetSequence("Equipments");
		foreach (Node item in sequence.nodesInside)
		{
			Mapping mapping = item as Mapping;
			if (mapping != null)
			{
				Equipment.Add(SF3.Items.Equipment.Create(mapping));
			}
		}
		_itemGivenMe = null;
		_itemGivenEnemy = null;
	}

	public RandomEquipmentRule(Dictionary<string, JsValue> rule)
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
		_itemGivenMe = GetRandomEquipment();
		switch (ApplyTo)
		{
		case EPlayerType.This:
			ModelsManager.Instance.Player.EquipItemNotExisted(_itemGivenMe);
			break;
		case EPlayerType.Enemy:
			ModelsManager.Instance.Enemy.EquipItemNotExisted(_itemGivenMe);
			break;
		case EPlayerType.Both:
			ModelsManager.Instance.Player.EquipItemNotExisted(_itemGivenMe);
			_itemGivenEnemy = GetRandomEquipment();
			ModelsManager.Instance.Enemy.EquipItemNotExisted(_itemGivenEnemy);
			break;
		}
	}

	private void TakeAwayItem()
	{
		if (ApplyTo == EPlayerType.This)
		{
			ModelsManager.Instance.Player.UnEquipItemNotExisted(_itemGivenMe);
		}
		else if (ApplyTo == EPlayerType.Enemy)
		{
			ModelsManager.Instance.Enemy.UnEquipItemNotExisted(_itemGivenMe);
		}
		else if (ApplyTo == EPlayerType.Both)
		{
			ModelsManager.Instance.Player.UnEquipItemNotExisted(_itemGivenMe);
			ModelsManager.Instance.Enemy.UnEquipItemNotExisted(_itemGivenEnemy);
		}
	}

	private Equipment GetRandomEquipment()
	{
		return Equipment[NekkiMath.randomInt(0, Equipment.Count)];
	}
}
