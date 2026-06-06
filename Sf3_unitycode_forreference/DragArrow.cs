using UnityEngine;

public class DragArrow : MonoBehaviour
{
	public TutorialComponent dragable;

	public TutorialComponent target;

	[SerializeField]
	private TweenPosition tween;

	[SerializeField]
	private TweenAlpha tweenAplha;

	[SerializeField]
	private UISprite sprite;

	private void Update()
	{
		float num = Mathf.Atan2(tween.to.x - tween.from.x, tween.to.y - tween.from.y) * 57.29578f;
		sprite.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - num);
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
		if (visible)
		{
			tween.ResetToBeginning();
			tweenAplha.ResetToBeginning();
			Debug.Log("SHOW");
		}
	}
}
