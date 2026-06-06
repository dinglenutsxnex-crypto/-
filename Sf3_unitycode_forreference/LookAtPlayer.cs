using System.Linq;
using SF3.GameModels;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
	private Transform _player;

	private Transform _transform;

	private Vector3 _lookDirection;

	public string BoneName;

	public float RotationSpeed;

	public float MaxRestrictionX;

	public float MinRestrictionX;

	public float MaxRestrictionY;

	public float MinRestrictionY;

	private void Awake()
	{
		_transform = base.transform;
		if (string.IsNullOrEmpty(BoneName))
		{
			BoneName = "head";
		}
		if (RotationSpeed <= 0f)
		{
			RotationSpeed = 75f;
		}
	}

	private void Update()
	{
		if (null == _player)
		{
			SearchForPlayer();
		}
		else
		{
			TrackPlayer();
		}
	}

	private void TrackPlayer()
	{
		_lookDirection = _player.position - _transform.position;
		TrimToRestriction();
		Rotate();
	}

	private void Rotate()
	{
		Quaternion b = Quaternion.LookRotation(_lookDirection.normalized);
		float num = Quaternion.Angle(_transform.rotation, b);
		float num2 = num / RotationSpeed;
		float t = Mathf.Min(1f, Time.deltaTime / num2);
		Quaternion rotation = Quaternion.Slerp(_transform.rotation, b, t);
		if (!float.IsNaN(rotation.x + rotation.y + rotation.z + rotation.w))
		{
			_transform.rotation = rotation;
		}
	}

	private void TrimToRestriction()
	{
		TrimToX();
		TrimToY();
	}

	private void TrimToX()
	{
		if (_lookDirection.x < MinRestrictionX)
		{
			_lookDirection.x = MinRestrictionX;
		}
		else if (_lookDirection.x > MaxRestrictionX)
		{
			_lookDirection.x = MaxRestrictionX;
		}
	}

	private void TrimToY()
	{
		if (_lookDirection.y < MinRestrictionY)
		{
			_lookDirection.y = MinRestrictionY;
		}
		else if (_lookDirection.y > MaxRestrictionY)
		{
			_lookDirection.y = MaxRestrictionY;
		}
	}

	private void SearchForPlayer()
	{
		Model model2 = Object.FindObjectsOfType<Model>().FirstOrDefault((Model model) => model.isPlayer);
		if (!(null == model2))
		{
			_player = model2.GetBone(BoneName).transform;
		}
	}
}
