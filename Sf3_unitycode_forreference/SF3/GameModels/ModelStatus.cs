using System;

namespace SF3.GameModels
{
	public abstract class ModelStatus
	{
		public enum EStatusType
		{
			MODHP = 0
		}

		public const float PERMANENT_DURATION_FLAG = -1f;

		protected Model targetModel;

		protected float updateTimer;

		protected bool isPermanent;

		public string name { get; protected set; }

		public float duration { get; protected set; }

		public EStatusType statusType { get; protected set; }

		public bool stackable { get; protected set; }

		public event Action<ModelStatus> onDurationEnd = delegate
		{
		};

		public ModelStatus(ModelStatusPrototype prototype)
		{
			name = prototype.name;
			statusType = prototype.statusType;
			duration = prototype.duration;
			stackable = prototype.stackable;
			isPermanent = duration == -1f;
		}

		public static ModelStatus CreateInstance(ModelStatusPrototype prototype)
		{
			if (prototype.statusType == EStatusType.MODHP)
			{
				return new ModelHPStatus((ModelStatusHPPrototype)prototype);
			}
			return null;
		}

		public virtual void Activate(Model targetModel)
		{
			this.targetModel = targetModel;
			updateTimer = GameTimeController.battleTime + duration;
		}

		protected abstract void OnUpdate();

		protected virtual void OnDurationEnd()
		{
			this.onDurationEnd(this);
		}

		public virtual bool Update()
		{
			OnUpdate();
			if (!isPermanent && GameTimeController.battleTime >= updateTimer)
			{
				OnDurationEnd();
				return true;
			}
			return false;
		}
	}
}
