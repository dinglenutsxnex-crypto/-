namespace SF3.Effects
{
	public interface IGameEffect
	{
		void Create();

		void Initialize();

		void Play();

		void Stop();

		void StopForce();

		void Reset();
	}
}
