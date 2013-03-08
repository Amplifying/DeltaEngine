using DeltaEngine.Input.Tests.Devices;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Input.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new MouseTests().CountPressingAndReleasing(TestStarter.OpenGL);
		}
	}
}