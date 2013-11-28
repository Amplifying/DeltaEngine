using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class DeviceTests
	{
		[TestFixtureSetUp]
		public void InitializeDevice()
		{
			device = new MockDevice();
		}

		private MockDevice device;

		private class MockDevice : Device
		{
			public MockDevice()
			{
				Name = "No emulator or connected device available for this platform.";
				IsEmulator = false;
			}

			public override bool IsAppInstalled(AppInfo app)
			{
				return false;
			}

			protected override void InstallApp(AppInfo app) { }
			protected override void UninstallApp(AppInfo app) { }
			protected override void LaunchApp(AppInfo app) {}
		}

		[Test]
		public void DeviceNameIsNotEmpty()
		{
			Assert.IsNotEmpty(device.Name);
		}

		[Test]
		public void IsAppInstalledIsAlwaysFalse()
		{
			Assert.IsFalse(device.IsAppInstalled(null));
		}

		[Test]
		public void InstallOfAnInvalidAppShouldFail()
		{
			Assert.Throws<Device.NoAppSpecified>(() => device.Install(null));
		}

		[Test]
		public void UninstallOfAnInvalidAppShouldFail()
		{
			Assert.Throws<Device.NoAppSpecified>(() => device.Uninstall(null));
		}

		[Test]
		public void LaunchOfAnInvalidAppShouldFail()
		{
			Assert.Throws<Device.StartApplicationFailedOnDevice>(() => device.Launch(null));
		}
	}
}
