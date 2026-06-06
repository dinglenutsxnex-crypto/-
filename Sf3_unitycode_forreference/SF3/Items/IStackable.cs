namespace SF3.Items
{
	public interface IStackable
	{
		double GetStackLevel();

		void SetStackLevel(double value);

		void MergeSimilarItems(IStackable newItem);

		int GetLevelUpCount(IStackable newItem);

		float GetBarValue();

		float GetLevelUpBar(IStackable newItem);
	}
}
