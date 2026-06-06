using System.Collections.Generic;
using SF3;
using SF3.GameModels;
using sf3DTO;

internal class RuleAppearance : Rule
{
	private string _head;

	private Color _hairColor;

	private Color _skinColor;

	public RuleAppearance(string id, string type, Dictionary<string, string> attributes, EPlayerType playerType = EPlayerType.This)
		: base(id, type, attributes, playerType)
	{
	}

	public override void Cache(ModelInfo modelInfo)
	{
		base.Cache(modelInfo);
		_head = modelInfo.head;
		_hairColor = modelInfo.hairColor;
		_skinColor = modelInfo.skinColor;
	}

	public override void Reset()
	{
		base.Reset();
		_modelInfo.SetHead(_head);
		_modelInfo.SetHairColor(_hairColor);
		_modelInfo.SetSkinColor(_skinColor);
	}
}
