using Nekki.UI;
using UnityEngine;
using sf3DTO;

public class CountableReward : MonoBehaviour
{
	[SerializeField]
	private float _sizeScaleKoef = 0.38f;

	[SerializeField]
	private NekkiUISprite _sprite;

	[SerializeField]
	private NekkiUILabel _value;

	[SerializeField]
	private NekkiUILabel _label;

	[SerializeField]
	private int _labelOffset = 12;

	[SerializeField]
	private string _expSpriteName;

	private void Init(string spriteName, string val)
	{
		_sprite.spriteName = spriteName;
		_value.text = val;
		_label.gameObject.SetActive(false);
		ResizeSprite();
		RepositionValue(_sprite);
	}

	public void InitCurrency(Currency currency)
	{
		string spriteName = currency.CurrencyType.ToString().ToLower();
		string val = currency.Value.ToString();
		Init(spriteName, val);
	}

	public void InitExp(float exp)
	{
		Init(_expSpriteName, exp.ToString());
	}

	private void RepositionValue(UIWidget baseWidget)
	{
		float num = (float)baseWidget.width / 2f;
		float num2 = (float)_value.width / 2f;
		Vector3 localPosition = _value.transform.localPosition;
		localPosition.x = _label.transform.position.x + num + num2 + (float)_labelOffset;
		_value.transform.localPosition = localPosition;
	}

	private void ResizeSprite()
	{
		UISpriteData sprite = _sprite.atlas.GetSprite(_sprite.spriteName);
		if (sprite != null)
		{
			_sprite.width = (int)((float)sprite.width * _sizeScaleKoef);
			_sprite.height = (int)((float)sprite.height * _sizeScaleKoef);
		}
	}
}
