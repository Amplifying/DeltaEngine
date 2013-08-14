using System;
using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.AppBuilder
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
			return File.Exists(app.FilePath);
		}


		public void Install(AppInfo app)
		{
		}

		public void Uninstall(AppInfo app)
		{
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
				: base(app.ToString()) {}
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}