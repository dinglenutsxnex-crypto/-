using UnityEngine;

public class FloorController : MonoBehaviour
{
	private static FloorController _instance;

	[SerializeField]
	private GameObject[] _floor;

	public static FloorController Instance
	{
		get
		{
			return _instance;
		}
	}

	public static void SetShadows(bool triggerShadow)
	{
		if ((bool)_instance)
		{
			GameObject[] floor = _instance._floor;
			foreach (GameObject gameObject in floor)
			{
				gameObject.layer = LayerMask.NameToLayer((!triggerShadow) ? "Default" : "Ground");
			}
		}
	}

	public bool IsFloor(GameObject go)
	{
		GameObject[] floor = _floor;
		foreach (GameObject gameObject in floor)
		{
			if (gameObject.Equals(go))
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		_instance = this;
		SetShadows(true);
	}
}
