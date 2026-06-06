using System;
using Nekki.Yaml;
using SF3.UserData;

namespace SF3.Moves
{
	public class TriggerActionSetUserVariable : TriggerActionQuest
	{
		private readonly string _valueVariable;

		public TriggerActionSetUserVariable(Node yamlNode)
			: base(EActionType.SET_USER_VARIABLE, yamlNode)
		{
			if (yamlNode is Scalar)
			{
				throw new Exception("Action SetUserVariable can not be Scalar");
			}
			TryGetString(out _valueVariable, "Value", string.Empty, string.Empty, this);
		}

		protected override void ApplyAction(object modelData)
		{
			UserManager.SetGlobalVariable(base.name.AsRpn(), _valueVariable.AsRpn());
		}
	}
}
