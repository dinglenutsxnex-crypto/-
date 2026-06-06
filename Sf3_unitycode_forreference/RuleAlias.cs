using System.Collections.Generic;
using SF3;
using SF3.GameModels;

internal class RuleAlias : Rule
{
	private string _alias;

	public RuleAlias(string id, string type, Dictionary<string, string> attributes, EPlayerType playerType = EPlayerType.This)
		: base(id, type, attributes, playerType)
	{
	}

	public override void Cache(ModelInfo modelInfo)
	{
		base.Cache(modelInfo);
		_alias = modelInfo.alias;
	}

	public override void Reset()
	{
		base.Reset();
		_modelInfo.SetAlias(_alias);
	}
}
