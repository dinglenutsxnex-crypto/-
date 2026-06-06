using UnityEngine;

public class ShadowMapController : MonoBehaviour
{
	[SerializeField]
	private Texture2D _shadowMapNormal;

	[SerializeField]
	private Texture2D _shadowMapInShadowForm;

	[SerializeField]
	private float _changeShadowMapByFrame = 60f;

	[SerializeField]
	private float _leftBorder = -660f;

	[SerializeField]
	private float _rightBorder = 767f;

	[SerializeField]
	private float _offset = 88f;

	private static ShadowMapController _instance;

	private Texture2D _currentShadowMap;

	private Texture2D _lastShadowMap;

	private bool _isShadowMapChanging;

	private float _currentMapShadowChangeDelta;

	private float _currentMapShadowChangeTimer;

	public static ShadowMapController Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		_leftBorder = SceneConfig.LeftBorderX;
		_rightBorder = SceneConfig.RightBorderX;
		_currentShadowMap = _shadowMapNormal;
		_isShadowMapChanging = false;
	}

	public Color CalculateShadowAtPoint(float xPos)
	{
		if (_shadowMapNormal == null)
		{
			Debug.LogError("Missing shadowMapNormal at ShadowMapController");
			return Color.white;
		}
		if (_shadowMapInShadowForm == null)
		{
			Debug.LogError("Missing shadowMapInShadowForm at ShadowMapController");
			return Color.white;
		}
		float num = Mathf.InverseLerp(_leftBorder - _offset, _rightBorder + _offset, xPos);
		int x = Mathf.RoundToInt((float)_currentShadowMap.width * num);
		if (!_isShadowMapChanging)
		{
			return _currentShadowMap.GetPixel(x, 0);
		}
		Color pixel = ((!(_currentShadowMap == _shadowMapNormal)) ? _shadowMapNormal : _shadowMapInShadowForm).GetPixel(x, 0);
		Color pixel2 = _currentShadowMap.GetPixel(x, 0);
		return Color.Lerp(pixel2, pixel, _currentMapShadowChangeDelta);
	}

	public void SwitchShadowMap(bool isInShadowForm, bool instantly = false)
	{
		if (isInShadowForm)
		{
			_currentShadowMap = _shadowMapInShadowForm;
		}
		else
		{
			_currentShadowMap = _shadowMapNormal;
		}
		if (instantly)
		{
			_isShadowMapChanging = true;
			_currentMapShadowChangeTimer = _changeShadowMapByFrame;
		}
	}

	private void Update()
	{
		if (_isShadowMapChanging)
		{
			_currentMapShadowChangeTimer -= 1f;
			_currentMapShadowChangeDelta = _currentMapShadowChangeTimer / _changeShadowMapByFrame;
			if (_currentMapShadowChangeTimer <= 0f)
			{
				_isShadowMapChanging = false;
				_currentMapShadowChangeTimer = 0f;
			}
		}
	}
}
