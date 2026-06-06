using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractController : ExtentionBehaviour
{
	public class Key
	{
		public KeyCode KeyCode { get; private set; }

		public EQuadrants Quadrant { get; private set; }

		public int ModelID { get; private set; }

		public Key(KeyCode keyCode, EQuadrants quadrant, int modelID)
		{
			Quadrant = quadrant;
			KeyCode = keyCode;
			ModelID = modelID;
		}
	}

	public const int onKeyPress = 0;

	public const int onKeyRelease = 1;

	protected List<Key> keyCodes = new List<Key>();

	public void AddTrakedKey(KeyCode key, EQuadrants quadrant, int modelID = -1)
	{
		keyCodes.Add(new Key(key, quadrant, modelID));
	}

	protected void TrackKeys()
	{
		for (int i = 0; i < keyCodes.Count; i++)
		{
			if (Input.GetKeyDown(keyCodes[i].KeyCode))
			{
				callEvent(0, keyCodes[i]);
			}
			if (Input.GetKeyUp(keyCodes[i].KeyCode))
			{
				callEvent(1, keyCodes[i]);
			}
		}
	}

	public void OpenInterface()
	{
		foreach (Key keyCode in keyCodes)
		{
			if (Input.GetKey(keyCode.KeyCode))
			{
				callEvent(1, new Key(keyCode.KeyCode, keyCode.Quadrant, 1));
			}
		}
	}
}
