using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventFightEnter : TriggerEvent
	{
		private readonly int _battleId;

		private readonly int _fightId;

		private IntentTransitionData.TransitionData transitionData;

		public TriggerEventFightEnter(Mapping eventMap)
			: base(ETriggerEvents.QEVENT_FIGHT_ENTER, eventMap)
		{
			_battleId = -1;
			_fightId = -1;
			string result;
			YamlUtils.TryGetString(out result, eventMap, "ID", string.Empty);
			int[] outResult;
			if (!string.IsNullOrEmpty(result) && TryGetBattleIdentifier(out outResult, result, string.Empty, this))
			{
				_battleId = outResult[0];
				_fightId = outResult[1];
			}
		}

		protected override void SetArguments(object[] args)
		{
			base.SetArguments(args);
			transitionData = ((IntentTransitionData)arguments[0]).ToData;
		}

		protected override bool Equal()
		{
			return _battleId == -1 || (_battleId == transitionData.BattleID && _fightId == transitionData.FightID);
		}

		public override object GetArgument(string field)
		{
			switch (field)
			{
			case "BattleID":
				return transitionData.BattleID;
			case "FightID":
				return transitionData.FightID;
			case "ID":
				return transitionData.BattleID + "." + (transitionData.FightID + 1);
			default:
				return base.GetArgument(field);
			}
		}
	}
}
