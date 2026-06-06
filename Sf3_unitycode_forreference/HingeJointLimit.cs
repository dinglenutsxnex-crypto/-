using UnityEngine;

public class HingeJointLimit : MonoBehaviour
{
	public Transform boneEnd;

	public Transform boneStart;

	public Transform parent;

	public string boneStartName;

	public string boneEndName;

	public float offset = 40f;

	private HingeJoint joint;

	public AnimationCurve animationCurve;

	public GameObject anchor;

	private void Update()
	{
		if (!(joint == null))
		{
			float num = 0f;
			if (boneEnd == null || boneStart == null)
			{
				num = offset;
			}
			else
			{
				Vector3 vector = boneEnd.position - boneStart.position;
				num = Vector3.Angle(vector, Vector3.ProjectOnPlane(vector, Vector3.up));
				num = (0f - animationCurve.Evaluate(num / 90f)) * 90f + offset;
			}
			JointLimits limits = joint.limits;
			limits.min = num;
			joint.limits = limits;
		}
	}

	private void InitAnchor()
	{
		anchor = new GameObject(base.gameObject.name + "_anchor");
		anchor.transform.parent = base.transform.parent;
		anchor.transform.position = base.transform.position;
		anchor.transform.rotation = base.transform.rotation;
		base.transform.parent = null;
	}

	private void Start()
	{
		joint = GetComponent<HingeJoint>();
		InitAnchor();
	}
}
