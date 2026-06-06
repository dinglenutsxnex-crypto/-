using System.Collections.Generic;

namespace SF3.Items
{
	public abstract class ClassHolder<T>
	{
		protected Dictionary<string, List<T>> Holder;

		protected ClassHolder()
		{
			Holder = new Dictionary<string, List<T>>();
		}
	}
}
