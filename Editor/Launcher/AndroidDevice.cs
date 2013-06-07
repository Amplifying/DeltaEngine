using System;
using System.IO;
using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Represents a Android device (emulator or real connected one) that provides the functionality
	/// to install, uninstall and launch applications on it.
	/// </summary>
	public class AndroidDevice : Device
	{
		public AndroidDevice(AndroidDebugBridgeRunner adbRunner, string adbDeviceId, string deviceState)
		{
			this.adbRunner = adbRunner;
			AdbId = adbDeviceId;
			state = deviceState;
			DetermineDeviceName();
		}

		private readonly AndroidDebugBridgeRunner adbRunner;
		internal string AdbId { get; private set; }
		private readonly string state;

		private void DetermineDeviceName()
		{
			Name = adbRunner.GetDeviceName(AdbId);
		}

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

		public bool IsAppInstalled(AppPackage app)
		{
			return false;
		}

		public void Install(AppPackage app)
		{
			SavePackage(app);
			adbRunner.InstallPackage(this, GetPackageSaveFilePath(app));
		}

		private static void SavePackage(AppPackage app)
		{
			string packageSavePath = GetPackageSaveFilePath(app);
			string packageDirectory = Path.GetDirectoryName(packageSavePath);
			Console.WriteLine(Environment.CurrentDirectory);
			if (!Directory.Exists(packageDirectory))
				Directory.CreateDirectory(packageDirectory);

			File.WriteAllBytes(GetPackageSaveFilePath(app), app.PackageFileData);
		}

		private static string GetPackageSaveFilePath(AppPackage app)
		{
			return Path.Combine(PackagesFolderName, app.PackageFileName);
		}

		private const string PackagesFolderName = "Packages";

		public void Uninstall(AppPackage app)
		{
			throw new NotImplementedException();
		}

		public void Launch(AppPackage app)
		{
			adbRunner.StartApplication(this, app.AppName);
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}