using System;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppInfoTests
	{
		[Test]
		public void LaunchingAnAppWithoutAvailableDeviceShouldThrowAnException()
		{
			var appInfo = new FakeAppInfoWithoutAvailableDevices();
			Assert.Throws<AppInfo.NoDeviceAvailable>(appInfo.LaunchAppOnPrimaryDevice);
		}

		private class FakeAppInfoWithoutAvailableDevices : AppInfo
		{
			public FakeAppInfoWithoutAvailableDevices()
				: base("FakeAppInfoWithoutAvailableDevices", Guid.Empty, (PlatformName)77, DateTime.Now) {}

			protected override Device[] GetAvailableDevices()
			{
				return new Device[0];
			}
		}
	}
}