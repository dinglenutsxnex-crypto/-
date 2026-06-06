using System.Linq;
using UnityEngine;

public class UnityCollisionRecognizer : MonoBehaviour
{
	private Collider _collider;

	private void Awake()
	{
		_collider = GetComponents<Collider>().FirstOrDefault((Collider c) => c.isTrigger);
		if (null == _collider)
		{
			Debug.LogError("Collider of wall is null");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		ModelFinder component = other.gameObject.GetComponent<ModelFinder>();
		if (!(null == component))
		{
			component.ModelOfObject.WallHit(WallConfig.instance.GetWallType(_collider), _collider);
		}
	}
}
