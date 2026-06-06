using System;
using System.Text;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class AnimatedTransform
	{
		public static readonly AnimatedTransform IDENTITY;

		public bool animateThisFrame;

		public Quaternion rotation { get; private set; }

		public Vector3 position { get; private set; }

		static AnimatedTransform()
		{
			IDENTITY = new AnimatedTransform(Vector3.zero, Quaternion.identity);
			IDENTITY = new AnimatedTransform(Vector3.zero, Quaternion.identity);
		}

		public AnimatedTransform(Vector3 newPosition, Quaternion newRotation)
		{
			position = newPosition;
			rotation = newRotation;
			animateThisFrame = false;
		}

		public AnimatedTransform()
			: this(Vector3.zero, Quaternion.identity)
		{
		}

		public void SetRotation(Quaternion newRotation)
		{
			rotation = newRotation;
		}

		public void SetPosition(Vector3 newPosition)
		{
			position = newPosition;
		}

		public void AddPosition(Vector3 addToPos)
		{
			position += addToPos;
		}

		public void AddRotation(Quaternion addRotation)
		{
			rotation *= addRotation;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat("Position: ", position, "; Rotation: ", rotation.eulerAngles));
			return stringBuilder.ToString();
		}

		public static void CopyBoneTransform(AnimatedTransform from, AnimatedTransform to)
		{
			to.SetPosition(from.position);
			to.SetRotation(from.rotation);
		}
	}
}
