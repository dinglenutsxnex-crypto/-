using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSlicerSFManager : MonoBehaviour
{
	private List<SpriteSlicer2DSliceInfo> _slicedSpriteInfo = new List<SpriteSlicer2DSliceInfo>();

	private TrailRenderer _trailRenderer;

	private const float MAX_ANGLE_DEVIATION_WITHIN_SWIPE = 5f;

	private const float MOUSE_RECORD_INTERVAL = 0.05f;

	private const int MAX_MOUSE_POSITIONS = 5;

	private List<Vector2> _mousePositions = new List<Vector2>();

	private float _mouseRecordTimer;

	private Vector2 _swipeStart = Vector3.zero;

	private Vector2 _swipeEnd = Vector3.zero;

	[SerializeField]
	public float cutForceMultiplier;

	[SerializeField]
	public float slowingFactor;

	[SerializeField]
	public float swipeLengthMultiplier;

	[SerializeField]
	public ParticleSystem sliceParticles;

	public GameObject slashParent;

	public float explodeTorqueForce = 1f;

	public GameObject slashPrf;

	public Action onSliced;

	private List<Rigidbody2D> _childSpriteRigidbodies;

	private List<BoosterpackSlash> _slashs;

	public Material SharedMaterial
	{
		get
		{
			if (_slicedSpriteInfo.Count > 0 && _slicedSpriteInfo[0].ChildObjects.Count > 0)
			{
				return _slicedSpriteInfo[0].ChildObjects[0].GetComponent<SlicedSprite>().MeshRenderer.sharedMaterial;
			}
			return null;
		}
	}

	private void Start()
	{
		_trailRenderer = GetComponentInChildren<TrailRenderer>();
		EnableTrailParticles(false);
		_childSpriteRigidbodies = new List<Rigidbody2D>();
		_slashs = new List<BoosterpackSlash>();
	}

	private void Update()
	{
		UpdateVelocity();
	}

	private void OnEnable()
	{
		EnableTrailParticles(false);
	}

	public void OnDragStart()
	{
		RecordMousePosition();
		UpdateTrail();
	}

	public void OnDrag(Vector2 delta)
	{
		RecordMousePosition();
		UpdateTrail();
		if (TryEndSwipe())
		{
			SwipeEnd();
		}
	}

	public void OnDragEnd()
	{
		EnableTrailParticles(false);
		if (_swipeEnd == Vector2.zero && _mousePositions.Count > 0)
		{
			_swipeEnd = _mousePositions[_mousePositions.Count - 1];
		}
		SwipeEnd();
	}

	private void SwipeEnd()
	{
		SliceIntersectedSprites();
		ClearPositions();
		FixChildrensLayer();
		_slicedSpriteInfo.Clear();
	}

	private bool RecordMousePosition()
	{
		_mouseRecordTimer -= Time.deltaTime;
		if (_mouseRecordTimer <= 0f)
		{
			Vector3 vector = default(Vector3);
			vector = UICamera.lastWorldPosition;
			_mousePositions.Add(vector);
			_mouseRecordTimer = 0.05f;
			if (_swipeStart == Vector2.zero)
			{
				_swipeStart = vector;
			}
			if (_mousePositions.Count > 5)
			{
				_mousePositions.RemoveAt(0);
			}
			return true;
		}
		return false;
	}

	private bool TryEndSwipe()
	{
		if (_mousePositions.Count < 2 || _swipeStart == Vector2.zero)
		{
			return false;
		}
		Vector2 vector = _mousePositions[_mousePositions.Count - 1];
		Vector3 from = vector - _mousePositions[_mousePositions.Count - 2];
		Vector3 to = vector - _swipeStart;
		float value = Vector3.Angle(from, to);
		if (Math.Abs(value) < 5f)
		{
			return false;
		}
		_swipeEnd = vector;
		return true;
	}

	public void Explode(Vector3 center, float radius, float force)
	{
		for (int i = 0; i < _slashs.Count; i++)
		{
			_slashs[i].PlayBeforeExplosionAnimation();
		}
		for (int j = 0; j < _childSpriteRigidbodies.Count; j++)
		{
			Rigidbody2D rigidbody2D = _childSpriteRigidbodies[j];
			if (rigidbody2D != null)
			{
				Vector3 vector = rigidbody2D.transform.position - center;
				float num = 1f - vector.magnitude / radius;
				rigidbody2D.AddForce(vector.normalized * force * num);
				rigidbody2D.freezeRotation = false;
				float num2 = ((!(vector.x < 0f)) ? 1f : (-1f));
				rigidbody2D.AddTorque(explodeTorqueForce * num2);
			}
		}
	}

	public void ClearPositions()
	{
		_mousePositions.Clear();
		_swipeStart = Vector3.zero;
		_swipeEnd = Vector3.zero;
	}

	public void SimulateSlicing(Vector3 worldPositionStart, Vector3 worldPositionEnd)
	{
		ClearPositions();
		_mousePositions.Add(worldPositionStart);
		UpdateTrail();
		_mousePositions.Add(worldPositionEnd);
		UpdateTrail();
		_swipeStart = worldPositionStart;
		_swipeEnd = worldPositionEnd;
		SliceIntersectedSprites();
		ClearPositions();
		_slicedSpriteInfo.Clear();
	}

	private bool SliceIntersectedSprites()
	{
		if (_swipeEnd == Vector2.zero || !IsSwipeHitsSlicableSprite(_swipeStart, _swipeEnd))
		{
			return false;
		}
		Vector2 vector = _swipeEnd - _swipeStart;
		Vector2 vector2 = _swipeStart + vector * (0f - (swipeLengthMultiplier - 1f));
		Vector2 vector3 = _swipeEnd + vector * (swipeLengthMultiplier - 1f);
		vector.Normalize();
		SpriteSlicer2D.SliceAllSprites(vector2, vector3, true, ref _slicedSpriteInfo);
		Vector2 vector4 = vector;
		_childSpriteRigidbodies.RemoveAll((Rigidbody2D rb) => rb == null);
		if (_slicedSpriteInfo.Count > 0)
		{
			for (int i = 0; i < _slicedSpriteInfo.Count; i++)
			{
				vector4.x = vector.y;
				vector4.y = 0f - vector.x;
				Vector2 sliceEnterWorldPosition = _slicedSpriteInfo[i].SliceEnterWorldPosition;
				Vector2 sliceExitWorldPosition = _slicedSpriteInfo[i].SliceExitWorldPosition;
				Vector2 position = (sliceExitWorldPosition - sliceEnterWorldPosition) / 2f + sliceEnterWorldPosition;
				for (int j = 0; j < _slicedSpriteInfo[i].ChildObjects.Count; j++)
				{
					GameObject gameObject = _slicedSpriteInfo[i].ChildObjects[j];
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					vector4.x = 0f - vector4.x;
					vector4.y = 0f - vector4.y;
					component.AddForceAtPosition(vector4 * cutForceMultiplier * component.mass, position);
					_childSpriteRigidbodies.Add(component);
				}
			}
			BoosterpackSlash boosterpackSlash = CreateSlash(vector2, vector3, _slicedSpriteInfo[0].SliceEnterWorldPosition, _slicedSpriteInfo[_slicedSpriteInfo.Count - 1].SliceExitWorldPosition);
			boosterpackSlash.PlaySwipeTrailAnimation();
			_slashs.Add(boosterpackSlash);
			if (onSliced != null)
			{
				onSliced();
			}
			return true;
		}
		return false;
	}

	public void EnableTrailParticles(bool isEnabled = true)
	{
		if (isEnabled)
		{
			if (!sliceParticles.isPlaying)
			{
				sliceParticles.Play();
			}
		}
		else if (sliceParticles.isPlaying)
		{
			sliceParticles.Stop();
		}
	}

	private bool IsSwipeHitsSlicableSprite(Vector3 cutStart, Vector3 cutEnd)
	{
		Vector3 vector = Vector3.Normalize(_swipeEnd - _swipeStart);
		float distance = Vector3.Distance(_swipeEnd, _swipeStart);
		RaycastHit2D[] array = Physics2D.RaycastAll(_swipeStart, vector, distance, -1);
		return array.Length != 0;
	}

	private void ResetTrail()
	{
		if ((bool)_trailRenderer)
		{
			_trailRenderer.Clear();
		}
	}

	private void UpdateTrail()
	{
		if ((bool)_trailRenderer && _mousePositions.Count > 0)
		{
			_trailRenderer.transform.position = _mousePositions[_mousePositions.Count - 1];
			_trailRenderer.transform.position = new Vector3(_trailRenderer.transform.position.x - 0.5f, _trailRenderer.transform.position.y, _trailRenderer.transform.position.z - 1f);
			EnableTrailParticles();
		}
	}

	private void UpdateVelocity()
	{
		for (int i = 0; i < _childSpriteRigidbodies.Count; i++)
		{
			Rigidbody2D rigidbody2D = _childSpriteRigidbodies[i];
			if (rigidbody2D != null && rigidbody2D.velocity.magnitude > 0f)
			{
				rigidbody2D.velocity *= slowingFactor;
			}
		}
	}

	private void FixChildrensLayer()
	{
		for (int i = 0; i < _slicedSpriteInfo.Count; i++)
		{
			for (int j = 0; j < _slicedSpriteInfo[i].ChildObjects.Count; j++)
			{
				GameObject gameObject = _slicedSpriteInfo[i].ChildObjects[j];
				if (!(gameObject == null))
				{
					Vector3 localPosition = gameObject.transform.localPosition;
					localPosition.z = -1f;
					gameObject.transform.localPosition = localPosition;
				}
			}
		}
	}

	private BoosterpackSlash CreateSlash(Vector3 start, Vector3 end, Vector3 cutEnter, Vector3 cutOut)
	{
		GameObject gameObject = NGUITools.AddChild(slashParent, slashPrf);
		Vector3 vector = end - start;
		Vector3 to = vector;
		to.Normalize();
		float num = Vector3.Angle(Vector3.right, to);
		if (end.y < start.y)
		{
			num *= -1f;
		}
		gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, num);
		Vector3 position = new Vector3(start.x + vector.x / 2f, start.y + vector.y / 2f, 0f);
		gameObject.transform.position = position;
		BoosterpackSlash component = gameObject.GetComponent<BoosterpackSlash>();
		component.SetFierPositionAndSize(cutEnter, cutOut);
		return component;
	}
}
