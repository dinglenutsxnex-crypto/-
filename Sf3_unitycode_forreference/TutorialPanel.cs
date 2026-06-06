using DG.Tweening;
using SF3.TutorialAnimations;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
	[SerializeField]
	private LocalizationText _description;

	[SerializeField]
	private RectTransform _panel;

	private SF3.TutorialAnimations.Animation _currentAnimation;

	private void Awake()
	{
		PauseWindow.OnPauseDisabled += ShowPanel;
		PauseWindow.OnPauseEnabled += HidePanel;
	}

	public void SetAnimation(string animationName, Vector2 offset)
	{
		switch (animationName)
		{
		case "BattleAnimation":
			_currentAnimation = new BattleAnimation(_panel, offset);
			break;
		case "MovingFromCenterAnimation":
			_currentAnimation = new MovingFromCenterAnimation(_panel, offset);
			break;
		default:
			_currentAnimation = new SF3.TutorialAnimations.Animation(_panel, offset);
			break;
		}
	}

	public void PlayInAnimation()
	{
		_currentAnimation.InAnimation();
	}

	private void ShowPanel()
	{
		_description.gameObject.SetActive(true);
		_panel.gameObject.SetActive(true);
	}

	private void HidePanel()
	{
		_description.gameObject.SetActive(false);
		_panel.gameObject.SetActive(false);
	}

	public void PlayOutAnimation(TweenCallback callback)
	{
		_currentAnimation.OutAnimation(callback);
	}

	public void SetMessage(string alias)
	{
		_description.SetAlias(alias);
	}

	public void SetMessage(string alias, string[] args)
	{
		_description.SetAlias(alias, args);
	}

	public void ToogleDescription(Vector3 viewportPosition, Vector2 offset)
	{
		if (viewportPosition.y > 0.75f)
		{
			DescriptionSetToBottom();
		}
		else
		{
			DescriptionSetToUp();
		}
		_panel.anchoredPosition = offset;
	}

	private void DescriptionSetToBottom()
	{
		_panel.anchorMin = new Vector2(0f, 0f);
		_panel.anchorMax = new Vector2(1f, 0f);
		_panel.pivot = new Vector2(0.5f, 0f);
	}

	private void DescriptionSetToUp()
	{
		_panel.anchorMin = new Vector2(0f, 1f);
		_panel.anchorMax = new Vector2(1f, 1f);
		_panel.pivot = new Vector2(0.5f, 1f);
	}

	private void OnDestroy()
	{
		PauseWindow.OnPauseDisabled -= ShowPanel;
		PauseWindow.OnPauseEnabled -= HidePanel;
	}
}
