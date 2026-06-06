using System;
using System.Collections.Generic;
using Nekki.Yaml;
using sf3DTO;

namespace SF3.Items
{
	public interface ISlotItem : ICloneable
	{
		string GetImage();

		int GetId();

		string GetAlias();

		EquipmentType GetTargetItemType();

		Faction GetTargetFactionType();

		List<string> GetTags();

		List<Node> ToYaml();
	}
}
