using System;
using System.Collections;
using UnityEngine;

public class TestLoad : ExtentionBehaviour
{
	private static string baseServerAddress = string.Empty;

	public static void GetImage(string name, Action<object> onDone, Action<Exception> onError)
	{
	}

	private IEnumerator Load(string data, Action<object> onDone, Action<Exception> onError)
	{
		yield return new WWW(string.Format("{0}/{1}", baseServerAddress, data));
	}

	internal void Start()
	{
	}

	internal void Update()
	{
	}
}
