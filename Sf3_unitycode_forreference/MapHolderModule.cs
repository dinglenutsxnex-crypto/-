public class MapHolderModule : HolderModuleManager
{
	protected override void Mount(string name = "")
	{
		base.Mount("MapModule");
	}
}
