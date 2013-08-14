namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Represents a general interface for a device object of any platform.
	/// </summary>
	public interface Device
	{
		string Name { get; }
		bool IsEmulator { get; }
		bool IsAppInstalled(AppInfo app);
		void Install(AppInfo app);
		void Uninstall(AppInfo app);
		void Launch(AppInfo app);
	}
}