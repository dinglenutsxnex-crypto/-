using System;
using Nekki;
using SF3_Attributes;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class Capsule
	{
		public string name { get; private set; }

		public string rigidBodyName { get; private set; }

		public CollisionVertex startVertex { get; private set; }

		public CollisionVertex finishVertex { get; private set; }

		public float radius { get; private set; }

		public Vector3 startMarginPosition { get; private set; }

		public Vector3 finishMarginPosition { get; private set; }

		public EquationLine equationLine { get; private set; }

		public bool collisible { get; private set; }

		public bool repulsive { get; private set; }

		public AttributeType defense { get; private set; }

		public Capsule leftMirrorCapsule { get; private set; }

		public Vector3 Center
		{
			get
			{
				return (startVertex.position + finishVertex.position) / 2f;
			}
		}

		public Capsule(CollisionVertex startVertexVal, CollisionVertex finishVertexVal, float newRadius, bool isCollisible, string newName, string _rigidBodyName)
		{
			rigidBodyName = _rigidBodyName;
			startVertex = startVertexVal;
			finishVertex = finishVertexVal;
			radius = newRadius;
			equationLine = new EquationLine();
			name = newName;
			SetCollisible(isCollisible);
			SetRepulsive(false);
		}

		public Capsule(CollisionVertex newStart, CollisionVertex newFinish, float newRadius, string newName, string _rigidBodyName)
			: this(newStart, newFinish, newRadius, false, newName, _rigidBodyName)
		{
		}

		public void SetLeftMirror(Capsule value)
		{
			leftMirrorCapsule = value;
		}

		public void SetCollisible(bool value)
		{
			collisible = value;
		}

		public void SetRepulsive(bool value)
		{
			repulsive = value;
		}

		public void SetDefense(AttributeType value)
		{
			defense = value;
		}

		public void RenderEquationLine()
		{
			RenderMargins();
			Vector2D.getEquationLine(startMarginPosition, finishMarginPosition, equationLine);
		}

		private void RenderMargins()
		{
			startMarginPosition = startVertex.position;
			finishMarginPosition = finishVertex.position;
		}
	}
}
