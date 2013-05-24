using System;
using System.IO;
using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	public class AndroidDevice : Device
	{
		public AndroidDevice(string adbDeviceId, string deviceState, string deviceName)
		{
			AdbId = adbDeviceId;
			state = deviceState;
			Name = deviceName;
		}

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

		private const string ConnectedState = "device";
		private const string DisconnectedState = "offline";

		public bool IsAppInstalled(AppPackage app)
		{
			return false;
		}

		public void Install(AppPackage app)
		{
			SavePackage(app);
			var adbRunner = new AndroidDebugBridgeRunner();
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
			var adbRunner = new AndroidDebugBridgeRunner();
			adbRunner.StartApplication(this, app.AppName);
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}