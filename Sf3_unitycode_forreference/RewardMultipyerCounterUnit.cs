public class RewardMultipyerCounterUnit
{
	public static readonly RewardMultipyerCounterUnit Empty = new RewardMultipyerCounterUnit(string.Empty, 0, 0.0);

	public double Value { get; private set; }

	public string Name { get; private set; }

	public int ID { get; private set; }

	public RewardMultipyerCounterUnit(string name, int id, double value)
	{
		Name = name;
		ID = id;
		Value = value;
	}

	public void SetValue(double value)
	{
		Value = value;
	}

	public void Add(double value)
	{
		Value += value;
	}
}
