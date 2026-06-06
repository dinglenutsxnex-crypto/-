using SF3;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
	private EnterPoint _enterPoint;

	public GameObject NekkiLogo;

	internal void Awake()
	{
		NekkiLogo.SetActive(true);
	}

	internal void Start()
	{
		Sf3ConsoleCommands.AddCommands();
		_enterPoint = new EnterPoint();
		_enterPoint.Init();
	}

	internal void OnDestroy()
	{
		Debug.Log("Destroy GameLoad " + NekkiLogo.GetInstanceID());
		GlobalLoad.Unload(NekkiLogo);
	}
}
