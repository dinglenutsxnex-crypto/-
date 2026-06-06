using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.UI;
using UnityEngine;

public class NekkiTooltip : MonoBehaviour, IHasAlias
{
	[SerializeField]
	private string _alias;

	[NonSerialized]
	public LocaleImport.LocaleString LocaleString;

	[SerializeField]
	public string[] LastReplacement;

	[SerializeField]
	public List<NekkiUILabel.ImageData> ImagesInfo;

	[SerializeField]
	public UIAtlas Images;

	public int MaxSymbols = 30;

	public float ShowTime = 4f;

	public string Alias
	{
		get
		{
			return _alias;
		}
		set
		{
			_alias = value;
			if (string.IsNullOrEmpty(_alias) || !Localization.Contains(_alias, true))
			{
				return;
			}
			LocaleString = Localization.Get(_alias);
			if (LocaleString.Simple)
			{
				return;
			}
			ImagesInfo = new List<NekkiUILabel.ImageData>();
			foreach (LocaleImport.LocaleString.Section section in LocaleString.Sections)
			{
				if (section.SectionType == LocaleImport.LocaleString.SectionTypes.Image)
				{
					NekkiUILabel.ImageData imageData = new NekkiUILabel.ImageData();
					imageData.sectionID = section.ID;
					ImagesInfo.Add(imageData);
				}
			}
			ImagesInfo.Sort((NekkiUILabel.ImageData a, NekkiUILabel.ImageData b) => a.sectionID.CompareTo(b.sectionID));
		}
	}

	public void SelectReplacementAtIndex(string source)
	{
		string[] array = source.Split('|');
		int num = int.Parse(array[1]);
		LastReplacement[num] = array[0];
	}

	public NekkiUILabel.ImageData GetImageDataByID(int sectionID)
	{
		if (ImagesInfo == null)
		{
			return null;
		}
		foreach (NekkiUILabel.ImageData item in ImagesInfo)
		{
			if (item.sectionID == sectionID)
			{
				return item;
			}
		}
		NekkiUILabel.ImageData imageData = new NekkiUILabel.ImageData();
		imageData.sectionID = sectionID;
		ImagesInfo.Add(imageData);
		return imageData;
	}

	public void Format(NekkiUILabel label, UISprite back, GameObject arrow)
	{
		label.Images = Images;
		label.MaxSymbols = MaxSymbols;
		StartCoroutine(FormatDellay(label, back, arrow));
	}

	private IEnumerator FormatDellay(NekkiUILabel label, UISprite back, GameObject arrow)
	{
		label.LastReplacement = LastReplacement;
		label.Alias = Alias;
		for (int i = 0; i < 3; i++)
		{
			yield return new WaitForEndOfFrame();
			label.Format(LastReplacement);
			Vector3 size = label.localSize;
			back.width = (int)size.x + 50;
			back.height = (int)size.y + 20;
			float ARROW_LONG = 50f;
			float ARROW_SHORT = 40f;
			switch (arrow.name)
			{
			case "0":
				arrow.transform.localPosition = new Vector3((float)(-back.width) / 2f, (float)back.height / 2f);
				back.transform.localPosition = new Vector3((float)back.width / 2f, (float)(-back.height) / 2f - ARROW_LONG);
				break;
			case "1":
				arrow.transform.localPosition = new Vector3(0f, (float)back.height / 2f);
				back.transform.localPosition = new Vector3(0f, (float)(-back.height) / 2f - ARROW_SHORT);
				break;
			case "2":
				arrow.transform.localPosition = new Vector3((float)back.width / 2f, (float)back.height / 2f);
				back.transform.localPosition = new Vector3((float)(-back.width) / 2f, (float)(-back.height) / 2f - ARROW_LONG);
				break;
			case "3":
				arrow.transform.localPosition = new Vector3((float)(-back.width) / 2f, 0f);
				back.transform.localPosition = new Vector3((float)back.width / 2f + ARROW_SHORT, 0f);
				break;
			case "4":
				arrow.transform.localPosition = new Vector3((float)back.width / 2f, 0f);
				back.transform.localPosition = new Vector3((float)(-back.width) / 2f - ARROW_SHORT, 0f);
				break;
			case "5":
				arrow.transform.localPosition = new Vector3((float)(-back.width) / 2f, (float)(-back.height) / 2f);
				back.transform.localPosition = new Vector3((float)back.width / 2f, (float)back.height / 2f + ARROW_LONG);
				break;
			case "6":
				arrow.transform.localPosition = new Vector3(0f, (float)(-back.height) / 2f);
				back.transform.localPosition = new Vector3(0f, (float)back.height / 2f + ARROW_SHORT);
				break;
			case "7":
				arrow.transform.localPosition = new Vector3((float)back.width / 2f, (float)(-back.height) / 2f);
				back.transform.localPosition = new Vector3((float)(-back.width) / 2f, (float)back.height / 2f + ARROW_LONG);
				break;
			}
			back.enabled = true;
			arrow.SetActive(true);
		}
	}

	private void Start()
	{
		LocaleString = Localization.Get(_alias);
	}

	protected void OnValidate()
	{
		Start();
	}
}
