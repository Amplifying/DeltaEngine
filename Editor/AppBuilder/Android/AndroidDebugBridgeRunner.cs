using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.AppBuilder.Android
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

		public AndroidDeviceInfo[] GetInfosOfAvailableDevices()
		{
			var androidDevicesNames = new List<AndroidDeviceInfo>();
			var processRunner = CreateAdbProcess("devices");
			processRunner.StandardOutputEvent += outputMessage =>
			{
				if (IsDeviceName(outputMessage))
					androidDevicesNames.Add(CreateDeviceInfo(outputMessage));
			};
			TryRunAdbProcess(processRunner);
			return androidDevicesNames.ToArray();
		}

		private ProcessRunner CreateAdbProcess(string arguments)
		{
			return new ProcessRunner(adbProvider.GetAdbPath(), arguments, 15 * 1000);
		}

		private static bool IsDeviceName(string devicesRequestMessage)
		{
			return !(devicesRequestMessage.StartsWith("list", StringComparison.OrdinalIgnoreCase) ||
				String.IsNullOrWhiteSpace(devicesRequestMessage));
		}

		private static AndroidDeviceInfo CreateDeviceInfo(string deviceInfoString)
		{
			string[] infoParts = deviceInfoString.Split('\t');
			return new AndroidDeviceInfo
			{
				AdbDeviceId = infoParts[0],
				DeviceState = infoParts.Length > 1 ? infoParts[1] : ""
			};
		}

		private static void TryRunAdbProcess(ProcessRunner adbProcess)
		{
			try
			{
				adbProcess.Start();
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				Logger.Warning("Output:" + adbProcess.Output);
				Logger.Warning("Error:" + adbProcess.Errors);
				throw;
			}
		}

		public void InstallPackage(AndroidDevice device, string apkFilePath)
		{
			try
			{
				TryRunAdbProcess("-s " + device.AdbId + " install -r " +
					PathExtensions.GetAbsolutePath(apkFilePath));
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

		public void UninstallPackage(AndroidDevice device, string fullAppName)
		{
			try
			{
				TryRunAdbProcess("-s " + device.AdbId + " shell pm uninstall " + fullAppName);
			}
			// ncrunch: no coverage start
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new UninstallationFailedOnDevice(device.Name);
			}			
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(string deviceName) : base(deviceName) {}
		}
		// ncrunch: no coverage end

		public bool IsAppInstalled(AndroidDevice device, string fullAppName)
		{
			ProcessRunner adbProcess = CreateAdbProcess("-s " + device.AdbId + " shell pm list packages");
			TryRunAdbProcess(adbProcess);
			return adbProcess.Output.Contains(fullAppName);
		}

		public void StartEngineBuiltApplication(AndroidDevice device, string fullAppName)
		{
			TryRunAdbProcess("-s " + device.AdbId + " shell am start -a android.intent.action.MAIN" +
				" -n " + fullAppName + "/.DeltaEngineActivity");
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
				? manufacturerName.ConvertFirstCharacterToUpperCase() : manufacturerName;
		}

		private string GetDeviceModelName(string adbDeviceId)
		{
			return GetGrepInfo(adbDeviceId, "ro.product.model");
		}

		private string GetGrepInfo(string adbDeviceId, string grepParameter)
		{
			ProcessRunner adbProcess = CreateAdbProcess("-s " + adbDeviceId +
				" shell cat /system/build.prop | grep \"" + grepParameter + "\"");
			TryRunAdbProcess(adbProcess);

			return adbProcess.Output.Replace(grepParameter + "=", "").Trim();
		}

		public class DeterminationDeviceNameFailed : Exception
		{
			public DeterminationDeviceNameFailed(string adbDeviceId) : base(adbDeviceId) { }
		}
	}
}