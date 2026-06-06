using Nekki.Yaml;
using SimpleJSON;

public abstract class ItemIntentModule : IntentModule
{
	public int ItemId { get; private set; }

	public override void Init(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		base.Init(type, args);
		ItemId = -1;
		if (HasProperties())
		{
			SetItemId(GetItemID(args[0]));
		}
	}

	protected virtual int GetItemID(object itemObject)
	{
		IntentParametrs intentParametrs = itemObject as IntentParametrs;
		if (intentParametrs != null)
		{
			return intentParametrs.ItemID;
		}
		return -1;
	}

	public void SetItemId(int id)
	{
		ItemId = id;
		Update();
	}

	public override Mapping ToYaml()
	{
		Mapping mapping = base.ToYaml();
		if (ItemId != -1)
		{
			mapping.Add(new Scalar("ItemID", ItemId.ToString()));
		}
		return mapping;
	}

	public override JSONClass ToJSON()
	{
		JSONClass jSONClass = base.ToJSON();
		if (ItemId != -1)
		{
			jSONClass.Add("ItemID", new JSONData(ItemId));
		}
		return jSONClass;
	}
}
