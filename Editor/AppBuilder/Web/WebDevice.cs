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

		public override bool IsAppInstalled(AppInfo app)
		{
			return File.Exists(app.FilePath);
		}

		public override void Install(AppInfo app) { }

		public override void Uninstall(AppInfo app)
		{
			if (app == null)
				throw new UninstallationFailedOnDevice(this, "null");
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(WebDevice device, string appName)
				: base(appName + " on " + device) { }
		}

		protected override void LaunchApp(AppInfo app)
		{
			Process.Start(app.FilePath);
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}