namespace SF3.GameModels
{
	public interface IEventSender
	{
		void ThrowEvent(BattleEventArgs args);
	}
}
