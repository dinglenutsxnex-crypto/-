using System.Collections.Generic;
using System.Linq;
using Nekki;
using SF3.GameModels;
using SF3.Moves;

namespace SF3.Tactics
{
	public class TacticsBehaviorTriggers : IReactionAnalyzer
	{
		private readonly Model _me;

		private readonly BattleEventArgs _args;

		private readonly List<InfoTrigger> _triggers;

		public TacticsBehaviorTriggers(Model model)
		{
			_me = model;
			_args = new BattleEventArgs(ETriggerEvents.EVENT_TACTICS_REACTIONS, _me.id);
			_triggers = new List<InfoTrigger>();
		}

		public List<InfoTrigger> GetReaction()
		{
			Clear();
			if (_me.modelAnimation.inUninterrupt)
			{
				return _triggers;
			}
			Add(GetTriggersPassedConditions());
			if (_triggers.Count > 1)
			{
				InfoTrigger randomTrigger = GetRandomTrigger(_triggers);
				if (randomTrigger != null)
				{
					Clear();
					Add(randomTrigger);
				}
				else
				{
					Messenger.Error("Random trigger selected is null.");
				}
			}
			return _triggers;
		}

		private InfoTrigger GetRandomTrigger(List<InfoTrigger> triggers)
		{
			SortByTacticsMathWeight(triggers);
			int max = triggers.Sum((InfoTrigger t) => t.tactics.mathWeight);
			int num = NekkiMath.randomInt(0, max);
			int num2 = 0;
			foreach (InfoTrigger trigger in triggers)
			{
				num2 += trigger.tactics.mathWeight;
				if (num2 > num && trigger.actions.Count > 0)
				{
					return trigger;
				}
			}
			return null;
		}

		private void Add(InfoTrigger trigger)
		{
			_triggers.Add(trigger);
		}

		private void Add(IEnumerable<InfoTrigger> triggers)
		{
			_triggers.AddRange(triggers);
		}

		private void Clear()
		{
			_triggers.Clear();
		}

		private void SortByTacticsMathWeight(List<InfoTrigger> triggers)
		{
			triggers.Sort((InfoTrigger a, InfoTrigger b) => (a.tactics.mathWeight != b.tactics.mathWeight) ? ((a.tactics.mathWeight >= b.tactics.mathWeight) ? 1 : (-1)) : 0);
		}

		private IEnumerable<InfoTrigger> GetTriggersPassedConditions()
		{
			return from trigger in GetTriggersWithTacticsSetting()
				where TriggersVerification.CheckConditionsEqual(_me, _args, trigger.tactics.conditions)
				select trigger;
		}

		private IEnumerable<InfoTrigger> GetTriggersWithTacticsSetting()
		{
			return _me.modelMoves.currentTriggers.Where((InfoTrigger trigger) => trigger.tactics != null);
		}
	}
}
