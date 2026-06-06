using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using Nekki.Core;
using Nekki.GamingService;
using SF3.UserData;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameRestarter
{
	public static void ShowRestartDialog(string title, string message = "restart_game", bool isError = false)
	{
		GameInit.CanContinueInit = false;
		SystemMessage systemMessage = SystemMessage.ShowAlert(message, title);
		if (isError)
		{
			systemMessage.SetTextColor(Color.red);
		}
		systemMessage.AddButton("continue", delegate
		{
			Restart();
		});
		systemMessage.SetBlockPriority(UIBlocker.Priority.ServerSystemAlert);
		systemMessage.Show();
	}

	internal static void Register(Action clean)
	{
	}

	public static void Restart()
	{
		Cleanup();
		Relaunch();
	}

	private static void Cleanup()
	{
		Analytics.current.DestroySelf();
		NetworkInitializer.current.DestroySelf();
		NetworkConnection.current.DestoySelf();
		GamingService.Instance.logout();
		StaticObjectsManager.Clear();
		RulesController.Clear();
		QuestController.Clear();
		BaseModuleController.Clear();
		LoadScreen.Clear();
		UserDataController.Clear();
		UIBlocker.Instance.Clear();
		CleanSceneObjects();
	}

	private static void CleanSceneObjects()
	{
		List<GameObject> list = new List<GameObject>();
		SceneManager.GetActiveScene().GetRootGameObjects(list);
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			GameObject gameObject2 = ((!(gameObject.transform.root == null)) ? gameObject.transform.root.gameObject : gameObject);
			if (!gameObject2.GetComponent<DOTweenComponent>() && !list.Contains(gameObject2))
			{
				list.Add(gameObject2);
			}
		}
		foreach (GameObject item in list)
		{
			UnityEngine.Object.Destroy(item);
		}
	}

	private static void Relaunch()
	{
		InternalSettingsSF3.Init();
		SceneManager.LoadScene(0);
	}
}
