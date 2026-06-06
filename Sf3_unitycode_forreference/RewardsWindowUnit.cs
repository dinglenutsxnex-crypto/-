using SF3.Items;
using UnityEngine;

public class RewardsWindowUnit : MonoBehaviour
{
	[SerializeField]
	private UILabel _label;

	[SerializeField]
	private UISprite _sprite;

	[SerializeField]
	private CardItem _baseItem;

	private CardItem _item;

	public void Set(int line, int index, int itemsCount, string text, string sprite)
	{
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(sprite))
		{
			_label.text = text;
			_sprite.spriteName = sprite;
			UpdatePosition(line, index, itemsCount);
		}
		else
		{
			Empty();
		}
	}

	public void Set(int line, int index, int itemsCount, BaseItem item)
	{
		if (item != null)
		{
			_item = Object.Instantiate(_baseItem);
			_item.transform.parent = base.transform;
			_item.gameObject.SetActive(true);
			_item.Init(item);
			_item.transform.localScale = Vector3.one;
			base.transform.localScale = Vector3.one * 0.8f;
			_item.Appear();
			_item.UpdateDepth(5);
			_item.UpdateShade(0f);
			_item.transform.localPosition = Vector3.zero;
			_label.text = string.Empty;
			_sprite.spriteName = string.Empty;
			UpdatePosition(line, index, itemsCount);
		}
		else
		{
			Empty();
		}
	}

	private void Empty()
	{
		Object.Destroy(base.gameObject);
	}

	private void UpdatePosition(int line, int index, int itemsCount)
	{
		float y = (float)line * -200f;
		float x = ((float)index - ((float)(itemsCount / 2) - ((itemsCount % 2 != 0) ? 0f : 0.5f))) * 210f;
		base.transform.localPosition = new Vector3(x, y, 0f);
	}
}
