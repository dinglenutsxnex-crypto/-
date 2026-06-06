using UnityEngine;

public class TutorialBlockNative : MonoBehaviour
{
	protected const int REDUCING_TEXTURE = 2;

	protected const int TRANSPARENT_RENDER_QUEUE = 3000;

	public LocalizationText localizationLabel;

	[SerializeField]
	private RectTransform _description;

	[SerializeField]
	private GameObject _arrowPrf;

	[SerializeField]
	private GameObject _maskContainer;

	[SerializeField]
	private Camera _maskCamera;

	[SerializeField]
	private ImageWrapper _darknessTexture;

	[SerializeField]
	private GameObject _maskGeneratorPrf;

	private TutorialMaskGenerator _maskGenerator;

	public float animationDuration;

	private RenderTexture _darknessMask;

	private void Awake()
	{
		_maskGenerator = TutorialMaskGenerator.Instance;
		Material material = new Material(Shader.Find("UI/Alpha Mask"));
		material.SetTexture("_Mask", _maskGenerator.GetTexture());
		material.SetColor("_Color", _darknessTexture.color);
		material.renderQueue = 3000;
		_darknessTexture.material = material;
	}

	public ButtonSelectionNative AddSelection(GameObject selector)
	{
		return NewTutorialPointer<ButtonSelectionNative>(selector);
	}

	public TutorialArrowNative CreateArrow()
	{
		return NewTutorialPointer<TutorialArrowNative>(_arrowPrf);
	}

	private T NewTutorialPointer<T>(GameObject prefab) where T : TutorialPointer
	{
		GameObject gameObject = Object.Instantiate(prefab);
		gameObject.transform.SetParent(base.transform);
		T component = gameObject.GetComponent<T>();
		component.Init(animationDuration);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return component;
	}

	public void Select()
	{
		TutorialMaskGenerator.Instance.Enable(true);
	}

	public void Release()
	{
		TutorialMaskGenerator.Instance.Enable(false);
	}

	public RectTransform AddMask(GameObject mask)
	{
		return _maskGenerator.AddMask(mask).GetComponent<RectTransform>();
	}

	public void ShowDarkness(bool show)
	{
		_darknessTexture.enabled = show;
	}
}
