using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventIntervalEnd : TriggerEvent
	{
		public TriggerEventIntervalEnd(Mapping eventMap)
			: base(ETriggerEvents.EVENT_INTERVAL_END, eventMap)
		{
		}

		public override bool Equal(BattleEventArgs args)
		{
			if (!base.Equal(args))
			{
				return false;
			}
			IntervalAnimation intervalAnimation = (IntervalAnimation)args.EventData;
			return name.Length == 0 || name.Equals(intervalAnimation.name);
		}
	}
}
