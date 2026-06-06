using System;
using UnityEngine;

public class BattleBadgeUnit : MonoBehaviour
{
	[SerializeField]
	private UISprite _sprite;

	[SerializeField]
	private UILabel _label;

	public int Id { get; private set; }

	public event Action<bool> onUpdateState;

	private void OnDestroy()
	{
		if ((bool)UserBadgesManager.Instance)
		{
			UserBadgesManager.Instance.UnregisterUnit(this);
		}
	}

	public void SetId(int idBattle)
	{
		Id = idBattle;
		UserBadgesManager.Instance.RegisterUnit(this);
	}

	public void Refresh(bool show)
	{
		base.gameObject.SetActive(show);
		this.onUpdateState(show);
	}

	public void SetDepth(int depth)
	{
		_sprite.depth = depth;
		_label.depth = depth + 1;
	}
}
