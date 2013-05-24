using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Runs the ADB tool (which is provided by the Android SDK) via command line.
	/// </summary>
	/// <see cref="http://developer.android.com/tools/help/adb.html"/>
	public class AndroidDebugBridgeRunner
	{
		public string[] GetInfosOfAvailableDevices()
		{
			var androidDevicesNames = new List<string>();
			try
			{
				var processRunner = new ProcessRunner(GetAdbPath(), "devices");
				processRunner.StandardOutputEvent += outputMessage =>
				{
					if (IsDeviceName(outputMessage))
						androidDevicesNames.Add(outputMessage);
				};
				processRunner.Start();
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				Console.WriteLine("The adb tool has returned with an error");
			}

			return androidDevicesNames.ToArray();
		}

		private static string GetAdbPath()
		{
			string programFilesDirectory =
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			Console.WriteLine("programFilesDirectory: " + programFilesDirectory);
			Console.WriteLine("GetAdbPath: " + Path.Combine(programFilesDirectory, "Google",
				"adt-bundle-windows-x86_64", "sdk", "platform-tools", "adb.exe"));
			return Path.Combine(programFilesDirectory, "Google", "adt-bundle-windows-x86_64", "sdk",
				"platform-tools", "adb.exe");
		}

		private static bool IsDeviceName(string devicesRequestMessage)
		{
			return !(devicesRequestMessage.StartsWith("list", StringComparison.OrdinalIgnoreCase) ||
				String.IsNullOrWhiteSpace(devicesRequestMessage));
		}

		public void InstallPackage(AndroidDevice device, string apkFilePath)
		{
			try
			{
				TryRunAdbProcess("-s " + device.AdbId + " install -r " + apkFilePath);
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new InstallationFailedOnDevice(device.Name);
			}
		}

		private static void TryRunAdbProcess(string arguments)
		{
			ProcessRunner processRunner = null;
			try
			{
				processRunner = new ProcessRunner(GetAdbPath(), arguments);
				processRunner.Start();
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				Console.WriteLine("Output:" + processRunner.Output);
				Console.WriteLine("Error:" + processRunner.Errors);
				throw;
			}
		}

		public class InstallationFailedOnDevice : Exception
		{
			public InstallationFailedOnDevice(string deviceName) : base(deviceName) {}
		}

		public void UninstallPackage(AndroidDevice device, string packageName)
		{
			try
			{
				TryRunAdbProcess("-s " + device.AdbId + " shell pm uninstall " + packageName);
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new UninstallationFailedOnDevice(device.Name);
			}			
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(string deviceName) : base(deviceName) {}
		}

		public bool IsAppInstalled(AndroidDevice device, string packageName)
		{
			var processRunner =
				new ProcessRunner(GetAdbPath(), "-s " + device.AdbId + " shell pm list packages");
			processRunner.Start();
			return processRunner.Output.Contains(packageName);
		}

		public void StartApplication(AndroidDevice device, string appName)
		{
			try
			{
				TryRunAdbProcess("-s " + device.AdbId + " shell am start -a android.intent.action.MAIN" +
					" -n " + PackagePrefix + appName + "/.DeltaEngineActivity");
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new StartApplicationFailedOnDevice(device.Name);
			}
		}

		private const string PackagePrefix = "net.DeltaEngine.";

		public class StartApplicationFailedOnDevice : Exception
		{
			public StartApplicationFailedOnDevice(string deviceName) : base(deviceName) { }
		}

		public string GetDeviceName(string adbDeviceId)
		{
			try
			{
				// Reference:
				// http://stackoverflow.com/questions/6377444/can-i-use-adb-exe-to-find-a-description-of-a-phone
				string manufacturer = GetDeviceManufacturerName(adbDeviceId);
				string modelName = GetDeviceModelName(adbDeviceId);
				string fullDeviceName = manufacturer + " " + modelName;
				return fullDeviceName.Contains("not found")
					? "Device (AdbId=" + adbDeviceId + ")" : fullDeviceName;
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new DeterminationDeviceNameFailed(adbDeviceId);
			}
		}

		private static string GetDeviceManufacturerName(string adbDeviceId)
		{
			string manufacturerName = GetGrepInfo(adbDeviceId, "ro.product.manufacturer");
			return manufacturerName.IsFirstCharacterInLowerCase()
				? manufacturerName.ConvertFirstCharactertoUpperCase() : manufacturerName;
		}

		private static string GetDeviceModelName(string adbDeviceId)
		{
			string modelName = GetGrepInfo(adbDeviceId, "ro.product.model");
			return modelName;
		}

		private static string GetGrepInfo(string adbDeviceId, string grepParameter)
		{
			var processRunner = new ProcessRunner(GetAdbPath(),
				"-s " + adbDeviceId + " shell cat /system/build.prop | grep \"" + grepParameter + "\"");
			processRunner.Start();
			return processRunner.Output.Replace(grepParameter + "=", "");
		}

		public class DeterminationDeviceNameFailed : Exception
		{
			public DeterminationDeviceNameFailed(string adbDeviceId) : base(adbDeviceId) { }
		}
	}
}