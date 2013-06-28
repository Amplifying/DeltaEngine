using System;
using System.IO;
using System.Linq;
using DeltaEngine.Content.Online;
using DeltaEngine.Core;
using DeltaEngine.Graphics.GLFW;
using DeltaEngine.Input;
using DeltaEngine.Input.GLFW;
using DeltaEngine.Logging;
using DeltaEngine.Logging.Basic;
using DeltaEngine.Multimedia.GLFW;
using DeltaEngine.Networking.Sockets;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Platforms
{
	internal class GLFWResolver : AutofacStarter
	{
		public GLFWResolver()
		{
			MakeSureGlfwDllsAreAvailable();
			new DeveloperOnlineContentLoader(new AutofacContentDataResolver(this));
			if (!(Time.Current is StopwatchTime))
				Time.Current = new StopwatchTime();
			RegisterInstance(Time.Current);
			var settings = new FileSettings();
			RegisterInstance(settings);
			if (!(Logger.Current is DefaultLogger))
				Logger.Current = new DefaultLogger(new TcpSocket(), settings);
			RegisterSingleton<FileSettings>();
			RegisterSingleton<GLFWWindow>();
			RegisterSingleton<GLFWDevice>();
			RegisterSingleton<GLFWDrawing>();
			RegisterSingleton<GLFWScreenshotCapturer>();
			RegisterSingleton<QuadraticScreenSpace>();
			RegisterSingleton<LookAtCamera>();
			RegisterSingleton<InputCommands>();
			RegisterSingleton<FarseerPhysics>();
			RegisterSingleton<GLFWSoundDevice>();
			RegisterSingleton<GLFWMouse>();
			RegisterSingleton<GLFWKeyboard>();
			RegisterSingleton<GLFWTouch>();
			RegisterSingleton<GLFWGamePad>();
			RegisterSingleton<GLFWSystemInformation>();
			RegisterSingleton<RelativeScreenSpace>();
		}

		private void MakeSureGlfwDllsAreAvailable()
		{
			if (AreNativeDllsMissing())
				TryCopyNativeDlls();
		}

		private bool AreNativeDllsMissing()
		{
			return nativeDllsNeeded.Any(nativeDll => !File.Exists(nativeDll));
		}

		private readonly string[] nativeDllsNeeded = { "glfw.dll", "openal32.dll", "wrap_oal.dll" };

		private void TryCopyNativeDlls()
		{
			try
			{
				CopyNativeDlls();
			}
			catch (Exception ex)
			{
				throw new FailedToCopyNativeGlfwDllFiles("Please provide the glfw.dll files!", ex);
			}
		}

		private void CopyNativeDlls()
		{
			var path = Path.Combine("..", "..");
			while (!IsPackagesDirectory(path))
			{
				path = Path.Combine(path, "..");
				if (path.Length > 18)
					break;
			}
			foreach (var nativeDll in nativeDllsNeeded)
				File.Copy(
					Path.Combine(path, "packages", "Pencil.Gaming.1.0.4915", "NativeBinaries", "x86",
						nativeDll), nativeDll, true);
		}

		private static bool IsPackagesDirectory(string path)
		{
			return Directory.Exists(Path.Combine(path, "packages"));
		}

		private class FailedToCopyNativeGlfwDllFiles : Exception
		{
			public FailedToCopyNativeGlfwDllFiles(string message, Exception innerException)
				: base(message, innerException) {}
		}
	}
}