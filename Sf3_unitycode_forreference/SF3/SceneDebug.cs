using System.Collections.Generic;
using Nekki;
using UnityEngine;

namespace SF3
{
	public class SceneDebug : MonoBehaviour
	{
		[SerializeField]
		private MultiTweenTransition _multiTween;

		[SerializeField]
		private UIButton _bt;

		[SerializeField]
		private GameObject _testLabel;

		public GameObject objToDisable;

		private bool _active = true;

		private Vector3 lastLocalPos;

		private Dictionary<string, GameObject> nameSceneObjects;

		private void Start()
		{
			nameSceneObjects = new Dictionary<string, GameObject>();
			lastLocalPos = _testLabel.transform.localPosition;
			if (NekkiUtils.IsDebug)
			{
				DebugInfoHider.setSceneDebugObj = base.gameObject;
				ShowHide();
			}
		}

		private void Update()
		{
		}

		public void ShowHide()
		{
			_active = !_active;
			objToDisable.SetActive(_active);
			if (_active)
			{
				GetSceneObjects();
			}
		}

		private void GetSceneObjects()
		{
			nameSceneObjects.Clear();
			Transform[] array = Object.FindObjectsOfType<Transform>();
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				if (transform.parent == null)
				{
					GameObject gameObject = Object.Instantiate(_testLabel);
					gameObject.transform.parent = objToDisable.transform;
					lastLocalPos.y -= 20f;
					gameObject.transform.localPosition = lastLocalPos;
					gameObject.transform.localRotation = _testLabel.transform.localRotation;
					gameObject.transform.localScale = _testLabel.transform.localScale;
					gameObject.GetComponent<UILabel>().text = transform.name;
					gameObject.name = transform.name;
					nameSceneObjects.Add(transform.name, transform.gameObject);
				}
			}
		}

		public void Clicking(GameObject off)
		{
			if (nameSceneObjects.ContainsKey(off.name))
			{
				nameSceneObjects[off.name].SetActive(!nameSceneObjects[off.name].activeSelf);
			}
		}

		private void OnButtonClick()
		{
			if (_active)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		public void SetButtonPositionX(float newPositionX)
		{
			Vector3 localPosition = _bt.transform.localPosition;
			localPosition.x = newPositionX;
			_bt.transform.localPosition = localPosition;
		}

		private void Show()
		{
			_bt.normalSprite = "Debug_minimize";
			_multiTween.TweenIn();
			_active = true;
		}

		private void Hide()
		{
			_bt.normalSprite = "Debug_expand";
			_multiTween.TweenOut();
			_active = false;
		}
	}
}
