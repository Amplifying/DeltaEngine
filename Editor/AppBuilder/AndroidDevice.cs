namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Represents a Android device (emulator or real connected one) that provides the functionality
	/// to install, uninstall and launch applications on it.
	/// </summary>
	public class AndroidDevice : Device
	{
		public AndroidDevice(AndroidDebugBridgeRunner adbRunner, AndroidDeviceInfo deviceInfo)
		{
			this.adbRunner = adbRunner;
			AdbId = deviceInfo.AdbDeviceId;
			state = deviceInfo.DeviceState;
			Name = adbRunner.GetDeviceName(AdbId);
		}

		private readonly AndroidDebugBridgeRunner adbRunner;
		internal string AdbId { get; private set; }
		private readonly string state;

		public string Name { get; private set; }

		public bool IsEmulator
		{
			get { return Name.StartsWith("emulator"); }
		}

		public bool IsConnected
		{
			get { return state == ConnectedState; }
		}

		// The value for the disconnected state is "offline"
		private const string ConnectedState = "device";

		public bool IsAppInstalled(AppInfo app)
		{
			return adbRunner.IsAppInstalled(this, app.GetFullAppNameForEngineApp());
		}
		
		public void Install(AppInfo app)
		{
			adbRunner.InstallPackage(this, app.FilePath);
		}

		public void Uninstall(AppInfo app)
		{
			adbRunner.UninstallPackage(this, app.GetFullAppNameForEngineApp());
		}

		public void Launch(AppInfo app)
		{
			adbRunner.StartEngineBuiltApplication(this, app.GetFullAppNameForEngineApp());
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}