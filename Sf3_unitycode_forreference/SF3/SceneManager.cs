using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF3
{
	public class SceneManager : MonoBehaviour
	{
		private static SceneManager _instance;

		private ESceneType _sceneType;

		public Action OnSceneLoadedEvent;

		public Action OnLocationSceneLoadedEvent;

		private SceneInitializer _sceneInitializer = new SceneInitializer();

		private AsyncOperation _asyncSceneLoader;

		public static SceneManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public ESceneType sceneType
		{
			get
			{
				return _sceneType;
			}
		}

		public string locationName { get; private set; }

		public static void CreateObject()
		{
			if (_instance == null)
			{
				_instance = new GameObject("scene_manager").AddComponent<SceneManager>();
				StaticObjectsManager.AddObject(_instance.gameObject);
			}
		}

		private void LoadFightScene(Action onLoad = null)
		{
			_sceneType = ESceneType.Fight;
			OnSceneLoadedEvent = (Action)Delegate.Combine(OnSceneLoadedEvent, onLoad);
			_asyncSceneLoader = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("fight");
			_asyncSceneLoader.allowSceneActivation = true;
		}

		public void LoadLocationScene(string locationNameValue, Action onLoad = null)
		{
			OnLocationSceneLoadedEvent = (Action)Delegate.Combine(OnLocationSceneLoadedEvent, onLoad);
			locationName = locationNameValue;
			if (_sceneType == ESceneType.None)
			{
				LoadFightScene();
			}
			StartCoroutine(LoadLocationSceneProcess());
		}

		private IEnumerator LoadLocationSceneProcess()
		{
			TimerNode.Clear();
			TimerNode.SetParent(new TimerNode("SceneLoader"));
			using (TimerNode timerNode = new TimerNode("scene CreateInitializers", "SceneLoader"))
			{
				if (_asyncSceneLoader != null)
				{
					while (!_asyncSceneLoader.isDone)
					{
						yield return null;
					}
					_asyncSceneLoader = null;
					if (OnSceneLoadedEvent != null)
					{
						OnSceneLoadedEvent();
						OnSceneLoadedEvent = null;
					}
					_sceneInitializer.CreateInitializers();
				}
			}
			yield return StartCoroutine(_sceneInitializer.InitializeNewLocationScene());
			if (OnLocationSceneLoadedEvent != null)
			{
				OnLocationSceneLoadedEvent();
				OnLocationSceneLoadedEvent = null;
			}
			TimerNode.LogHierarchy();
			TimerNode.Clear();
		}
	}
}
