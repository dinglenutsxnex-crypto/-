using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EventObjectInIn<InType1, InType2>
{
	public delegate void DelegateType(InType1 type1, InType2 type2);

	private event DelegateType OnEvent;

	public void add(DelegateType dlg)
	{
		OnEvent += dlg;
	}

	public static EventObjectInIn<InType1, InType2> operator +(EventObjectInIn<InType1, InType2> e, DelegateType dlg)
	{
		e.OnEvent += dlg;
		return e;
	}

	public static EventObjectInIn<InType1, InType2> operator -(EventObjectInIn<InType1, InType2> e, DelegateType dlg)
	{
		e.OnEvent -= dlg;
		return e;
	}

	public void Invoke(InType1 value1, InType2 value2)
	{
		if (this.OnEvent != null)
		{
			this.OnEvent(value1, value2);
		}
	}
}
