using System.Collections.Generic;
using SF3;
using UnityEngine;

public class MapBattleStack : MonoBehaviour
{
	private MapBattleUI _battleUi;

	private readonly List<LocationStack> _locationStacks = new List<LocationStack>();

	public static MapBattleStack Instance { get; private set; }

	public static void Initialize(MapBattleUI mapBattleUi)
	{
		if (!Instance)
		{
			Instance = new GameObject("go").AddComponent<MapBattleStack>();
			StaticObjectsManager.AddObject(Instance.gameObject);
			Instance._battleUi = mapBattleUi;
		}
		else
		{
			GameObject gameObject = Instance.gameObject;
			Object.Destroy(Instance);
			Instance = gameObject.AddComponent<MapBattleStack>();
			Instance._battleUi = mapBattleUi;
		}
	}

	private LocationStack GetStack(LocationInfo location)
	{
		foreach (LocationStack locationStack2 in _locationStacks)
		{
			if (locationStack2.Location.InSamePlace(location))
			{
				return locationStack2;
			}
		}
		Vector3 battleIconPosition = Instance._battleUi.GetBattleIconPosition(location);
		LocationStack locationStack = new LocationStack(location, battleIconPosition);
		_locationStacks.Add(locationStack);
		return locationStack;
	}

	public void Add(MapBattleButton battle, IBattleInfo info)
	{
		LocationStack stack = GetStack(info.GetLocation());
		stack.Add(battle, info);
	}

	public void Remove(int battleId)
	{
		LocationStack locationStack = null;
		foreach (LocationStack locationStack2 in _locationStacks)
		{
			if (locationStack2.HasBattle(battleId))
			{
				locationStack2.Remove(battleId);
				if (locationStack2.Size == 0)
				{
					locationStack = locationStack2;
				}
			}
		}
		if (locationStack != null)
		{
			_locationStacks.Remove(locationStack);
		}
	}

	private void LateUpdate()
	{
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		foreach (LocationStack locationStack in _locationStacks)
		{
			locationStack.UpdatePosition();
		}
	}

	public void Clear()
	{
		foreach (LocationStack locationStack in _locationStacks)
		{
			locationStack.Clear();
		}
		_locationStacks.Clear();
	}
}
