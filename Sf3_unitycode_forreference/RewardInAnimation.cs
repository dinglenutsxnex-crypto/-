using System;
using System.Collections;
using DG.Tweening;
using DOTweenUtils;
using SF3.Audio;
using UnityEngine;

public class RewardInAnimation : MonoBehaviour
{
	public delegate void OnAnimationEnd();

	private const string OPEN_SOUND = "ui/in_fight_labels/fight";

	[SerializeField]
	private UITexture texture;

	[SerializeField]
	private UIWidget darkness;

	[SerializeField]
	private UIWidget header;

	[SerializeField]
	private Material material;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float alphaCorrDuration = 0.25f;

	[SerializeField]
	private float alphaCorrOffset = 0.2f;

	[SerializeField]
	private float fadeDuration = 0.3f;

	[SerializeField]
	private AnimationCurve curve;

	[SerializeField]
	private Vector2 from;

	[SerializeField]
	private Vector2 to;

	[SerializeField]
	private float _debugFrame;

	[SerializeField]
	private string maskTextureName = "_Mask";

	private IEnumerator corutine;

	private float endDarknessValue;

	private Sequence fadeSequence;

	private Sequence scaleSequence;

	private Coroutine firstDrawCallWhait;

	public event OnAnimationEnd onAnimationEnd = delegate
	{
	};

	private void Awake()
	{
		texture.material = UnityEngine.Object.Instantiate(material);
	}

	[ContextMenu("Play")]
	public void Play()
	{
		Material material = GetMaterial();
		material.SetTextureScale(maskTextureName, from);
		material.SetTextureOffset(maskTextureName, CalcOffset(from));
		material.SetFloat("_AlphaCorrection", 0f);
		endDarknessValue = darkness.alpha;
		darkness.alpha = 0f;
		header.alpha = 0f;
		firstDrawCallWhait = StartCoroutine(WaitForFirstDrawCall(delegate
		{
			fadeSequence = GetFadeSequence().AppendCallback(delegate
			{
				scaleSequence = GetScaleSequence();
				scaleSequence.PlayForward();
			});
			fadeSequence.PlayForward();
		}));
	}

	public void Break()
	{
		StopCoroutine(firstDrawCallWhait);
		fadeSequence.Pause();
		scaleSequence.Pause();
		ResetMaskScale();
	}

	private IEnumerator WaitForFirstDrawCall(Action callback)
	{
		yield return new WaitForEndOfFrame();
		callback();
	}

	private Sequence GetScaleSequence()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			AudioManager.Instance.PlaySound("ui/in_fight_labels/fight");
		});
		sequence.Append(texture.DOTiling(to, maskTextureName, duration).SetEase(curve));
		sequence.Join(texture.DOOffset(CalcOffset(to), maskTextureName, duration).SetEase(curve));
		sequence.Join(texture.DOFloat(1f, "_AlphaCorrection", alphaCorrDuration).SetDelay(duration - alphaCorrOffset));
		sequence.AppendCallback(ResetMaskScale);
		sequence.AppendCallback(delegate
		{
			this.onAnimationEnd();
		});
		return sequence;
	}

	private Sequence GetFadeSequence()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(DONgui.Fade(darkness, endDarknessValue, fadeDuration));
		sequence.Join(DONgui.Fade(header, 1f, fadeDuration));
		return sequence;
	}

	private Vector2 CalcOffset(Vector2 scale)
	{
		return new Vector2((1f - scale.x) / 2f, (1f - scale.y) / 2f);
	}

	private void ResetMaskScale()
	{
		texture.material.SetTextureScale(maskTextureName, Vector2.zero);
		texture.material.SetTextureOffset(maskTextureName, Vector2.zero);
		if (texture.drawCall != null)
		{
			texture.drawCall.dynamicMaterial.SetTextureScale(maskTextureName, Vector2.zero);
			texture.drawCall.dynamicMaterial.SetTextureOffset(maskTextureName, Vector2.zero);
		}
	}

	private Material GetMaterial()
	{
		return (!(texture.drawCall != null)) ? texture.material : texture.drawCall.dynamicMaterial;
	}
}
