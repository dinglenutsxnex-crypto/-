using System;
using System.Collections.Generic;
using System.Linq;
using SF3.Audio;
using SF3.Effects;
using SF3.GameModels;

namespace SF3
{
	public class ShadowFormController
	{
		private bool _active;

		private int _modelsCountInShadowForm;

		private readonly Dictionary<int, ModelShadowForm> _modelsShadowForm;

		public static ShadowFormController Instance { get; private set; }

		public ShadowFormController()
		{
			Instance = this;
			_modelsShadowForm = new Dictionary<int, ModelShadowForm>();
		}

		public void Initialize()
		{
			_active = false;
			_modelsCountInShadowForm = 0;
		}

		public void DisposeUnused()
		{
			for (int i = 0; i < _modelsShadowForm.Count; i++)
			{
				KeyValuePair<int, ModelShadowForm> keyValuePair = _modelsShadowForm.ElementAt(i);
				if (keyValuePair.Value == null)
				{
					_modelsShadowForm.Remove(keyValuePair.Key);
					i--;
				}
				keyValuePair.Value.Reset();
			}
		}

		public void Update()
		{
			foreach (ModelShadowForm value in _modelsShadowForm.Values)
			{
				value.Update();
			}
		}

		public void EnableUse(bool isEnableUse, int modelID = -1)
		{
			foreach (KeyValuePair<int, ModelShadowForm> item in _modelsShadowForm)
			{
				if (modelID == -1 || item.Key == modelID)
				{
					item.Value.EnableUseSF(isEnableUse);
				}
			}
		}

		private bool CanUseSF(int modelID)
		{
			if (_modelsShadowForm.ContainsKey(modelID))
			{
				return _modelsShadowForm[modelID].EnableUse;
			}
			return false;
		}

		public void SetLocationShadowFormEnabled(bool enable, bool instant = false)
		{
			_active = enable;
			if (LocationColorAnimation.Instance != null)
			{
				LocationColorAnimation.Instance.ShadowForm(enable, instant);
			}
			if (LocationAudioSettings.Instance != null)
			{
				LocationAudioSettings.Instance.ActivateShadowFormEQSetting(enable);
			}
			if (!enable)
			{
				GlowEffectController.instance.DisableGlow();
				foreach (InteractiveModelObject droppedInteractiveObject in InteractiveModelObject.droppedInteractiveObjects)
				{
					droppedInteractiveObject.DisableShadowForm();
				}
			}
			ShadowMapController.Instance.SwitchShadowMap(enable);
		}

		public void ClearShadowEffect()
		{
			ClearShadowEffect(ModelsManager.Instance.Player, ModelsManager.Instance.Enemy);
		}

		private void ClearShadowEffect(params Model[] models)
		{
			foreach (Model model in models)
			{
				if (model != null)
				{
					_modelsShadowForm[model.id].ClearEffect(model.id);
				}
			}
		}

		public void ActivateShadowForm(int modelId, string effectName, bool isInfinityForm = false)
		{
			if (!CanUseSF(modelId))
			{
				return;
			}
			if (!_modelsShadowForm[modelId].shadowFormActive)
			{
				if (modelId == 1)
				{
					StickHelper.Instance.ShowShadowHint();
				}
				_modelsCountInShadowForm++;
				_modelsShadowForm[modelId].SetShadowFormEffect(effectName);
				EffectsManager.PlayCustomEffect(ModelsManager.Instance.GetModelById(modelId), effectName, "pelvis");
				_modelsShadowForm[modelId].ActivateShadowForm(false, isInfinityForm);
				FloorController.SetShadows(false);
			}
			if (!_active)
			{
				SetLocationShadowFormEnabled(true);
			}
		}

		public void DisableShadowForm(int modelId)
		{
			if (!CanUseSF(modelId))
			{
				return;
			}
			if (_modelsShadowForm.ContainsKey(modelId))
			{
				if (modelId == 1)
				{
					StickHelper.Instance.HideShadowHint();
				}
				if (_modelsShadowForm[modelId].shadowFormActive)
				{
					_modelsCountInShadowForm--;
					_modelsShadowForm[modelId].DisableShadowForm();
					FloorController.SetShadows(true);
				}
			}
			if (_active && (_modelsCountInShadowForm <= 0 || modelId == -2))
			{
				SetLocationShadowFormEnabled(false);
			}
		}

		public void EnableShadowChargeBurnDown(int modelId, bool enable)
		{
			if (_modelsShadowForm.ContainsKey(modelId))
			{
				_modelsShadowForm[modelId].EnableEnergyBurnDown(enable);
			}
		}

		public void SetDissolveWeaponIn(Action callback)
		{
			if (LocationColorAnimation.Instance != null)
			{
				LocationColorAnimation.Instance.SetDissolveWeaponIn(callback);
			}
		}

		public void SetDissolveWeaponOut(Action callback)
		{
			if (LocationColorAnimation.Instance != null)
			{
				LocationColorAnimation.Instance.SetDissolveWeaponOut(callback);
			}
		}

		public void SetShadowCharge(int modelID, float value)
		{
			if (CanUseSF(modelID))
			{
				_modelsShadowForm[modelID].SetShadowCharge(value);
			}
		}

		public void AddModelShadowForm(int modelID, ModelShadowForm msf)
		{
			_modelsShadowForm[modelID] = msf;
			if (modelID == 1 && StickHelper.Instance != null)
			{
				StickHelper.Instance.HideShadowHint();
			}
		}

		public void RegisterForEvent_SetShadowCharge(int modelID, Action<float> callback)
		{
			if (_modelsShadowForm.ContainsKey(modelID))
			{
				_modelsShadowForm[modelID].OnSetShadowCharge += callback;
			}
		}
	}
}
