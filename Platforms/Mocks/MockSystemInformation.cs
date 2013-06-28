using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms.Mocks
{
	class MockSystemInformation : SystemInformation
	{
		public override float AvailableRam
		{
			get { return MockSystemInformationValue; }
		}

		public const int MockSystemInformationValue = 123456;

		public override int CoreCount
		{
			get { return MockSystemInformationValue; }
		}
		public override string CpuName
		{
			get { return "MockCpuName"; }
		}
		public override float CpuSpeed
		{
			get { return MockSystemInformationValue; }
		}
		public override float[] CpuUsage
		{
			get { return new float[] { MockSystemInformationValue }; }
		}
		public override string GpuName
		{
			get { return "MockGpuName"; }
		}
		public override bool IsConsole
		{
			get { return false; }
		}
		public override bool IsMobileDevice
		{
			get { return false; }
		}
		public override bool IsTablet
		{
			get { return false; }
		}
		public override string MachineName
		{
			get { return "MockMachineName"; }
		}
		public override float MaxRam
		{
			get { return MockSystemInformationValue; }
		}
		public override Size MaxResolution
		{
			get { return new Size(640, 360); }
		}
		public override NetworkState NetworkState
		{
			get { return NetworkState.Disconnected; }
		}
		public override string PlatformName
		{
			get { return "MockPlatformName"; }
		}
		public override string PlatformVersion
		{
			get { return "MockPlatformVersion"; }
		}
		public override bool SoundCardAvailable
		{
			get { return false; }
		}
		public override float UsedRam
		{
			get { return MockSystemInformationValue; }
		}
		public override string Username
		{
			get { return "MockUsername"; }
		}
		public override Version Version
		{
			get { return MockVersion; }
		}

		public static readonly Version MockVersion = new Version("1.2.3.4");
	}
}
