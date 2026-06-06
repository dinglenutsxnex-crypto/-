using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Yaml;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : UIModuleHolder
{
	private class LoadScreenProcess
	{
		public bool Show { get; set; }

		public bool Hide { get; set; }

		public LoadScreenProcess()
		{
			Clear();
		}

		public void Clear()
		{
			Show = false;
			Hide = false;
		}
	}

	private static LoadScreen _instance;

	private readonly List<string> _aliases = new List<string>();

	private int _tipIndex;

	[SerializeField]
	private UnityEngine.UI.Text _tip;

	[SerializeField]
	private RawImage _loader;

	[SerializeField]
	private float _defaultTweenDuration;

	[SerializeField]
	private float _hideRoundEndDuration;

	private LoadScreenProcess _processLoader;

	private readonly List<Action> _onHideLoader = new List<Action>();

	public static LoadScreen Instance
	{
		get
		{
			if (_instance == null)
			{
				NekkiUIRootModules.Instance.MountNativeModule("LoadScreen");
			}
			return _instance;
		}
	}

	public static bool LoaderVisible
	{
		get
		{
			return _instance != null && Instance._loader.color.a > 0.001f;
		}
	}

	public static void Clear()
	{
		_instance = null;
	}

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
		string loadTextInternal = GlobalLoad.GetLoadTextInternal("GameSettings", "lore_hint_config.yaml");
		Node root = YamlDocumentNekki.FromYamlContent(loadTextInternal).GetRoot("Lines");
		foreach (object item in root)
		{
			_aliases.Add(item.ToString());
		}
		_processLoader = new LoadScreenProcess();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private void Update()
	{
		if (!LoaderVisible && _onHideLoader.Count > 0)
		{
			OnDisableLoader();
		}
	}

	private void OnDisable()
	{
		OnDisableLoader();
		_instance = null;
		Routiner.Go(Unmount());
	}

	private IEnumerator Unmount()
	{
		yield return new WaitForEndOfFrame();
		NekkiUIRootModules.Instance.UnmountModule("LoadScreen");
	}

	public static void ShowLoader(Action onDone = null, float showDelay = 0f, bool instantly = false)
	{
		Instance._loader.gameObject.SetActive(true);
		float duration = ((!instantly) ? Instance._defaultTweenDuration : 0f);
		Instance.SetLoaderVisible(onDone, duration, showDelay);
	}

	public static void HideLoader(Action onDone)
	{
		HideLoader(onDone, Instance._hideRoundEndDuration);
	}

	private static void HideLoader(Action onDone = null, float duration = 0f, float delay = 0f)
	{
		Instance._processLoader.Hide = true;
		Instance._tip.gameObject.SetActive(false);
		Instance.TweenLoader(0f, duration, delay, onDone);
	}

	public void SheduleOnHideLoader(Action onHide)
	{
		_onHideLoader.Add(onHide);
	}

	public void UnsheduleOnHideLoader(Action onHide)
	{
		_onHideLoader.Remove(onHide);
	}

	private void OnDisableLoader()
	{
		_processLoader.Clear();
		foreach (Action item in _onHideLoader)
		{
			item();
		}
		_onHideLoader.Clear();
	}

	private void LoaderTweenFinished()
	{
		if (!LoaderVisible)
		{
			_processLoader.Hide = false;
			_loader.gameObject.SetActive(false);
		}
		else
		{
			_tip.gameObject.SetActive(true);
			_processLoader.Show = false;
		}
	}

	public void RefreshTip()
	{
		_tipIndex = UnityEngine.Random.Range(0, _aliases.Count);
		Instance._tip.text = Localization.Get(_aliases[_tipIndex]);
	}

	public void ShowFightStart(Action eventToThrow)
	{
		ScreenTexture.Instance.SetTexture(base.name, ScreenTexture.TextureOutputCamera.Main, ScreenTexture.TextureOutputFilter.None, delegate
		{
			if (LoaderVisible)
			{
				HideLoader(eventToThrow.InvokeSafe, 0.25f);
			}
			else
			{
				eventToThrow.InvokeSafe();
			}
		});
	}

	public void HideRoundEnd(Action onDone = null)
	{
		ScreenTexture.Instance.Clear(base.name);
		onDone.InvokeSafe();
	}

	public bool LoaderIsReady()
	{
		return !_processLoader.Hide && !_processLoader.Show;
	}

	private void SetLoaderVisible(Action onDone = null, float duration = 0f, float delay = 0f)
	{
		_processLoader.Show = true;
		RefreshTip();
		TweenLoader(1f, duration, delay, onDone);
	}

	private void TweenLoader(float value, float duration = 0f, float delay = 0f, Action onDone = null)
	{
		if (duration > 0f || delay > 0f)
		{
			_loader.DOFade(value, duration).SetDelay(delay).OnComplete(delegate
			{
				onDone.InvokeSafe();
				LoaderTweenFinished();
			});
		}
		else
		{
			_loader.color = new Color(_loader.color.r, _loader.color.g, _loader.color.b, value);
			onDone.InvokeSafe();
			LoaderTweenFinished();
		}
	}

	private void OnDialogOpened(object obj)
	{
		_loader.gameObject.SetActive(false);
	}

	private void OnDialogClosed(object obj)
	{
		if ((bool)_loader)
		{
			_loader.gameObject.SetActive(true);
		}
	}
}
