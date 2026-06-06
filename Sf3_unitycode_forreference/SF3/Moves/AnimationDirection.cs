using System;
using Nekki.Yaml;
using SF3.GameModels;

namespace SF3.Moves
{
	[Serializable]
	public class AnimationDirection
	{
		[Flags]
		public enum ApplyingType : byte
		{
			Player = 1,
			Bot = 2,
			All = 3
		}

		private ApplyingType _applyingType;

		private DistancePoint _from;

		private DistancePoint _to;

		private bool _impulseReverse;

		public AnimationDirection(Mapping directionEntry)
		{
			Scalar text = directionEntry.GetText("ApplyTo");
			if (text == null)
			{
				_applyingType = ApplyingType.All;
			}
			else
			{
				_applyingType = (ApplyingType)Enum.Parse(typeof(ApplyingType), text.text);
			}
			text = directionEntry.GetText("Impulse");
			if (text != null)
			{
				_impulseReverse = (text.text.Equals("-1") ? true : false);
				_from = (_to = null);
				return;
			}
			Mapping mapping = directionEntry.GetMapping("From");
			_from = new DistancePoint(mapping);
			mapping = directionEntry.GetMapping("To");
			_to = new DistancePoint(mapping);
		}

		public EDirectionType MoveDirection(IAnimatedModel animatedModel)
		{
			if (_from == null)
			{
				if (_impulseReverse)
				{
					return (!(Model.hitResult.Impulse.x > 0f)) ? EDirectionType.RIGHT : EDirectionType.LEFT;
				}
				return (Model.hitResult.Impulse.x > 0f) ? EDirectionType.RIGHT : EDirectionType.LEFT;
			}
			if (_from.GetPositionX(animatedModel) < _to.GetPositionX(animatedModel))
			{
				return EDirectionType.RIGHT;
			}
			return EDirectionType.LEFT;
		}

		public bool IsAppliedTo(bool isBot)
		{
			ApplyingType applyingType = ((!isBot) ? ApplyingType.Player : ApplyingType.Bot);
			return (int)(_applyingType & applyingType) > 0;
		}
	}
}
