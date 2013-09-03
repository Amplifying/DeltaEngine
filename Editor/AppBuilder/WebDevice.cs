using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.AppBuilder
{
	public class WebDevice : Device
	{
		public WebDevice()
		{
			Name = "Web"; // later support other browsers too
			IsEmulator = false;
		}

		public string Name { get; private set; }
		public bool IsEmulator { get; private set; }

		public bool IsAppInstalled(AppInfo app)
		{
			return File.Exists(app.FilePath);
		}

		public void Install(AppInfo app) {}
		public void Uninstall(AppInfo app) {}

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