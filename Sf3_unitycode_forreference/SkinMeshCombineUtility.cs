using UnityEngine;

public class SkinMeshCombineUtility
{
	public struct MeshInstance
	{
		public Mesh mesh;

		public int subMeshIndex;

		public Matrix4x4 transform;
	}

	public static Mesh Combine(MeshInstance[] combines)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < combines.Length; i++)
		{
			MeshInstance meshInstance = combines[i];
			if ((bool)meshInstance.mesh)
			{
				num += meshInstance.mesh.vertexCount;
			}
		}
		for (int j = 0; j < combines.Length; j++)
		{
			MeshInstance meshInstance2 = combines[j];
			if ((bool)meshInstance2.mesh)
			{
				num2 += meshInstance2.mesh.GetTriangles(meshInstance2.subMeshIndex).Length;
			}
		}
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector4[] array3 = new Vector4[num];
		Vector2[] array4 = new Vector2[num];
		Vector2[] array5 = new Vector2[num];
		int offset = 0;
		for (int k = 0; k < combines.Length; k++)
		{
			MeshInstance meshInstance3 = combines[k];
			if ((bool)meshInstance3.mesh)
			{
				Copy(meshInstance3.mesh.vertexCount, meshInstance3.mesh.vertices, array, ref offset, meshInstance3.transform);
			}
		}
		offset = 0;
		for (int l = 0; l < combines.Length; l++)
		{
			MeshInstance meshInstance4 = combines[l];
			if ((bool)meshInstance4.mesh)
			{
				Matrix4x4 transform = meshInstance4.transform;
				transform = transform.inverse.transpose;
				CopyNormal(meshInstance4.mesh.vertexCount, meshInstance4.mesh.normals, array2, ref offset, transform);
			}
		}
		offset = 0;
		for (int m = 0; m < combines.Length; m++)
		{
			MeshInstance meshInstance5 = combines[m];
			if ((bool)meshInstance5.mesh)
			{
				Matrix4x4 transform2 = meshInstance5.transform;
				transform2 = transform2.inverse.transpose;
				CopyTangents(meshInstance5.mesh.vertexCount, meshInstance5.mesh.tangents, array3, ref offset, transform2);
			}
		}
		offset = 0;
		for (int n = 0; n < combines.Length; n++)
		{
			MeshInstance meshInstance6 = combines[n];
			if ((bool)meshInstance6.mesh)
			{
				Copy(meshInstance6.mesh.vertexCount, meshInstance6.mesh.uv, array4, ref offset);
			}
		}
		offset = 0;
		for (int num3 = 0; num3 < combines.Length; num3++)
		{
			MeshInstance meshInstance7 = combines[num3];
			if ((bool)meshInstance7.mesh)
			{
				Copy(meshInstance7.mesh.vertexCount, meshInstance7.mesh.uv2, array5, ref offset);
			}
		}
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array4;
		mesh.uv2 = array5;
		mesh.tangents = array3;
		mesh.subMeshCount = combines.Length;
		for (int num7 = 0; num7 < combines.Length; num7++)
		{
			MeshInstance meshInstance8 = combines[num7];
			int[] triangles = meshInstance8.mesh.GetTriangles(meshInstance8.subMeshIndex);
			int[] array6 = new int[triangles.Length];
			for (int num8 = 0; num8 < triangles.Length; num8++)
			{
				array6[num8] = triangles[num8] + num5;
			}
			num4 += triangles.Length;
			mesh.SetTriangles(array6, num6++);
			num5 += meshInstance8.mesh.vertexCount;
		}
		mesh.name = "Combined Mesh";
		return mesh;
	}

	private static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = transform.MultiplyPoint(src[i]);
		}
		offset += vertexcount;
	}

	private static void CopyBoneWei(int vertexcount, BoneWeight[] src, BoneWeight[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = src[i];
		}
		offset += vertexcount;
	}

	private static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
		}
		offset += vertexcount;
	}

	private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = src[i];
		}
		offset += vertexcount;
	}

	private static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			Vector4 vector = src[i];
			Vector3 v = new Vector3(vector.x, vector.y, vector.z);
			v = transform.MultiplyVector(v).normalized;
			dst[i + offset] = new Vector4(v.x, v.y, v.z, vector.w);
		}
		offset += vertexcount;
	}
}
