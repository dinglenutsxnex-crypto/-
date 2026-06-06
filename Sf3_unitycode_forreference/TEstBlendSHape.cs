using UnityEngine;

public class TEstBlendSHape : MonoBehaviour
{
	public SkinnedMeshRenderer skinnedMeshRenderer;

	public bool playbl;

	private void Start()
	{
	}

	private void Update()
	{
		if (playbl)
		{
			playbl = false;
		}
	}
}
