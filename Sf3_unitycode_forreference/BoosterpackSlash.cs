using DG.Tweening;
using DOTweenUtils;
using UnityEngine;

public class BoosterpackSlash : MonoBehaviour
{
	private const float appearFadeDuration = 0.2f;

	private const float disappearFadeDuration = 1.2f;

	private const float afterexplosionFadeDuration = 0.1f;

	private const float timeToShowSwipeTrail = 0.4f;

	[SerializeField]
	private UIWidget swipeTrail;

	[SerializeField]
	private UIWidget fier;

	[SerializeField]
	private UIWidget fierTop;

	public void SetFierPositionAndSize(Vector3 cutInPosition, Vector3 cutOutPosition)
	{
		Vector3 vector = cutOutPosition - cutInPosition;
		fier.transform.position = new Vector3(cutInPosition.x + vector.x / 2f, cutInPosition.y + vector.y / 2f);
		Vector3 a = base.transform.parent.transform.InverseTransformPoint(cutInPosition);
		Vector3 b = base.transform.parent.transform.InverseTransformPoint(cutOutPosition);
		fier.width = (int)Vector3.Distance(a, b);
		fierTop.width = fier.width;
	}

	public void PlaySwipeTrailAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		swipeTrail.alpha = 0f;
		sequence.Append(DONgui.Fade(swipeTrail, 1f, 0.2f).SetEase(Ease.InCubic));
		sequence.AppendInterval(0.4f);
		sequence.Append(DONgui.Fade(swipeTrail, 0f, 1.2f).SetEase(Ease.OutCubic));
		sequence.Play();
	}

	public void PlayBeforeExplosionAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		swipeTrail.alpha = 0f;
		sequence.Append(DONgui.Fade(fier, 0f, 0.1f));
		sequence.Join(DONgui.Fade(swipeTrail, 0f, 0.1f));
		sequence.Play();
	}
}
