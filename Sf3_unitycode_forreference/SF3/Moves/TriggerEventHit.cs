using Nekki.Yaml;
using SF3.GameModels;
using SF3_Attributes;

namespace SF3.Moves
{
	public class TriggerEventHit : TriggerEvent
	{
		private readonly string _animationName;

		private readonly AttributeType _defenseName;

		public TriggerEventHit(Mapping eventMap)
			: base(ETriggerEvents.EVENT_HIT, eventMap)
		{
			_animationName = string.Empty;
			if (eventMap != null)
			{
				string outResult;
				if (TryGetString(out outResult, "Defense", string.Empty, string.Empty, null, false))
				{
					_defenseName = ComplianceUtils.GetAttributeTypeByName(outResult);
				}
				if (TryGetString(out outResult, "Animation", string.Empty, string.Empty, null, false))
				{
					_animationName = outResult;
				}
			}
		}

		public override bool Equal(BattleEventArgs args)
		{
			if (!base.Equal(args))
			{
				return false;
			}
			if (_defenseName == AttributeType.None && _animationName.Length == 0)
			{
				return true;
			}
			bool result = false;
			if (_defenseName != 0)
			{
				AttributeType defenseAttribute = Model.hitResult.StrikeData.intervalAttack.damage.defenseAttribute;
				Capsule collisionEdge = Model.hitResult.StrikeData.collisionEdge;
				if (defenseAttribute != 0)
				{
					if (_defenseName.Equals(defenseAttribute))
					{
						result = true;
					}
				}
				else
				{
					if (collisionEdge == null)
					{
						return false;
					}
					if (_defenseName.Equals(collisionEdge.defense))
					{
						result = true;
					}
				}
			}
			if (_animationName.Length > 0 && args.EventData != null)
			{
				StrikeData strikeData = (StrikeData)args.EventData;
				string[] animationNames = strikeData.attackingModel.modelAnimation.animationNames;
				string text = _animationName.ToLower();
				for (int i = 0; i < animationNames.Length; i++)
				{
					if (text.Equals(animationNames[i]))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
