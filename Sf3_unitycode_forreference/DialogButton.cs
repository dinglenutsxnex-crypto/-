using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogButton : MonoBehaviour
{
	[SerializeField]
	private UnityEngine.UI.Text _label;

	[SerializeField]
	private Image _arrow;

	[SerializeField]
	private Button _button;

	private LocalizationText _localizationText;

	private void Awake()
	{
		_localizationText = _label.gameObject.GetComponent<LocalizationText>();
		if (_localizationText == null)
		{
			Debug.LogError("Localization component not found");
		}
	}

	public void SetLabel(string alias, bool hasArrow = true)
	{
		_localizationText.SetAlias(alias);
		_arrow.gameObject.SetActive(hasArrow);
		RectTransform component = GetComponent<RectTransform>();
		Vector2 sizeDelta = component.sizeDelta;
		sizeDelta.x = _arrow.GetComponent<RectTransform>().rect.width + _label.preferredWidth;
		component.sizeDelta = sizeDelta;
	}

	public void SetColor(Color? color)
	{
		if (color.HasValue)
		{
			_label.color = color.Value;
			if (_arrow.gameObject.activeSelf)
			{
				_arrow.color = color.Value;
			}
		}
	}

	public void AddCallback(UnityAction callback)
	{
		_button.onClick.AddListener(callback);
	}

	public void AddCallback(UnityAction<string, object> callback, object arg)
	{
		_button.onClick.AddListener(delegate
		{
			callback(_localizationText.GetAlias(), arg);
		});
	}
}
