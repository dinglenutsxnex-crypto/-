using SF3;
using UnityEngine;

public class VertexAnimation : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve vertexMovementCurve;

	[SerializeField]
	private float factorX;

	[SerializeField]
	private float factorY;

	[SerializeField]
	private float animCycleTime = 1f;

	private Material material;

	private bool moveForward;

	private float timer;

	private const string MATERIAL_PARAMETER_NAME_1 = "_VertexAnimationFactorX";

	private const string MATERIAL_PARAMETER_NAME_2 = "_VertexAnimationFactorY";

	private MeshRenderer meshRender;

	private void Start()
	{
		meshRender = GetComponent<MeshRenderer>();
		material = meshRender.material;
	}

	private void Update()
	{
		if (meshRender.material != material)
		{
			material = meshRender.material;
		}
		timer += GameTimeController.deltaTimePaused;
		float num = timer / animCycleTime;
		if (!moveForward)
		{
			num = 1f - num;
		}
		material.SetFloat("_VertexAnimationFactorX", vertexMovementCurve.Evaluate(num) * factorX);
		material.SetFloat("_VertexAnimationFactorY", vertexMovementCurve.Evaluate(num) * factorY);
		if (timer >= animCycleTime)
		{
			timer = 0f;
			moveForward = !moveForward;
		}
	}
}
