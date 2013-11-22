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

		public override bool IsAppInstalled(AppInfo app)
		{
			return false;
		}

		public override void Install(AppInfo app) { }

		public override void Uninstall(AppInfo app) { }

		protected override void LaunchApp(AppInfo app) { }
	}
}