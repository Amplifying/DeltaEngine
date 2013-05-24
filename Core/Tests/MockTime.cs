namespace DeltaEngine.Core.Tests
{
	internal class MockTime : Time
	{
		protected override long GetTicks()
		{
			return ticks++;
		}

		private int ticks;

		protected override long TicksPerSecond
		{
			get { return 10; }
		}
	}
}