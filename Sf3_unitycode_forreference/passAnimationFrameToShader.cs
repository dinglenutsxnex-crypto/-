using UnityEngine;

public class passAnimationFrameToShader : MonoBehaviour
{
	private void Update()
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		float normalizedTime = component["Scene"].normalizedTime;
		float num = 1f;
		float value = 1f;
		if (normalizedTime <= 0.2f)
		{
			num = normalizedTime / 0.18f;
		}
		else if (normalizedTime > 0.2f && normalizedTime <= 0.53f)
		{
			num = 1f;
		}
		else if (normalizedTime > 0.53f && normalizedTime <= 0.75f)
		{
			num = 1f - (normalizedTime - 0.53f) / 0.22000003f;
			value = num;
		}
		else
		{
			num = 0f;
			value = 0f;
		}
		Debug.Log(num);
		Renderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material.SetFloat("_AnimState", num);
			componentsInChildren[i].material.SetFloat("_Transparency", value);
		}
	}
}
