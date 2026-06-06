using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using Nekki.UI;
using UnityEngine;

public class ShopAttributeDiff : MonoBehaviour
{
	private NekkiUILabel _label;

	private UIWidget _widget;

	private List<float> _data;

	private string _key;

	private float _showDuration;

	private void Init()
	{
		_label = GetComponent<NekkiUILabel>();
		_widget = GetComponent<UIWidget>();
	}

	public void SetData(List<float> data, float duration = 0f)
	{
		_data = data;
		_key = null;
		_showDuration = duration;
		SetDataOuter();
	}

	public void Clear()
	{
		_label.text = string.Empty;
		_label.Alias = string.Empty;
	}

	public void SetDataFromLocalization(string key, float duration = 0f)
	{
		_key = key;
		_data = null;
		_showDuration = duration;
		SetDataOuter();
	}

	private void Awake()
	{
		if (_label == null)
		{
			Init();
		}
	}

	private void SetDataOuter()
	{
		if (_showDuration > 0f)
		{
			DONgui.Fade(_widget, 0f, _showDuration).OnComplete(SetDataInner);
		}
		else
		{
			SetDataInner();
		}
	}

	private void SetDataInner()
	{
		Clear();
		if (_data != null)
		{
			bool flag = true;
			foreach (float datum in _data)
			{
				NekkiUILabel label = _label;
				label.text = label.text + (flag ? string.Empty : "|") + ((!(datum > 0f)) ? string.Empty : "+") + datum;
				flag = false;
			}
		}
		else
		{
			if (_key == null)
			{
				return;
			}
			_label.Alias = _key;
		}
		if (_showDuration > 0f)
		{
			DONgui.Fade(_widget, 0f, 1f, _showDuration);
		}
		else
		{
			_widget.alpha = 1f;
		}
	}
}
