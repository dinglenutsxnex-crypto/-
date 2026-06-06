using System.Collections;
using UnityEngine;

public class Object_Universal_Controller : MonoBehaviour
{
	public float StartTime;

	private bool ifStart;

	public float MoveSpeedX;

	public float MoveSpeedY;

	public float MoveSpeedZ;

	public float RotateSpeedX;

	public float RotateSpeedY;

	public float RotateSpeedZ;

	public float ScaleSpeedX;

	public float ScaleSpeedY;

	public float ScaleSpeedZ;

	public float MaterialFadeIn;

	public float MaterialLifeTime;

	public float MaterialFadeOut;

	private void Start()
	{
		StartCoroutine(Example());
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(StartTime);
		ifStart = true;
		yield return new WaitForSeconds(MaterialFadeIn + MaterialLifeTime);
		ifStart = false;
	}

	private void Update()
	{
		if (ifStart)
		{
			base.transform.Translate(MoveSpeedX * Time.deltaTime, MoveSpeedY * Time.deltaTime, MoveSpeedZ * Time.deltaTime, Space.Self);
			base.transform.Rotate(RotateSpeedX * Time.deltaTime, RotateSpeedY * Time.deltaTime, RotateSpeedZ * Time.deltaTime, Space.Self);
			base.transform.localScale += new Vector3(ScaleSpeedX * Time.deltaTime, ScaleSpeedY * Time.deltaTime, ScaleSpeedZ * Time.deltaTime);
		}
	}
}
