using System.Collections.Generic;
using SF3;
using SF3.Moves;
using UnityEngine;

public class PerkCarousel : MonoBehaviour
{
	private enum Owners
	{
		None = 0,
		Player = 1,
		Enemy = 2
	}

	private class ImageInfo
	{
		public string Name { get; private set; }

		public Texture2D Texture { get; private set; }

		public Rect Rect { get; set; }

		public ImageInfo(Texture2D texture, string name)
		{
			Texture = texture;
			Name = name;
		}
	}

	private static UIAtlas _perkAltas;

	private static readonly Dictionary<string, ImageInfo> SpritesQueue = new Dictionary<string, ImageInfo>();

	private static readonly Dictionary<int, PerkCarousel> PerkCarousels = new Dictionary<int, PerkCarousel>();

	private readonly List<PerkUnit> _perks = new List<PerkUnit>();

	[SerializeField]
	private PerkUnit _perkBase;

	[SerializeField]
	private Owners Owner;

	public static void Init(List<InfoTrigger> triggers)
	{
		foreach (InfoTrigger trigger in triggers)
		{
			foreach (ITriggerAction action in trigger.actions)
			{
				TriggerActionShowStatusIcon triggerActionShowStatusIcon = action as TriggerActionShowStatusIcon;
				if (triggerActionShowStatusIcon != null && !string.IsNullOrEmpty(triggerActionShowStatusIcon.Icon) && !SpritesQueue.ContainsKey(triggerActionShowStatusIcon.Icon))
				{
					SpritesQueue.Add(triggerActionShowStatusIcon.Icon, new ImageInfo(GlobalLoad.GetLoadTexture2D("UI/Perks/" + triggerActionShowStatusIcon.Icon), triggerActionShowStatusIcon.Icon));
				}
			}
		}
	}

	public static void ModelsInitialized()
	{
		ImageInfo[] array = new ImageInfo[SpritesQueue.Count];
		SpritesQueue.Values.CopyTo(array, 0);
		RefreshAltas(array);
	}

	private static void RefreshAltas(ImageInfo[] icons)
	{
		if ((bool)_perkAltas)
		{
			if ((bool)_perkAltas.texture)
			{
				Object.Destroy(_perkAltas.texture);
			}
			Object.Destroy(_perkAltas);
		}
		if (icons == null || icons.Length == 0)
		{
			Debug.LogWarning("icons is null or empty");
			return;
		}
		List<ImageInfo> list = new List<ImageInfo>();
		ImageInfo[] array = icons;
		foreach (ImageInfo imageInfo in array)
		{
			if (imageInfo != null && imageInfo.Texture != null && !string.IsNullOrEmpty(imageInfo.Name))
			{
				list.Add(imageInfo);
			}
		}
		icons = list.ToArray();
		if (icons.Length == 0)
		{
			Debug.LogWarning("icons is empty (after null check)");
			return;
		}
		Texture2D[] array2 = new Texture2D[icons.Length];
		for (int j = 0; j < icons.Length; j++)
		{
			array2[j] = icons[j].Texture;
		}
		Texture2D texture2D = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
		Rect[] array3 = texture2D.PackTextures(array2, 0, 1024);
		for (int k = 0; k < icons.Length; k++)
		{
			icons[k].Rect = array3[k];
		}
		Material material = new Material(Shader.Find("Unlit/Transparent Colored"));
		material.SetTexture("_MainTex", texture2D);
		_perkAltas = new GameObject("_perkAtlas", typeof(UIAtlas)).GetComponent<UIAtlas>();
		_perkAltas.spriteMaterial = material;
		StaticObjectsManager.AddObject(_perkAltas.gameObject);
		ImageInfo[] array4 = icons;
		foreach (ImageInfo imageInfo2 in array4)
		{
			UISpriteData uISpriteData = new UISpriteData();
			uISpriteData.name = imageInfo2.Name;
			uISpriteData.x = Mathf.RoundToInt((float)texture2D.width * imageInfo2.Rect.xMin);
			uISpriteData.y = texture2D.height - Mathf.RoundToInt((float)texture2D.height * imageInfo2.Rect.yMin) - imageInfo2.Texture.height;
			uISpriteData.width = imageInfo2.Texture.width;
			uISpriteData.height = imageInfo2.Texture.height;
			UISpriteData item = uISpriteData;
			_perkAltas.spriteList.Add(item);
		}
		Resources.UnloadUnusedAssets();
	}

	private void Awake()
	{
		switch (Owner)
		{
		case Owners.Player:
			PerkCarousels[1] = this;
			break;
		case Owners.Enemy:
			PerkCarousels[2] = this;
			break;
		}
	}

	private static bool IsAvailableAction(int modelId)
	{
		if (PerkCarousels.Count == 0)
		{
			Messenger.Error(string.Format("No perk carousel available. Current location: [{0}]", BaseModuleController.CurrentType.ToString()));
			return false;
		}
		return true;
	}

	public static void ShowStatusIcon(int modelId, TriggerActionShowStatusIcon action)
	{
		if (IsAvailableAction(modelId))
		{
			PerkCarousels[modelId].Show(action);
		}
	}

	public static void ClearStatusIcon(int modelId, TriggerActionClearStatusIcon action)
	{
		if (IsAvailableAction(modelId))
		{
			PerkCarousels[modelId].Clear(action);
		}
	}

	private void Show(TriggerActionShowStatusIcon action)
	{
		PerkUnit component = Object.Instantiate(_perkBase.gameObject).GetComponent<PerkUnit>();
		component.transform.parent = base.transform;
		component.transform.localScale = Vector3.one;
		component.transform.localPosition = Vector3.zero;
		int index = _perks.Count;
		bool flag = false;
		if (!action.Stackable)
		{
			for (int i = 0; i < _perks.Count; i++)
			{
				if (_perks[i].name == action.name)
				{
					Object.Destroy(_perks[i].gameObject);
					index = i;
					flag = true;
					break;
				}
			}
		}
		component.Init(_perkAltas, action.Color, action.Icon, action.Duration, delegate(object o)
		{
			PerkUnit perkUnit = (PerkUnit)o;
			_perks.Remove(perkUnit);
			perkUnit.AnimateHide(delegate
			{
				for (int k = 0; k < _perks.Count; k++)
				{
					_perks[k].UpdateSize(GetPerkSize());
					_perks[k].UpdateIndex(k);
				}
			});
		}, index, GetPerkSize(), action.Infinite, action.ShowExpiration, action.name);
		if (flag)
		{
			_perks[index] = component;
		}
		else
		{
			_perks.Add(component);
		}
		for (int j = 0; j < _perks.Count; j++)
		{
			_perks[j].UpdateSize(GetPerkSize());
			_perks[j].UpdateIndex(j);
		}
	}

	private void Clear(TriggerActionClearStatusIcon action)
	{
		if (string.IsNullOrEmpty(action.name))
		{
			foreach (PerkUnit perk in _perks)
			{
				Object.Destroy(perk.gameObject);
			}
			_perks.Clear();
		}
		else
		{
			List<PerkUnit> list = new List<PerkUnit>();
			foreach (PerkUnit perk2 in _perks)
			{
				if (perk2.name == action.name)
				{
					list.Add(perk2);
				}
			}
			foreach (PerkUnit item in list)
			{
				_perks.Remove(item);
				Object.Destroy(item.gameObject);
			}
			list.Clear();
		}
		for (int i = 0; i < _perks.Count; i++)
		{
			_perks[i].UpdateSize(GetPerkSize());
			_perks[i].UpdateIndex(i);
		}
	}

	private float GetPerkSize()
	{
		return Mathf.Clamp(400f / (float)_perks.Count, 1f, 40f);
	}
}
