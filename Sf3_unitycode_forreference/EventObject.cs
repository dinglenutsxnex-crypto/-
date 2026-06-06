using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EventObject
{
	public delegate void DelegateType();

	private event DelegateType OnEvent;

	public void add(DelegateType dlg)
	{
		OnEvent += dlg;
	}

	public static EventObject operator +(EventObject e, DelegateType dlg)
	{
		e.OnEvent += dlg;
		return e;
	}

	public static EventObject operator -(EventObject e, DelegateType dlg)
	{
		e.OnEvent -= dlg;
		return e;
	}

	public void Invoke()
	{
		if (this.OnEvent != null)
		{
			this.OnEvent();
		}
	}
}
