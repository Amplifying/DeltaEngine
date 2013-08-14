using System;
using System.IO;
using System.Linq;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.AppBuilder
{
	public abstract class AppInfo
	{
		protected AppInfo(string fullAppDataFilePath, Guid appGuid, PlatformName platform)
		{
			FilePath = fullAppDataFilePath;
			AppGuid = appGuid;
			Name = Path.GetFileNameWithoutExtension(fullAppDataFilePath);
			Platform = platform;
		}

		public string FilePath { get; private set; }
		public Guid AppGuid { get; private set; }
		public string Name { get; protected set; }
		public PlatformName Platform { get; private set; }

		public Device[] AvailableDevices
		{
			get
			{
				if (availableDevices == null)
					availableDevices = GetAvailableDevices();
				return availableDevices;
			}
		}

		private static Device[] availableDevices;

		protected abstract Device[] GetAvailableDevices();

		public bool IsDeviceAvailable
		{
			get { return AvailableDevices.Length > 0; }
		}

		public string SolutionFilePath { get; set; }

		public bool IsSolutionPathAvailable
		{
			get { return SolutionFilePath != null && SolutionFilePath.EndsWith(".sln"); }
		}

		public void LaunchApp(Device device)
		{
			ValidateDevice(device);
			if (device.IsAppInstalled(this))
				device.Uninstall(this);
			device.Install(this);
			device.Launch(this);
		}

		private void ValidateDevice(Device device)
		{
			if (device == null)
				throw new NoDeviceSpecified();
			if (availableDevices.All(availableDevice => device != availableDevice))
				throw new WrongDeviceForApp(device, this);
		}

		public class NoDeviceSpecified : Exception { }

		public class WrongDeviceForApp : Exception
		{
			public WrongDeviceForApp(Device device, AppInfo appInfo)
				: base("Device=" + device + ", App=" + appInfo) { }
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + " for " + Platform + ")";
		}
	}
}