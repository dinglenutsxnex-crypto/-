using UnityEngine;

namespace SF3.GameModels
{
	public class DebugInfo_Skin : ExtentionBehaviour
	{
		public bool logInfo;

		private void Start()
		{
		}

		private void Update()
		{
			if (logInfo)
			{
				logInfo = false;
				WriteSkinInfo();
			}
		}

		private void WriteSkinInfo()
		{
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			Debug.Log("----------  " + component.name + "  ----------");
			Debug.Log("rootBone : " + component.rootBone);
			Debug.Log("bones : ");
			Transform[] bones = component.bones;
			foreach (Transform transform in bones)
			{
				Debug.Log(transform.name);
			}
			Debug.Log(string.Empty);
		}
	}
}
