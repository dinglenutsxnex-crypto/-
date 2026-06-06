using Nekki;
using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventStageChange : TriggerEvent
	{
		private readonly FightController.EFightStage _fightStage;

		public TriggerEventStageChange(Mapping eventMap)
			: base(ETriggerEvents.EVENT_STAGE_CHANGE, eventMap)
		{
			string outResult;
			if (eventMap != null && TryGetString(out outResult, "Stage", string.Empty, string.Empty, this))
			{
				_fightStage = EnumsCompliancer.GetEnumerator<FightController.EFightStage>(outResult);
			}
		}

		public override bool Equal(BattleEventArgs args)
		{
			if (!base.Equal(args))
			{
				return false;
			}
			return _fightStage == FightController.EFightStage.None || _fightStage == BattleController.Instance.fightController.FightStage;
		}
	}
}
