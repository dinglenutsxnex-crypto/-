using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF3
{
	public class ShaderParamsRandomizer : MonoBehaviour
	{
		[Serializable]
		public class RandomizeIntProp
		{
			[HideInInspector]
			public string description;

			[HideInInspector]
			public string name;

			public int value1;

			public int value2;

			public int GetRandom()
			{
				return UnityEngine.Random.Range(value1, value2);
			}

			public RandomizeIntProp Clone()
			{
				return MemberwiseClone() as RandomizeIntProp;
			}
		}

		[Serializable]
		public class RandomizeTextureProp
		{
			[HideInInspector]
			public string description;

			[HideInInspector]
			public string name;

			public Vector2 tiling_value1;

			public Vector2 tiling_value2;

			public Vector2 offset_value1;

			public Vector2 offset_value2;

			public Vector2 GetRandomTiling()
			{
				Vector2 result = default(Vector2);
				result.x = UnityEngine.Random.Range(tiling_value1.x, tiling_value2.x);
				result.y = UnityEngine.Random.Range(tiling_value1.y, tiling_value2.y);
				return result;
			}

			public Vector2 GetRandomOffset()
			{
				Vector2 result = default(Vector2);
				result.x = UnityEngine.Random.Range(offset_value1.x, offset_value2.x);
				result.y = UnityEngine.Random.Range(offset_value1.y, offset_value2.y);
				return result;
			}

			public RandomizeTextureProp Clone()
			{
				return MemberwiseClone() as RandomizeTextureProp;
			}
		}

		[Serializable]
		public class RandomizeFloatProp
		{
			[HideInInspector]
			public string description;

			[HideInInspector]
			public string name;

			public float value1;

			public float value2;

			public float GetRandom()
			{
				return UnityEngine.Random.Range(value1, value2);
			}

			public RandomizeFloatProp Clone()
			{
				return MemberwiseClone() as RandomizeFloatProp;
			}
		}

		[Serializable]
		public class RandomizeVectorProp
		{
			[HideInInspector]
			public string description;

			[HideInInspector]
			public string name;

			public Vector4 value1;

			public Vector4 value2;

			public Vector4 GetRandom()
			{
				Vector4 result = default(Vector4);
				result.x = UnityEngine.Random.Range(value1.x, value2.x);
				result.y = UnityEngine.Random.Range(value1.y, value2.y);
				result.z = UnityEngine.Random.Range(value1.z, value2.z);
				result.w = UnityEngine.Random.Range(value1.w, value2.w);
				return result;
			}

			public RandomizeVectorProp Clone()
			{
				return MemberwiseClone() as RandomizeVectorProp;
			}
		}

		[Serializable]
		public class RandomizeColorProp
		{
			[HideInInspector]
			public string description;

			[HideInInspector]
			public string name;

			public Color value1;

			public Color value2;

			public Color GetRandom()
			{
				return Color.Lerp(value1, value2, UnityEngine.Random.Range(0f, 1f));
			}

			public RandomizeColorProp Clone()
			{
				return MemberwiseClone() as RandomizeColorProp;
			}
		}

		public bool randomizeOnEnable = true;

		public List<RandomizeIntProp> intProperties = new List<RandomizeIntProp>();

		public List<RandomizeFloatProp> floatProperties = new List<RandomizeFloatProp>();

		public List<RandomizeColorProp> colorProperties = new List<RandomizeColorProp>();

		public List<RandomizeVectorProp> vectorProperties = new List<RandomizeVectorProp>();

		public List<RandomizeTextureProp> textureProperties = new List<RandomizeTextureProp>();

		public Material currentMaterial;

		private bool _isInited;

		public void Randomize()
		{
			if (!_isInited)
			{
				currentMaterial = GetComponent<SkinnedMeshRenderer>().material;
				_isInited = true;
			}
			foreach (RandomizeIntProp intProperty in intProperties)
			{
				currentMaterial.SetInt(intProperty.name, intProperty.GetRandom());
			}
			foreach (RandomizeFloatProp floatProperty in floatProperties)
			{
				currentMaterial.SetFloat(floatProperty.name, floatProperty.GetRandom());
			}
			foreach (RandomizeColorProp colorProperty in colorProperties)
			{
				currentMaterial.SetColor(colorProperty.name, colorProperty.GetRandom());
			}
			foreach (RandomizeVectorProp vectorProperty in vectorProperties)
			{
				currentMaterial.SetVector(vectorProperty.name, vectorProperty.GetRandom());
			}
			foreach (RandomizeTextureProp textureProperty in textureProperties)
			{
				currentMaterial.SetTextureOffset(textureProperty.name, textureProperty.GetRandomOffset());
				currentMaterial.SetTextureScale(textureProperty.name, textureProperty.GetRandomTiling());
			}
		}

		private void OnEnable()
		{
			if (randomizeOnEnable)
			{
				Randomize();
			}
		}
	}
}
