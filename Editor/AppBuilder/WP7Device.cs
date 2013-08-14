using System;
using Microsoft.SmartDevice.Connectivity;
using MsDevice = Microsoft.SmartDevice.Connectivity.Device;

namespace DeltaEngine.Editor.AppBuilder
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

		public bool IsAppInstalled(AppInfo app)
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

		public void Uninstall(AppInfo app)
		{
			MakeSureDeviceConnectionIsEstablished();
			RemoteApplication appOnDevice = nativeDevice.GetApplication(app.AppGuid);
			appOnDevice.Uninstall();
		}

		public void Install(AppInfo app)
		{
			MakeSureDeviceConnectionIsEstablished();
			nativeDevice.InstallApplication(app.AppGuid, app.AppGuid, "Apps.Normal", "", app.FilePath);
		}

		public void Launch(AppInfo app)
		{
			if (!IsAppInstalled(app))
				throw new AppNotInstalled(app);
			RemoteApplication appOnDevice = nativeDevice.GetApplication(app.AppGuid);
			appOnDevice.Launch();
		}

		public class AppNotInstalled : Exception
		{
			public AppNotInstalled(AppInfo app)
				: base(app.ToString()) { }
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}