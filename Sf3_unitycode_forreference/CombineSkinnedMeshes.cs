using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CombineSkinnedMeshes : MonoBehaviour
{
	public bool combineAllMeshesNow;

	public void Init()
	{
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Hashtable hashtable = new Hashtable();
		int num = 0;
		int num2 = 0;
		List<SkinMeshCombineUtility.MeshInstance> list = new List<SkinMeshCombineUtility.MeshInstance>();
		List<Material> list2 = new List<Material>();
		int num3 = 0;
		List<Transform> list3 = new List<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			num3 += componentsInChildren[i].sharedMesh.boneWeights.Length;
			Transform[] bones = componentsInChildren[i].bones;
			foreach (Transform item in bones)
			{
				if (!list3.Contains(item))
				{
					list3.Add(item);
				}
			}
		}
		BoneWeight[] array = new BoneWeight[num3];
		Transform[] array2 = new Transform[list3.Count];
		Matrix4x4[] array3 = new Matrix4x4[list3.Count];
		Transform[] array4 = new Transform[array2.Length];
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = componentsInChildren[k];
			if (skinnedMeshRenderer == null)
			{
				continue;
			}
			SkinMeshCombineUtility.MeshInstance item2 = default(SkinMeshCombineUtility.MeshInstance);
			item2.mesh = skinnedMeshRenderer.sharedMesh;
			for (int l = 0; l < skinnedMeshRenderer.sharedMaterials.Length; l++)
			{
				list2.Add(skinnedMeshRenderer.sharedMaterials[l]);
			}
			if (!skinnedMeshRenderer.enabled || !(item2.mesh != null))
			{
				continue;
			}
			item2.transform = worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
			skinnedMeshRenderer.sharedMaterials = new Material[1];
			for (int m = 0; m < skinnedMeshRenderer.sharedMesh.subMeshCount; m++)
			{
				item2.subMeshIndex = m;
				list.Add(item2);
			}
			for (int n = 0; n < skinnedMeshRenderer.bones.Length; n++)
			{
				bool flag = false;
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					if (array4[num4] != null && skinnedMeshRenderer.bones[n] == array4[num4])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				for (int num5 = 0; num5 < array2.Length; num5++)
				{
					if (array4[num5] == null)
					{
						array4[num5] = skinnedMeshRenderer.bones[n];
						break;
					}
				}
				array2[num] = skinnedMeshRenderer.bones[n];
				array3[num] = skinnedMeshRenderer.sharedMesh.bindposes[n];
				hashtable.Add(skinnedMeshRenderer.bones[n].name, num);
				num++;
			}
			for (int num6 = 0; num6 < skinnedMeshRenderer.sharedMesh.boneWeights.Length; num6++)
			{
				array[num2] = recalculateIndexes(skinnedMeshRenderer.sharedMesh.boneWeights[num6], hashtable, skinnedMeshRenderer.bones);
				num2++;
			}
			componentsInChildren[k].enabled = false;
		}
		SkinMeshCombineUtility.MeshInstance[] combines = list.ToArray();
		if (GetComponent<SkinnedMeshRenderer>() == null)
		{
			base.gameObject.AddComponent<SkinnedMeshRenderer>();
		}
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		component.sharedMesh = SkinMeshCombineUtility.Combine(combines);
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		component.lightProbeUsage = LightProbeUsage.Off;
		component.reflectionProbeUsage = ReflectionProbeUsage.Off;
		component.sharedMesh.bindposes = array3;
		component.sharedMesh.boneWeights = array;
		component.bones = array2;
		component.sharedMaterials = list2.ToArray();
		component.sharedMesh.RecalculateNormals();
		component.sharedMesh.RecalculateBounds();
		component.enabled = true;
	}

	private static BoneWeight recalculateIndexes(BoneWeight bw, Hashtable boneHash, Transform[] meshBones)
	{
		BoneWeight result = bw;
		result.boneIndex0 = (int)boneHash[meshBones[bw.boneIndex0].name];
		result.boneIndex1 = (int)boneHash[meshBones[bw.boneIndex1].name];
		result.boneIndex2 = (int)boneHash[meshBones[bw.boneIndex2].name];
		result.boneIndex3 = (int)boneHash[meshBones[bw.boneIndex3].name];
		return result;
	}

	private void Update()
	{
		if (combineAllMeshesNow)
		{
			combineAllMeshesNow = false;
			Init();
		}
	}
}
