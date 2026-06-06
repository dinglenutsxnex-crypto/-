public class InventoryHolderModule : HolderModuleManager
{
	public const float FadeDuration = 1f;

	protected override void Mount(string name = "")
	{
		base.Mount("Inventory");
	}
}
