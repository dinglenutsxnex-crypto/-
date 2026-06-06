namespace SF3
{
	public class ModelRoundData
	{
		public string name { get; private set; }

		public float shadowEnergy { get; set; }

		public ModelRoundData(string name)
		{
			this.name = name;
		}

		public void ApplyToModel(int modelID)
		{
			ShadowFormController.Instance.SetShadowCharge(modelID, shadowEnergy);
		}
	}
}
