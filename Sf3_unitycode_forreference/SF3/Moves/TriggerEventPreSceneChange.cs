using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerEventPreSceneChange : TriggerEvent
	{
		private readonly string from;

		private readonly string to;

		private IntentTransitionData transitionData;

		public TriggerEventPreSceneChange(Mapping eventMap)
			: base(ETriggerEvents.QEVENT_PRE_SCENE_CHANGE, eventMap)
		{
			if (eventMap != null)
			{
				if (TryGetString(out from, "From", string.Empty, string.Empty, this))
				{
					from = from.ToLower();
				}
				if (TryGetString(out to, "To", string.Empty, string.Empty, this))
				{
					to = to.ToLower();
				}
			}
		}

		protected override bool Equal()
		{
			if (from.IsNullOrEmpty() && to.IsNullOrEmpty())
			{
				return true;
			}
			if (arguments.Length > 0 && arguments[0] != null)
			{
				transitionData = (IntentTransitionData)arguments[0];
				string value = transitionData.FromData.Name.ToLower();
				string value2 = transitionData.ToData.Name.ToLower();
				return (!from.IsNullOrEmpty() && from.Equals(value)) || (!to.IsNullOrEmpty() && from.Equals(value2));
			}
			return false;
		}

		protected override void SetArguments(object[] args)
		{
			base.SetArguments(args);
			if (arguments.Length > 0 && arguments[0] != null)
			{
				transitionData = (IntentTransitionData)arguments[0];
			}
		}

		public override object GetArgument(string field)
		{
			if (transitionData != null)
			{
				switch (field)
				{
				case "ModuleTo":
					return transitionData.ToData.Name;
				case "TabTo":
					return transitionData.ToData.Category;
				case "ModuleFrom":
					return transitionData.FromData.Name;
				case "TabFrom":
					return transitionData.FromData.Category;
				}
			}
			return base.GetArgument(field);
		}
	}
}
