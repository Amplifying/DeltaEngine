using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.AppBuilder.Windows
{
	public class WindowsDevice : Device
	{
		public WindowsDevice()
		{
			Name = Environment.UserDomainName;
			IsEmulator = false;
		}

		public override bool IsAppInstalled(AppInfo app)
		{
			return app != null && Directory.Exists(GetAppExtractionDirectory(app));
		}

		private static string GetAppExtractionDirectory(AppInfo app)
		{
			string fullFilePath = Path.IsPathRooted(app.FilePath)
				? app.FilePath : Path.Combine(Environment.CurrentDirectory, app.FilePath);
			return Path.Combine(Path.GetDirectoryName(fullFilePath), app.Name);
		}

		public override void Install(AppInfo app)
		{
			var startedProcess = Process.Start(app.FilePath, "/S /D=" + GetAppExtractionDirectory(app));
			startedProcess.WaitForExit();
		}

		public override void Uninstall(AppInfo app)
		{
			if (IsAppInstalled(app))
				Directory.Delete(GetAppExtractionDirectory(app), true);
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(WindowsDevice device, string appName)
				: base(appName + " on " + device) { }
		}

		protected override void LaunchApp(AppInfo app)
		{
			if (!IsAppInstalled(app))
				throw new AppNotInstalled(app);
			try
			{
				string exeFilePath = Path.Combine(GetAppExtractionDirectory(app), app.Name + ".exe");
				string exeDirectory = PathExtensions.GetAbsolutePath(Path.GetDirectoryName(exeFilePath));
				var processRunner = new ProcessRunner(exeFilePath) { WorkingDirectory = exeDirectory };
				processRunner.Start();
			}
			catch (Exception ex)
			{
				Logger.Warning(app.Name + " was closed with error: " + ex);
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