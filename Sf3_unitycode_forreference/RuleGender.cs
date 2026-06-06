using System.Collections.Generic;
using SF3;
using SF3.GameModels;
using sf3DTO;

internal class RuleGender : Rule
{
	private Gender _genderValue;

	public RuleGender(string id, string type, Dictionary<string, string> attributes, EPlayerType playerType = EPlayerType.This)
		: base(id, type, attributes, playerType)
	{
	}

	public override void Cache(ModelInfo modelInfo)
	{
		base.Cache(modelInfo);
		_genderValue = modelInfo.gender;
	}

	public override void Reset()
	{
		base.Reset();
		_modelInfo.SetGender(_genderValue);
	}
}
