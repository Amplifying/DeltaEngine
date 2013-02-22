namespace DeltaEngine.Core.Tests
{
	internal class MockElapsedTime : ElapsedTime
	{
		public long GetTicks()
		{
			return ticks++;
		}

		private int ticks;

		public long TicksPerSecond
		{
			get { return 10; }
		}
	}
}