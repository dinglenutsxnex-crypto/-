using System.Collections.Generic;
using SF3.Moves;

namespace SF3.GameModels
{
	public class ModelDissolveWeapon
	{
		private enum DissolveEventType
		{
			None = 0,
			Start = 1,
			End = 2,
			Active = 3
		}

		private enum AnimationEventType
		{
			None = 0,
			Start = 1,
			StartWithDis = 2,
			EndWithDis = 3
		}

		private ModelComponents _modelComponents;

		private ModelShadowForm _modelShadowForm;

		private ModelAnimation _modelAnimation;

		private DissolveEventType _disIntervalPrevEvent;

		private DissolveEventType _disIntervalCurrEvent;

		private AnimationEventType _AnimationEvent;

		public ModelDissolveWeapon(ModelComponents modelComponents, ModelShadowForm modelShadowForm, ModelAnimation modelAnimation)
		{
			_modelComponents = modelComponents;
			_modelShadowForm = modelShadowForm;
			_modelAnimation = modelAnimation;
		}

		public List<IntervalAnimation> GetIntervalExist(EIntervalsType intervalType)
		{
			return _modelAnimation.GetIntervalTypeExist(intervalType);
		}

		public void ApplyEvent(BattleEventArgs args)
		{
			switch (args.EventType)
			{
			case ETriggerEvents.EVENT_ANIMATION_END:
				DissolveWeaponAnimEndEvent(args);
				break;
			case ETriggerEvents.EVENT_ANIMATION_START:
				DissolveWeaponAnimStartEvent(args);
				break;
			case ETriggerEvents.EVENT_INTERVAL_START:
				DissolveWeaponOnIntervalEvent(args, true);
				break;
			case ETriggerEvents.EVENT_INTERVAL_END:
				DissolveWeaponOnIntervalEvent(args, false);
				break;
			}
		}

		private void DissolveWeaponOnIntervalEvent(BattleEventArgs args, bool enable)
		{
			if (((IntervalAnimation)args.EventData).type == EIntervalsType.INTERVAL_DISSOLVE_WEAPON)
			{
				_disIntervalCurrEvent = (enable ? DissolveEventType.Start : DissolveEventType.End);
			}
		}

		private void DissolveWeaponAnimStartEvent(BattleEventArgs args)
		{
			int count = GetIntervalExist(EIntervalsType.INTERVAL_DISSOLVE_WEAPON).Count;
			_AnimationEvent = ((count == 0) ? AnimationEventType.Start : AnimationEventType.StartWithDis);
		}

		private void DissolveWeaponAnimEndEvent(BattleEventArgs args)
		{
			int count = GetIntervalExist(EIntervalsType.INTERVAL_DISSOLVE_WEAPON).Count;
			_AnimationEvent = ((count != 0) ? AnimationEventType.EndWithDis : AnimationEventType.None);
		}

		private DissolveEventType AnalyzeEvents()
		{
			DissolveEventType result = DissolveEventType.None;
			if (_disIntervalCurrEvent == DissolveEventType.Start)
			{
				result = DissolveEventType.Start;
			}
			if (_AnimationEvent == AnimationEventType.Start)
			{
				result = DissolveEventType.End;
			}
			if (_disIntervalCurrEvent == DissolveEventType.End)
			{
				if (_AnimationEvent == AnimationEventType.None)
				{
					result = DissolveEventType.End;
				}
				else if (_AnimationEvent == AnimationEventType.EndWithDis)
				{
					result = DissolveEventType.None;
				}
			}
			_disIntervalCurrEvent = DissolveEventType.None;
			_AnimationEvent = AnimationEventType.None;
			return result;
		}

		public void EndFrameUpdate()
		{
			if (!_modelShadowForm.shadowFormActive)
			{
				if (_disIntervalPrevEvent == DissolveEventType.Active)
				{
					_modelComponents.DissolveWeaponMeshrendererOn();
					_modelComponents.DissolveWeaponMaterialOff();
				}
				_disIntervalPrevEvent = DissolveEventType.None;
				_disIntervalCurrEvent = DissolveEventType.None;
				_AnimationEvent = AnimationEventType.None;
				return;
			}
			DissolveEventType dissolveEventType = AnalyzeEvents();
			if (dissolveEventType != 0 && (_disIntervalPrevEvent != DissolveEventType.Active || dissolveEventType != DissolveEventType.Start) && (_disIntervalPrevEvent != 0 || dissolveEventType != DissolveEventType.End))
			{
				if (dissolveEventType == DissolveEventType.Start)
				{
					_modelComponents.SetDissolveWeaponMaterial();
					ShadowFormController.Instance.SetDissolveWeaponIn(_modelComponents.DissolveWeaponMeshrendererOff);
					_disIntervalPrevEvent = DissolveEventType.Active;
				}
				else
				{
					_modelComponents.DissolveWeaponMeshrendererOn();
					ShadowFormController.Instance.SetDissolveWeaponOut(_modelComponents.DissolveWeaponMaterialOff);
					_disIntervalPrevEvent = DissolveEventType.None;
				}
			}
		}
	}
}
