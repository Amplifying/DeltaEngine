using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;

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
			return app != null && Directory.Exists(GetAppExtractionDirectory(app));
		}

		private static string GetAppExtractionDirectory(AppInfo app)
		{
			string fullFilePath = Path.IsPathRooted(app.FilePath)
				? app.FilePath : Path.Combine(Environment.CurrentDirectory, app.FilePath);
			return Path.Combine(Path.GetDirectoryName(fullFilePath), app.Name);
		}

		public void Install(AppInfo app)
		{
			Process.Start(app.FilePath, "/S /D=" + GetAppExtractionDirectory(app));
			Thread.Sleep(50);
		}

		public void Uninstall(AppInfo app)
		{
			if (IsAppInstalled(app))
				Directory.Delete(GetAppExtractionDirectory(app), true);
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
			try
			{
				string exeFilePath = Path.Combine(GetAppExtractionDirectory(app), app.Name + ".exe");
				new ProcessRunner(exeFilePath).Start();
			}
			catch (Exception)
			{
				Logger.Warning(app.Name + " was closed with error");
			}
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