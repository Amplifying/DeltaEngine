using System;
using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.AppBuilder.Windows
{
	public class WindowsDevice : Device
	{
		public WindowsDevice()
		{
			Name = Environment.UserDomainName;
			IsEmulator = false;
		}

		public string Name { get; private set; }
		public bool IsEmulator { get; private set; }

		public bool IsAppInstalled(AppInfo app)
		{
			return app != null && File.Exists(app.FilePath);
		}


		public void Install(AppInfo app)
		{
		}

		public void Uninstall(AppInfo app)
		{
			if (app == null)
				throw new UninstallationFailedOnDevice(this, "null");
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(WindowsDevice device, string appName)
				: base(appName + " on " + device) { }
		}

		public void Launch(AppInfo app)
		{
			if (!IsAppInstalled(app))
				throw new AppNotInstalled(app);
			Process.Start(app.FilePath);
		}

		public class AppNotInstalled : Exception
		{
			public AppNotInstalled(AppInfo app)
				: base(app != null ? app.ToString() : "null") {}
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}