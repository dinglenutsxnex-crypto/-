using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextPic : UnityEngine.UI.Text
{
	private class Attributes
	{
		private Dictionary<string, string> attributes;

		public Attributes(Dictionary<string, string> attributes)
		{
			this.attributes = attributes;
		}

		public string GetValue(string name, string defaultValue = "")
		{
			if (!attributes.ContainsKey(name))
			{
				return defaultValue;
			}
			return attributes[name];
		}

		public float GetValueAsFloat(string name, float defaultValue = 0f)
		{
			float result = defaultValue;
			if (HasAttribute(name))
			{
				float.TryParse(GetValue(name, string.Empty), out result);
			}
			return result;
		}

		public Color GetValueAsColor(string name, Color defaultValue = default(Color))
		{
			Color color = defaultValue;
			if (HasAttribute(name))
			{
				ColorUtility.TryParseHtmlString(GetValue(name, string.Empty), out color);
			}
			return color;
		}

		public bool HasAttribute(string name)
		{
			return attributes.ContainsKey(name);
		}
	}

	[Serializable]
	public struct IconName
	{
		public string name;

		public Sprite sprite;
	}

	private readonly List<ImageWrapper> m_ImagesPool = new List<ImageWrapper>();

	private readonly List<int> m_ImagesVertexIndex = new List<int>();

	private static readonly Regex s_Regex = new Regex("<quad (.*?)/>", RegexOptions.Singleline);

	private static readonly Regex s_AttributeRegex = new Regex("([a-zA-Z0-9]+)\\s*=\\s*([#a-zA-Z0-9/_\\-\\+\\\\\\.]+)", RegexOptions.Singleline);

	private string fixedString;

	private string m_OutputText;

	public IconName[] inspectorIconList;

	private Dictionary<string, Sprite> iconList = new Dictionary<string, Sprite>();

	public float ImageScalingFactor = 1f;

	[SerializeField]
	public float VerticalTextToImageOffset;

	private List<Vector2> positions = new List<Vector2>();

	public override void SetVerticesDirty()
	{
		base.SetVerticesDirty();
		UpdateQuadImage();
	}

	private new void Start()
	{
		if (inspectorIconList != null && inspectorIconList.Length > 0)
		{
			IconName[] array = inspectorIconList;
			for (int i = 0; i < array.Length; i++)
			{
				IconName iconName = array[i];
				Debug.Log(iconName.sprite.name);
				iconList.Add(iconName.name, iconName.sprite);
			}
		}
	}

	protected void UpdateQuadImage()
	{
		m_OutputText = text;
		m_ImagesVertexIndex.Clear();
		foreach (Match item2 in s_Regex.Matches(m_OutputText))
		{
			int index = item2.Index;
			int item = index * 4 + 3;
			m_ImagesVertexIndex.Add(item);
			m_ImagesPool.RemoveAll((ImageWrapper image) => image == null);
			if (m_ImagesPool.Count == 0)
			{
				GetComponentsInChildren(m_ImagesPool);
			}
			if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
			{
				AddNewImageToPool();
			}
			InitImage(m_ImagesPool[m_ImagesVertexIndex.Count - 1], ParseAttributes(item2.Groups[1].Value));
		}
		for (int i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
		{
			if ((bool)m_ImagesPool[i])
			{
				m_ImagesPool[i].enabled = false;
			}
		}
	}

	private void InitImage(ImageWrapper img, Attributes attributes)
	{
		string value = attributes.GetValue("name", string.Empty);
		float valueAsFloat = attributes.GetValueAsFloat("width", 1f);
		float valueAsFloat2 = attributes.GetValueAsFloat("size", 1f);
		if (img.sprite == null || img.sprite.name != value)
		{
			if (attributes.HasAttribute("atlas"))
			{
				img.sprite = GlobalLoad.GetLoadSpriteFromAtlas(attributes.GetValue("atlas", string.Empty), value);
			}
			else
			{
				img.sprite = GlobalLoad.GetLoadSprite(value);
			}
		}
		img.color = attributes.GetValueAsColor("color", Color.white);
		img.rectTransform.localRotation = Quaternion.Euler(0f, 0f, attributes.GetValueAsFloat("angle"));
		img.rectTransform.sizeDelta = new Vector2(valueAsFloat2 * valueAsFloat, valueAsFloat2);
		img.enabled = true;
		if (positions.Count >= m_ImagesVertexIndex.Count)
		{
			img.rectTransform.anchoredPosition = positions[m_ImagesVertexIndex.Count - 1];
		}
	}

	private void AddNewImageToPool()
	{
		GameObject gameObject = new GameObject("ImageWrapper");
		gameObject.AddComponent<ImageWrapper>();
		gameObject.layer = base.gameObject.layer;
		gameObject.layer = base.gameObject.layer;
		RectTransform rectTransform = gameObject.transform as RectTransform;
		if ((bool)rectTransform)
		{
			rectTransform.SetParent(base.rectTransform);
			rectTransform.localPosition = Vector3.zero;
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = Vector3.one;
		}
		m_ImagesPool.Add(gameObject.GetComponent<ImageWrapper>());
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		string text = m_Text;
		m_Text = m_OutputText;
		base.OnPopulateMesh(toFill);
		m_Text = text;
		positions.Clear();
		UIVertex vertex = default(UIVertex);
		for (int i = 0; i < m_ImagesVertexIndex.Count; i++)
		{
			int num = m_ImagesVertexIndex[i];
			RectTransform rectTransform = m_ImagesPool[i].rectTransform;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			if (num < toFill.currentVertCount)
			{
				toFill.PopulateUIVertex(ref vertex, num);
				positions.Add(new Vector2(vertex.position.x + sizeDelta.x / 2f, vertex.position.y + sizeDelta.x / 2f));
				toFill.PopulateUIVertex(ref vertex, num - 3);
				Vector3 position = vertex.position;
				int num2 = num;
				int num3 = num - 3;
				while (num2 > num3)
				{
					toFill.PopulateUIVertex(ref vertex, num);
					vertex.position = position;
					toFill.SetUIVertex(vertex, num2);
					num2--;
				}
			}
		}
		if (m_ImagesVertexIndex.Count != 0)
		{
			for (int j = 0; j < toFill.currentVertCount; j++)
			{
				toFill.PopulateUIVertex(ref vertex, j);
				vertex.position -= new Vector3(0f, VerticalTextToImageOffset);
				toFill.SetUIVertex(vertex, j);
			}
			m_ImagesVertexIndex.Clear();
		}
		UpdateQuadImage();
	}

	private Attributes ParseAttributes(string attributesStr)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		MatchCollection matchCollection = s_AttributeRegex.Matches(attributesStr);
		foreach (Match item in s_AttributeRegex.Matches(attributesStr))
		{
			dictionary.Add(item.Groups[1].Value, item.Groups[2].Value);
		}
		return new Attributes(dictionary);
	}
}
