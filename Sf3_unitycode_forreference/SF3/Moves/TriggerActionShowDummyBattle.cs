using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionShowDummyBattle : TriggerActionQuest
	{
		private readonly int _battleID;

		public TriggerActionShowDummyBattle(Node yamlNode)
			: base(EActionType.SHOW_DUMMY_BATTLE, yamlNode)
		{
			string outResult;
			if (TryGetString(out outResult, "ID", string.Empty, string.Empty, this))
			{
				_battleID = int.Parse(outResult);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			BattlesManager.instance.AddBattleLocal(_battleID);
			CloseAction();
		}
	}
}
