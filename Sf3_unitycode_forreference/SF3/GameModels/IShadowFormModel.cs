namespace SF3.GameModels
{
	public interface IShadowFormModel
	{
		void ActivateShadowForm(bool instant = false);

		void DisableShadowForm();

		bool GetShadowFormActive();

		void UpdateShadowForm();
	}
}
