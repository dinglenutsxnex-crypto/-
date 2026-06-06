using UnityEngine;
using UnityEngine.UI;

public class BadgeUnit : MonoBehaviour
{
	public UserBadgesManager.BadgeTypes Type;

	[SerializeField]
	private UnityEngine.UI.Text _text;

	[SerializeField]
	private UILabel _label;

	private void Awake()
	{
		UserBadgesManager.Instance.RegisterUnit(this);
	}

	private void OnDestroy()
	{
		if ((bool)UserBadgesManager.Instance)
		{
			UserBadgesManager.Instance.UnregisterUnit(this);
		}
	}

	public void Refresh(long badgeNum)
	{
		base.gameObject.SetActive(badgeNum != 0);
		if (_text != null)
		{
			_text.text = badgeNum.ToString();
		}
		else
		{
			_label.text = badgeNum.ToString();
		}
	}
}
