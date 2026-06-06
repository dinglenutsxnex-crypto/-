namespace SF3.GameModels
{
	public interface IModelHUD
	{
		string GetAlias();

		float GetCurrentLife();

		float GetMaxLife();

		int GetScore();
	}
}
