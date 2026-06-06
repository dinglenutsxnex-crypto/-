using System;
using SF3.GameModels;
using SF3.Settings;
using UnityEngine;

namespace SF3.CollisionEditor
{
	[ExecuteInEditMode]
	public class EditorVertex : MonoBehaviour
	{
		private Transform _currentTransform;

		public Transform parentBoneTransform;

		public string nodeName = string.Empty;

		public bool resetPosition;

		public bool resetName;

		public TextMesh nameTextMesh;

		private Vector3 lastposition;

		private Vector3 _vertexOffset = Vector3.zero;

		private Action<EditorVertex> _updateCallback;

		public string boneName;

		public Model owner;

		private GameObject marker;

		private bool markerEnabled;

		public Transform currentTransform
		{
			get
			{
				if (_currentTransform == null)
				{
					_currentTransform = base.transform;
				}
				return _currentTransform;
			}
		}

		public string parentBoneName
		{
			get
			{
				return parentBoneTransform.name;
			}
		}

		public bool visible
		{
			get
			{
				return markerEnabled;
			}
		}

		public Vector3 vertexOffset
		{
			get
			{
				return _vertexOffset;
			}
			set
			{
				_vertexOffset = value;
			}
		}

		private void Start()
		{
			resetPosition = false;
			resetName = false;
			markerEnabled = false;
			lastposition = currentTransform.position;
		}

		private void Update()
		{
			if (resetPosition)
			{
				resetPosition = false;
				ResetPosition();
			}
			if (resetName)
			{
				resetName = false;
				ResetName();
			}
			CheckNodePosition();
			CheckTacticsPivot();
		}

		private void CheckTacticsPivot()
		{
			if ((bool)marker)
			{
				marker.transform.position = parentBoneTransform.position;
				if (boneName == TacticsSettings.GetPivotBoneForModel(owner).boneName)
				{
					marker.SetActive(markerEnabled);
				}
				else
				{
					marker.SetActive(false);
				}
			}
		}

		public void InitVertex(Transform parentVertexTransform, TextMesh text3dPrefab)
		{
			parentBoneTransform = parentVertexTransform;
			ResetPosition();
			ResetName();
			if (text3dPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(text3dPrefab.gameObject);
				nameTextMesh = gameObject.GetComponent<TextMesh>();
				nameTextMesh.transform.parent = currentTransform;
				nameTextMesh.transform.localPosition = Vector3.zero;
				nameTextMesh.text = nodeName;
				nameTextMesh.gameObject.SetActive(false);
			}
			CreateMarker();
		}

		public void ShowMarker(bool enable)
		{
			markerEnabled = enable;
		}

		private void CreateMarker()
		{
			if (boneName.Equals(TacticsSettings.GetParamByName("TacticsPivot")) || boneName.Equals(TacticsSettings.GetParamByName("TacticsMirroredPivot")))
			{
				marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				marker.name = "tactics_pivot_marker";
				UnityEngine.Object.Destroy(marker.GetComponent<Collider>());
				marker.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 1f);
				marker.transform.localScale = new Vector3(15f, 15f, 15f);
				marker.transform.parent = base.transform;
				marker.SetActive(false);
			}
		}

		public void ResetPosition()
		{
			_vertexOffset = Vector3.zero;
			UpdateVertexTransform();
		}

		public void ResetName()
		{
			nodeName = parentBoneTransform.name + "_vertex";
			UpdateVertexName();
		}

		private void CheckNodePosition()
		{
			if (lastposition != currentTransform.position)
			{
				lastposition = currentTransform.position;
				_updateCallback(this);
			}
		}

		public void UpdateVertexTransform()
		{
			currentTransform.position = parentBoneTransform.TransformPoint(_vertexOffset);
		}

		public void UpdateVertexName()
		{
			if (nodeName.Trim().Equals(string.Empty))
			{
				ResetName();
			}
			currentTransform.name = nodeName;
			if (nameTextMesh != null)
			{
				nameTextMesh.text = nodeName;
			}
		}

		public void UpdateVertex()
		{
			UpdateVertexTransform();
			UpdateVertexName();
		}

		public void ShowText(bool isShow)
		{
			if (nameTextMesh != null)
			{
				nameTextMesh.gameObject.SetActive(isShow);
			}
		}

		public void AddUpdateCallback(Action<EditorVertex> cb)
		{
			_updateCallback = cb;
		}
	}
}
