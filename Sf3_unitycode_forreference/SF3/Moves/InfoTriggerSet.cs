using System;
using System.Collections.Generic;
using System.Text;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	[Serializable]
	public class InfoTriggerSet : IHasFileName
	{
		[SerializeField]
		private string _name;

		[SerializeField]
		private List<string> _templates;

		[SerializeField]
		private List<InfoTrigger> _triggers;

		[SerializeField]
		private List<IConditionEqual> _locks;

		[SerializeField]
		private string _fileName;

		public string name
		{
			get
			{
				return _name;
			}
		}

		public List<string> templates
		{
			get
			{
				return _templates;
			}
		}

		public List<InfoTrigger> triggers
		{
			get
			{
				return _triggers;
			}
		}

		public List<IConditionEqual> locks
		{
			get
			{
				return _locks;
			}
		}

		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		public InfoTriggerSet(string nameVar)
		{
			_triggers = new List<InfoTrigger>();
			_templates = new List<string>();
			_locks = new List<IConditionEqual>();
			_name = nameVar;
		}

		public void SetFileName(string fileName)
		{
			_fileName = fileName;
		}

		public void SetTemplates(List<string> templatesVar)
		{
			_templates = templatesVar;
		}

		public void AddTrigger(InfoTrigger triggerVar)
		{
			if (triggerVar != null)
			{
				_triggers.Add(triggerVar);
			}
			else
			{
				Debug.LogWarning("Trigger is null!!! Somesing wrong.. :)");
			}
		}

		public void SetLocks(List<IConditionEqual> newLocks)
		{
			_locks = newLocks;
		}

		public bool CheckLocks(Model model)
		{
			return TriggersVerification.CheckConditionsEqual(model, null, _locks);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Name: [{0}]", _name);
			return stringBuilder.ToString();
		}
	}
}
