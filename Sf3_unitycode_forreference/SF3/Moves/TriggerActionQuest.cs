using Nekki.Yaml;

namespace SF3.Moves
{
	public class TriggerActionQuest : TriggerAction
	{
		private readonly bool _autoClose;

		public TriggerActionQuest(EActionType type, Node node, bool autoClose = true)
			: base(type, node)
		{
			_autoClose = autoClose;
		}

		protected sealed override void ApplyThisAction(object modelData)
		{
			base.ApplyThisAction(modelData);
			if (_autoClose)
			{
				CloseAction();
			}
		}

		public void CloseAction()
		{
			QuestController.Instance.CloseAction(this);
		}
	}
}
