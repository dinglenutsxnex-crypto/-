using UnityEngine;

public abstract class BaseDialog : NekkiUIDialog
{
	private EventDelegate.Callback _baseDialogOnClose;

	protected MonoBehaviour _content;

	public virtual string Title
	{
		get
		{
			return ((SimpleDialogInfo)dlgScript).TitleLabel.text;
		}
		set
		{
			((SimpleDialogInfo)dlgScript).TitleLabel.Alias = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public virtual void OnClose(EventDelegate.Callback onClose)
	{
		_baseDialogOnClose = onClose;
		((SimpleDialogInfo)dlgScript).Close.onClick.Add(new EventDelegate(CloseMethod));
	}

	private void CloseMethod()
	{
		if (_baseDialogOnClose != null)
		{
			_baseDialogOnClose();
		}
	}

	protected override void Init()
	{
	}
}
