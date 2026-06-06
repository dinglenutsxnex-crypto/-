using System;
using UnityEngine;

namespace SF3.CollisionEditor
{
	[ExecuteInEditMode]
	public class EditorCapsule : MonoBehaviour
	{
		public enum ECapsuleType
		{
			NONE = 0,
			NORMAL = 1,
			COLLISIBLE = 2,
			ATTACK = 3
		}

		public EditorVertex startNode;

		public EditorVertex endNode;

		private MeshRenderer meshRenderer;

		public ECapsuleType capsuleType;

		public float radius = 5f;

		private float diameter;

		public float marginStart;

		public float marginEnd;

		public string defense = string.Empty;

		private float _lastCapsulesRadius;

		private float _lastMarginStart;

		private float _lastMarginEnd;

		private ECapsuleType _lastCapsuleType;

		public ECapsuleType savedType;

		private Action<EditorCapsule> updateCallback;

		private bool needUpdateTransform;

		public string leftMirrorCapsule = string.Empty;

		public bool repulsive;

		public bool visible
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		private void Start()
		{
			_lastCapsulesRadius = radius;
			_lastMarginStart = marginStart;
			_lastMarginEnd = marginEnd;
			_lastCapsuleType = capsuleType;
			UpdateCapsulesRadius();
		}

		public void InitCapsule(EditorVertex startNodeVal, EditorVertex endNodeVal)
		{
			capsuleType = ECapsuleType.NONE;
			base.name = capsuleType.ToString();
			startNode = startNodeVal;
			endNode = endNodeVal;
			meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (_lastCapsuleType != capsuleType)
			{
				_lastCapsuleType = capsuleType;
				UpdateCapsuleType();
			}
			if (_lastCapsulesRadius != radius)
			{
				_lastCapsulesRadius = radius;
				needUpdateTransform = true;
				SetRadius(radius);
				UpdateCapsulesRadius();
			}
			if (_lastMarginStart != marginStart)
			{
				_lastMarginStart = marginStart;
				needUpdateTransform = true;
			}
			if (_lastMarginEnd != marginEnd)
			{
				_lastMarginEnd = marginEnd;
				needUpdateTransform = true;
			}
		}

		private void LateUpdate()
		{
			if (needUpdateTransform)
			{
				needUpdateTransform = false;
				UpdateCapsuleTransform();
			}
		}

		public void SetName(string nameVal)
		{
			base.gameObject.name = nameVal;
		}

		public void SetCollisible(ECapsuleType capsuleTypeVal)
		{
			_lastCapsuleType = (capsuleType = capsuleTypeVal);
			UpdateCapsuleType();
		}

		private void UpdateCapsulesRadius()
		{
			Vector3 zero = Vector3.zero;
			zero = base.transform.localScale;
			zero.x = (zero.z = diameter);
			base.transform.localScale = zero;
		}

		private void UpdateCapsuleType()
		{
			updateCallback(this);
		}

		public void SetMaterial(Material materialVal)
		{
			meshRenderer.sharedMaterial = materialVal;
		}

		public void SetRadius(float value)
		{
			radius = value;
			diameter = radius * 2f;
			needUpdateTransform = true;
		}

		public void SetMarginStart(float value)
		{
			marginStart = value;
			needUpdateTransform = true;
		}

		public void SetMarginEnd(float value)
		{
			marginEnd = value;
			needUpdateTransform = true;
		}

		public void UpdateCapsuleTransform()
		{
			Vector3 localScale = new Vector3(diameter, radius, diameter);
			Vector3 position = startNode.currentTransform.position;
			Vector3 position2 = endNode.currentTransform.position;
			base.transform.position = (position + position2) / 2f;
			localScale.y = Vector3.Distance(position, position2) / 2f + radius;
			base.transform.localScale = localScale;
			Vector3 toDirection = position2 - position;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, toDirection);
		}

		public void AddUpdateCallback(Action<EditorCapsule> cb)
		{
			updateCallback = cb;
		}
	}
}
