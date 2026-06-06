using Nekki;
using UnityEngine;

namespace SF3.GameDebug
{
	public class DebugActionApplier : MonoBehaviour
	{
		public EDebugActions currentDebugAction;

		private void Start()
		{
			if (!NekkiUtils.IsDebug)
			{
				Object.Destroy(this);
			}
			else
			{
				GetComponent<UIButton>().onClick.Add(new EventDelegate(DebugAction));
			}
		}

		private void DebugAction()
		{
			GameDebugController.CheckDebugAction(currentDebugAction);
		}
	}
}
