using UnityEngine;

namespace SF3.VerlePhysics
{
	internal static class ControllerUtils
	{
		public static void ExtractTransform(Matrix4x4 m, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			position = m.GetColumn(3);
			rotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
			scale = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
		}

		public static Matrix4x4 GenerateMatrixFrom3Nodes(Node main, Node help1, Node help2)
		{
			Matrix4x4 result = default(Matrix4x4);
			Vector3[] array = new Vector3[3];
			Vector4 v = main.position;
			v[3] = 1f;
			array[0] = Vector3.Normalize(help1.position - main.position);
			array[1] = Vector3.Normalize(Vector3.Cross(array[0], help2.position - help1.position));
			array[2] = Vector3.Normalize(Vector3.Cross(array[0], array[1]));
			result.SetColumn(0, array[0]);
			result.SetColumn(1, array[1]);
			result.SetColumn(2, array[2]);
			result.SetColumn(3, v);
			return result;
		}
	}
}
