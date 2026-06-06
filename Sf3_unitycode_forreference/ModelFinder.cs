using SF3.GameModels;
using UnityEngine;

public class ModelFinder : MonoBehaviour
{
	private Model _modelOfObject;

	public Model ModelOfObject
	{
		get
		{
			return _modelOfObject ?? (_modelOfObject = GetComponentInParent<Model>());
		}
	}

	private void Start()
	{
		_modelOfObject = GetComponentInParent<Model>();
		if (null == _modelOfObject)
		{
			Debug.LogError("Model of object not found.");
		}
	}
}
