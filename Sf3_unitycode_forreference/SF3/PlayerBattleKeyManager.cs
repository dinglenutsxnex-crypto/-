using System;
using System.Collections.Generic;
using SF3.KeyPressInfo;

namespace SF3
{
	public class PlayerBattleKeyManager
	{
		private readonly int _modelId;

		private bool _activated;

		private bool _inverted;

		private bool _stateIsNormal;

		private bool _isControl;

		private readonly KeysHolder _keysHolder;

		private readonly Action<int, KeyData> _mainListener;

		private readonly List<KeyData> _ultimateKeyData;

		private readonly HashSet<EQuadrants> _keysDisabled;

		private PlayerBattleKeyManager()
		{
			_activated = false;
			_stateIsNormal = true;
			_ultimateKeyData = new List<KeyData>
			{
				new KeyData(EQuadrants.Any, KeyPressState.Ultimate)
			};
			_keysHolder = new KeysHolder(BattleKeyManager.INVERTION_MAP, BattleKeyManager.CONFLICTING_MAP);
			_keysHolder.OnKeyPressStateChanged += OnKeyPressStateChanged;
			_keysDisabled = new HashSet<EQuadrants>();
			GameController.Instance.addEventListener(0, OnKeyDown);
			GameController.Instance.addEventListener(1, OnKeyUp);
		}

		public PlayerBattleKeyManager(int modelID, Action<int, KeyData> mainListener)
			: this()
		{
			_modelId = modelID;
			_mainListener = mainListener;
		}

		public void Activate(bool activate)
		{
			_activated = activate;
		}

		public int GetModelId()
		{
			return _modelId;
		}

		public bool IsInverted()
		{
			return _inverted;
		}

		public void Activate(EQuadrants quadrant, bool activate)
		{
			if (activate)
			{
				if (_keysDisabled.Contains(quadrant))
				{
					_keysDisabled.Remove(quadrant);
				}
			}
			else
			{
				_keysDisabled.Add(quadrant);
			}
			_keysHolder.ResetState(quadrant);
		}

		public void SwitchState()
		{
			_stateIsNormal = !_stateIsNormal;
		}

		public void Initialize()
		{
			_keysHolder.Initialize();
			if (ModelsManager.Instance.Player.id == _modelId)
			{
				_isControl = ModelsManager.Instance.Player.isControl;
			}
			else
			{
				_isControl = ModelsManager.Instance.Enemy.isControl;
			}
			_keysDisabled.Clear();
		}

		public void SetControll(bool controlled)
		{
			_isControl = controlled;
		}

		public void Clear()
		{
			_keysHolder.Dispose();
		}

		public void Update(float gameTimeDelta)
		{
			if (!BattleKeyManager.paused)
			{
				_keysHolder.Update(gameTimeDelta);
			}
		}

		public void Invert(bool invert)
		{
			_keysHolder.Invert(invert);
			_inverted = invert;
		}

		public List<KeyData> GetKeys()
		{
			if (_isControl)
			{
				return _keysHolder.GetKeys();
			}
			if (_stateIsNormal)
			{
				return null;
			}
			return _ultimateKeyData;
		}

		private void OnKeyDown(CallEventArgs callEventArgs)
		{
			OnKey(true, (int[])callEventArgs.Content);
		}

		private void OnKeyUp(CallEventArgs callEventArgs)
		{
			OnKey(false, (int[])callEventArgs.Content);
		}

		private void OnKey(bool down, int[] content)
		{
			if (!_activated && (_activated || down))
			{
				return;
			}
			int num = content[0];
			EQuadrants eQuadrants = (EQuadrants)content[1];
			if (!_keysDisabled.Contains(eQuadrants) && (num < 0 || num == _modelId))
			{
				if (down)
				{
					_keysHolder.KeyDown(eQuadrants);
				}
				else
				{
					_keysHolder.KeyUp(eQuadrants);
				}
			}
		}

		private void OnKeyPressStateChanged(KeyData keyState)
		{
			if (!BattleKeyManager.paused && _isControl)
			{
				_mainListener(_modelId, keyState);
			}
		}

		public void Dispose()
		{
			GameController.Instance.removeEventListener(0, OnKeyDown);
			GameController.Instance.removeEventListener(1, OnKeyUp);
			Clear();
		}
	}
}
