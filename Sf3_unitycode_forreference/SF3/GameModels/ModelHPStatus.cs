namespace SF3.GameModels
{
	public class ModelHPStatus : ModelStatus
	{
		private float stepTime;

		public float hpPerFrame { get; private set; }

		public float step { get; private set; }

		public ModelHPStatus(ModelStatusHPPrototype prototype)
			: base(prototype)
		{
			hpPerFrame = prototype.hpPerFrame;
			step = prototype.step;
			stepTime = 0f;
		}

		protected override void OnUpdate()
		{
			stepTime += GameTimeController.gameTimeDelta;
			if (stepTime >= step)
			{
				targetModel.modelInfo.ChangeLife(hpPerFrame * GameTimeController.gameTimeDelta);
				stepTime -= step;
			}
		}
	}
}
