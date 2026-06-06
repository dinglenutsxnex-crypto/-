using System.Collections.Generic;
using DG.Tweening;
using SF3.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace SF3
{
	public class SlideMenu : UIModuleHolder
	{
		private Dictionary<string, ConstantsSF3.ELocationSceneModule> _menuTypeVsSceneModule = new Dictionary<string, ConstantsSF3.ELocationSceneModule>
		{
			{
				"map",
				ConstantsSF3.ELocationSceneModule.Map
			},
			{
				"shop",
				ConstantsSF3.ELocationSceneModule.Shop
			},
			{
				"dojo",
				ConstantsSF3.ELocationSceneModule.DojoInterface
			},
			{
				"inventory",
				ConstantsSF3.ELocationSceneModule.Inventory
			},
			{
				"boosterpacks",
				ConstantsSF3.ELocationSceneModule.BoosterpacksScreen
			}
		};

		private static SlideMenu _instance;

		private const string _openSoundName = "ui/buttons/panel_in";

		private const string _closeSoundName = "ui/buttons/panel_out";

		[SerializeField]
		private GameObject _subMenuObject;

		[SerializeField]
		private GameObject _backplane;

		private Image _backplaneImage;

		private TutorialComponentNative _tutorialComponent;

		[SerializeField]
		private float _expandingMenuDuration = 0.2f;

		[SerializeField]
		private float _backplaneAlpha = 0.5f;

		private bool _isOpen;

		private float _cooldownTime;

		private RectTransform _menuPanelRect;

		private float _menuPanelWidth;

		private SlideMenuButton[] _slideMenuButtons;

		public static bool isEnabled
		{
			get
			{
				if (_instance != null)
				{
					return _instance.enabled;
				}
				return false;
			}
			set
			{
				if (_instance != null)
				{
					_instance.enabled = value;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_instance = this;
			_isOpen = false;
			_cooldownTime = 0f;
			_menuPanelRect = _subMenuObject.GetComponent<RectTransform>();
			_menuPanelWidth = _menuPanelRect.rect.width;
			_backplaneImage = _backplane.GetComponent<Image>();
			_tutorialComponent = base.gameObject.GetComponent<TutorialComponentNative>();
			_slideMenuButtons = GetComponentsInChildren<SlideMenuButton>();
			Vector3 localPosition = _menuPanelRect.localPosition;
			localPosition.x = 0f - _menuPanelWidth;
			_menuPanelRect.localPosition = localPosition;
			Color color = _backplaneImage.color;
			color.a = 0f;
			_backplaneImage.color = color;
			_subMenuObject.SetActive(false);
			_backplane.SetActive(false);
			AudioManager.Instance.LoadSound("ui/buttons/panel_in");
			AudioManager.Instance.LoadSound("ui/buttons/panel_out");
			GamepadController.Instance.addEventListener(0, OnKeyboardKeyPress);
			ActionButtons.Instance.addEventListener(0, OnKeyboardKeyPress);
			KeyboardController.Instance.addEventListener(0, OnKeyboardKeyPress);
		}

		protected override void OnDestroy()
		{
			GamepadController.Instance.removeEventListener(0, OnKeyboardKeyPress);
			ActionButtons.Instance.removeEventListener(0, OnKeyboardKeyPress);
			KeyboardController.Instance.removeEventListener(0, OnKeyboardKeyPress);
		}

		private void OnKeyboardKeyPress(CallEventArgs e)
		{
			CloseWithoutCooldown();
		}

		public void OpenMenu(string menu)
		{
			if (!UIBlocker.Instance.IsLocked)
			{
				if (_tutorialComponent != null && _tutorialComponent.IsLocked)
				{
					_tutorialComponent.OnClick();
				}
				if (_menuTypeVsSceneModule.ContainsKey(menu))
				{
					ConstantsSF3.ELocationSceneModule eLocationSceneModule = _menuTypeVsSceneModule[menu];
					if (eLocationSceneModule != BaseModuleController.CurrentType)
					{
						BaseModuleController.GoToModule(eLocationSceneModule);
					}
				}
				else
				{
					Debug.LogError(string.Format("Wrong menu name '{0}'", menu));
				}
			}
			CloseWithoutCooldown();
		}

		public void Open()
		{
			MenuMoveController(true);
		}

		public void Close()
		{
			MenuMoveController(false);
		}

		public void CloseWithoutCooldown()
		{
			_cooldownTime = 0f;
			Close();
		}

		private void MenuMoveController(bool open)
		{
			if (open == _isOpen || _instance == null || _cooldownTime > Time.time)
			{
				return;
			}
			if (open)
			{
				SF3UiLogger.instance.AddMenuOpenEvent();
				AudioManager.Instance.PlaySound("ui/buttons/panel_in");
				_backplane.SetActive(true);
				_backplaneImage.DOFade(_backplaneAlpha, _expandingMenuDuration);
				_subMenuObject.SetActive(true);
				UIBlocker.Instance.BlockNGUI();
				_menuPanelRect.DOLocalMoveX(0f, _expandingMenuDuration);
				_cooldownTime = Time.time + 1f;
			}
			else
			{
				AudioManager.Instance.PlaySound("ui/buttons/panel_out");
				_backplaneImage.DOFade(0f, _expandingMenuDuration).OnComplete(delegate
				{
					_backplane.SetActive(false);
				});
				_menuPanelRect.DOLocalMoveX(0f - _menuPanelWidth, _expandingMenuDuration).OnComplete(delegate
				{
					SlideMenuButton[] slideMenuButtons = _slideMenuButtons;
					foreach (SlideMenuButton slideMenuButton in slideMenuButtons)
					{
						slideMenuButton.Deselect();
					}
					_subMenuObject.SetActive(false);
					UIBlocker.Instance.UnblockNGUI();
				});
				_cooldownTime = Time.time + _expandingMenuDuration;
			}
			_isOpen = open;
		}
	}
}
