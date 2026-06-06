using UnityEngine;

public class TestRagdoll : ExtentionBehaviour
{
	[SerializeField]
	private UIButton Kill;

	[SerializeField]
	private UIButton Restart;

	[SerializeField]
	private Transform foe;

	private Animator foeAnimator;

	private CapsuleCollider foeCollider;

	private Vector3 foeBasePosition;

	internal void Start()
	{
		foeAnimator = foe.GetComponent<Animator>();
		foeCollider = foe.GetComponent<CapsuleCollider>();
		Kill.onClick.Add(new EventDelegate(DoKill));
		Restart.onClick.Add(new EventDelegate(DoRestart));
		foeBasePosition = foe.position;
		DoRestart();
	}

	private void DoKill()
	{
		Kill.gameObject.SetActive(false);
		Restart.gameObject.SetActive(true);
		foeCollider.enabled = false;
		foeAnimator.enabled = false;
	}

	private void DoRestart()
	{
		Kill.gameObject.SetActive(true);
		Restart.gameObject.SetActive(false);
		foeCollider.enabled = true;
		foeAnimator.enabled = true;
		foe.position = foeBasePosition;
	}

	internal void Update()
	{
	}
}
