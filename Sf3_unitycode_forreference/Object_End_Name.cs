using System.Collections;
using UnityEngine;

public class Object_End_Name : MonoBehaviour
{
	public float EndTime;

	public GameObject myObject;

	private bool ifStart;

	private void Start()
	{
		StartCoroutine(Example());
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(EndTime);
		ifStart = true;
	}

	private void Update()
	{
		if (ifStart)
		{
			myObject.gameObject.SetActive(false);
		}
	}
}
