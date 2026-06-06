using System;
using Nekki.Yaml;
using SF3.Effects;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerHitEffect : TriggerAction
	{
		private readonly RpnValue<int> _countParticles;

		private readonly RpnValue<bool> _follow;

		private int CountParticles
		{
			get
			{
				int num = _countParticles;
				return (num >= 0) ? num : 0;
			}
		}

		public TriggerHitEffect(Node yamlNode)
			: base(EActionType.HIT_EFFECT, yamlNode)
		{
			string outResult;
			TryGetString(out outResult, "ParticlesCount", "0", string.Empty, null, false);
			_countParticles = outResult;
			_follow = false;
			if (TryGetString(out outResult, "Follow", "0", string.Empty, null, false))
			{
				_follow = outResult;
			}
		}

		protected override void ApplyAction(object modelData)
		{
			if (!Model.disableEffects)
			{
				Vector3 vector = new Vector3(0f, 0f, 1f);
				vector.z = (0f - Vector2.Angle(Vector2.up, Model.hitResult.CapsuleAttackDirection)) * (float)Math.Sign(Model.hitResult.Impulse.x);
				Vector3 angle = vector;
				EffectsManager.PlayHitEffect((Model)modelData, base.name, Model.hitResult.StrikeData.strikeEffectPoint, angle, CountParticles, _follow, ((Model)modelData).id);
			}
		}
	}
}
