using System;
using System.IO;
using DeltaEngine.Editor.Builder;
using Microsoft.SmartDevice.Connectivity;
using MsDevice = Microsoft.SmartDevice.Connectivity.Device;

namespace DeltaEngine.Editor.Launcher
{
	// AssemblyReferences:
	// Deploy automation
	// @see http://justinangel.net/WindowsPhone7EmulatorAutomation
	// Starting the emulator
	// @see http://geekswithblogs.net/cwilliams/archive/2010/08/03/141171.aspx

	/// <summary>
	/// Represents a WP7 device (emulator or real connected one) that provides the functionality to
	/// install, uninstall and launch applications on it.
	/// </summary>
	public class WP7Device : Device
	{
		internal WP7Device(MsDevice nativeDevice)
		{
			this.nativeDevice = nativeDevice;
		}

		private readonly MsDevice nativeDevice;

		public string Name
		{
			get { return nativeDevice.Name; }
		}

		public bool IsEmulator
		{
			get { return Name.Contains("Emulator"); }
		}

		public bool IsAppInstalled(AppPackage app)
		{
			MakeSureDeviceConnectionIsEstablished();
			return nativeDevice.IsApplicationInstalled(app.PackageGuid);
		}

		private void MakeSureDeviceConnectionIsEstablished()
		{
			if (nativeDevice.IsConnected())
				return;

			EstablishConnection();
		}

		private void EstablishConnection()
		{
			try
			{
				nativeDevice.Connect();
			}
			catch (Exception ex)
			{
				ThrowExceptionBasedOnReason(ex);
			}
		}

		private static void ThrowExceptionBasedOnReason(Exception ex)
		{
			string orgMessage = ex.Message;
			if (orgMessage.StartsWith("Zune software is not launched."))
				throw new ZuneNotLaunchedException();

			if (orgMessage.Contains("it is pin locked."))
				throw new ScreenLockedException();

			throw new CannotConnectException(orgMessage);
		}

		public class ZuneNotLaunchedException : Exception { }

		public class ScreenLockedException : Exception { }

		public class CannotConnectException : Exception
		{
			public CannotConnectException(string setMessage)
				: base(setMessage) { }
		}

		public void Uninstall(AppPackage app)
		{
			MakeSureDeviceConnectionIsEstablished();
			RemoteApplication appOnDevice = nativeDevice.GetApplication(app.PackageGuid);
			appOnDevice.Uninstall();
		}

		public void Install(AppPackage app)
		{
			MakeSureDeviceConnectionIsEstablished();
			SavePackage(app);
			nativeDevice.InstallApplication(app.PackageGuid, app.PackageGuid, "Apps.Normal", "",
				GetPackageSaveFilePath(app));
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

		public void Launch(AppPackage app)
		{
			MakeSureDeviceConnectionIsEstablished();
			RemoteApplication appOnDevice = nativeDevice.GetApplication(app.PackageGuid);
			appOnDevice.Launch();
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}