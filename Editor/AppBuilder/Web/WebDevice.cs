using System;
using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.AppBuilder.Web
{
	public class WebDevice : Device
	{
		public WebDevice()
		{
			Name = "Web";
			IsEmulator = false;
		}

		public string Name { get; private set; }
		public bool IsEmulator { get; private set; }

		public bool IsAppInstalled(AppInfo app)
		{
			return File.Exists(app.FilePath);
		}

		public void Install(AppInfo app) {}

		public void Uninstall(AppInfo app)
		{
			if (app == null)
				throw new UninstallationFailedOnDevice(this, "null");
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(WebDevice device, string appName)
				: base(appName + " on " + device) { }
		}

		public void Launch(AppInfo app)
		{
			Process.Start(app.FilePath);
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}