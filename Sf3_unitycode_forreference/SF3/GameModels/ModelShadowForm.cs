using System;
using SF3.Effects;
using SF3.Moves;
using SF3.Settings;
using UnityEngine;

namespace SF3.GameModels
{
	public class ModelShadowForm
	{
		private IEventSender _eventSender;

		private bool _isPlayer;

		private IShadowFormModel _modelComponents;

		private bool infiniteEnergy;

		private string currentEffect;

		public int modelID;

		private bool _enableUse;

		public float shadowEnergy { get; private set; }

		public bool shadowFormActive { get; private set; }

		public bool EnableUse
		{
			get
			{
				return _enableUse;
			}
		}

		public event Action<float> OnSetShadowCharge;

		public ModelShadowForm(int modelID, IEventSender eventSenderValue, bool isPlayerValue, IShadowFormModel modelComponentsValue)
		{
			_modelComponents = modelComponentsValue;
			_eventSender = eventSenderValue;
			_isPlayer = isPlayerValue;
			shadowEnergy = 0f;
			shadowFormActive = false;
			infiniteEnergy = false;
			this.modelID = modelID;
			EnableUseSF(true);
			ShadowFormController.Instance.AddModelShadowForm(modelID, this);
		}

		public void EnableUseSF(bool enb)
		{
			_enableUse = enb;
		}

		public void Update()
		{
			if (shadowFormActive && !infiniteEnergy)
			{
				SetShadowCharge(shadowEnergy - FightSettings.shadowFormParams.burnDownPerFrame * GameTimeController.timeScale);
			}
		}

		public void Reset()
		{
			shadowEnergy = 0f;
			infiniteEnergy = false;
			DisableShadowForm();
			if (this.OnSetShadowCharge != null)
			{
				Delegate[] invocationList = this.OnSetShadowCharge.GetInvocationList();
				Delegate[] array = invocationList;
				for (int i = 0; i < array.Length; i++)
				{
					Action<float> value = (Action<float>)array[i];
					OnSetShadowCharge -= value;
				}
			}
		}

		public void SetShadowCharge(float value)
		{
			if (infiniteEnergy)
			{
				return;
			}
			float num = Mathf.Clamp01(value);
			if (num == shadowEnergy)
			{
				return;
			}
			shadowEnergy = Mathf.Clamp01(value);
			if (shadowFormActive)
			{
				if (shadowEnergy == 0f)
				{
					_eventSender.ThrowEvent(new BattleEventArgs(ETriggerEvents.EVENT_SHADOW_CHARGE_BURNDOWN, -2));
				}
			}
			else if (shadowEnergy == 1f && _isPlayer)
			{
				ActionButtons.PlayShadowFull();
				StickHelper.Instance.ShowShadowHint();
			}
			if (this.OnSetShadowCharge != null)
			{
				this.OnSetShadowCharge(shadowEnergy);
			}
		}

		internal void ClearEffect(int id)
		{
			if (!currentEffect.IsNullOrEmpty())
			{
				EffectsManager.StopAll(currentEffect, id);
			}
		}

		internal void SetShadowFormEffect(string effectName)
		{
			currentEffect = effectName;
		}

		public void ActivateShadowForm(bool instant = false, bool isInfinityForm = false)
		{
			if (!shadowFormActive)
			{
				infiniteEnergy = isInfinityForm;
				GlowEffectController.instance.EnableGlow();
				shadowFormActive = true;
				_modelComponents.ActivateShadowForm(instant);
				if (_isPlayer)
				{
					ActionButtons.Instance.StopShadowFull();
				}
				SetShadowPerkState(ShadowPerksState.Active);
			}
		}

		public void DisableShadowForm()
		{
			if (shadowFormActive)
			{
				SetShadowPerkState(ShadowPerksState.Ready);
				shadowEnergy = 0f;
				shadowFormActive = false;
				_modelComponents.DisableShadowForm();
			}
		}

		private void SetShadowPerkState(ShadowPerksState state)
		{
			BattleInterface.Instance.SetShadowPerkState(_isPlayer ? 1 : 2, state);
		}

		public void EnableEnergyBurnDown(bool enable)
		{
			infiniteEnergy = !enable;
		}
	}
}
