using Nekki.UI;
using SF3.Items;
using SF3.Settings;
using UnityEngine;

public class ItemReward : MonoBehaviour
{
	[SerializeField]
	private NekkiUISprite sprite;

	public void Init(BaseRewardInfo item)
	{
		sprite.spriteName = item.GetSpriteName();
		SetRarable(item as IRarable);
	}

	private void SetRarable(IRarable item)
	{
		if (item != null)
		{
			sprite.color = GameSettings.ItemSettings.GetRarityColor(item.GetRarityType());
		}
	}
}
