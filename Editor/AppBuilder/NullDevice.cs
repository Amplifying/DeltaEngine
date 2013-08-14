using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Represents a proxy device class which is thought for the case that no real device is
	/// currently available for the current chosen platform.
	/// </summary>
	public class NullDevice : Device
	{
		public NullDevice()
		{
			Name = "No emulator or connected device available for this platform.";
			IsEmulator = false;
		}

		public string Name { get; private set; }
		public bool IsEmulator { get; private set; }

		public bool IsAppInstalled(AppInfo app)
		{
			return false;
		}

		public void Install(AppInfo app)
		{
		}

		public void Uninstall(AppInfo app)
		{
		}

		public void Launch(AppInfo app)
		{
		}
	}
}
