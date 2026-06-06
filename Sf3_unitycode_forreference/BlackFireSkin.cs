using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SF3.GameModels;
using UnityEngine;

public class BlackFireSkin : MonoBehaviour
{
	private struct ClothBone
	{
		public Transform transform;

		public Vector3 localPos;

		public bool useGlobalDirection;
	}

	private Dictionary<string, ClothBone> mirrorsUp;

	private bool init;

	public Transform mainBone;

	public int upperMaterialRenderQueue;

	private Model model;

	public float baseAlpha = 2f;

	public Material[] materials;

	public Vector3 directionFactor;

	public Vector3 directionFacorMirror;

	public List<SkinnedMeshRenderer> skinnedMeshes;

	private float mirrorFactor;

	public float mirroringSpeed = 2f;

	private float directionLerp;

	public float directionChangeSpeed = 2f;

	private void Awake()
	{
		mirrorsUp = new Dictionary<string, ClothBone>();
		AddNodesToClothBones(mainBone, true);
		materials[0].renderQueue = upperMaterialRenderQueue;
	}

	private void AddNodesToClothBones(Transform root, bool useGlobalDirection)
	{
		List<Transform> list = (from x in root.GetComponentsInChildren<Transform>()
			where x.name.Contains("_cl")
			select x).ToList();
		foreach (Transform item in list)
		{
			mirrorsUp.Add(item.parent.name, new ClothBone
			{
				localPos = item.transform.localPosition,
				transform = item.transform,
				useGlobalDirection = useGlobalDirection
			});
		}
	}

	public void Init(GameObject target, Transform baseOrientation, Model model)
	{
		this.model = model;
		StartCoroutine(ChangeColourCoroutine());
		SetColor(0f);
		InitSkeleton();
		init = true;
		mirrorFactor = ((!model.GetMirrored()) ? 0f : 1f);
		directionLerp = model.moveControl.directionSign;
	}

	private void InitSkeleton()
	{
		foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshes)
		{
			List<Transform> list = new List<Transform>();
			Transform[] bones = skinnedMesh.bones;
			foreach (Transform transform in bones)
			{
				if (!transform.name.Contains("_cl"))
				{
					Transform item = model.GetBone(transform.name).transform;
					list.Add(item);
				}
				else
				{
					list.Add(transform);
					transform.transform.parent = base.transform;
				}
			}
			skinnedMesh.rootBone = model.GetBone(skinnedMesh.rootBone.name).transform;
			skinnedMesh.bones = list.ToArray();
			if (!init)
			{
				Object.Destroy(mainBone.gameObject);
			}
		}
	}

	private IEnumerator ChangeColourCoroutine()
	{
		float t2;
		for (t2 = 0f; t2 < 1f; t2 += Time.deltaTime)
		{
			SetColor(t2);
			t2 += Time.deltaTime;
			yield return null;
		}
	}

	private void SetColor(float t)
	{
		Material[] array = materials;
		foreach (Material material in array)
		{
			material.SetFloat("_vertexAlphaMult", baseAlpha * t);
		}
	}

	private Vector3 GetMirrorFactor()
	{
		if (model.GetMirrored())
		{
			mirrorFactor += Time.deltaTime * mirroringSpeed;
		}
		else
		{
			mirrorFactor -= Time.deltaTime * mirroringSpeed;
		}
		mirrorFactor = Mathf.Clamp01(mirrorFactor);
		return Vector3.Lerp(directionFactor, directionFacorMirror, mirrorFactor);
	}

	private float GetDirectionFactor()
	{
		directionLerp += (float)model.moveControl.directionSign * Time.deltaTime * directionChangeSpeed;
		directionLerp = Mathf.Clamp(directionLerp, -1f, 1f);
		return 0f - directionLerp;
	}

	private void LateUpdate()
	{
		if (!init)
		{
			return;
		}
		Vector3 vector = GetMirrorFactor();
		foreach (KeyValuePair<string, ClothBone> item in mirrorsUp)
		{
			Transform transform = model.GetBone(item.Key).transform;
			ClothBone value = item.Value;
			Vector3 vector2 = vector.x * Vector3.right * GetDirectionFactor() + vector.y * Vector3.up + vector.z * Vector3.forward;
			value.transform.position = transform.transform.position + vector2 * value.localPos.magnitude;
		}
	}
}
