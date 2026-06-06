using System.Collections;
using UnityEngine;

public class Object_Start_Name : MonoBehaviour
{
	public float StartTime;

	public GameObject myObject;

	private bool ifStart;

	private void Start()
	{
		StartCoroutine(Example());
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(StartTime);
		ifStart = true;
	}

	private void Update()
	{
		if (ifStart)
		{
			myObject.gameObject.SetActive(true);
		}
	}
}
