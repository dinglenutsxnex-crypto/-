using Nekki.Yaml;
using SF3.GameModels;
using SF3.Utils;

namespace SF3.Moves
{
	public class TriggerActionDestroyMe : TriggerAction
	{
		private readonly RpnValue<int> _fadeFrames;

		public TriggerActionDestroyMe(Node yamNode)
			: base(EActionType.DESTROY_ME, yamNode)
		{
			_fadeFrames = 0;
			if (!(yamNode is Scalar))
			{
				Scalar text = ((Mapping)yamNode).GetMapping("DestroyMe").GetText("Frames");
				if (text != null)
				{
					_fadeFrames = text.text;
				}
			}
		}

		protected override void ApplyAction(object modelData)
		{
			Model model = (Model)modelData;
			model.DisableSlowMotion();
			if ((int)_fadeFrames > 0)
			{
				BehaviourTimer.CreateFramesTimer(_fadeFrames, delegate(float value)
				{
					model.SetTransparent(1f - value);
				}, null, delegate
				{
					OnDisable(model);
				});
			}
			else
			{
				OnDisable(model);
			}
		}

		private void OnDisable(Model model)
		{
			ModelsManager.Instance.DisableModel(model.id);
			model.Activate(false);
		}
	}
}
