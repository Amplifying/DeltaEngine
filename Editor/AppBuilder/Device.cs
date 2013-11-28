using System;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Represents a general interface for a device object of any platform.
	/// </summary>
	public abstract class Device
	{
		public string Name { get; protected set; }
		public bool IsEmulator { get; protected set; }
		public abstract bool IsAppInstalled(AppInfo app);

		public void Install(AppInfo app)
		{
			if (app == null)
				throw new NoAppSpecified();
			if (!IsAppInstalled(app))
				InstallApp(app);
		}

		public class NoAppSpecified : Exception {}

		protected abstract void InstallApp(AppInfo app);

		public class InstallationFailedOnDevice : Exception
		{
			public InstallationFailedOnDevice(Device device, AppInfo app)
				: base(app.Name + " on " + device) { }
		}

		public void Uninstall(AppInfo app)
		{
			if (app == null)
				throw new NoAppSpecified();
			if (IsAppInstalled(app))
				UninstallApp(app);
		}

		protected abstract void UninstallApp(AppInfo app);

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(Device device, AppInfo app)
				: base(app.Name + " on " + device) { }
		}

		public void Launch(AppInfo app)
		{
			try
			{
				if (app == null)
					throw new NoAppSpecified();
				if (!IsAppInstalled(app))
					throw new AppNotInstalled(this, app);
				LaunchApp(app);
			}
			catch (Exception ex)
			{
				throw new StartApplicationFailedOnDevice(Name, ex);
			}
		}

		public class AppNotInstalled : Exception
		{
			public AppNotInstalled(Device device, AppInfo app)
				: base(app.Name + " on " + device) { }
		}

		protected abstract void LaunchApp(AppInfo app);

		public class StartApplicationFailedOnDevice : Exception
		{
			public StartApplicationFailedOnDevice(string deviceName, Exception reason) :
				base(deviceName, reason)
			{
				DeviceName = deviceName;
			}

			public string DeviceName { get; private set; }
		}
	}
}