using System.Collections.Generic;
using System.Globalization;
using Jint.Native;
using UnityEngine;

public struct LocationInfo
{
	private string _music;

	private Vector3 _position;

	private string _group;

	private string _locationName;

	public string music
	{
		get
		{
			return _music;
		}
	}

	public Vector3 position
	{
		get
		{
			return _position;
		}
	}

	public string group
	{
		get
		{
			return _group;
		}
	}

	public string locationName
	{
		get
		{
			return _locationName;
		}
	}

	public LocationInfo(Dictionary<string, JsValue> dictionary)
	{
		_locationName = dictionary["LocationName"].ToString();
		_position.z = 0f;
		_position.x = int.Parse(dictionary["X"].ToString(), CultureInfo.InvariantCulture);
		_position.y = int.Parse(dictionary["Y"].ToString(), CultureInfo.InvariantCulture);
		_group = dictionary["Group"].ToString();
		_music = ((!dictionary.ContainsKey("Music")) ? string.Empty : dictionary["Music"].ToString());
	}

	public bool InSamePlace(LocationInfo other)
	{
		return _position.x == other._position.x && _position.y == other._position.y;
	}
}
