using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EventObjectIn<InType>
{
	public delegate void DelegateType(InType type);

	private event DelegateType OnEvent;

	public void add(DelegateType dlg)
	{
		OnEvent += dlg;
	}

	public static EventObjectIn<InType> operator +(EventObjectIn<InType> e, DelegateType dlg)
	{
		e.OnEvent += dlg;
		return e;
	}

	public static EventObjectIn<InType> operator -(EventObjectIn<InType> e, DelegateType dlg)
	{
		e.OnEvent -= dlg;
		return e;
	}

	public void Invoke(InType value)
	{
		if (this.OnEvent != null)
		{
			this.OnEvent(value);
		}
	}
}
