using SF3.GameModels;
using SF3.Moves;
using SF3.Settings;

namespace SF3.Tactics
{
	public class EnemyAnimationListener
	{
		private readonly Model _me;

		private readonly ChancesCalculator _calculator;

		public EnemyAnimationListener(Model model)
		{
			_me = model;
			_calculator = new ChancesCalculator(_me);
			BattleController.RegisterEventCallback(ETriggerEvents.EVENT_ANIMATION_START, AnimationStarted);
			BattleController.RegisterEventCallback(ETriggerEvents.EVENT_POST_HIT, AnimationHitMe);
		}

		~EnemyAnimationListener()
		{
			BattleController.RemoveEventCallback(ETriggerEvents.EVENT_ANIMATION_START, AnimationStarted);
			BattleController.RemoveEventCallback(ETriggerEvents.EVENT_POST_HIT, AnimationHitMe);
		}

		public ChanceType GetReactionType()
		{
			return _calculator.GetReactionType(GetAnimationNameEnemy());
		}

		private void AnimationStarted(BattleEventArgs args)
		{
			if (!IsMe(args.SenderID))
			{
				IncrementCounter(FactorType.AnimationCount);
			}
		}

		private void AnimationHitMe(BattleEventArgs args)
		{
			if (!IsMe(args.SenderID))
			{
				IncrementCounter(FactorType.AnimationHit);
				IncrementCounter(FactorType.AnimationDamage, ((HitResult)args.EventData).Damage);
			}
		}

		private void IncrementCounter(FactorType type, double value = 1.0)
		{
			if (!IsIgnoredAnimationEnemy())
			{
				_calculator.IncrementCounter(GetAnimationNameEnemy(), type, value);
			}
		}

		private bool IsIgnoredAnimationEnemy()
		{
			return TacticsSettings.IsGroupNoReactions(GetAnimationEnemy().groupNames, _me.GetAiMode());
		}

		private string GetAnimationNameEnemy()
		{
			return GetAnimationEnemy().name;
		}

		private InfoAnimation GetAnimationEnemy()
		{
			return _me.enemy.AnimationCurrent;
		}

		private bool IsMe(int id)
		{
			return _me.id == id;
		}
	}
}
