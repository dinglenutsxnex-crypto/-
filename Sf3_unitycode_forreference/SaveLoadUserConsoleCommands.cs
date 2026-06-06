using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;
using Network.core.events;
using SF3;
using UnityEngine;

public class SaveLoadUserConsoleCommands
{
	private const string testUsersDir = "TestUsersDir";

	private static string TestUsersLocalPath(string name)
	{
		return Combine(GlobalPath.ExternalPath, "TestUsersDir", name);
	}

	private static string TestUsersUrl(string name)
	{
		return Combine(GlobalPath.GetInternalPath("TestUserURL", string.Empty), "TestUsersDir", name);
	}

	private static string TestUsersPathStorage(string name)
	{
		Regex regex = new Regex("https?://((\\d+\\.)+\\d+)(:\\d+)?");
		return regex.Replace(TestUsersUrl(name), "//$1/Web");
	}

	private static string Combine(string string1, string string2, string string3, bool forwardSlash = true)
	{
		string text = Path.Combine(Path.Combine(string1, string2), string3);
		return forwardSlash ? text.Replace('\\', '/') : text;
	}

	public static string SaveTestUser(string[] args)
	{
		return Sf3ConsoleCommands.SafeCommandExecution(delegate
		{
			if (args.Length == 0)
			{
				return "Enter user name, please.";
			}
			string text = args[0];
			List<string> paths = new List<string> { TestUsersLocalPath(text) };
			if (Application.isEditor)
			{
				paths.Add(TestUsersPathStorage(text));
			}
			NetworkConnection.Send("cheat_get_player", new Empty(), delegate(NetworkEvent eventData)
			{
				StringValue extensible = eventData.getExtensible<StringValue>();
				if (!extensible.Value.IsNullOrEmpty())
				{
					SaveTestUser(extensible.Value, paths);
					Debug.Log("[User progress data saved]");
				}
				else
				{
					Debug.LogError("[Cant save user progress data]");
				}
			});
			return string.Format("Saving user \"{0}\" to {1}", text, string.Join(" and ", paths.ToArray()));
		});
	}

	private static void SaveTestUser(string battleDataValue, IEnumerable<string> paths)
	{
		foreach (string path in paths)
		{
			FilesUtil.WriteFileText(path, battleDataValue);
		}
	}

	public static string LoadTestUser(string[] args)
	{
		return Sf3ConsoleCommands.SafeCommandExecution(delegate
		{
			string text = args[0];
			string text2 = TestUsersUrl(text);
			NekkiWebHelper.SendRequest(text2, OnLoadTestUser, OnErrorLoadTestUser, null, text);
			return "Trying to load " + text + " from " + text2;
		});
	}

	private static string LoadTestUser(string name, out string path)
	{
		path = TestUsersPathStorage(name);
		string text = FilesUtil.ReadFileText(path);
		if (text != null)
		{
			return text;
		}
		path = TestUsersLocalPath(name);
		return FilesUtil.ReadFileText(path);
	}

	private static void OnLoadTestUser(NekkiWebRequest request)
	{
		FinishLoadTestUser(request.Text);
	}

	private static void OnErrorLoadTestUser(NekkiWebRequest request)
	{
		string path = string.Empty;
		FinishLoadTestUser(LoadTestUser(request.GetExternalData<string>(), out path));
	}

	private static void FinishLoadTestUser(string userData)
	{
		if (!string.IsNullOrEmpty(userData))
		{
			NetworkConnection.Send("cheat_set_player", new StringValue
			{
				Value = userData
			}, delegate
			{
				NetworkConnection.current.RestartConnection("load test user cheat");
			});
			Debug.Log("Load test user successful");
		}
		else
		{
			Debug.LogError("Cant load test user");
		}
	}
}
