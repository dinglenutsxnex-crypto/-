using System;
using System.Collections.Generic;
using SF3;
using UnityEngine;
using sf3DTO;

public class LocationStack
{
	private class LocationStackUnit
	{
		public MapBattleButton Battle { get; private set; }

		public IBattleInfo Info { get; private set; }

		public int Id
		{
			get
			{
				return Info.GetID();
			}
		}

		public LocationStackUnit(MapBattleButton battle, IBattleInfo info)
		{
			Battle = battle;
			Info = info;
		}
	}

	private readonly Dictionary<int, LocationStackUnit> _units;

	public LocationInfo Location { get; private set; }

	public Vector3 LocationPosition { get; private set; }

	public int Size
	{
		get
		{
			return _units.Count;
		}
	}

	public LocationStack(LocationInfo info, Vector3 position)
	{
		_units = new Dictionary<int, LocationStackUnit>();
		Location = info;
		LocationPosition = position;
	}

	public void Add(MapBattleButton battle, IBattleInfo info)
	{
		LocationStackUnit locationStackUnit = new LocationStackUnit(battle, info);
		if (!_units.ContainsKey(locationStackUnit.Id))
		{
			_units.Add(locationStackUnit.Id, locationStackUnit);
		}
		else
		{
			_units[locationStackUnit.Id] = locationStackUnit;
		}
	}

	public bool HasBattle(int battleId)
	{
		return _units.ContainsKey(battleId);
	}

	public void Remove(int battleId)
	{
		if (_units.ContainsKey(battleId))
		{
			_units.Remove(battleId);
			UpdatePosition();
		}
	}

	public void Clear()
	{
		_units.Clear();
	}

	public void UpdatePosition()
	{
		List<MapBattleButton> buttons = GetButtons();
		for (int i = 0; i < buttons.Count; i++)
		{
			Vector3 offset = GetOffset(buttons[i], i + 1, buttons.Count);
			buttons[i].transform.position = buttons[i].TargetAnchor.TransformPoint(offset);
		}
	}

	private Vector3 GetOffset(MapBattleButton button, int index, int count)
	{
		Vector3 locationPosition = LocationPosition;
		if (count == 1)
		{
			return locationPosition;
		}
		Vector3 localScale = button.TargetAnchor.localScale;
		float num = button.GetSize().x / localScale.x;
		float num2 = num / (2f * Mathf.Sin((float)Math.PI / (float)count));
		locationPosition.x += num2 * Mathf.Cos((float)Math.PI * 2f / (float)count * (float)(index - 1));
		locationPosition.y += num2 * Mathf.Sin((float)Math.PI * 2f / (float)count * (float)(index - 1));
		return locationPosition;
	}

	public List<MapBattleButton> GetButtons()
	{
		List<MapBattleButton> list = new List<MapBattleButton>();
		foreach (KeyValuePair<int, LocationStackUnit> unit in _units)
		{
			bool flag = unit.Value.Info.GetIsHidden() && unit.Value.Info.GetBattleType() != sf3DTO.BattleType.Daily;
			if (unit.Value.Battle != null && unit.Value.Battle.gameObject.activeSelf && !flag)
			{
				list.Add(unit.Value.Battle);
			}
		}
		return list;
	}
}
