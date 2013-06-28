using DeltaEngine.Core;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockTime : Time
	{
		protected override long GetTicks()
		{
			return ++ticks;
		}

		private int ticks;

		protected override long TicksPerSecond
		{
			get { return 60; }
		}
	}
}
