using System.Collections.Generic;

namespace SF3.Moves
{
	public class ConditionList : IConditionEqual
	{
		private readonly List<IConditionEqual> _conditions;

		private readonly LogicOperationType _typeList;

		public ConditionList(string typeName, List<IConditionEqual> conditionsVar)
		{
			_conditions = new List<IConditionEqual>(conditionsVar);
			if (typeName != null)
			{
				_typeList = Condition.GetLogicOperationByName(typeName);
			}
		}

		public bool? IsEqual()
		{
			bool value = false;
			if (_typeList == LogicOperationType.AND)
			{
				value = true;
				foreach (IConditionEqual condition in _conditions)
				{
					if (condition.IsEqual() == false)
					{
						value = false;
						break;
					}
				}
			}
			else if (_typeList == LogicOperationType.OR)
			{
				value = false;
				foreach (IConditionEqual condition2 in _conditions)
				{
					if (condition2.IsEqual() != false)
					{
						value = true;
						break;
					}
				}
			}
			else if (_typeList == LogicOperationType.NOT)
			{
				value = true;
				foreach (IConditionEqual condition3 in _conditions)
				{
					if (condition3.IsEqual() == true)
					{
						value = false;
						break;
					}
				}
			}
			return value;
		}
	}
}
