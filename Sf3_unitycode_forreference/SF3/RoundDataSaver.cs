using System.Collections.Generic;
using SF3.GameModels;

namespace SF3
{
	public class RoundDataSaver
	{
		private Dictionary<string, ModelRoundData> modelsRoundData = new Dictionary<string, ModelRoundData>();

		public void Initialize()
		{
			modelsRoundData.Clear();
		}

		public void SaveRoundData(Model model)
		{
			GetModelData(model.name).shadowEnergy = ((!model.modelShadowForm.shadowFormActive) ? model.modelShadowForm.shadowEnergy : 0f);
		}

		public ModelRoundData GetModelData(string modelName)
		{
			if (!modelsRoundData.ContainsKey(modelName))
			{
				modelsRoundData[modelName] = new ModelRoundData(modelName);
			}
			return modelsRoundData[modelName];
		}
	}
}
