using UnityEngine;

public class ClientSettings : ScriptableObject
{
	[SerializeField]
	private float _delayForWaitResponseBeforeBlock = 0.5f;

	public float DelayForWaitResponseBeforeBlock
	{
		get
		{
			return _delayForWaitResponseBeforeBlock;
		}
	}
}
