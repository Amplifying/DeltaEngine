using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Editor.Builder;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Runs the ADB tool (which is provided by the Android SDK) via command line.
	/// <see cref="http://developer.android.com/tools/help/adb.html"/>
	/// </summary>
	public class AndroidDebugBridgeRunner
	{
		public AndroidDebugBridgeRunner()
		{
			adbProvider = new AdbPathProvider();
		}

		private readonly AdbPathProvider adbProvider;

		public string[] GetInfosOfAvailableDevices()
		{
			var androidDevicesNames = new List<string>();
			var processRunner = CreateAdbProcess("devices");
			processRunner.StandardOutputEvent += outputMessage =>
			{
				if (IsDeviceName(outputMessage))
					androidDevicesNames.Add(outputMessage);
			};
			TryRunAdbProcess(processRunner);

			return androidDevicesNames.ToArray();
		}

		private ProcessRunner CreateAdbProcess(string arguments)
		{
			return new ProcessRunner(adbProvider.GetAdbPath(), arguments);
		}

		private static bool IsDeviceName(string devicesRequestMessage)
		{
			return !(devicesRequestMessage.StartsWith("list", StringComparison.OrdinalIgnoreCase) ||
				String.IsNullOrWhiteSpace(devicesRequestMessage));
		}

		private static void TryRunAdbProcess(ProcessRunner adbProcess)
		{
			try
			{
				adbProcess.Start();
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				Console.WriteLine("Output:" + adbProcess.Output);
				Console.WriteLine("Error:" + adbProcess.Errors);
				throw;
			}
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

		private void TryRunAdbProcess(string arguments)
		{
			ProcessRunner adbProcess = CreateAdbProcess(arguments);
			TryRunAdbProcess(adbProcess);
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
			ProcessRunner adbProcess = CreateAdbProcess("-s " + device.AdbId + " shell pm list packages");
			TryRunAdbProcess(adbProcess);

			return adbProcess.Output.Contains(packageName);
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

		private string GetDeviceManufacturerName(string adbDeviceId)
		{
			string manufacturerName = GetGrepInfo(adbDeviceId, "ro.product.manufacturer");
			return manufacturerName.IsFirstCharacterInLowerCase()
				? manufacturerName.ConvertFirstCharactertoUpperCase() : manufacturerName;
		}

		private string GetDeviceModelName(string adbDeviceId)
		{
			string modelName = GetGrepInfo(adbDeviceId, "ro.product.model");
			return modelName;
		}

		private string GetGrepInfo(string adbDeviceId, string grepParameter)
		{
			ProcessRunner adbProcess = CreateAdbProcess("-s " + adbDeviceId +
				" shell cat /system/build.prop | grep \"" + grepParameter + "\"");
			TryRunAdbProcess(adbProcess);

			return adbProcess.Output.Replace(grepParameter + "=", "");
		}

		public class DeterminationDeviceNameFailed : Exception
		{
			public DeterminationDeviceNameFailed(string adbDeviceId) : base(adbDeviceId) { }
		}
	}
}