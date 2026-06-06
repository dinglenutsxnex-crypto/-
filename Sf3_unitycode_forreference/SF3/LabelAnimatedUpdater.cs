using Nekki.UI;
using SF3.Audio;
using UnityEngine;

namespace SF3
{
	public class LabelAnimatedUpdater : MonoBehaviour
	{
		[SerializeField]
		private int valueRange = 666;

		[SerializeField]
		private int minFrames = 60;

		[SerializeField]
		private int maxFrames = 180;

		public AnimationCurve timeValueRelation;

		public AnimationCurve valueChangingRate;

		[SerializeField]
		private string _onStartSoundName;

		private int _lastValue;

		private int _newValue;

		private bool _processActive;

		private float _processTime;

		private float _endTime;

		private bool _syncBlockForSetText;

		private NekkiUILabel _nUIl;

		private void Awake()
		{
			_processActive = false;
			_syncBlockForSetText = false;
		}

		private void Start()
		{
			_nUIl = base.gameObject.GetComponent<NekkiUILabel>();
			if (_nUIl != null)
			{
				_nUIl.OnTextChangeEvent += OnTextChange;
			}
			if (_onStartSoundName.Length > 0)
			{
				AudioManager.Instance.LoadSound(_onStartSoundName);
			}
		}

		private void Update()
		{
			if (_processActive && _nUIl != null)
			{
				if (_endTime < (float)GameTimeController.frameCount)
				{
					_processActive = false;
				}
				else
				{
					UpdateTextSync(((int)Mathf.Lerp(_newValue, _lastValue, valueChangingRate.Evaluate((_endTime - (float)GameTimeController.frameCount) / _processTime))).ToString());
				}
			}
		}

		private void OnTextChange(string lastValue, string newValue)
		{
			if (_syncBlockForSetText)
			{
				_syncBlockForSetText = false;
			}
			else if (int.TryParse(lastValue, out _lastValue) && int.TryParse(newValue, out _newValue) && _lastValue != _newValue)
			{
				_processActive = true;
				UpdateTextSync(_lastValue.ToString());
				_processTime = Mathf.Lerp(minFrames, maxFrames, timeValueRelation.Evaluate(Mathf.Abs(_lastValue - _newValue) / valueRange));
				_endTime = (float)GameTimeController.frameCount + _processTime;
				if (_onStartSoundName.Length > 0)
				{
					AudioManager.Instance.PlaySound(_onStartSoundName);
				}
			}
		}

		private void UpdateTextSync(string newValue)
		{
			_syncBlockForSetText = true;
			_nUIl.text = newValue;
		}
	}
}
