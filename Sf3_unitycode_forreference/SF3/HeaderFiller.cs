using Nekki.UI;

namespace SF3
{
	public abstract class HeaderFiller
	{
		private NekkiUILabel _header;

		private FightResult _result;

		public void Set(FightResult result, NekkiUILabel header)
		{
			_result = result;
			_header = header;
		}

		public bool IsFilled()
		{
			if (!IsEqualConditions(_result))
			{
				return false;
			}
			Fill(_result, _header);
			return true;
		}

		protected void SetAlias(string alias)
		{
			_header.Alias = alias;
		}

		protected void SetFormat(params object[] replacement)
		{
			_header.Format(replacement);
		}

		protected abstract bool IsEqualConditions(FightResult result);

		protected abstract void Fill(FightResult result, NekkiUILabel header);
	}
}
