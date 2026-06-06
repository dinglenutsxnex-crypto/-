using Nekki.Yaml;
using SF3.UserData;

namespace SF3.Moves
{
	public class TriggerActionSetDojo : TriggerActionQuest
	{
		private readonly int _dojoBattleID;

		private readonly int _dojoFightID;

		public TriggerActionSetDojo(Node yamlNode)
			: base(EActionType.SET_DOJO, yamlNode)
		{
			string outResult;
			if (TryGetString(out outResult, "ID", string.Empty, string.Empty, this))
			{
				string[] array = outResult.Split('.');
				_dojoBattleID = int.Parse(array[0]);
				_dojoFightID = int.Parse(array[1]);
			}
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			UserManager.SetCurrentDojo(_dojoBattleID, _dojoFightID);
			CloseAction();
		}
	}
}
