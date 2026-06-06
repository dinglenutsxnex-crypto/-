using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebugUI : UIModuleHolder
{
	public class Unit
	{
		private bool expand;

		public int iteration;

		public Transform VisualObject;

		public Transform transform;

		public List<MonoBehaviour> scripts = new List<MonoBehaviour>();

		public List<Unit> childs = new List<Unit>();

		public bool isRoot
		{
			get
			{
				return (bool)transform && transform.parent == null;
			}
		}

		public Unit(Transform root)
		{
			transform = root;
			scripts = new List<MonoBehaviour>(transform.GetComponents<MonoBehaviour>());
		}

		public void DrawScripts(UIScrollView scrollView, VisualDebugUiScript baseUnit)
		{
			foreach (MonoBehaviour script in scripts)
			{
				VisualDebugUiScript component = NGUITools.AddChild(scrollView.gameObject, baseUnit.gameObject).GetComponent<VisualDebugUiScript>();
				component.SetSctipt(script);
			}
		}

		public void Show(UIScrollView scrollView, VisualDebugUiUnit baseUnit, string parentName, int iteretion, int order)
		{
			if ((bool)VisualObject)
			{
				Object.Destroy(VisualObject);
			}
			iteration = iteretion;
			VisualDebugUiUnit component = NGUITools.AddChild(scrollView.gameObject, baseUnit.gameObject).GetComponent<VisualDebugUiUnit>();
			component.lblUnitName.text = transform.name;
			component.name = string.Format("{0}_{1}", parentName, order);
			component.gameObject.SetActive(true);
			VisualObject = component.transform;
			component.lblUnitName.color = ((!transform.gameObject.activeSelf) ? Color.gray : Color.black);
			if (childs.Count == 0)
			{
				component.btnExpand.gameObject.SetActive(false);
				component.btnUnExpand.gameObject.SetActive(false);
			}
			else if (expand)
			{
				component.btnExpand.gameObject.SetActive(false);
				component.btnUnExpand.gameObject.SetActive(true);
			}
			else
			{
				component.btnExpand.gameObject.SetActive(true);
				component.btnUnExpand.gameObject.SetActive(false);
			}
			component.BroadcastMessage("SetUnit", this, SendMessageOptions.DontRequireReceiver);
			if (expand)
			{
				for (int i = 0; i < childs.Count; i++)
				{
					Unit unit = childs[i];
					unit.Show(scrollView, baseUnit, component.name, iteretion + 1, i);
				}
			}
		}

		public void Expand()
		{
			expand = !expand;
		}

		public void Init(Dictionary<int, Unit> unitsDict)
		{
			childs.Clear();
			foreach (KeyValuePair<int, Unit> item in unitsDict)
			{
				if (item.Value.transform.parent == transform)
				{
					childs.Add(item.Value);
				}
			}
		}
	}

	private List<Unit> _units = new List<Unit>();

	private Dictionary<int, Unit> _unitsDict = new Dictionary<int, Unit>();

	private static Unit _current;

	[SerializeField]
	private UIWrapContent _wrapObjects;

	[SerializeField]
	private UIWrapContent _wrapScripts;

	[SerializeField]
	private UIScrollView _scrollViewObjects;

	[SerializeField]
	private UIScrollView _scrollViewScripts;

	[SerializeField]
	private VisualDebugUiUnit _baseUnitObjects;

	[SerializeField]
	private VisualDebugUiScript _baseUnitScripts;

	[SerializeField]
	private UIButton _btnRefresh;

	[SerializeField]
	private UIButton _btnClose;

	[SerializeField]
	private VisualDebugUiSelected _currenUI;

	public static VisualDebugUI Instance { get; private set; }

	private void RefreshUnits()
	{
		_units.Clear();
		if (_current != null && _current.transform == null)
		{
			_current = null;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, Unit> item in _unitsDict)
		{
			if (!item.Value.transform)
			{
				list.Add(item.Key);
			}
		}
		foreach (int item2 in list)
		{
			_unitsDict.Remove(item2);
		}
		List<Transform> objects = GetObjects();
		foreach (Transform item3 in objects)
		{
			if (!_unitsDict.ContainsKey(item3.GetInstanceID()) && item3 != base.gameObject.transform)
			{
				_unitsDict.Add(item3.GetInstanceID(), new Unit(item3));
			}
		}
		foreach (KeyValuePair<int, Unit> item4 in _unitsDict)
		{
			item4.Value.Init(_unitsDict);
		}
		foreach (Unit value in _unitsDict.Values)
		{
			if (value.isRoot)
			{
				_units.Add(value);
			}
		}
		Redraw();
	}

	private List<Transform> GetObjects()
	{
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		List<GameObject> list = new List<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.transform.parent == null)
			{
				list.Add(gameObject);
			}
		}
		List<Transform> list2 = new List<Transform>();
		foreach (GameObject item in list)
		{
			list2.Add(item.transform);
			list2.AddRange(item.GetComponentsInChildren<Transform>(true));
		}
		return list2;
	}

	private void Start()
	{
		_currenUI.gameObject.SetActive(false);
		_currenUI.Activate.onClick.Add(new EventDelegate(ActivateClick));
		Instance = this;
		RefreshUnits();
		_baseUnitObjects.gameObject.SetActive(false);
		_baseUnitScripts.gameObject.SetActive(false);
		_btnRefresh.onClick.Add(new EventDelegate(RefreshUnits));
		_btnClose.onClick.Add(new EventDelegate(Close));
	}

	public static void Open()
	{
		if ((bool)Instance && (bool)Instance.gameObject)
		{
			Instance.gameObject.SetActive(true);
		}
		else
		{
			NekkiUIRootModules.Instance.MountNGUIModule("VisualDebug");
		}
	}

	public static void Close()
	{
		if ((bool)Instance || !Instance.gameObject)
		{
			Instance.gameObject.SetActive(false);
		}
	}

	private void ActivateClick()
	{
		if (_current != null && (bool)_current.transform)
		{
			_current.transform.gameObject.SetActive(!_current.transform.gameObject.activeSelf);
			_currenUI.CheckMark.SetActive((bool)_current.transform && _current.transform.gameObject.activeSelf);
			_current.VisualObject.GetComponentInChildren<UILabel>().color = ((!_current.transform || !_current.transform.gameObject.activeSelf) ? Color.gray : Color.black);
		}
	}

	public void OnSelect(Unit unit)
	{
		_current = unit;
		_currenUI.Label.text = unit.transform.name;
		_currenUI.gameObject.SetActive(true);
		_currenUI.CheckMark.SetActive((bool)_current.transform && _current.transform.gameObject.activeSelf);
		VisualDebugUiScript.Clear();
		if (_current != null)
		{
			_current.DrawScripts(_scrollViewScripts, _baseUnitScripts);
		}
		StartCoroutine(SortNextFrame());
	}

	public void OnExpand(Unit unit)
	{
		unit.Expand();
		Redraw();
	}

	public void Redraw()
	{
		VisualDebugUiUnit.Clear();
		VisualDebugUiScript.Clear();
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].Show(_scrollViewObjects, _baseUnitObjects, string.Empty, 0, i);
		}
		if (_current != null)
		{
			_current.DrawScripts(_scrollViewScripts, _baseUnitScripts);
		}
		StartCoroutine(SortNextFrame());
	}

	private IEnumerator SortNextFrame()
	{
		yield return new WaitForEndOfFrame();
		_wrapObjects.SortAlphabetically();
		_wrapScripts.SortAlphabetically();
		foreach (Unit value in _unitsDict.Values)
		{
			if ((bool)value.VisualObject)
			{
				value.VisualObject.localPosition += new Vector3(value.iteration * 10, 0f, 0f);
			}
		}
	}
}
