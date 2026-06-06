using System;
using System.Collections.Generic;
using Jint.Native;
using sf3DTO;

namespace SF3
{
	public class JSBattlesHolder
	{
		public class JSBattlesGraph
		{
			public class JSBattlesGraphNode
			{
				public int battleID;

				public JSBattlesGraphNodeParent parent;
			}

			public class JSBattlesGraphNodeParent
			{
				public int battleID;

				public int fightsWon;

				public JSBattlesGraphNodeParent(Dictionary<string, JsValue> parentData)
				{
					battleID = int.Parse(parentData["BattleId"].ToString());
					fightsWon = int.Parse(parentData["FightWins"].ToString());
				}
			}

			private Dictionary<int, JSBattlesGraphNode> _graphNodes = new Dictionary<int, JSBattlesGraphNode>();

			public JSBattlesGraph(List<Dictionary<string, JsValue>> battlesDataNodes)
			{
				foreach (Dictionary<string, JsValue> battlesDataNode in battlesDataNodes)
				{
					if (battlesDataNode["Type"].AsInteger() == 1 || battlesDataNode["Type"].AsInteger() == 2)
					{
						AddNode(battlesDataNode);
					}
				}
			}

			private void AddNode(Dictionary<string, JsValue> battleData)
			{
				JSBattlesGraphNode jSBattlesGraphNode = new JSBattlesGraphNode();
				jSBattlesGraphNode.battleID = battleData["ID"].AsInteger();
				if (battleData.ContainsKey("Parent"))
				{
					jSBattlesGraphNode.parent = new JSBattlesGraphNodeParent(battleData["Parent"].AsDictionary());
					_graphNodes.Add(jSBattlesGraphNode.battleID, jSBattlesGraphNode);
				}
			}

			public List<int> GetAvailableBattlesFromGraph(int parentBattleID, int wonFightsInBattle)
			{
				List<int> list = new List<int>();
				foreach (JSBattlesGraphNode value in _graphNodes.Values)
				{
					if (value.parent.battleID == parentBattleID && value.parent.fightsWon <= wonFightsInBattle)
					{
						list.Add(value.battleID);
					}
				}
				return list;
			}

			public List<int> GetAvailableBattlesFromGraph(BattleInfo parentBattle)
			{
				return GetAvailableBattlesFromGraph(parentBattle.GetID(), parentBattle.GetWonFights());
			}

			public List<int> GetAvailableBattlesFromGraph(List<BattleInfo> parentBattles)
			{
				List<int> list = new List<int>();
				foreach (BattleInfo parentBattle in parentBattles)
				{
					list.AddRange(GetAvailableBattlesFromGraph(parentBattle.GetID(), parentBattle.GetWonFights()));
				}
				for (int i = 0; i < list.Count; i++)
				{
					foreach (BattleInfo parentBattle2 in parentBattles)
					{
						if (parentBattle2.GetID() == list[i])
						{
							list.RemoveAt(i);
							i--;
							break;
						}
					}
				}
				return list;
			}
		}

		private List<BattleInfo> _battles;

		private JSBattlesGraph _battlesGraph;

		public JSBattlesGraph battlesGraph
		{
			get
			{
				return _battlesGraph;
			}
		}

		public JSBattlesHolder()
		{
			_battles = new List<BattleInfo>();
			List<Dictionary<string, JsValue>> list = new List<Dictionary<string, JsValue>>();
			Dictionary<string, JsValue> dictionary = JS.Instance.GetScope("Battles").AsDictionary();
			foreach (KeyValuePair<string, JsValue> item in dictionary)
			{
				Dictionary<string, JsValue> dictionary2 = item.Value.AsDictionary();
				sf3DTO.BattleType battleType = (sf3DTO.BattleType)Enum.Parse(typeof(sf3DTO.BattleType), dictionary2["Type"].ToString(), true);
				if (battleType == sf3DTO.BattleType.Daily)
				{
					_battles.Add(new DailyBattleInfo(dictionary2));
				}
				else
				{
					_battles.Add(new BattleInfo(dictionary2));
				}
				list.Add(dictionary2);
			}
			_battlesGraph = new JSBattlesGraph(list);
		}

		public BattleInfo GetBattle(int battleID)
		{
			foreach (BattleInfo battle in _battles)
			{
				if (battle.GetID() == battleID)
				{
					return battle.Clone() as BattleInfo;
				}
			}
			return null;
		}

		public List<BattleInfo> GetBattles(sf3DTO.BattleType type)
		{
			List<BattleInfo> list = new List<BattleInfo>();
			foreach (BattleInfo battle in _battles)
			{
				if (battle.GetBattleType() == type)
				{
					list.Add(battle.Clone() as BattleInfo);
				}
			}
			return list;
		}
	}
}
