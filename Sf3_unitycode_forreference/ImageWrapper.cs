using UnityEngine;
using UnityEngine.UI;

public class ImageWrapper : Image
{
	public new Sprite sprite
	{
		get
		{
			return base.sprite;
		}
		set
		{
			if (value != null)
			{
				if (base.sprite != null && !base.sprite.name.Equals(value.name))
				{
					TexturesUtils.ReleaseTexture(sprite.texture);
				}
				base.sprite = value;
				TexturesUtils.AddTexture(value.texture);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (sprite != null)
		{
			TexturesUtils.AddTexture(sprite.texture);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (sprite != null)
		{
			TexturesUtils.ReleaseTexture(sprite.texture);
		}
	}
}
