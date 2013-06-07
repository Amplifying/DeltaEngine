using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Represents a proxy device class which is thought for the case that no real device is
	/// currently available for the current chosen platform.
	/// </summary>
	class NullDevice : Device
	{
		public NullDevice()
		{
			Name = "No emulator or connected device available for this platform.";
			IsEmulator = false;
		}

		public string Name { get; private set; }
		public bool IsEmulator { get; private set; }

		public bool IsAppInstalled(AppPackage app)
		{
			return false;
		}

		public void Install(AppPackage app)
		{
		}

		public void Uninstall(AppPackage app)
		{
		}

		public void Launch(AppPackage app)
		{
		}
	}
}
