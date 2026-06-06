using System.Collections.Generic;
using UnityEngine;

public class ObjectDebug : MonoBehaviour
{
	private class Unit
	{
		private bool expand;

		public Transform transform;

		public List<MonoBehaviour> scripts = new List<MonoBehaviour>();

		public List<Unit> childs = new List<Unit>();

		private static readonly Dictionary<int, string> Offsets = new Dictionary<int, string>();

		public Unit(Transform root)
		{
			transform = root;
			scripts = new List<MonoBehaviour>(transform.GetComponents<MonoBehaviour>());
			for (int i = 0; i < root.childCount; i++)
			{
				childs.Add(new Unit(root.GetChild(i)));
			}
		}

		public void Draw(int iteration)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(GetOffset(iteration));
			if (childs.Count > 0 && GUILayout.Button((!expand) ? "+" : "-", GUILayout.MaxWidth(20f)))
			{
				expand = !expand;
			}
			if (_current != this)
			{
				if (GUILayout.Button(">", GUILayout.MaxWidth(20f)))
				{
					_current = this;
				}
			}
			else
			{
				GUILayout.Label(">", GUILayout.MaxWidth(20f));
			}
			if (GUILayout.Button((!transform.gameObject.activeSelf) ? "^" : "x", GUILayout.MaxWidth(20f)))
			{
				transform.gameObject.SetActive(!transform.gameObject.activeSelf);
			}
			GUILayout.Label(transform.name, GUILayout.MinWidth(250f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if (expand)
			{
				for (int i = 0; i < childs.Count; i++)
				{
					childs[i].Draw(iteration + 1);
				}
			}
		}

		public void DrawScripts()
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button((!transform.gameObject.activeSelf) ? "^" : "x", GUILayout.MaxWidth(20f)))
			{
				transform.gameObject.SetActive(!transform.gameObject.activeSelf);
			}
			GUILayout.Label(transform.name, GUILayout.MinWidth(250f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			foreach (MonoBehaviour script in scripts)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button((!script.enabled) ? "^" : "x", GUILayout.MaxWidth(20f)))
				{
					script.enabled = !script.enabled;
				}
				GUILayout.Label(script.GetType().ToString(), GUILayout.MinWidth(250f));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		private string GetOffset(int iteration)
		{
			if (!Offsets.ContainsKey(iteration))
			{
				string text = "__";
				for (int i = 0; i < iteration; i++)
				{
					text += "_|_";
				}
				Offsets.Add(iteration, text);
			}
			return Offsets[iteration];
		}
	}

	private const int WINDOW = 33;

	private List<Unit> _units = new List<Unit>();

	private static Unit _current;

	private Rect _windowPosition = new Rect(0f, 0f, Screen.width, Screen.height);

	private Vector2 _scroll1 = Vector2.zero;

	private Vector2 _scroll2 = Vector2.zero;

	private void GetUnits()
	{
		_units.Clear();
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.transform.parent == null && (bool)gameObject.transform && gameObject != base.gameObject)
			{
				_units.Add(new Unit(gameObject.transform));
			}
		}
	}

	private void Draw()
	{
		_windowPosition = GUI.Window(33, _windowPosition, Window, "object debug");
	}

	private void Window(int id)
	{
		if (GUI.Button(new Rect(_windowPosition.width - 30f, 5f, 25f, 25f), "X"))
		{
			Object.Destroy(base.gameObject);
		}
		_scroll1 = GUI.BeginScrollView(new Rect(5f, 20f, _windowPosition.width / 2f, _windowPosition.height - 25f), _scroll1, new Rect(0f, 0f, _windowPosition.width - 100f, 2000f));
		GUILayout.BeginVertical();
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].Draw(0);
		}
		GUILayout.EndVertical();
		GUI.EndScrollView();
		if (_current != null)
		{
			_scroll2 = GUI.BeginScrollView(new Rect(10f + _windowPosition.width / 2f, 20f, _windowPosition.width / 2f - 15f, _windowPosition.height - 25f), _scroll2, new Rect(0f, 0f, _windowPosition.width - 100f, 2000f));
			GUILayout.BeginVertical();
			_current.DrawScripts();
			GUILayout.EndVertical();
			GUI.EndScrollView();
		}
		GUI.DragWindow(new Rect(0f, 0f, _windowPosition.width, _windowPosition.height));
	}

	private void Start()
	{
		_current = null;
		GetUnits();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		Draw();
	}
}
