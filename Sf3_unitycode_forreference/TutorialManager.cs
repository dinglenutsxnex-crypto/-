using System;
using System.Collections;
using System.Collections.Generic;
using SF3;
using SF3.Moves;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private class ShowTutorialPanelIntent
	{
		public string alias { get; protected set; }

		public string[] args { get; protected set; }

		public string animation { get; protected set; }

		public Vector2 offset { get; protected set; }

		public string type { get; protected set; }

		public ShowTutorialPanelIntent(string type, string alias, string[] args, string animation, Vector2 offset)
		{
			this.alias = alias;
			this.args = args;
			this.animation = animation;
			this.offset = offset;
			this.type = type;
		}
	}

	public class SetLayerCallData
	{
		public List<string> IdTargets { get; private set; }

		public string Description { get; private set; }

		public bool ScreenBlock { get; private set; }

		public Vector2 Offset { get; private set; }

		public List<string> NotClickable { get; private set; }

		public SetLayerCallData(List<string> idTargets, string description, bool screenBlock, Vector2 offset, List<string> notClickable)
		{
			IdTargets = idTargets;
			Description = description;
			ScreenBlock = screenBlock;
			Offset = offset;
			NotClickable = notClickable;
		}
	}

	public class OpenTutorialCallData
	{
		public string Type { get; private set; }

		public string Alias { get; private set; }

		public string[] Args { get; private set; }

		public string Animation { get; private set; }

		public Vector2 Offset { get; private set; }

		public OpenTutorialCallData(string type, string alias, string[] args, string animation, Vector2 offset)
		{
			Type = type;
			Alias = alias;
			Args = args;
			Animation = animation;
			Offset = offset;
		}
	}

	public delegate void TutorialEvent(string targetId);

	public delegate void TutorialShowEvent();

	public static TutorialManager Instance;

	private Dictionary<string, TutorialComponent> components;

	private TutorialBlockNGUI _tutorialBlockNGUI;

	private TutorialBlockNative _tutorialBlockNative;

	[SerializeField]
	private Transform blockAnchor;

	private ShowTutorialPanelIntent currentIntent;

	private bool pannelIsHide;

	private string tutorialPannelType;

	private SetLayerCallData _callData;

	private OpenTutorialCallData _openTutorialCallData;

	private bool _fixSceduled;

	private Action callbackReleaseLayer;

	public TutorialBlockNGUI TutorialBlockNGUI
	{
		get
		{
			return _tutorialBlockNGUI;
		}
	}

	public TutorialBlockNative TutorialBlockNative
	{
		get
		{
			if (_tutorialBlockNative == null)
			{
				_tutorialBlockNative = NekkiUIRootModules.Instance.MountNativeModule("TutorialBlockNative").GetComponent<TutorialBlockNative>();
			}
			return _tutorialBlockNative;
		}
	}

	public TutorialPanel tutorialPanel { get; protected set; }

	public event TutorialEvent onSet;

	public event TutorialEvent onRelease;

	public event TutorialShowEvent onBlockSet;

	public event TutorialShowEvent onBlockRelease;

	private void Awake()
	{
		Instance = this;
		components = new Dictionary<string, TutorialComponent>();
		RefreshTutorialBlock();
		MapController.OnMapUpdate += MapTutorialDisconnectConsequencesFix;
	}

	private void RefreshTutorialBlock()
	{
		if ((bool)_tutorialBlockNGUI)
		{
			GlobalLoad.Unload(_tutorialBlockNGUI.gameObject);
		}
		_tutorialBlockNGUI = UnityEngine.Object.Instantiate(GlobalLoad.GetPrefab("UI/Tutorial/TutorialBlock").GetComponent<TutorialBlockNGUI>());
		_tutorialBlockNGUI.gameObject.SetActive(false);
		_tutorialBlockNGUI.transform.SetParent(blockAnchor, true);
		_tutorialBlockNGUI.transform.localScale = Vector3.one;
	}

	public void Add(string id, TutorialComponent component)
	{
		if (!Instance.components.ContainsKey(id) && !string.IsNullOrEmpty(id))
		{
			Instance.components.Add(id, component);
		}
	}

	public void Remove(string id)
	{
		if (components.ContainsKey(id))
		{
			components.Remove(id);
		}
	}

	public bool RemoveWithCheck(string id, GameObject check)
	{
		if (Instance.components.ContainsKey(id) && components[id].gameObject.GetInstanceID() != check.GetInstanceID())
		{
			return false;
		}
		Instance.components.Remove(id);
		return true;
	}

	public void ShowPanel(string type, string alias)
	{
		ShowPanel(new OpenTutorialCallData(type, alias, new string[0], string.Empty, Vector2.zero));
	}

	public void ShowPanel(OpenTutorialCallData openPanelCallData)
	{
		_openTutorialCallData = openPanelCallData;
		tutorialPannelType = _openTutorialCallData.Type;
		if (pannelIsHide)
		{
			currentIntent = new ShowTutorialPanelIntent(tutorialPannelType, _openTutorialCallData.Alias, _openTutorialCallData.Args, _openTutorialCallData.Animation, _openTutorialCallData.Offset);
			return;
		}
		NekkiUIModule nekkiUIModule = NekkiUIRootModules.Instance.MountNativeModule(tutorialPannelType + "Panel");
		tutorialPanel = nekkiUIModule.GetComponent<TutorialPanel>();
		tutorialPanel.SetMessage(_openTutorialCallData.Alias, _openTutorialCallData.Args);
		tutorialPanel.SetAnimation(_openTutorialCallData.Animation, _openTutorialCallData.Offset);
		tutorialPanel.PlayInAnimation();
	}

	public void HidePanel(bool instant = false)
	{
		if (!(tutorialPanel != null))
		{
			return;
		}
		Action onClose = delegate
		{
			NekkiUIRootModules.Instance.UnmountModule(tutorialPannelType + "Panel");
			tutorialPanel = null;
			pannelIsHide = false;
			if (currentIntent != null)
			{
				ShowPanel(new OpenTutorialCallData(currentIntent.type, currentIntent.alias, currentIntent.args, currentIntent.animation, currentIntent.offset));
				currentIntent = null;
			}
		};
		if (!instant)
		{
			pannelIsHide = true;
			_callData = null;
			tutorialPanel.PlayOutAnimation(delegate
			{
				onClose();
			});
		}
		else
		{
			onClose();
		}
	}

	private IEnumerator MapTutorialDisconnectConsequencesFixRoutine()
	{
		if ((bool)MapController.Instance)
		{
			MapController.Instance.ScaleActive = false;
		}
		yield return new WaitForEndOfFrame();
		if (_callData != null && _callData.NotClickable != null)
		{
			foreach (string idTarget in _callData.IdTargets)
			{
				ReleaseLayer(idTarget, true);
			}
		}
		RefreshTutorialBlock();
		if ((bool)MapController.Instance)
		{
			do
			{
				yield return new WaitForEndOfFrame();
			}
			while ((bool)MapController.Instance && MapController.Instance.CenteringActive);
			MapController.Instance.UpdateDragBounds();
			yield return new WaitForEndOfFrame();
		}
		if (_callData != null)
		{
			RefreshComponentsList();
			foreach (string idTarget2 in _callData.IdTargets)
			{
				if (components.ContainsKey(idTarget2))
				{
					components[idTarget2].SetBlockLayer(true);
				}
			}
			SetLayer(_callData);
		}
		if ((bool)MapController.Instance)
		{
			MapController.Instance.ScaleActive = true;
		}
		_fixSceduled = false;
	}

	private void MapTutorialDisconnectConsequencesFix()
	{
		if (!_fixSceduled)
		{
			_fixSceduled = true;
			Routiner.Go(MapTutorialDisconnectConsequencesFixRoutine());
		}
	}

	public void SetLayer(SetLayerCallData callData)
	{
		if (this.onBlockSet != null)
		{
			this.onBlockSet();
		}
		_callData = callData;
		TutorialComponent tutorialComponent = SetActiveTargets(callData.IdTargets, callData.ScreenBlock);
		if (!string.IsNullOrEmpty(callData.Description))
		{
			SetDefaults(callData.Description);
			if (tutorialComponent != null && tutorialPanel != null)
			{
				tutorialPanel.ToogleDescription(tutorialComponent.Viewport, callData.Offset);
			}
		}
		if (callData.ScreenBlock)
		{
			GameTimeController.GameTimePause();
		}
	}

	private void RefreshComponentsList()
	{
		TutorialComponent[] array = UnityEngine.Object.FindObjectsOfType<TutorialComponent>();
		TutorialComponent[] array2 = array;
		foreach (TutorialComponent tutorialComponent in array2)
		{
			if (components.ContainsKey(tutorialComponent.id))
			{
				components[tutorialComponent.id] = tutorialComponent;
			}
			else
			{
				components.Add(tutorialComponent.id, tutorialComponent);
			}
		}
	}

	private TutorialComponent SetActiveTargets(List<string> idTargets, bool ScreenBlock = true)
	{
		List<GameObject> list = new List<GameObject>();
		RefreshComponentsList();
		TutorialComponent tutorialComponent = null;
		foreach (string idTarget in idTargets)
		{
			if (components.ContainsKey(idTarget))
			{
				if (components[idTarget] is TutorialComponentNative)
				{
					list.Add(components[idTarget].gameObject);
				}
				components[idTarget].Select();
				if (tutorialComponent == null)
				{
					tutorialComponent = components[idTarget];
				}
				if (this.onSet != null)
				{
					this.onSet(idTarget);
				}
			}
		}
		if (ScreenBlock)
		{
			UIBlocker.Instance.Block(list);
		}
		return tutorialComponent;
	}

	private void SetDefaults(string description)
	{
		TutorialBlockNative.Select();
		_tutorialBlockNGUI.Select();
		base.gameObject.tag = "block";
		base.gameObject.SetActive(true);
	}

	public void SetReleaseLayerCallback(Action _callback)
	{
		callbackReleaseLayer = _callback;
	}

	public List<string> GetIdsByTag(string tag)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, TutorialComponent> component in components)
		{
			if (component.Value.groupTag == tag)
			{
				list.Add(component.Value.id);
			}
		}
		return list;
	}

	public void SetUnclickable(List<string> idTargets)
	{
		foreach (string idTarget in idTargets)
		{
			if (components.ContainsKey(idTarget))
			{
				components[idTarget].SetBlockLayer(false);
			}
		}
	}

	public void Release(string idTarget)
	{
		if (idTarget != null)
		{
			QuestController.Instance.ThrowEvent(ETriggerEvents.EVENT_TUTORIAL_BUTTON_PRESSED, idTarget);
		}
		ReleaseLayer(idTarget);
	}

	public void ReleaseDrag(string idDraggablre, string itTarget)
	{
		QuestController.Instance.ThrowEvent(ETriggerEvents.EVENT_TUTORIAL_DRAG_END, itTarget);
		ReleaseLayer(idDraggablre);
	}

	private void ReleaseLayer(string id, bool hideIstant = false)
	{
		GameTimeController.GameTimeResume();
		base.gameObject.tag = "Untagged";
		foreach (KeyValuePair<string, TutorialComponent> component in components)
		{
			if (component.Value.IsLocked)
			{
				component.Value.Release();
			}
		}
		TutorialBlockNative.Release();
		UIBlocker.Instance.Unblock();
		_tutorialBlockNGUI.gameObject.SetActive(false);
		HidePanel(hideIstant);
		if (this.onRelease != null)
		{
			this.onRelease(id);
		}
		if (this.onBlockRelease != null)
		{
			this.onBlockRelease();
		}
		if (!hideIstant)
		{
			callbackReleaseLayer.InvokeSafe();
			callbackReleaseLayer = null;
		}
	}

	public void SetVisibleComponents(string[] ids, bool visible)
	{
		if (ids == null || ids.Length == 0 || components == null)
		{
			return;
		}
		foreach (string key in ids)
		{
			if (components.ContainsKey(key))
			{
				components[key].SetVisible(visible);
			}
		}
	}

	public bool IsAllComponentsVisible(string[] ids)
	{
		foreach (string key in ids)
		{
			if (components.ContainsKey(key) && !components[key].GetVisible())
			{
				return false;
			}
		}
		return true;
	}

	public bool IsAllComponentsInvisible(string[] ids)
	{
		foreach (string key in ids)
		{
			if (components.ContainsKey(key) && components[key].GetVisible())
			{
				return false;
			}
		}
		return true;
	}

	[ContextMenu("Test")]
	private void Test()
	{
		QuestController.Instance.ThrowEvent(ETriggerEvents.EVENT_TUTORIAL_BUTTON_PRESSED, "21");
	}
}
