using System.Collections.Generic;
using Nekki.UI;
using UnityEngine;

public class TutorialBlockNGUI : MonoBehaviour
{
	protected const int reducingTexture = 2;

	private List<TutorialComponent> activeTargets;

	public NekkiUILabel label;

	[SerializeField]
	private UIWidget description;

	[SerializeField]
	private GameObject arrowPrf;

	[SerializeField]
	private UITexture darknessTexture;

	[SerializeField]
	private GameObject maskContainer;

	public float animationDuration;

	private void Awake()
	{
		activeTargets = new List<TutorialComponent>();
		Material material = Object.Instantiate(darknessTexture.material);
		Texture texture = material.GetTexture("_Mask");
		texture.width = Screen.width / 2;
		texture.height = Screen.height / 2;
	}

	public ButtonSelectionNGUI AddSelection(GameObject selector)
	{
		return NewTutorialPointer<ButtonSelectionNGUI>(selector);
	}

	public TutorialArrowNGUI CreateArrow()
	{
		return NewTutorialPointer<TutorialArrowNGUI>(arrowPrf);
	}

	private T NewTutorialPointer<T>(GameObject prefab) where T : TutorialPointer
	{
		GameObject gameObject = Object.Instantiate(prefab);
		gameObject.transform.parent = base.transform;
		T component = gameObject.GetComponent<T>();
		component.Init(animationDuration);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return component;
	}

	public void SetActiveTargets(List<TutorialComponent> targets)
	{
		activeTargets.Clear();
		activeTargets = targets;
	}

	public void Select()
	{
		base.gameObject.SetActive(true);
	}

	public void Release()
	{
		activeTargets.Clear();
		base.gameObject.SetActive(false);
	}

	public UIWidget AddMask(GameObject mask)
	{
		GameObject gameObject = NGUITools.AddChild(maskContainer, mask);
		return gameObject.GetComponent<UIWidget>();
	}

	public void ShowDarkness(bool show)
	{
		darknessTexture.enabled = show;
	}
}
