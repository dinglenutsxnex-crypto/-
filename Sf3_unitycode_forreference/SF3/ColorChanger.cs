using UnityEngine;

namespace SF3
{
	public class ColorChanger : MonoBehaviour
	{
		public Color fromColor = Color.white;

		public Color toColor = Color.black;

		public bool onlyAlpha;

		public float forFrames = 60f;

		public AnimationCurve tweenCurve;

		public string shaderColorField = string.Empty;

		public bool playOnEnable = true;

		private Material _currentMaterial;

		private float _startFrame;

		private float _endFrame;

		private bool _active;

		private bool _inited;

		private void Init()
		{
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				_currentMaterial = component.material;
			}
			else
			{
				MeshRenderer component2 = GetComponent<MeshRenderer>();
				if (component2 != null)
				{
					_currentMaterial = component2.material;
				}
			}
			_active = false;
			_inited = true;
		}

		private void OnEnable()
		{
			if (playOnEnable)
			{
				Play();
			}
		}

		private void Update()
		{
			if (_active)
			{
				Color color = Color.Lerp(fromColor, toColor, tweenCurve.Evaluate((GameTimeController.battleTime - _startFrame) / forFrames));
				if (shaderColorField.Length != 0)
				{
					_currentMaterial.SetColor(shaderColorField, color);
				}
				else
				{
					_currentMaterial.color = color;
				}
				if (GameTimeController.battleTime >= _endFrame)
				{
					_currentMaterial.color = toColor;
					_active = false;
				}
			}
		}

		public void Play()
		{
			if (!_inited)
			{
				Init();
			}
			if (onlyAlpha)
			{
				float a = fromColor.a;
				float a2 = toColor.a;
				if (shaderColorField.Length != 0)
				{
					fromColor = (toColor = _currentMaterial.GetColor(shaderColorField));
				}
				else
				{
					fromColor = (toColor = _currentMaterial.color);
				}
				fromColor.a = a;
				toColor.a = a2;
			}
			_active = true;
			_currentMaterial.color = fromColor;
			_startFrame = GameTimeController.battleTime;
			_endFrame = _startFrame + forFrames;
		}
	}
}
