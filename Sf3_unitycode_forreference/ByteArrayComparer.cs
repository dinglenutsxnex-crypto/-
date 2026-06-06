using System;
using System.Collections.Generic;

public class ByteArrayComparer : IEqualityComparer<byte[]>
{
	public bool Equals(byte[] left, byte[] right)
	{
		if (left == null || right == null)
		{
			return left == right;
		}
		if (left.Length != right.Length)
		{
			return false;
		}
		for (int i = 0; i < left.Length; i++)
		{
			if (left[i] != right[i])
			{
				return false;
			}
		}
		return true;
	}

	public int GetHashCode(byte[] key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int num = 0;
		foreach (byte b in key)
		{
			num += b;
		}
		return num;
	}
}
