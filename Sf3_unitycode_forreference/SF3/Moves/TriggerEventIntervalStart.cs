using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventIntervalStart : TriggerEvent
	{
		public TriggerEventIntervalStart(Mapping eventMap)
			: base(ETriggerEvents.EVENT_INTERVAL_START, eventMap)
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
