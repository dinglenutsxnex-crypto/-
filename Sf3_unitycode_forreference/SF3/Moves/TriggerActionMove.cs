using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	public class TriggerActionMove : TriggerAction
	{
		private readonly float _acceleration;

		private readonly int _frames;

		private readonly float _speed;

		public TriggerActionMove(Node yamlNode)
			: base(EActionType.MOVE, yamlNode)
		{
			TryGetFloat(out _speed, "Speed", 0f, string.Empty, this);
			TryGetFloat(out _acceleration, "Acceleration", 0f, string.Empty, this);
			TryGetInt(out _frames, "Frames", 0, string.Empty, this);
		}

		protected override void ApplyAction(object modelData)
		{
			((Model)modelData).modelAnimation.Move(_speed, _acceleration, _frames);
		}
	}
}
