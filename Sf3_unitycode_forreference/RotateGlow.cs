using SF3;
using UnityEngine;

public class RotateGlow : MonoBehaviour
{
	public Vector3 Angle;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.rotation *= Quaternion.Euler(Angle * GameTimeController.unscaledDeltaTime);
	}
}
