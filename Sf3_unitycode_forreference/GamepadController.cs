using System;
using System.Linq;
using Nekki;
using UnityEngine;

public class GamepadController : AbstractController
{
	private static GamepadController _instance;

	private const float RELEASE_SQR_RADIUS = 0.01f;

	private EQuadrants[] lastQuadrants = new EQuadrants[2]
	{
		EQuadrants.None,
		EQuadrants.None
	};

	private int lastJoystickCount;

	private float[] angelsArray;

	public int Count
	{
		get
		{
			return lastJoystickCount;
		}
	}

	public static GamepadController Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = new GameObject("gamePadController").AddComponent<GamepadController>();
			StaticObjectsManager.AddObject(_instance.gameObject, false);
			return _instance;
		}
	}

	public event Action<int> JoystickConnect = delegate
	{
	};

	public event Action<int> JoystickDisconnect = delegate
	{
	};

	public void SubscribeUIElement(GameObject gameObject)
	{
		if (lastJoystickCount > 0)
		{
			gameObject.SetActive(false);
		}
		JoystickConnect += delegate
		{
			gameObject.SetActive(false);
		};
		JoystickDisconnect += delegate(int count)
		{
			if (count == 0)
			{
				gameObject.SetActive(true);
			}
		};
	}

	private void Awake()
	{
		Initialize(112.5f, new float[8] { 45f, 45f, 45f, 45f, 45f, 45f, 45f, 45f });
	}

	private void Initialize(float offset, float[] quadrantRanges)
	{
		angelsArray = new float[quadrantRanges.Length];
		float num = offset;
		for (int i = 0; i < angelsArray.Length; i++)
		{
			angelsArray[i] = num;
			if (angelsArray[i] < 0f)
			{
				angelsArray[i] = 360f + angelsArray[i];
			}
			num -= quadrantRanges[i];
		}
		if (!(360f + num).IsEqualByEpsilon(offset))
		{
			Debug.LogError("Bad Quadrant Ranges!");
		}
	}

	private void CheckKeys()
	{
		Array values = Enum.GetValues(typeof(KeyCode));
		for (int i = 0; i < values.GetLength(0); i++)
		{
			if (Input.GetKeyDown((KeyCode)values.GetValue(i)))
			{
				Debug.Log(values.GetValue(i).ToString());
			}
		}
	}

	private void CheckAxis()
	{
		if (Mathf.Abs(Input.GetAxis("TEST_X")) > 0.1f)
		{
			print("TEXT_X " + Input.GetAxis("TEST_X"));
		}
		if (Mathf.Abs(Input.GetAxis("TEST_Y")) > 0.1f)
		{
			print("TEXT_Y " + Input.GetAxis("TEST_Y"));
		}
		for (int i = 3; i <= 15; i++)
		{
			string text = "TEST_" + i;
			if (Mathf.Abs(Input.GetAxis(text)) > 0.1f)
			{
				print(text + " " + Input.GetAxis(text));
			}
		}
	}

	private void print(string text)
	{
	}

	private void ListenDirectionChanging(int gpIndex)
	{
		float axis = Input.GetAxis("JStickX" + gpIndex);
		float num = Input.GetAxis("JStickY" + gpIndex);
		if (axis * axis + num * num <= 0.01f)
		{
			axis = Input.GetAxis("JCrossX" + gpIndex + "_Android");
			num = 0f - Input.GetAxis("JCrossY" + gpIndex + "_Android");
		}
		EQuadrants quadrantForPoint = GetQuadrantForPoint(axis, num);
		int num2 = gpIndex - 1;
		if (quadrantForPoint != EQuadrants.None)
		{
			if (quadrantForPoint != lastQuadrants[num2])
			{
				callEvent(0, new Key(KeyCode.None, quadrantForPoint, gpIndex));
				if (lastQuadrants[num2] != EQuadrants.None)
				{
					callEvent(1, new Key(KeyCode.None, lastQuadrants[num2], gpIndex));
				}
				lastQuadrants[num2] = quadrantForPoint;
			}
		}
		else
		{
			if (lastQuadrants[num2] != EQuadrants.None)
			{
				callEvent(1, new Key(KeyCode.None, lastQuadrants[num2], gpIndex));
			}
			lastQuadrants[num2] = quadrantForPoint;
		}
	}

	private void Update()
	{
		CheckConnection();
		ListenDirectionChanging(1);
		ListenDirectionChanging(2);
		TrackKeys();
	}

	private void CheckConnection()
	{
		int count = Input.GetJoystickNames().ToList().FindAll((string joystick) => joystick != string.Empty)
			.Count;
		if (lastJoystickCount < count)
		{
			lastJoystickCount = count;
			this.JoystickConnect(count);
		}
		else if (lastJoystickCount > count)
		{
			lastJoystickCount = count;
			this.JoystickDisconnect(count);
		}
	}

	private EQuadrants GetQuadrantForPoint(float x, float y)
	{
		if (x * x + y * y <= 0.01f)
		{
			return EQuadrants.None;
		}
		float num = GetAngleForPoint(x, y) * 57.29578f;
		for (int i = 0; i < angelsArray.Length; i++)
		{
			float num2 = ((i + 1 >= angelsArray.Length) ? angelsArray[0] : angelsArray[i + 1]);
			if (angelsArray[i] > num2)
			{
				if (angelsArray[i] > num && num2 < num)
				{
					return (EQuadrants)(i + 1);
				}
			}
			else if (angelsArray[i] > num || num2 < num)
			{
				return (EQuadrants)(i + 1);
			}
		}
		return EQuadrants.None;
	}

	private float GetAngleForPoint(float x, float y)
	{
		if (x.IsEqualByEpsilon(0f))
		{
			return (!(y >= 0f)) ? 4.712389f : ((float)Math.PI / 2f);
		}
		float num = Mathf.Atan(Mathf.Abs(y) / Mathf.Abs(x));
		if (x > 0f && y >= 0f)
		{
			return num;
		}
		if (x < 0f && y >= 0f)
		{
			return (float)Math.PI - num;
		}
		if (x < 0f && y < 0f)
		{
			return num + (float)Math.PI;
		}
		if (x > 0f && y < 0f)
		{
			return (float)Math.PI * 2f - num;
		}
		return float.NaN;
	}
}
