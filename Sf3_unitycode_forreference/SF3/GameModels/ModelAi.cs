using System;
using System.Collections.Generic;
using SF3.Moves;
using SF3.Settings;
using SF3.Tactics;
using sf3DTO;

namespace SF3.GameModels
{
	public class ModelAi
	{
		private readonly Model _me;

		private readonly BattleEventArgs _args;

		private int _delayCounter = -1;

		private TacticsMode _tacticsMode;

		private IReactionAnalyzer _reactionAnalyze;

		public ModelAi(Model model)
		{
			_me = model;
			_args = new BattleEventArgs(ETriggerEvents.EVENT_TACTICS_REACTIONS, _me.id);
			SetMode(model.GetAiMode());
		}

		private void SetMode(AiMode mode)
		{
			_tacticsMode = TacticsSettings.GetTacticsMode(mode);
		}

		public void Initialize()
		{
			SetAnalizer();
		}

		private void SetAnalizer()
		{
			_reactionAnalyze = GetAnalyzerByMode(_tacticsMode.GetAiMode());
		}

		public void ChangeAiModeTo(AiMode mode)
		{
			SetMode(mode);
			SetAnalizer();
		}

		private IReactionAnalyzer GetAnalyzerByMode(AiMode mode)
		{
			switch (mode)
			{
			case AiMode.RegularMode:
			case AiMode.NoneMode:
			case AiMode.SenseiMode:
			case AiMode.DojoMode:
				return new TacticsBehaviorRegular(_me);
			default:
				Messenger.Error(string.Format("Current mode: [{0}] is not supported.", mode.ToString()), this);
				throw new ArgumentOutOfRangeException("mode", mode.ToString(), null);
			}
		}

		public void ResetDelay()
		{
			_delayCounter = _tacticsMode.GetDelay();
		}

		public void UpdateAi()
		{
			if (_delayCounter == 0)
			{
				MakeDecision();
			}
			else if (_delayCounter > 0)
			{
				_delayCounter--;
			}
		}

		private void MakeDecision()
		{
			List<InfoTrigger> triggers = GetTriggers();
			if (triggers != null && triggers.Count > 0)
			{
				StartActions(triggers);
			}
			_delayCounter = -1;
		}

		private List<InfoTrigger> GetTriggers()
		{
			return TacticsSettings.IsGroupNoReactions(_me.enemy.AnimationCurrent.groupNames, _tacticsMode.GetAiMode()) ? null : _reactionAnalyze.GetReaction();
		}

		private void StartActions(List<InfoTrigger> triggers)
		{
			TriggersVerification.SelectConditions(_me, _args, triggers);
			BattleController.ThrowEvent(_args);
		}
	}
}
