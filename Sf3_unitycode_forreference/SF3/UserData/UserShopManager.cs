using System;
using Google.Protobuf;
using Nekki.Utils;
using Nekki.Yaml;
using SF3.Items;
using UnityEngine;
using sf3DTO;

namespace SF3.UserData
{
	public class UserShopManager : MonoBehaviour, IUserDataManager
	{
		private static UserShopManager _instance;

		private bool _isSaveFileByTimer;

		private string _shopPath = string.Empty;

		private YamlDocumentNekki _shopYaml;

		private Mapping _shopConfigurationNode;

		private Sequence _categoriesAndItemsNode;

		private Sequence _shopDataNode;

		private UserShopConfiguration _shopConfiguration;

		private UserShopData _shopData;

		public static UserShopManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public UserShopConfiguration shopConfiguration
		{
			get
			{
				return _shopConfiguration;
			}
		}

		public static event Action OnUserShopUpdated;

		private void Awake()
		{
			_instance = this;
			UserDataController.AddUserDataManager(UserDataManagerType.Shop, this);
		}

		public static void Create()
		{
			if (_instance == null)
			{
				UserShopManager userShopManager = new GameObject("_userShopManager").AddComponent<UserShopManager>();
				StaticObjectsManager.AddObject(userShopManager.gameObject);
			}
		}

		public void Initialize()
		{
			GlobalTimer.Instnce.addEventListener(0, OnTimerTick);
			_shopPath = GlobalPath.GameDataCombine("User/shop.txt");
			string fileOrResourcesText = GlobalLoad.GetFileOrResourcesText(_shopPath, "GameSettings", "shop_default.yaml");
			_shopYaml = YamlDocumentNekki.FromYamlContent(fileOrResourcesText);
			Mapping mapping = _shopYaml.GetRoot().GetMapping("ShopRoot");
			_shopConfigurationNode = mapping.GetMapping("ShopConfiguration");
			_categoriesAndItemsNode = mapping.GetSequence("CategoriesAndItems");
			if (_shopConfigurationNode == null || _categoriesAndItemsNode == null)
			{
				Debug.LogError("Wrong shop yaml format");
				return;
			}
			_shopConfiguration = new UserShopConfiguration(_shopConfigurationNode, SaveFileShop);
			_shopConfiguration.UpdateData(null);
			_shopData = new UserShopData(_categoriesAndItemsNode, true, SaveFileShop);
			SaveFileShop(true);
		}

		public void UpdateUserData(IMessage dataObject)
		{
			Shop shop = null;
			if (dataObject != null)
			{
				shop = ((!(dataObject is Player)) ? (dataObject as Shop) : ((Player)dataObject).Shop);
			}
			if (shop != null)
			{
				_shopConfiguration.UpdateData(shop);
				_shopData.UpdateData(shop);
				UserShopManager.OnUserShopUpdated();
			}
		}

		public UserShopCategory GetShopCategoryData(ShopCategoryType categoryType)
		{
			return _shopData.GetShopCategoryData(categoryType);
		}

		public int GetShopCategoryItemCount(ShopCategoryType categoryType)
		{
			return _shopData.GetShopCategoryData(categoryType).items.Count;
		}

		private void OnTimerTick(CallEventArgs callEventArgs)
		{
			if (_isSaveFileByTimer)
			{
				_isSaveFileByTimer = false;
				SaveFileShop(true);
			}
		}

		public void SaveFileShop(bool forcibly = false)
		{
			if (forcibly)
			{
				SaveToFile();
			}
			else
			{
				_isSaveFileByTimer = true;
			}
		}

		private void SaveToFile()
		{
			_shopYaml.SaveToFile(_shopPath, false);
		}

		static UserShopManager()
		{
			UserShopManager.OnUserShopUpdated = delegate
			{
			};
		}
	}
}
