using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventAnimationStart : TriggerEvent
	{
		public TriggerEventAnimationStart(Mapping eventMap)
			: base(ETriggerEvents.EVENT_ANIMATION_START, eventMap)
		{
			name = name.ToLower();
		}

		public override bool Equal(BattleEventArgs args)
		{
			if (!base.Equal(args))
			{
				return false;
			}
			ModelInfoAnimation modelInfoAnimation = (ModelInfoAnimation)args.EventData;
			return name.Length == 0 || modelInfoAnimation.animation.HasNameOrGroup(name);
		}
	}
}
