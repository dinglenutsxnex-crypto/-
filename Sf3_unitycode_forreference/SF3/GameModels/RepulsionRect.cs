using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SF3.GameModels
{
	[Serializable]
	public class RepulsionRect
	{
		public float widthScale = 1f;

		public float heightScale = 1f;

		public int uniqueKey;

		public float minimumSize;

		public List<string> exclusiveKeys;

		public Dictionary<string, Transform> points;

		public Vector3 center { get; private set; }

		public Vector2 size { get; private set; }

		public float pointLeft { get; private set; }

		public float pointRight { get; private set; }

		public float pointUp { get; private set; }

		public float pointBot { get; private set; }

		public RepulsionRect(Dictionary<string, Transform> _points, float widthScale, float heightScale, float? minimumSize = null)
		{
			points = _points;
			this.widthScale = widthScale;
			this.heightScale = heightScale;
			if (minimumSize.HasValue)
			{
				this.minimumSize = minimumSize.Value;
			}
		}

		public RepulsionRect(List<Transform> _points, float widthScale, float heightScale, float? minimumSize = null)
		{
			points = new Dictionary<string, Transform>();
			foreach (Transform _point in _points)
			{
				points.Add(uniqueKey++.ToString(), _point);
			}
			this.widthScale = widthScale;
			this.heightScale = heightScale;
			if (minimumSize.HasValue)
			{
				this.minimumSize = minimumSize.Value;
			}
		}

		public void Calclulate()
		{
			if (points == null || points.Count == 0)
			{
				return;
			}
			int num = 0;
			Vector3 vector = default(Vector3);
			Transform value = points.First().Value;
			pointLeft = value.position.x;
			pointRight = value.position.x;
			pointUp = value.position.y;
			pointBot = value.position.y;
			foreach (KeyValuePair<string, Transform> point in points)
			{
				if (exclusiveKeys == null || !exclusiveKeys.Contains(point.Key))
				{
					Transform value2 = point.Value;
					vector += value2.position;
					if (value2.position.x < pointLeft)
					{
						pointLeft = value2.position.x;
					}
					if (value2.position.x > pointRight)
					{
						pointRight = value2.position.x;
					}
					if (value2.position.y > pointUp)
					{
						pointUp = value2.position.y;
					}
					if (value2.position.y < pointBot)
					{
						pointBot = value2.position.y;
					}
					num++;
				}
			}
			center = vector / num;
			size = new Vector2((pointRight - pointLeft) * widthScale, (pointUp - pointBot) * heightScale);
			if (size.x < minimumSize)
			{
				size = new Vector2(minimumSize, size.y);
			}
			if (size.y < minimumSize)
			{
				size = new Vector2(size.x, minimumSize);
			}
			pointLeft = center.x - size.x / 2f;
			pointRight = center.x + size.x / 2f;
			pointBot = center.y - size.y / 2f;
			pointUp = center.y + size.y / 2f;
		}

		public void SetScale(Vector2 scale)
		{
			widthScale = scale.x;
			heightScale = scale.y;
		}

		public float GetMinYPosition()
		{
			float y = points.First().Value.position.y;
			foreach (KeyValuePair<string, Transform> point in points)
			{
				Transform value = point.Value;
				if (value.position.y < y)
				{
					y = value.position.y;
				}
			}
			return y;
		}
	}
}
