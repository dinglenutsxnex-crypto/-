using System;
using Nekki.Yaml;
using SF3.UserData;
using SimpleJSON;

public abstract class IntentModule
{
	public enum ModuleTransitionType
	{
		Float = 0,
		Jump = 1
	}

	private const string MODULE_KEY = "Module";

	private const string INTENT_KEY = "Intent";

	public Action CallbackOpenModule;

	protected object[] Properties;

	public ConstantsSF3.ELocationSceneModule TypeModule { get; set; }

	public ModuleTransitionType TransitionType { get; set; }

	public IntentTransitionData TransitionData { get; private set; }

	public bool IsSkip { get; set; }

	public bool IsInterrupted { get; set; }

	public virtual void Init(ConstantsSF3.ELocationSceneModule type, params object[] args)
	{
		TypeModule = type;
		Properties = args;
		TransitionType = ModuleTransitionType.Float;
		IsSkip = false;
		IsInterrupted = false;
	}

	public virtual bool Equal(IntentModule value)
	{
		return TypeModule == value.TypeModule;
	}

	public virtual void CreateTransitionData(IntentModule intent)
	{
		TransitionData = new IntentTransitionData(intent, this);
	}

	protected virtual bool HasProperties()
	{
		return Properties != null && Properties.Length > 0 && Properties[0] != null;
	}

	public bool IsInstant()
	{
		return TransitionType == ModuleTransitionType.Jump;
	}

	protected bool IsIntentParametrs(object o)
	{
		return o is IntentParametrs;
	}

	public virtual void Update()
	{
		UserManager.SetIntentModule(ToJSON());
	}

	public virtual void RunCallbackOpenModule()
	{
		CallbackOpenModule.InvokeSafe();
	}

	public override string ToString()
	{
		return TypeModule.ToString();
	}

	public virtual Mapping ToYaml()
	{
		return new Mapping("Intent", new Scalar("Module", TypeModule.ToString()));
	}

	public virtual JSONClass ToJSON()
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass.Add("Module", TypeModule.ToString());
		return jSONClass;
	}
}
