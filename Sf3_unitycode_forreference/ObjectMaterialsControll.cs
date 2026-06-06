using UnityEngine;

public class ObjectMaterialsControll : MonoBehaviour
{
	private SkinnedMeshRenderer[] _skMeshRenderers;

	public Color changeTo = Color.red;

	public bool changeNow;

	private void Awake()
	{
		changeNow = false;
	}

	private void Start()
	{
		_skMeshRenderers = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
	}

	private void Update()
	{
		if (!changeNow)
		{
			return;
		}
		changeNow = false;
		SkinnedMeshRenderer[] skMeshRenderers = _skMeshRenderers;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skMeshRenderers)
		{
			for (int j = 0; j < skinnedMeshRenderer.sharedMesh.colors.Length; j++)
			{
				skinnedMeshRenderer.sharedMesh.colors[j] = changeTo;
			}
		}
	}
}
