using System;
using System.Collections.Generic;

namespace SF3.GameModels
{
	[Serializable]
	public class ModelStatusControl
	{
		private Model _modelState;

		private Dictionary<string, List<ModelStatus>> _statuses;

		public ModelStatusControl(Model modelstate)
		{
			_modelState = modelstate;
			_statuses = new Dictionary<string, List<ModelStatus>>();
		}

		public void Reset()
		{
			_statuses.Clear();
		}

		public void UpdateStatuses()
		{
			foreach (List<ModelStatus> value in _statuses.Values)
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i].Update())
					{
						i--;
					}
				}
			}
		}

		public void ApplyStatus(ModelStatusPrototype prototype)
		{
			if (!_statuses.ContainsKey(prototype.name))
			{
				_statuses.Add(prototype.name, new List<ModelStatus>());
			}
			else if (!prototype.stackable)
			{
				_statuses[prototype.name].Clear();
			}
			ModelStatus modelStatus = ModelStatus.CreateInstance(prototype);
			modelStatus.Activate(_modelState);
			modelStatus.onDurationEnd += RemoveStatus;
			_statuses[prototype.name].Add(modelStatus);
		}

		public void ClearStatus(string name)
		{
			if (name.IsNullOrEmpty())
			{
				_statuses.Clear();
			}
			else if (_statuses.ContainsKey(name))
			{
				_statuses[name].Clear();
			}
		}

		private void RemoveStatus(ModelStatus status)
		{
			if (_statuses.ContainsKey(status.name) && _statuses[status.name].Count > 0)
			{
				_statuses[status.name].Remove(status);
			}
		}
	}
}
