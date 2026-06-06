using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace SF3.GameModels
{
	public class ModelCapsules
	{
		private readonly List<GameObject> _renderers = new List<GameObject>();

		private bool _shouldRender;

		private Dictionary<string, Material> _cachedMaterials = new Dictionary<string, Material>();

		private List<Capsule> _capsules { get; set; }

		public List<Capsule> collisionCapsules { get; private set; }

		public List<Capsule> repulsiveCapsules { get; private set; }

		public Dictionary<int, List<Capsule>> attackingCapsules { get; private set; }

		public RepulsionRect repulsionRect { get; private set; }

		public RepulsionRect floorRepulsion { get; private set; }

		public static bool ShowCapsules { get; set; }

		public ModelCapsules()
		{
			_capsules = new List<Capsule>();
			collisionCapsules = new List<Capsule>();
			repulsiveCapsules = new List<Capsule>();
			attackingCapsules = new Dictionary<int, List<Capsule>>();
			CheckShowCapsules();
		}

		public void Clear()
		{
			_capsules.Clear();
			collisionCapsules.Clear();
			repulsiveCapsules.Clear();
			attackingCapsules.Clear();
			RepulsionRect repulsionRect2 = (floorRepulsion = null);
			this.repulsionRect = repulsionRect2;
		}

		public void AddCapsule(Capsule newCapsule)
		{
			_capsules.Add(newCapsule);
			InitCapsule(newCapsule);
		}

		public void AddCapsules(List<Capsule> newCapsule)
		{
			_capsules.AddRange(newCapsule);
			foreach (Capsule item in newCapsule)
			{
				InitCapsule(item);
			}
		}

		public void SetRepulsionRect(Dictionary<string, Transform> transforms, float widthSc, float heightSc, float minSize)
		{
			repulsionRect = new RepulsionRect(transforms, widthSc, heightSc, minSize);
		}

		public void SetFloorRepulsion(List<Transform> transforms, float widthSc, float heightSc)
		{
			floorRepulsion = new RepulsionRect(transforms, widthSc, heightSc);
		}

		private void InitCapsule(Capsule capsule)
		{
			if (capsule.collisible)
			{
				collisionCapsules.Add(capsule);
			}
			if (capsule.repulsive)
			{
				repulsiveCapsules.Add(capsule);
			}
		}

		public Capsule GetCapsule(string capsuleName)
		{
			foreach (Capsule capsule in _capsules)
			{
				if (capsule.name.Equals(capsuleName))
				{
					return capsule;
				}
			}
			return null;
		}

		public void ClearAttackingCapsules()
		{
			if (attackingCapsules.Count != 0)
			{
				attackingCapsules.Clear();
			}
		}

		public void ClearAttackingCapsules(int intervalId)
		{
			if (attackingCapsules.ContainsKey(intervalId))
			{
				attackingCapsules.Remove(intervalId);
			}
		}

		public void AddAttackingCapsules(int intervalId, List<Capsule> capsules)
		{
			attackingCapsules.Add(intervalId, GetAttackingCapsules(capsules));
		}

		private List<Capsule> GetAttackingCapsules(List<Capsule> capsules)
		{
			List<Capsule> list = new List<Capsule>();
			foreach (Capsule capsule in capsules)
			{
				foreach (Capsule capsule2 in _capsules)
				{
					if (capsule.name.Equals(capsule2.name))
					{
						list.Add(capsule2);
						break;
					}
				}
			}
			return list;
		}

		public void UpdateEquationLineCollision()
		{
			if (attackingCapsules != null)
			{
				foreach (KeyValuePair<int, List<Capsule>> attackingCapsule in attackingCapsules)
				{
					foreach (Capsule item in attackingCapsule.Value)
					{
						item.RenderEquationLine();
					}
				}
			}
			if (collisionCapsules != null)
			{
				foreach (Capsule collisionCapsule in collisionCapsules)
				{
					collisionCapsule.RenderEquationLine();
				}
			}
			if (repulsiveCapsules == null)
			{
				return;
			}
			foreach (Capsule repulsiveCapsule in repulsiveCapsules)
			{
				repulsiveCapsule.RenderEquationLine();
			}
		}

		private Material GetMaterialByColor(Color color)
		{
			string key = color.ToString();
			Material material;
			if (_cachedMaterials.ContainsKey(key))
			{
				material = _cachedMaterials[key];
			}
			else
			{
				Material material2 = new Material(Shader.Find("Transparent/Diffuse"));
				material2.color = color;
				material2.renderQueue = 10000;
				material = material2;
				_cachedMaterials.Add(key, material);
			}
			return material;
		}

		private void ReleaseCachedMaterials()
		{
			foreach (KeyValuePair<string, Material> cachedMaterial in _cachedMaterials)
			{
				GlobalLoad.Unload(cachedMaterial.Value);
			}
			_cachedMaterials.Clear();
		}

		private void CreateRenderers()
		{
			DrawCapsules(collisionCapsules, Color.green);
			DrawCapsules(repulsiveCapsules, Color.white);
			DrawCapsules(attackingCapsules, Color.red);
			DrawCapsulesNotRendered(Color.yellow);
		}

		private void DrawCapsulesNotRendered(Color color)
		{
			DrawCapsules(_capsules.Where((Capsule capsule) => !_renderers.Any((GameObject o) => o.name.Equals(capsule.name))).ToList(), color);
		}

		private void DrawCapsules(Dictionary<int, List<Capsule>> capsules, Color color)
		{
			foreach (List<Capsule> value in capsules.Values)
			{
				DrawCapsules(value, color);
			}
		}

		private void DrawCapsules(List<Capsule> capsules, Color color)
		{
			for (int i = 0; i < capsules.Count; i++)
			{
				_renderers.Add(DrawLine(capsules[i], color));
			}
		}

		private GameObject DrawLine(Capsule capsule, Color color)
		{
			return DrawLine(capsule.startVertex.position, capsule.finishVertex.position, color, capsule.name, capsule.radius);
		}

		private GameObject DrawLine(Vector3 startPosition, Vector3 endPosition, Color color, string name = "", float radius = 2f)
		{
			LineRenderer lineRenderer = new GameObject(name).AddComponent<LineRenderer>();
			lineRenderer.sharedMaterial = GetMaterialByColor(color);
			lineRenderer.SetPosition(0, startPosition);
			lineRenderer.SetPosition(1, endPosition);
			lineRenderer.startWidth = radius * 2f;
			lineRenderer.numCapVertices = 10;
			lineRenderer.useWorldSpace = true;
			return lineRenderer.gameObject;
		}

		private void DrawRect(RepulsionRect rect)
		{
			float z = rect.points.First().Value.position.z;
			Vector3 vector = new Vector3(rect.pointLeft, rect.pointBot, z);
			Vector3 vector2 = new Vector3(rect.pointLeft, rect.pointUp, z);
			Vector3 startPosition = vector2;
			Vector3 vector3 = new Vector3(rect.pointRight, rect.pointUp, z);
			Vector3 startPosition2 = vector3;
			Vector3 vector4 = new Vector3(rect.pointRight, rect.pointBot, z);
			Vector3 startPosition3 = vector4;
			Vector3 endPosition = vector;
			_renderers.Add(DrawLine(vector, vector2, Color.white, string.Empty));
			_renderers.Add(DrawLine(startPosition, vector3, Color.white, string.Empty));
			_renderers.Add(DrawLine(startPosition2, vector4, Color.white, string.Empty));
			_renderers.Add(DrawLine(startPosition3, endPosition, Color.white, string.Empty));
		}

		private void CleanRenderers()
		{
			ReleaseCachedMaterials();
			if (!_renderers.IsEmpty())
			{
				for (int i = 0; i < _renderers.Count; i++)
				{
					GlobalLoad.Unload(_renderers[i]);
				}
				_renderers.Clear();
			}
		}

		private void CheckShowCapsules()
		{
			_shouldRender = true;
			Observable.EveryUpdate().Subscribe(delegate
			{
				CleanRenderers();
				if (ShowCapsules && _shouldRender)
				{
					CreateRenderers();
					DrawRect(repulsionRect);
				}
			});
		}

		public void RenderCapsules(bool enable)
		{
			_shouldRender = enable;
		}
	}
}
