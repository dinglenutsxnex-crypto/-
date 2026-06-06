using System.Linq;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
	[SerializeField]
	private ColorPreset[] skinColors;

	[SerializeField]
	private ColorPreset[] hairColors;

	public static ColorManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public ColorPreset GetSkinColor(int id)
	{
		return skinColors.FirstOrDefault((ColorPreset x) => x.ID == id);
	}

	public ColorPreset GetHairColor(int id)
	{
		return hairColors.FirstOrDefault((ColorPreset x) => x.ID == id);
	}
}
