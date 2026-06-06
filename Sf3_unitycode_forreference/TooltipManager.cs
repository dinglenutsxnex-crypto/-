using System;
using System.Collections;
using Nekki.UI;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
	[SerializeField]
	private TooltipUnit _prototype;

	private TooltipUnit _currentTip;

	private NekkiTooltip _currentTooltipHolder;

	private bool _inProcess;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !_inProcess)
		{
			StartCoroutine(LazyTooltip());
		}
	}

	private IEnumerator LazyTooltip()
	{
		_inProcess = true;
		UICamera.Raycast(Input.mousePosition);
		if (UICamera.lastRaycastedCollider != null)
		{
			NekkiTooltip nekkiTooltipObj = UICamera.lastRaycastedCollider.gameObject.GetComponent<NekkiTooltip>();
			if (nekkiTooltipObj != null && nekkiTooltipObj != _currentTooltipHolder)
			{
				if (_currentTip != null)
				{
					HideTip();
					while (_currentTip != null)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				_currentTooltipHolder = nekkiTooltipObj;
				UIWidget tooltipWidget = _currentTooltipHolder.GetComponent<UIWidget>();
				if (tooltipWidget != null && tooltipWidget.alpha > 0.1f)
				{
					yield return StartCoroutine(ShowTip(NekkiUIRoot.Camera.WorldToScreenPoint(tooltipWidget.transform.position), _currentTooltipHolder.ShowTime, _currentTooltipHolder.Format));
				}
			}
		}
		yield return new WaitForEndOfFrame();
		_inProcess = false;
	}

	private IEnumerator ShowTip(Vector3 tippose, float showTime, Action<NekkiUILabel, UISprite, GameObject> format)
	{
		_currentTip = UnityEngine.Object.Instantiate(_prototype, new Vector3(-10000f, -10000f), Quaternion.identity);
		_currentTip.Init(tippose, showTime, format, null);
		for (int i = 0; i < 4; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		Vector3 pos = NekkiUIRoot.Camera.ScreenToWorldPoint(tippose);
		_currentTip.transform.parent = NekkiUIRoot.Instance.GetAnchor(UIAnchor.Side.Center);
		_currentTip.transform.localScale = Vector3.one;
		_currentTip.transform.position = pos;
	}

	private void HideTip()
	{
		if (_currentTip != null)
		{
			UnityEngine.Object.Destroy(_currentTip.gameObject);
			_currentTip = null;
			_currentTooltipHolder = null;
		}
	}
}
