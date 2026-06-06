using Jint.Native.Array;
using Nekki.Yaml;
using SimpleJSON;

public static class SF3TypesConvertExtentions
{
	public static string[] ToStringArray(this ArrayInstance array)
	{
		if (array == null)
		{
			return new string[0];
		}
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].AsString();
		}
		return array2;
	}

	public static string[] ToStringArray(this JSONArray array)
	{
		if (array == null)
		{
			return new string[0];
		}
		string[] array2 = new string[array.Count];
		for (int i = 0; i < array.Count; i++)
		{
			array2[i] = array[i].Value;
		}
		return array2;
	}

	public static string[] ToStringArray(this Sequence array)
	{
		if (array == null)
		{
			return new string[0];
		}
		string[] array2 = new string[array.nodesInside.Count];
		for (int i = 0; i < array.nodesInside.Count; i++)
		{
			array2[i] = array.nodesInside[i].value.ToString();
		}
		return array2;
	}
}
