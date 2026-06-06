using System;
using System.Collections.Generic;
using Nekki.Yaml;
using Newtonsoft.Json.Linq;
using SF3.Moves;
using UnityEngine;

public class TriggerActionAnalyticsLog : TriggerAction
{
	[Serializable]
	private class LogDataUnit
	{
		public string Name;

		public string Log;

		[SerializeField]
		public Dictionary<string, string> Params;
	}

	private List<RpnParser.Formula> _formulas = new List<RpnParser.Formula>();

	private Mapping _params;

	private string _log;

	private string _name;

	private string _etype;

	public TriggerActionAnalyticsLog(Node yamNode)
		: base(EActionType.ANALYTICS_LOG, yamNode)
	{
		base.name = base.name.ToLower();
		Mapping mapping = (Mapping)((Mapping)yamNode).nodesInside[0];
		_params = mapping.GetMapping("Params");
		Scalar text = mapping.GetText("Name");
		if (text != null)
		{
			_name = text.text;
		}
		Scalar text2 = mapping.GetText("Log");
		if (text2 != null)
		{
			_log = text2.text;
		}
		Scalar text3 = mapping.GetText("EType");
		if (text3 != null)
		{
			_etype = text3.text;
		}
		if (string.IsNullOrEmpty(_etype))
		{
			Debug.LogError("TriggerActionAnalyticsLog: EType should not be null or empty!");
		}
	}

	protected override void ApplyAction(object modelData)
	{
		if (string.IsNullOrEmpty(_etype))
		{
			return;
		}
		object[] array = new object[_formulas.Count];
		for (int i = 0; i < _formulas.Count; i++)
		{
			array[i] = _formulas[i].calculate();
		}
		LogDataUnit logDataUnit = new LogDataUnit();
		logDataUnit.Log = _log;
		logDataUnit.Name = _name;
		logDataUnit.Params = new Dictionary<string, string>();
		LogDataUnit logDataUnit2 = logDataUnit;
		if (_params != null)
		{
			foreach (Node item in _params.nodesInside)
			{
				logDataUnit2.Params.Add(item.key, new RpnParser.Formula(item.value.ToString()).calculate().ToString());
			}
		}
		if (string.IsNullOrEmpty(_etype))
		{
			Debug.LogError("TriggerActionAnalyticsLog: EType should not be null or empty!");
		}
		else
		{
			Analytics.Logger.AddEvent(_etype, JObject.FromObject(logDataUnit2));
		}
	}
}
