using UnityEngine;

public class TestTextures : MonoBehaviour
{
	private static TestTextures _instance;

	private static bool _shedule;

	public Texture Body;

	public Texture Head;

	internal void Start()
	{
		_instance = this;
		if (_shedule)
		{
			Replace();
		}
		else
		{
			_shedule = true;
		}
	}

	private void Update()
	{
		if (_shedule)
		{
			Replace();
		}
	}

	public static void Replace()
	{
		if (!_instance)
		{
			_shedule = true;
			return;
		}
		_shedule = false;
		GameObject gameObject = GameObject.Find("test_player");
		SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			skinnedMeshRenderer.sharedMaterial = new Material(skinnedMeshRenderer.sharedMaterial);
			skinnedMeshRenderer.sharedMaterial.mainTexture = ((!skinnedMeshRenderer.gameObject.name.Contains("prefab")) ? _instance.Head : _instance.Body);
		}
	}
}
