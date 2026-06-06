using System;
using Nekki.UI;

public class SimpleDialog : BaseDialog
{
	private static Action _onClose;

	private static string contentText;

	private static string titleText;

	private static string buttonText;

	public static NekkiUIDialog Show()
	{
		return NekkiUIRootModules.Instance.MountNGUIModule<SimpleDialog>("SimpleDialog");
	}

	public static NekkiUIDialog Show(string title, string content, string button, Action onClose = null, string luaMethodCallback = null)
	{
		contentText = content;
		titleText = title;
		buttonText = button;
		_onClose = onClose;
		return Show();
	}

	public static void Hide()
	{
		contentText = string.Empty;
		NekkiUIRootModules.Instance.UnmountModule("SimpleDialog");
		if (_onClose != null)
		{
			_onClose();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		dialogPrefabPath = "UI/SimpleDialog/Design";
		CreateDialog();
		OnClose(Hide);
		_content = AddContent<SimpleDialogContent>("UI/SimpleDialog/Content");
		if (string.IsNullOrEmpty(titleText))
		{
			Title = "fight_end";
		}
		else
		{
			NekkiUILabel titleLabel = ((SimpleDialogInfo)dlgScript).TitleLabel;
			titleLabel.Type = NekkiUILabel.Types.Simple;
			titleLabel.text = titleText;
		}
		if (string.IsNullOrEmpty(contentText))
		{
			((SimpleDialogContent)_content).Text.Alias = "fight_end_message";
		}
		else
		{
			((SimpleDialogContent)_content).Text.text = contentText;
		}
		if (string.IsNullOrEmpty(buttonText))
		{
			NekkiUILabel componentInChildren = ((SimpleDialogContent)_content).GoDojo.GetComponentInChildren<NekkiUILabel>();
			componentInChildren.Alias = "go_dojo";
		}
		else
		{
			NekkiUILabel componentInChildren2 = ((SimpleDialogContent)_content).GoDojo.GetComponentInChildren<NekkiUILabel>();
			componentInChildren2.Type = NekkiUILabel.Types.Simple;
			componentInChildren2.text = buttonText;
		}
		((SimpleDialogContent)_content).GoDojo.onClick.Add(new EventDelegate(Hide));
	}
}
