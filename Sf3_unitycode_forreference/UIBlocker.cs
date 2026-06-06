using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UIBlocker : MonoBehaviour
{
	public enum Priority
	{
		Min = 1,
		Dialogs = 100,
		Feedback = 200,
		Preloader = 300,
		LoginEmailDialog = 350,
		ServerSystemAlert = 400,
		Max = 1000
	}

	private static UIBlocker _instance;

	private static SortedDictionary<int, List<BlockIgnoreObject>> _ignoredObjects = new SortedDictionary<int, List<BlockIgnoreObject>>();

	private List<UIPanel> _ignoredPanels = new List<UIPanel>();

	private bool _inited;

	private int _blockCounter;

	public static UIBlocker Instance
	{
		get
		{
			if (_instance == null)
			{
				NekkiUIRootModules.Instance.MountNativeModule("UIBlocker");
			}
			return _instance;
		}
	}

	public bool IsLocked
	{
		get
		{
			return _blockCounter > 0;
		}
	}

	private void Awake()
	{
		_instance = this;
		Init();
	}

	private void Init()
	{
		if (!_inited)
		{
			AddTriggerEntry(EventTriggerType.PointerDown, ExecuteEvents.pointerDownHandler);
			AddTriggerEntry(EventTriggerType.PointerUp, ExecuteEvents.pointerUpHandler);
			AddTriggerEntry(EventTriggerType.PointerUp, ExecuteEvents.dropHandler);
			AddTriggerEntry(EventTriggerType.PointerClick, ExecuteEvents.pointerClickHandler);
			AddTriggerEntry(EventTriggerType.Drag, ExecuteEvents.dragHandler);
			AddTriggerEntry(EventTriggerType.Scroll, ExecuteEvents.scrollHandler);
			AddTriggerEntry(EventTriggerType.InitializePotentialDrag, ExecuteEvents.initializePotentialDrag);
			AddTriggerEntry(EventTriggerType.BeginDrag, ExecuteEvents.beginDragHandler);
			AddTriggerEntry(EventTriggerType.EndDrag, ExecuteEvents.endDragHandler);
			_inited = true;
		}
	}

	private void AddTriggerEntry<T>(EventTriggerType type, ExecuteEvents.EventFunction<T> func) where T : IEventSystemHandler
	{
		EventTrigger component = GetComponent<EventTrigger>();
		EventTrigger.Entry entry = component.triggers.Find((EventTrigger.Entry x) => x.eventID == type);
		if (entry == null)
		{
			entry = new EventTrigger.Entry();
			entry.eventID = type;
			component.triggers.Add(entry);
		}
		entry.callback.AddListener(delegate(BaseEventData data)
		{
			Execute(data as PointerEventData, func);
		});
	}

	private void Update()
	{
		foreach (BlockIgnoreObject currentIgnoredObject in GetCurrentIgnoredObjects())
		{
			currentIgnoredObject.CheckPointerEnterOrExit(Input.mousePosition);
		}
	}

	private void Execute<T>(PointerEventData data, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
	{
		BlockIgnoreObject[] array = GetCurrentIgnoredObjects().ToArray();
		foreach (BlockIgnoreObject blockIgnoreObject in array)
		{
			blockIgnoreObject.Execute(data, functor);
		}
	}

	private void AddIgnoreObjects(List<GameObject> ignoredObjs, int priorityIdx)
	{
		foreach (GameObject ignoredObj in ignoredObjs)
		{
			_ignoredObjects[priorityIdx].Add(new BlockIgnoreObject(ignoredObj));
		}
	}

	public void AddIgnoreObject(UIPanel panel)
	{
		_ignoredPanels.Add(panel);
		ScreenTexture.Instance.AddOverlayPanel(panel);
	}

	private List<BlockIgnoreObject> GetCurrentIgnoredObjects()
	{
		List<BlockIgnoreObject> list = _ignoredObjects.Values.LastOrDefault();
		if (list == null)
		{
			list = new List<BlockIgnoreObject>();
		}
		return list;
	}

	private int GetCurrentPriority()
	{
		return _ignoredObjects.Keys.LastOrDefault();
	}

	private void ClearIgnoreObjects(int priorityIdx)
	{
		_ignoredObjects.Remove(priorityIdx);
	}

	private void ClearNGUI()
	{
		foreach (UIPanel ignoredPanel in _ignoredPanels)
		{
			ScreenTexture.Instance.RemoveOverlayPanel(ignoredPanel);
		}
		_ignoredPanels.Clear();
	}

	public void Clear()
	{
		_ignoredObjects.Clear();
		ClearNGUI();
		_blockCounter = 0;
	}

	public void Block(List<GameObject> ignoredObj, Priority priority = Priority.Min)
	{
		int num = FindFreeIndex(priority);
		_ignoredObjects[num] = new List<BlockIgnoreObject>();
		AddIgnoreObjects(ignoredObj, num);
		_blockCounter++;
		base.gameObject.SetActive(true);
		ScreenTexture.Instance.BlockScreen(base.name);
	}

	public void Block(GameObject ignoredObj, Priority priority = Priority.Min)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(ignoredObj);
		List<GameObject> ignoredObj2 = list;
		Block(ignoredObj2, priority);
	}

	public void Block(Priority priority = Priority.Min)
	{
		List<GameObject> ignoredObj = new List<GameObject>();
		Block(ignoredObj, priority);
	}

	public void BlockNGUI()
	{
		ScreenTexture.Instance.BlockScreen(base.name);
	}

	public void Unblock(Priority priority = Priority.Min)
	{
		if (!_ignoredObjects.ContainsKey((int)priority))
		{
			Debug.LogWarning("Unblock empty layer");
			return;
		}
		_blockCounter--;
		if (_blockCounter < 0)
		{
			Debug.LogWarning("More Unblock called than Block");
			_blockCounter = 0;
			return;
		}
		int priorityIdx = FindFreeIndex(priority) - 1;
		ClearIgnoreObjects(priorityIdx);
		if (_blockCounter == 0)
		{
			ClearNGUI();
			base.gameObject.SetActive(false);
			ScreenTexture.Instance.UnBlockScreen(base.name);
		}
	}

	public void UnblockNGUI()
	{
		if (_blockCounter == 0)
		{
			ScreenTexture.Instance.UnBlockScreen(base.name);
		}
	}

	private int FindFreeIndex(Priority priority)
	{
		int i;
		for (i = (int)priority; _ignoredObjects.ContainsKey(i); i++)
		{
		}
		return i;
	}
}
