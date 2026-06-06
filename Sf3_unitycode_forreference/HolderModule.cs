using System;
using System.Collections;
using SF3;
using SF3.Items;
using SF3.UserData;
using UnityEngine;

public class HolderModule
{
	public enum State
	{
		None = 0,
		Opening = 1,
		Opened = 2,
		Closed = 3
	}

	protected float _modelsFadeTime = 1f;

	private State _state;

	private Action<HolderModule> _onOpenedModule;

	private Action<HolderModule> _onClosedModule;

	public ConstantsSF3.ELocationSceneModule ModuleType { get; private set; }

	public IntentModule Intent { get; private set; }

	public virtual void Init(ConstantsSF3.ELocationSceneModule type, Action<HolderModule> onOpened, Action<HolderModule> onClosed)
	{
		ModuleType = type;
		_state = State.None;
		_onOpenedModule = onOpened;
		_onClosedModule = onClosed;
	}

	private void SetState(State state)
	{
		_state = state;
	}

	public void CallIntentOpen()
	{
		Intent.Update();
		Intent.RunCallbackOpenModule();
	}

	public void SetIntent(IntentModule intent)
	{
		Intent = intent;
	}

	public void Open(IntentModule intent)
	{
		SetIntent(intent);
		SetState(State.Opening);
		OpenModule(intent);
	}

	protected virtual void OpenModule(IntentModule intent)
	{
	}

	protected virtual void OpenedCallback()
	{
		SetState(State.Opened);
		Routiner.Go(OnOpenRoutine());
	}

	private IEnumerator OnOpenRoutine()
	{
		yield return new WaitForEndOfFrame();
		SetState(State.Opened);
		if (_onOpenedModule != null)
		{
			_onOpenedModule(this);
		}
	}

	public virtual bool IsCanOpen()
	{
		return _state != State.Opening && _state != State.Opened;
	}

	public virtual void Close(ConstantsSF3.ELocationSceneModule newType)
	{
		SetState(State.Closed);
		CloseModule(Intent);
		if (_onClosedModule != null)
		{
			_onClosedModule(this);
		}
	}

	protected virtual void CloseModule(IntentModule intent)
	{
	}

	public static void EnableControls(bool enable)
	{
		Stick.SetActive(enable);
		ActionButtons.SetActive(enable);
		ActionButtons.Instance.MissileButtonEnable(enable && UserManager.IsEquiped(EquipmentType.Ranged));
		BattleKeyManager.Instance.ActivateBattleKeys(enable);
	}

	public override string ToString()
	{
		return string.Concat("HolderModule [State:", _state, " - ", Intent, "]");
	}
}
