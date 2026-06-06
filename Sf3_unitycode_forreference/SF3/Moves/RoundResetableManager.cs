using System.Collections.Generic;

namespace SF3.Moves
{
	public class RoundResetableManager
	{
		private static RoundResetableManager _instance;

		private readonly List<TriggerActionRoundResetable> _activeActions;

		public static RoundResetableManager Instance
		{
			get
			{
				return _instance ?? (_instance = new RoundResetableManager());
			}
		}

		private RoundResetableManager()
		{
			_activeActions = new List<TriggerActionRoundResetable>();
		}

		public void AddRule(TriggerActionRoundResetable rule)
		{
			_activeActions.Add(rule);
		}

		public void ResetRules()
		{
			foreach (TriggerActionRoundResetable activeAction in _activeActions)
			{
				activeAction.Reset();
			}
			_activeActions.Clear();
		}
	}
}
