using UnityEngine;

public class RagdolAlignment : MonoBehaviour
{
	public float boundsWidth = 10f;

	public float maxAngle = 10f;

	public Transform transf;

	public Rigidbody rigidBody;

	private Vector3 _pos;

	private Vector3 _startPos;

	private bool _inPhysics;

	private float _startAngle;

	public void Initialize(Transform rootBone)
	{
		transf = rootBone;
		rigidBody = transf.GetComponent<Rigidbody>();
	}

	public void Activate(bool zImpulse)
	{
		_startPos = transf.position;
		_inPhysics = true;
	}

	private void ClampAngle(float baseAngle)
	{
		_pos = transf.localEulerAngles;
		float num = baseAngle - _pos.y;
		if (Mathf.Abs(num) > maxAngle)
		{
			_pos.y = baseAngle + maxAngle * Mathf.Sign(0f - num);
		}
		transf.localEulerAngles = _pos;
	}

	private void Update()
	{
		if (_inPhysics)
		{
			_pos = transf.position;
			_pos.z = Mathf.Clamp(_pos.z, _startPos.z - boundsWidth, _startPos.z + boundsWidth);
			transf.position = _pos;
			_inPhysics = !rigidBody.isKinematic;
		}
	}
}
