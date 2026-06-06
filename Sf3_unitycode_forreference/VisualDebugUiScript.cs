using System.Collections.Generic;
using UnityEngine;

public class VisualDebugUiScript : MonoBehaviour
{
	private static List<VisualDebugUiScript> _units = new List<VisualDebugUiScript>();

	public UIButton Activate;

	public GameObject CheckMark;

	public UILabel Label;

	public MonoBehaviour Behaviour;

	public static void Clear()
	{
		foreach (VisualDebugUiScript unit in _units)
		{
			Object.Destroy(unit.gameObject);
		}
		_units.Clear();
	}

	public void SetSctipt(MonoBehaviour behaviour)
	{
		_units.Add(this);
		Behaviour = behaviour;
		CheckMark.SetActive(Behaviour.enabled);
		Activate.onClick.Add(new EventDelegate(OnActivate));
		Label.text = behaviour.GetType().ToString();
		base.gameObject.SetActive(true);
	}

	private void OnActivate()
	{
		Behaviour.enabled = !Behaviour.enabled;
		CheckMark.SetActive(Behaviour.enabled);
	}
}
