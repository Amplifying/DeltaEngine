using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class NullDeviceTests
	{
		[TestFixtureSetUp]
		public void LoadNullDevice()
		{
			device = new NullDevice();
		}

		private NullDevice device;

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
		public void InstallHasNoEffect()
		{
			device.Install(null);
		}

		[Test]
		public void UninstallHasNoEffect()
		{
			device.Uninstall(null);
		}

		[Test]
		public void LaunchHasNoEffect()
		{
			device.Launch(null);
		}
	}
}
