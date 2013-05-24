using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Represents a general interface for a device object of any platform.
	/// </summary>
	public interface Device
	{
		string Name { get; }
		bool IsEmulator { get; }
		bool IsAppInstalled(AppPackage app);
		void Install(AppPackage app);
		void Uninstall(AppPackage app);
		void Launch(AppPackage app);
	}
}