using UnityEngine;

namespace SF3.Settings
{
	public class GameSettings : MonoBehaviour
	{
		private ItemSettings _itemSettings;

		private DojoSettings _dojoSettings;

		private ClientSettings _clientSettings;

		public static GameSettings Instance { get; private set; }

		public static ItemSettings ItemSettings
		{
			get
			{
				return Instance._itemSettings;
			}
		}

		public static DojoSettings DojoSettings
		{
			get
			{
				return Instance._dojoSettings;
			}
		}

		public static ClientSettings clientSettings
		{
			get
			{
				return Instance._clientSettings;
			}
		}

		public static void Initialize()
		{
			if (!(Instance != null))
			{
				GameObject gameObject = new GameObject("game_settings");
				StaticObjectsManager.AddObject(gameObject);
				Instance = gameObject.AddComponent<GameSettings>();
				Instance._itemSettings = GlobalLoad.GetLoadObjectInternal<ItemSettings>("GameSettings", "item_settings");
				Instance._dojoSettings = GlobalLoad.GetLoadObjectInternal<DojoSettings>("GameSettings", "dojo_settings");
				Instance._clientSettings = GlobalLoad.GetLoadObjectInternal<ClientSettings>("GameSettings", "clientSettings");
			}
		}
	}
}
