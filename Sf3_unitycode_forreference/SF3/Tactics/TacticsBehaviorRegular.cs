using System.Collections.Generic;
using SF3.GameModels;
using SF3.Moves;

namespace SF3.Tactics
{
	public class TacticsBehaviorRegular : IReactionAnalyzer
	{
		private static readonly List<InfoTrigger> EmptyList;

		private readonly Model _me;

		private readonly ReactionAnalyzer _reactionSensei;

		private readonly TacticsBehaviorTriggers _reactionTriggers;

		private readonly EnemyAnimationListener _listener;

		private readonly List<InfoTrigger> _triggers;

		static TacticsBehaviorRegular()
		{
			EmptyList = new List<InfoTrigger>();
		}

		public TacticsBehaviorRegular(Model model)
		{
			_me = model;
			_reactionSensei = new ReactionAnalyzer(_me);
			_reactionTriggers = new TacticsBehaviorTriggers(_me);
			_listener = new EnemyAnimationListener(_me);
			_triggers = new List<InfoTrigger>();
		}

		public List<InfoTrigger> GetReaction()
		{
			_triggers.Clear();
			if (_me.modelAnimation.inUninterrupt)
			{
				return _triggers;
			}
			ChanceType reactionType = _listener.GetReactionType();
			TacticsLogger.Instance.Log("Reaction type: " + reactionType, this);
			if (reactionType == ChanceType.Block || reactionType == ChanceType.Initiative)
			{
				return GetReactionByType(reactionType);
			}
			return GetBaseSenseiReaction(reactionType);
		}

		private List<InfoTrigger> GetBaseSenseiReaction(ChanceType reactionType)
		{
			_triggers.AddRange(GetReactionByType(reactionType));
			if (reactionType == ChanceType.Counterattack && _triggers.IsEmpty())
			{
				_triggers.AddRange(GetReactionByType(ChanceType.Dodge));
			}
			if (_triggers.IsEmpty())
			{
				TacticsLogger.Instance.Warning(string.Format("No sensei reaction found for animation: [{0}]", _me.enemy.AnimationCurrent.name), this);
			}
			return _triggers;
		}

		private List<InfoTrigger> GetReactionByType(ChanceType reactionType)
		{
			switch (reactionType)
			{
			case ChanceType.Counterattack:
				return _reactionSensei.GetReactionAttack();
			case ChanceType.Dodge:
				return _reactionSensei.GetReactionDodge();
			case ChanceType.Block:
				return EmptyList;
			case ChanceType.Initiative:
				return _reactionTriggers.GetReaction();
			default:
			{
				string msg = "Undefined tactics reaction found!";
				Messenger.Error(msg, this);
				TacticsLogger.Instance.Error(msg, this);
				return EmptyList;
			}
			}
		}
	}
}
