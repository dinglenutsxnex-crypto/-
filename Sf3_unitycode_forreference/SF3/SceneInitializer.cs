using System;
using System.Collections;
using System.Collections.Generic;
using SF3.Audio;
using SF3.Effects;
using UnityEngine;

namespace SF3
{
	public class SceneInitializer
	{
		private GameObject _locationPrefab;

		private List<ISceneInitializationObject> _sceneInitializationObjects;

		public void CreateInitializers()
		{
			_sceneInitializationObjects = new List<ISceneInitializationObject>
			{
				BattleController.Instance,
				BattleCamera.Instance,
				EffectsManager.Instance,
				ModelsManager.Instance,
				BattleInterface.Instance
			};
		}

		public IEnumerator InitializeNewLocationScene()
		{
			using (TimerNode timerNode = new TimerNode("LoadScreen", "SceneLoader"))
			{
				AudioManager.Instance.Mute(true);
				if (!LoadScreen.LoaderVisible)
				{
					bool canStartLoadProcess = false;
					LoadScreen.ShowLoader(delegate
					{
						canStartLoadProcess = true;
					});
					yield return new WaitUntil(() => canStartLoadProcess);
				}
				yield return new WaitForEndOfFrame();
			}
			using (new TimerNode("Dispose Previous Location", "SceneLoader"))
			{
				DisposePreviousLocationScene();
			}
			string locationName = SceneManager.Instance.locationName.ToLower();
			using (new TimerNode("LocationPrefab", "SceneLoader"))
			{
				_locationPrefab = GlobalLoad.GetPrefabInstanceInternal("locations", locationName + "/" + locationName);
				if (_locationPrefab == null)
				{
					throw new Exception(string.Format("Cant find scene [{0}]", locationName));
				}
			}
			using (TimerNode timerNode4 = new TimerNode("WaitSceneConfig", "SceneLoader"))
			{
				yield return new WaitUntil(() => SceneConfig.IsPresent);
			}
			using (new TimerNode("SingleTones", "SceneLoader"))
			{
				foreach (ISceneInitializationObject sceneInitializationObject in _sceneInitializationObjects)
				{
					using (new TimerNode(string.Concat(sceneInitializationObject.GetType(), " module"), "SingleTones"))
					{
						sceneInitializationObject.Initialize();
					}
				}
			}
			yield return new WaitForEndOfFrame();
			using (new TimerNode("Cache", "SceneLoader"))
			{
				if (BattlesManager.currentBattleType == BattleType.Dojo)
				{
					NekkiUIRootModules.Instance.CacheModules();
				}
			}
			using (TimerNode timerNode8 = new TimerNode("InitBattle", "SceneLoader"))
			{
				yield return BattleController.Instance.InitBattle();
			}
		}

		private void DisposePreviousLocationScene()
		{
			if (_locationPrefab != null)
			{
				foreach (ISceneInitializationObject sceneInitializationObject in _sceneInitializationObjects)
				{
					sceneInitializationObject.DisposePreviousLocation();
				}
				GlobalLoad.Unload(_locationPrefab);
			}
			NekkiUIRootModules.Instance.ForceClearCache();
			GlobalLoad.UnloadUnusedAssets();
		}
	}
}
