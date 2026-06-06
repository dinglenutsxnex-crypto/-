using UnityEngine;

public class passYposToShader : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material material = componentsInChildren[i].material;
			material.SetFloat("_GameObYPos", base.gameObject.transform.position.y);
		}
	}
}
