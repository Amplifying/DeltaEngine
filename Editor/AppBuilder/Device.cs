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
		public abstract void Install(AppInfo app);
		public abstract void Uninstall(AppInfo app);

		public void Launch(AppInfo app)
		{
			try
			{
				LaunchApp(app);
			}
			catch (Exception)
			{
				throw new StartApplicationFailedOnDevice(Name);
			}
		}

		protected abstract void LaunchApp(AppInfo app);

		public class StartApplicationFailedOnDevice : Exception
		{
			public StartApplicationFailedOnDevice(string deviceName) : base(deviceName)
			{
				DeviceName = deviceName;
			}

			public string DeviceName { get; private set; }
		}
	}
}