using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class AndroidDebugBridgeRunnerTests
	{
		[Test]
		public void CheckConnectedDevices()
		{
			IEnumerable<string> adbDeviceIds = GetAdbDeviceIds();
			foreach (string adbDeviceId in adbDeviceIds)
				Assert.IsNotEmpty(adbDeviceId);
		}

		private static IEnumerable<string> GetAdbDeviceIds()
		{
			string[] deviceInfos = GetInfosOfAvailableDevices();
			string[] deviceIds = new string[deviceInfos.Length];
			for (int i = 0; i < deviceInfos.Length; i++)
				deviceIds[i] = deviceInfos[i].SplitAndTrim('\t')[0];

			return deviceIds;
		}

		private static string[] GetInfosOfAvailableDevices()
		{
			var adbRunner = new AndroidDebugBridgeRunner();
			return adbRunner.GetInfosOfAvailableDevices();
		}

		[Test]
		public void GetDeviceName()
		{
			var adbRunner = new AndroidDebugBridgeRunner();
			IEnumerable<string> adbDeviceIds = GetAdbDeviceIds();
			foreach (string adbDeviceId in adbDeviceIds)
			{
				string deviceName = adbRunner.GetDeviceName(adbDeviceId);
				Console.WriteLine("Android device name: '" + deviceName + "' (AdbId=" + adbDeviceId + ")");
				Assert.IsNotEmpty(deviceName);
				Assert.AreNotEqual(deviceName, adbDeviceId);
			}
		}

		[Test]
		public void IsAppInstalled()
		{
			if (IsNoAndroidDeviceAvailable())
				return;

			var adbRunner = new AndroidDebugBridgeRunner();
			Assert.IsTrue(adbRunner.IsAppInstalled(GetFirstAndroidDevice(), "com.android.settings"));
		}

		private static bool IsNoAndroidDeviceAvailable()
		{
			return GetFirstAndroidDevice() == null;
		}

		private static AndroidDevice GetFirstAndroidDevice()
		{
			var deviceFinder = new AndroidDeviceFinder();
			return deviceFinder.GetAvailableDevices().FirstOrDefault();
		}

		[Test]
		public void CheckIfAppIsNotInstalled()
		{
			if (IsNoAndroidDeviceAvailable())
				return;

			var adbRunner = new AndroidDebugBridgeRunner();
			Assert.IsFalse(adbRunner.IsAppInstalled(GetFirstAndroidDevice(), "non.existing.package"));
		}

		[Test, Category("Slow")]
		public void InstallTestPackage()
		{
			if (IsNoAndroidDeviceAvailable())
				return;

			var adbRunner = new AndroidDebugBridgeRunner();
			var firstAndroidDevice = GetFirstAndroidDevice();
			Assert.IsFalse(adbRunner.IsAppInstalled(firstAndroidDevice, TestPackageName));
			adbRunner.InstallPackage(firstAndroidDevice, TestPackagePath);
			Assert.IsTrue(adbRunner.IsAppInstalled(firstAndroidDevice, TestPackageName));
		}

		private const string TestPackageName = "net.DeltaEngine.LogoApp";
		private const string TestPackagePath = @"\\DeltaEngineServer\Temp\AndroidNdkApps\LogoApp.apk";

		[Test, Category("Slow")]
		public void UninstallTestPackage()
		{
			var firstAndroidDevice = GetFirstAndroidDevice();
			var adbRunner = new AndroidDebugBridgeRunner();
			Assert.IsTrue(adbRunner.IsAppInstalled(firstAndroidDevice, TestPackageName));
			adbRunner.UninstallPackage(firstAndroidDevice, TestPackageName);
			Assert.IsFalse(adbRunner.IsAppInstalled(firstAndroidDevice, TestPackageName));
		}
	}
}