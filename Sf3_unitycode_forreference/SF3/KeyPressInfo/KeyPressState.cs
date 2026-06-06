using System;

namespace SF3.KeyPressInfo
{
	[Flags]
	public enum KeyPressState : byte
	{
		Undefined = 0,
		Down = 1,
		Up = 2,
		Hold = 4,
		UnHold = 8,
		Any = 0xF,
		Ultimate = 0x80,
		None = byte.MaxValue
	}
}
