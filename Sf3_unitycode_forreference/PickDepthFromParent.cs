using UnityEngine;

public class PickDepthFromParent : MonoBehaviour
{
	public UIPanel parent;

	public int depthOffset;

	private UIPanel current;

	private void Awake()
	{
		current = GetComponent<UIPanel>();
	}

	public void ManualUpdate()
	{
		if (current.depth != parent.depth + depthOffset)
		{
			current.depth = parent.depth + depthOffset;
		}
	}

	private void Update()
	{
		ManualUpdate();
	}
}
