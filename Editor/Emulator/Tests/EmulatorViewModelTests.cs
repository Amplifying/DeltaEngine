using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class EmulatorViewModelTests
	{
		[SetUp]
		public void CreateViewModel()
		{
			emulator = new EmulatorViewModel();
		}

		private EmulatorViewModel emulator;

		[Test]
		public void Nothing()
		{
		}
	}
}