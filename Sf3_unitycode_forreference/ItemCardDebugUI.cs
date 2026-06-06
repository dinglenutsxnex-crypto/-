using SF3.Items;
using UnityEngine;

public class ItemCardDebugUI : MonoBehaviour
{
	[SerializeField]
	private ReelItem _reelItem;

	[SerializeField]
	private UILabel _idLable;

	[SerializeField]
	private UILabel _idText;

	[SerializeField]
	private UILabel _slLable;

	[SerializeField]
	private UILabel _slText;

	private GameObject _gameObject;

	[SerializeField]
	private UIWidget _parentDepthWidget;

	private void Awake()
	{
		_gameObject = base.gameObject;
		if (InternalSettings.Local.Debug)
		{
			_reelItem.OnItemUpdate += UpdateItem;
			UpdateItem(_reelItem.Item);
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Start()
	{
		_gameObject.layer = _reelItem.gameObject.layer;
	}

	private void UpdateDepth()
	{
		_idLable.depth = _parentDepthWidget.depth + 1;
		_idText.depth = _parentDepthWidget.depth + 1;
		_slLable.depth = _parentDepthWidget.depth + 1;
		_slText.depth = _parentDepthWidget.depth + 1;
	}

	private void UpdateItem(BaseItem it)
	{
		if (it != null)
		{
			SetIDText(it.id.ToString());
			IStackable stackable = it as IStackable;
			if (stackable != null)
			{
				SetSLText(stackable.GetStackLevel().ToString());
			}
			UpdateDepth();
		}
	}

	public void SetIDText(string text)
	{
		_idText.text = text;
	}

	public void SetSLText(string text)
	{
		_slText.text = text;
	}

	public void Show()
	{
		_gameObject.SetActive(true);
	}

	public void Hide()
	{
		_gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		_reelItem.OnItemUpdate -= UpdateItem;
	}
}
