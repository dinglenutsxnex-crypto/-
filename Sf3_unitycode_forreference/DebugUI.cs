using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Nekki;
using Nekki.Yaml;
using Network.core.events;
using SF3;
using SF3.BattleUtils;
using SF3.UserData;
using UnityEngine;
using UnityEngine.Profiling;

public class DebugUI : UIModuleHolder
{
	private enum DebugUIStates
	{
		Short = 0,
		Full = 1
	}

	[SerializeField]
	private MultiTweenTransition _multiTween;

	[SerializeField]
	private UILabel fpsLabelShort;

	[SerializeField]
	private UIButton _expand;

	[SerializeField]
	private UIButton _collapse;

	[SerializeField]
	private UIButton _devServerChooserClose;

	[SerializeField]
	private Color FPSGood;

	[SerializeField]
	private Color FPSNorm;

	[SerializeField]
	private Color FPSLow;

	[SerializeField]
	private UILabel ConsoleTextOutput;

	[SerializeField]
	private GameObject fullGO;

	[SerializeField]
	private GameObject listContainerPrefab;

	[SerializeField]
	private GameObject serverSelectPrefab;

	[SerializeField]
	private DebugDevelopmentServerChooser devServerChooser;

	private List<DebugListContainerController> lineContainers = new List<DebugListContainerController>();

	private int currentContainerID;

	private DebugUIStates state;

	private static DebugUI _instance;

	private float nextUpdateTime;

	private static float _lastObjRequest = -1f;

	private static int _lastObjectsCount;

	private static float _lastDrawcallsRequest = -1f;

	private static int _lastDrawcallsCount;

	private MultiTweenTransition _devChooserTween;

	private bool _devChooserShowed;

	public static string MemoryInfo
	{
		get
		{
			return string.Format("mem: [a {0} r {1}] / dv {2} dg {3}]", GetAllocatedMemory, GetReservedMemory, SystemInfo.systemMemorySize.ToString("0.0"), SystemInfo.graphicsMemorySize.ToString("0.0"));
		}
	}

	public static string FpsInfo
	{
		get
		{
			if (!_instance)
			{
				GameObject gameObject = GameObject.Find("_debugInfo");
				if ((bool)gameObject)
				{
					_instance = gameObject.GetComponent<DebugUI>();
				}
				else
				{
					_instance = new GameObject("_debugInfo").AddComponent<DebugUI>();
				}
			}
			return SF3BattleUtils.GetFPS().ToString("0.0");
		}
	}

	public static float FpsInfoFloat
	{
		get
		{
			if (!_instance)
			{
				GameObject gameObject = GameObject.Find("_debugInfo");
				if ((bool)gameObject)
				{
					_instance = gameObject.GetComponent<DebugUI>();
				}
				else
				{
					_instance = new GameObject("_debugInfo").AddComponent<DebugUI>();
				}
			}
			return SF3BattleUtils.GetFPS();
		}
	}

	public static string FullInfo
	{
		get
		{
			return string.Format("{0} {1} {2} {3}", FpsInfo, MemoryInfo, GetDrawCalls, GetObjects);
		}
	}

	public static string GetDrawCalls
	{
		get
		{
			if (_lastDrawcallsRequest + 1f < GameTimeController.time)
			{
				_lastDrawcallsRequest = GameTimeController.time;
				GameObject[] array = Object.FindObjectsOfType<GameObject>();
				int num = 1;
				for (int i = 0; i < array.Length; i++)
				{
					if ((bool)array[i].GetComponent<Renderer>() && array[i].GetComponent<Renderer>().isVisible)
					{
						num++;
					}
				}
				_lastDrawcallsCount = num;
			}
			return string.Empty + _lastDrawcallsCount;
		}
	}

	public static string GetObjects
	{
		get
		{
			if (_lastObjRequest + 1f < GameTimeController.time)
			{
				_lastObjRequest = GameTimeController.time;
				_lastObjectsCount = Object.FindObjectsOfType<GameObject>().Length;
			}
			return string.Empty + _lastObjectsCount;
		}
	}

	public static string GetAllocatedMemory
	{
		get
		{
			return ((float)Profiler.GetTotalAllocatedMemory() / 1024f / 1024f).ToString("0.0");
		}
	}

	public static string GetSystemMemory
	{
		get
		{
			return SystemInfo.systemMemorySize.ToString("0.0");
		}
	}

	public static string GetGraphicsMemory
	{
		get
		{
			return SystemInfo.graphicsMemorySize.ToString("0.0");
		}
	}

	public static string GetReservedMemory
	{
		get
		{
			return ((float)Profiler.GetTotalReservedMemory() / 1024f / 1024f).ToString("0.0");
		}
	}

	private static string GetConnectionStatus
	{
		get
		{
			return (!NetworkConnection.current.IsNetworkEstablished()) ? "Cut off" : "Established";
		}
	}

	public static void RecursiveMappingOpening(Node node)
	{
		if (node is Mapping)
		{
			Debug.Log(">> " + node.key);
			{
				foreach (Node item in ((Mapping)node).nodesInside)
				{
					RecursiveMappingOpening(item);
				}
				return;
			}
		}
		if (node is Sequence)
		{
			Debug.Log("--- " + node.key);
			{
				foreach (Node item2 in ((Sequence)node).nodesInside)
				{
					RecursiveMappingOpening(item2);
				}
				return;
			}
		}
		if (node is Scalar)
		{
			Debug.Log(node.key + " = " + node.value);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
	}

	private void Start()
	{
		_expand.onClick.Add(new EventDelegate(Full));
		_collapse.onClick.Add(new EventDelegate(Short));
		_devServerChooserClose.onClick.Add(new EventDelegate(ToggleDevServerChooser));
		_devChooserTween = fullGO.GetComponent<MultiTweenTransition>();
		_multiTween.TweenOut();
		Clear();
		nextUpdateTime = Time.unscaledTime + 0.5f;
		DebugListContainerController debugListContainerController = AddListContainer("SYSTEM");
		debugListContainerController.AddLine("FPS", updateFPS);
		debugListContainerController.AddLine("Memory reserved", delegate(DebugTextLineController line)
		{
			line.value.text = string.Format("{0} Mb", GetReservedMemory);
		});
		debugListContainerController.AddLine("Memory allocated", delegate(DebugTextLineController line)
		{
			line.value.text = string.Format("{0} Mb", GetAllocatedMemory);
		});
		debugListContainerController.AddLine("System memory", delegate(DebugTextLineController line)
		{
			line.value.text = string.Format("{0} Mb", GetSystemMemory);
		});
		debugListContainerController.AddLine("Graphics memory", delegate(DebugTextLineController line)
		{
			line.value.text = string.Format("{0} Mb", GetGraphicsMemory);
		});
		debugListContainerController.AddLine("Draw calls", delegate(DebugTextLineController line)
		{
			line.value.text = GetDrawCalls;
		});
		debugListContainerController.AddLine("Objects", delegate(DebugTextLineController line)
		{
			line.value.text = GetObjects;
		});
		debugListContainerController.AddLine(string.Empty, delegate(DebugTextLineController line)
		{
			line.value.text = string.Empty;
		}).HideSeparator();
		debugListContainerController.AddButton("RESET", onResetButtonPress);
		DebugListContainerController debugListContainerController2 = AddListContainer("ONLINE");
		debugListContainerController2.AddLine(string.Empty, delegate(DebugTextLineController line)
		{
			line.value.text = NetworkConnection.current.UserData.Auth.Login;
		}).HideSeparator();
		debugListContainerController2.AddLine("Player ID", delegate(DebugTextLineController line)
		{
			line.value.text = UserManager.ServerPlayerID.ToString();
		});
		debugListContainerController2.AddLine("Ping", delegate(DebugTextLineController line)
		{
			line.value.text = string.Format("{0}ms", SF3BattleUtils.GetPing());
		});
		debugListContainerController2.AddLine("Network Status", updateNetworkStatus);
		debugListContainerController2.AddLine("Inside Pocket", updatePocketStatus);
		debugListContainerController2.AddLine("Current Config ver", delegate(DebugTextLineController line)
		{
			line.value.text = NetworkConnection.current.CurrentConfigVersion.full;
		});
		debugListContainerController2.AddLine("IP", updateServerIP).HideSeparator();
		debugListContainerController2.AddLine("Can Create Player", delegate(DebugTextLineController line)
		{
			line.value.text = NetworkConnection.current.canCreatePlayer().ToString();
		});
		debugListContainerController2.AddLine("Current State", delegate(DebugTextLineController line)
		{
			line.value.text = NetworkConnection.CurrentStateName;
		});
		AddServerLine(debugListContainerController2);
		DebugListContainerController debugListContainerController3 = AddListContainer("FIGHT");
		debugListContainerController3.AddLine("Module", delegate(DebugTextLineController line)
		{
			line.value.text = BaseModuleController.CurrentName;
		});
		BattleInfo battle = BattlesManager.currentBattle.GetBattleInfo();
		debugListContainerController3.AddLine("BattleID", delegate(DebugTextLineController line)
		{
			line.value.text = battle.id.ToString();
		});
		debugListContainerController3.AddLine("BattleName", delegate(DebugTextLineController line)
		{
			line.value.text = battle.name;
		});
		debugListContainerController3.AddLine("FightID", delegate(DebugTextLineController line)
		{
			line.value.text = (battle.GetCurrentFight().fightID + 1).ToString();
		});
		openContainerAtID(currentContainerID);
	}

	private void AddServerLine(DebugListContainerController online)
	{
		string name = InternalSettingsSF3.ForceBalancer;
		if (string.IsNullOrEmpty(name))
		{
			DebugCurrentServerController @object = online.AddLine(serverSelectPrefab, null) as DebugCurrentServerController;
			devServerChooser.DevServerChosen -= @object.OnDevServerChosen;
			devServerChooser.DevServerChosen += @object.OnDevServerChosen;
		}
		else
		{
			online.AddLine(string.Empty, delegate(DebugTextLineController line)
			{
				line.value.text = "Forcing server: " + name;
				line.value.color = Color.red;
			}).HideSeparator();
		}
	}

	private void updatePocketStatus(DebugTextLineController line)
	{
		bool insideDarkPocket = NetworkConnection.current.InsideDarkPocket;
		line.value.text = string.Format("{0}", insideDarkPocket.ToString());
		if (insideDarkPocket)
		{
			line.value.color = FPSNorm;
		}
		else
		{
			line.value.color = FPSGood;
		}
	}

	private void onResetButtonPress()
	{
		NetworkConnection.Send("cheat_reset_player", new Empty(), delegate(NetworkEvent e)
		{
			if (!e.success)
			{
				Debug.LogError("Error: Reset Player Failed");
			}
			NekkiUtils.ClearAllApplication();
		});
	}

	private void openContainerAtID(int ID)
	{
		foreach (DebugListContainerController lineContainer in lineContainers)
		{
			lineContainer.setVisible(false);
		}
		lineContainers[ID].setVisible(true);
	}

	public void onPrevListClick()
	{
		currentContainerID--;
		if (currentContainerID < 0)
		{
			currentContainerID = lineContainers.Count - 1;
		}
		openContainerAtID(currentContainerID);
	}

	public void onNextListClick()
	{
		currentContainerID++;
		if (currentContainerID >= lineContainers.Count)
		{
			currentContainerID = 0;
		}
		openContainerAtID(currentContainerID);
	}

	private void updateFPS(DebugTextLineController line)
	{
		UILabel value = line.value;
		value.text = FpsInfo;
		if (FpsInfoFloat >= 58f)
		{
			value.color = FPSGood;
		}
		else if (FpsInfoFloat >= 35f)
		{
			value.color = FPSNorm;
		}
		else
		{
			value.color = FPSLow;
		}
	}

	private void updateNetworkStatus(DebugTextLineController line)
	{
		line.value.text = string.Format("{0}", GetConnectionStatus);
		line.value.color = ((!(line.value.text == "Established")) ? FPSLow : FPSGood);
	}

	private void updateServerName(DebugTextLineController line)
	{
		line.value.text = "to be implemented";
	}

	private void updateServerIP(DebugTextLineController line)
	{
		line.value.text = NetworkConnection.current.Server + ":" + NetworkConnection.current.TCPPort;
	}

	private DebugListContainerController AddListContainer(string title)
	{
		GameObject gameObject = NGUITools.AddChild(fullGO, listContainerPrefab);
		DebugListContainerController component = gameObject.GetComponent<DebugListContainerController>();
		component.setup(title);
		component.setVisible(false);
		lineContainers.Add(component);
		return component;
	}

	private void Update()
	{
		if (Time.unscaledTime >= nextUpdateTime)
		{
			nextUpdateTime = Time.unscaledTime + 0.5f;
			UpdateInfo();
		}
	}

	public static void Log(string text)
	{
		if ((bool)_instance)
		{
			UILabel consoleTextOutput = _instance.ConsoleTextOutput;
			consoleTextOutput.text = consoleTextOutput.text + text + "\n";
		}
	}

	public static void Clear()
	{
		if ((bool)_instance && (bool)_instance.ConsoleTextOutput)
		{
			_instance.ConsoleTextOutput.text = string.Empty;
		}
	}

	private void UpdateInfo()
	{
		switch (state)
		{
		case DebugUIStates.Short:
			fpsLabelShort.text = FpsInfo;
			if (FpsInfoFloat >= 58f)
			{
				fpsLabelShort.color = FPSGood;
			}
			else if (FpsInfoFloat >= 35f)
			{
				fpsLabelShort.color = FPSNorm;
			}
			else
			{
				fpsLabelShort.color = FPSLow;
			}
			break;
		case DebugUIStates.Full:
		{
			foreach (DebugListContainerController lineContainer in lineContainers)
			{
				lineContainer.UpdateLines();
			}
			break;
		}
		}
	}

	private void Short()
	{
		_multiTween.TweenOut();
		state = DebugUIStates.Short;
	}

	private void Full()
	{
		_multiTween.TweenIn();
		state = DebugUIStates.Full;
	}

	public void ToggleDevServerChooser()
	{
		_devChooserShowed = !_devChooserShowed;
		if (_devChooserShowed)
		{
			_devChooserTween.TweenOut();
		}
		else
		{
			_devChooserTween.TweenIn();
		}
	}
}
