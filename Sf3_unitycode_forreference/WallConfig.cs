using System.Linq;
using UnityEngine;

public class WallConfig : MonoBehaviour
{
	public enum EWallType
	{
		NONE = 0,
		STICKY = 1,
		RECOIL = 2
	}

	public Collider[] stickyWalls;

	public Collider[] recoilWalls;

	public static WallConfig instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	public EWallType GetWallType(Collider colliderToCheck)
	{
		if (stickyWalls.Any((Collider coll) => coll == colliderToCheck))
		{
			return EWallType.STICKY;
		}
		return recoilWalls.Any((Collider coll) => coll == colliderToCheck) ? EWallType.RECOIL : EWallType.NONE;
	}
}
