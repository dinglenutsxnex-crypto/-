using System;
using System.Collections.Generic;
using DynamicShadowProjector;
using SF3;
using UnityEngine;

public class ModelShadow : MonoBehaviour
{
	[Serializable]
	public class JointData
	{
		public string name;

		public Transform transf;

		public float shadowWidth;

		public float projectionX;

		public JointData(string _name)
		{
			name = _name;
		}
	}

	[Serializable]
	public class ShadowPointData
	{
		public string name;

		public float x;

		public float shadow;

		public ShadowPointData(float _x, float _shadow)
		{
			ChangeParams(_x, _shadow);
		}

		public void ChangeParams(float _x, float _shadow)
		{
			x = _x;
			shadow = _shadow;
		}
	}

	public JointData[] jointData;

	public Vector3 shadowOffset;

	public float shadowHeight;

	private Transform _shadowTransf;

	public GameObject shadowPrefab;

	public float shadowArea = 1000f;

	private float _shadowAreaPercent = 0.25f;

	private float _c1;

	private float _c2;

	private float _r0;

	private Vector3 _pos;

	private Color _c;

	public List<ShadowPointData> totalShadowPoints = new List<ShadowPointData>();

	private MeshRenderer _shadowRenderer;

	private Transform _lightSource;

	private float _floorHeight;

	private float _lightSourceHeight;

	public bool lightSourceExists;

	[SerializeField]
	[HideInInspector]
	private float _lightDirectionAngle;

	[SerializeField]
	private GameObject shadowX;

	private GameObject _realShadow;

	private bool _shadowDisabled;

	public static bool UseRealShadow = true;

	private DrawTargetObject _shadow;

	private float _totalBlackness;

	private float _fromX;

	private float _toX;

	public float k = 1.5f;

	public ShadowTextureRenderer ShadowTextureRenderer { get; protected set; }

	public Color SavedShadowColor { get; protected set; }

	public float LightDirectionAngle
	{
		get
		{
			return _lightDirectionAngle;
		}
		set
		{
			_lightDirectionAngle = Mathf.Clamp(value, -1.57075f, 1.57075f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (totalShadowPoints == null)
		{
			return;
		}
		foreach (ShadowPointData totalShadowPoint in totalShadowPoints)
		{
			totalShadowPoint.name = totalShadowPoint.x + "  " + totalShadowPoint.shadow;
		}
	}

	private void Awake()
	{
		for (int i = 0; i < jointData.Length; i++)
		{
			jointData[i].transf = GetChildByName(base.transform, jointData[i].name);
			if (jointData[i].transf == null)
			{
				Debug.Log(jointData[i].name);
			}
		}
	}

	public void CreateShadow()
	{
		_shadow = UnityEngine.Object.Instantiate(shadowX).GetComponent<DrawTargetObject>();
		_shadow.target = base.transform.parent;
		_shadow.transform.parent = base.transform.parent.Find("centerOfMass");
		_shadow.transform.localPosition = new Vector3(-59.13672f, 47.67383f, 23.17383f);
		_shadow.transform.localEulerAngles = new Vector3(33f, 53f, -7f);
		_realShadow = _shadow.gameObject;
		Vector3 position = base.transform.position + Vector3.up;
		position.z -= 16f;
		_shadowTransf = UnityEngine.Object.Instantiate(shadowPrefab, position, shadowPrefab.transform.rotation).transform;
		Transform shadowTransf = _shadowTransf;
		shadowTransf.name = shadowTransf.name + " " + base.transform.parent.name;
		_shadowTransf.parent = base.transform;
		_shadowRenderer = _shadowTransf.GetComponent<MeshRenderer>();
		ShadowTextureRenderer = _shadow.GetComponent<ShadowTextureRenderer>();
		SavedShadowColor = ShadowTextureRenderer.shadowColor;
	}

	public void Initialize()
	{
		GameObject gameObject = GameObject.Find("LightSource");
		lightSourceExists = gameObject != null;
		if (lightSourceExists)
		{
			_lightSource = gameObject.transform;
			_floorHeight = base.transform.position.y + 1f;
			_lightSourceHeight = _lightSource.position.y - _floorHeight;
			lightSourceExists = false;
		}
		OnQualityLevelChange();
		QualityManager instance = QualityManager.Instance;
		instance.onQualityLvlChange = (Action)Delegate.Combine(instance.onQualityLvlChange, new Action(OnQualityLevelChange));
	}

	private void OnDestroy()
	{
		QualityManager instance = QualityManager.Instance;
		instance.onQualityLvlChange = (Action)Delegate.Remove(instance.onQualityLvlChange, new Action(OnQualityLevelChange));
	}

	private void OnQualityLevelChange()
	{
		if (QualityManager.Instance.IsShadowForcedOff)
		{
			_realShadow.SetActive(false);
			_shadowTransf.gameObject.SetActive(false);
			_shadowDisabled = true;
			return;
		}
		_shadowDisabled = false;
		switch (QualityManager.Preset.shadows)
		{
		case ShadowsTypes.OFF:
			_shadowDisabled = true;
			_realShadow.SetActive(false);
			_shadowTransf.gameObject.SetActive(false);
			break;
		case ShadowsTypes.SIMPLE:
			UseRealShadow = false;
			break;
		case ShadowsTypes.REAL:
			UseRealShadow = true;
			break;
		}
	}

	public void SetCommandBufferDirty()
	{
		if (_shadow != null)
		{
			_shadow.SetCommandBufferDirty();
		}
	}

	private Transform GetChildByName(Transform transf, string name)
	{
		if (transf.name == name)
		{
			return transf;
		}
		if (transf.childCount > 0)
		{
			for (int i = 0; i < transf.childCount; i++)
			{
				Transform childByName = GetChildByName(transf.GetChild(i), name);
				if (childByName != null)
				{
					return childByName;
				}
			}
		}
		return null;
	}

	private float CalculateShadowPoint(JointData jointData, float x)
	{
		float num = jointData.transf.position.y - _shadowTransf.position.y;
		if (num < jointData.shadowWidth)
		{
			num = jointData.shadowWidth;
		}
		float num2 = num / jointData.shadowWidth;
		num2 /= Mathf.Cos(_lightDirectionAngle);
		return 1f / num2 / (1f + Mathf.Pow(jointData.transf.position.x - x + Mathf.Tan(_lightDirectionAngle) * (jointData.transf.position.y - _shadowTransf.position.y), 2f) / Mathf.Pow(num2 * jointData.shadowWidth, 2f));
	}

	private float CalculateMiddleShadowX()
	{
		float num = 0f;
		for (int i = 0; i < jointData.Length; i++)
		{
			num += jointData[i].transf.position.x;
		}
		return num / (float)jointData.Length;
	}

	private float CalculateTotalShadowPoint(float x)
	{
		float num = 1f;
		for (int i = 0; i < jointData.Length; i++)
		{
			num *= 1f - CalculateShadowPoint(jointData[i], x);
		}
		return 1f - num;
	}

	private float FindCustomPercentOfShadowArea(float _shadowAreaPercent)
	{
		float num = _totalBlackness * _shadowAreaPercent;
		for (int i = 0; i < totalShadowPoints.Count - 1; i++)
		{
			float num2 = (totalShadowPoints[i].shadow + totalShadowPoints[i + 1].shadow) / 2f * (totalShadowPoints[i + 1].x - totalShadowPoints[i].x);
			if (num < num2)
			{
				float num3 = totalShadowPoints[i + 1].shadow - totalShadowPoints[i].shadow;
				float num4 = 2f * totalShadowPoints[i].shadow * (totalShadowPoints[i + 1].x - totalShadowPoints[i].x);
				float num5 = -2f * num * (totalShadowPoints[i + 1].x - totalShadowPoints[i].x);
				return (0f - num4 + Mathf.Sqrt(num4 * num4 - 4f * num3 * num5)) / (2f * num3) + totalShadowPoints[i].x;
			}
			num -= num2;
		}
		return totalShadowPoints[totalShadowPoints.Count - 1].x;
	}

	private void CalculateShadow()
	{
		_totalBlackness = 0f;
		float num = shadowArea / 25f;
		int num2 = 0;
		float num3 = CalculateMiddleShadowX();
		num2 = 0;
		_fromX = num3 - shadowArea;
		_toX = num3 + shadowArea;
		for (float num4 = _fromX; num4 < _toX; num4 += num)
		{
			if (totalShadowPoints.Count == num2)
			{
				totalShadowPoints.Add(new ShadowPointData(num4, CalculateTotalShadowPoint(num4)));
			}
			else
			{
				totalShadowPoints[num2].ChangeParams(num4, CalculateTotalShadowPoint(num4));
			}
			num2++;
		}
		if (num2 <= totalShadowPoints.Count - 1)
		{
			for (num2++; num2 < totalShadowPoints.Count; num2++)
			{
				totalShadowPoints.RemoveAt(num2);
			}
		}
		for (num2 = 0; num2 < totalShadowPoints.Count - 1; num2++)
		{
			_totalBlackness += (totalShadowPoints[num2].shadow + totalShadowPoints[num2 + 1].shadow) / 2f * num;
		}
		_c1 = FindCustomPercentOfShadowArea(_shadowAreaPercent);
		_c2 = FindCustomPercentOfShadowArea(1f - _shadowAreaPercent);
		_r0 = 0.5f * (_c2 - _c1);
		_pos = _shadowTransf.position;
		_pos.x = 0.5f * (_c2 + _c1);
		_shadowTransf.position = _pos;
		_shadowTransf.localScale = new Vector3(_r0, _r0, _r0);
		_c = _shadowRenderer.material.color;
		_c.a = _totalBlackness / _r0 / (float)Math.PI / k;
		_shadowRenderer.material.color = _c;
	}

	private void Update()
	{
		if (_shadowDisabled)
		{
			return;
		}
		if (UseRealShadow)
		{
			if (_shadowTransf != null && _shadowTransf.gameObject.activeSelf)
			{
				_shadowTransf.gameObject.SetActive(false);
			}
			if (_realShadow != null && !_realShadow.activeSelf)
			{
				_realShadow.SetActive(true);
			}
			return;
		}
		if (_shadowTransf != null && !_shadowTransf.gameObject.activeSelf)
		{
			_shadowTransf.gameObject.SetActive(true);
		}
		if (_realShadow != null && _realShadow.activeSelf)
		{
			_realShadow.SetActive(false);
		}
		CalculateShadow();
	}

	private Vector3 GetNearestToLightJoint()
	{
		Vector3 result = Vector3.zero;
		float num = 0f;
		for (int i = 0; i < jointData.Length; i++)
		{
			if (i == 0)
			{
				result = jointData[i].transf.position;
				num = Mathf.Abs(_lightSource.transform.position.x - jointData[i].transf.position.x);
			}
			else if (Mathf.Abs(_lightSource.transform.position.x - jointData[i].transf.position.x) < num)
			{
				result = jointData[i].transf.position;
				num = Mathf.Abs(_lightSource.transform.position.x - jointData[i].transf.position.x);
			}
		}
		return result;
	}

	private Vector3 GetModelHighesttPoint()
	{
		Vector3 result = Vector3.zero;
		for (int i = 0; i < jointData.Length; i++)
		{
			if (i == 0)
			{
				result = jointData[i].transf.position;
			}
			else if (jointData[i].transf.position.y > result.y)
			{
				result = jointData[i].transf.position;
			}
		}
		return result;
	}

	private Vector3 GetModelLowestPoint()
	{
		Vector3 result = Vector3.zero;
		for (int i = 0; i < jointData.Length; i++)
		{
			if (i == 0)
			{
				result = jointData[i].transf.position;
			}
			else if (jointData[i].transf.position.y < result.y)
			{
				result = jointData[i].transf.position;
			}
		}
		return result;
	}

	private Vector3 Vector3Multiply(Vector3 a, Vector3 b)
	{
		a.x *= b.x;
		a.y *= b.y;
		a.z *= b.z;
		return a;
	}

	private float CalculateProjectionPoint(Vector3 jointPos)
	{
		float num = jointPos.y - _floorHeight;
		float num2 = num * Mathf.Abs(_lightSource.position.x - jointPos.x) / (_lightSourceHeight - num);
		float num3 = Mathf.Sign(jointPos.x - _lightSource.position.x);
		return jointPos.x + num2 * num3;
	}
}
