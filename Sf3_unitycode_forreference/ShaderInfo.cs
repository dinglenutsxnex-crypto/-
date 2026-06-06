using UnityEngine;

public class ShaderInfo : MonoBehaviour
{
	public Shader Shader;

	public Renderer Renderer;

	internal void Start()
	{
		if ((bool)Renderer && (bool)Shader)
		{
			Renderer.sharedMaterial.shader = Shader;
		}
	}
}
