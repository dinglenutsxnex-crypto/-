namespace SF3.Effects
{
	public interface IGameEffectGeneral
	{
		void Create();

		void Initialize();

		void Play(string effectName, int modelUsedID = -1);

		void StopAll(bool forced = false);

		void Stop(string effectName, bool forced = false);

		void Stop(string effectName, int modelID = -1);

		void DisposeEffectsByModel(int modelID);

		void Reset(string effectName);
	}
}
