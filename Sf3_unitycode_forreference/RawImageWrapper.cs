using UnityEngine;
using UnityEngine.UI;

public class RawImageWrapper : RawImage
{
	public new Texture texture
	{
		get
		{
			return base.texture;
		}
		set
		{
			if (value != null)
			{
				if (base.texture != null && !base.texture.name.Equals(value.name))
				{
					TexturesUtils.ReleaseTexture(texture);
				}
				base.texture = value;
				TexturesUtils.AddTexture(texture);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (texture != null)
		{
			TexturesUtils.AddTexture(texture);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (texture != null)
		{
			TexturesUtils.ReleaseTexture(texture);
		}
	}
}
