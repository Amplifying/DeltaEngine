using System;
using Microsoft.SmartDevice.Connectivity;
using MsDevice = Microsoft.SmartDevice.Connectivity.Device;

namespace DeltaEngine.Editor.AppBuilder.WindowsPhone7
{
	/// <summary>
	/// Represents a WP7 device (emulator or real connected one) that provides the functionality to
	/// install, uninstall and launch applications on it.
	/// </summary>
	/// <remarks>
	/// Deploy automation: http://justinangel.net/WindowsPhone7EmulatorAutomation
	/// Starting the emulator: http://geekswithblogs.net/cwilliams/archive/2010/08/03/141171.aspx
	/// </remarks>
	public class WP7Device : Device
	{
		internal WP7Device(MsDevice nativeDevice)
		{
			this.nativeDevice = nativeDevice;
			Name = nativeDevice.Name;
			IsEmulator = Name.Contains("Emulator");
		}

		private readonly MsDevice nativeDevice;

		public override bool IsAppInstalled(AppInfo app)
		{
			MakeSureDeviceConnectionIsEstablished();
			return nativeDevice.IsApplicationInstalled(app.AppGuid);
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
			// ncrunch: no coverage start
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
		// ncrunch: no coverage end

		protected override void InstallApp(AppInfo app)
		{
			try
			{
				MakeSureDeviceConnectionIsEstablished();
				nativeDevice.InstallApplication(app.AppGuid, app.AppGuid, "Apps.Normal", "", app.FilePath);
			}
			catch (ArgumentException)
			{
				throw new InstallationFailedOnDevice(this, app);
			}
		}

		protected override void UninstallApp(AppInfo app)
		{
			try
			{
				MakeSureDeviceConnectionIsEstablished();
				RemoteApplication appOnDevice = nativeDevice.GetApplication(app.AppGuid);
				appOnDevice.Uninstall();
			}
			catch (SmartDeviceException)
			{
				throw new UninstallationFailedOnDevice(this, app);
			}
		}

		protected override void LaunchApp(AppInfo app)
		{
			RemoteApplication appOnDevice = nativeDevice.GetApplication(app.AppGuid);
			appOnDevice.Launch();
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}