using System;
using System.Collections;
using Nekki.UI;
using UnityEngine;

public class TooltipUnit : MonoBehaviour
{
	[SerializeField]
	private NekkiUILabel _tooltip;

	[SerializeField]
	private GameObject[] _arrows;

	[SerializeField]
	private UISprite _back;

	private Action _onDestroy;

	public void Init(Vector3 screenPosition, float showTime, Action<NekkiUILabel, UISprite, GameObject> format, Action onDestroy)
	{
		_onDestroy = onDestroy;
		int num = SelectBack(screenPosition);
		_back.enabled = false;
		for (int i = 0; i < 8; i++)
		{
			if (i == num)
			{
				format(_tooltip, _back, _arrows[i]);
			}
			_arrows[i].SetActive(false);
		}
		StartCoroutine(Hide(showTime));
	}

	private int SelectBack(Vector3 screenPosition)
	{
		if (screenPosition.x >= 0f && screenPosition.x < (float)Screen.width * 0.3f)
		{
			if (screenPosition.y >= 0f && screenPosition.y < (float)Screen.height * 0.3f)
			{
				return 5;
			}
			if (screenPosition.y >= (float)Screen.height * 0.3f && screenPosition.y < (float)Screen.height * 0.7f)
			{
				return 3;
			}
			return 0;
		}
		if (screenPosition.x >= (float)Screen.width * 0.3f && screenPosition.x < (float)Screen.width * 0.7f)
		{
			if (screenPosition.y >= 0f && screenPosition.y < (float)Screen.height * 0.5f)
			{
				return 6;
			}
			return 1;
		}
		if (screenPosition.y >= 0f && screenPosition.y < (float)Screen.height * 0.3f)
		{
			return 7;
		}
		if (screenPosition.y >= (float)Screen.height * 0.3f && screenPosition.y < (float)Screen.height * 0.7f)
		{
			return 4;
		}
		return 2;
	}

	private IEnumerator Hide(float showTime)
	{
		yield return new WaitForSeconds(showTime);
		if ((bool)base.gameObject)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (_onDestroy != null)
		{
			_onDestroy();
		}
	}
}
