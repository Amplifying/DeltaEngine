using DeltaEngine.Core;

namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	/// <summary>
	/// Mocks Core objects for testing
	/// </summary>
	public class CoreMocks : BaseMocks
	{
		internal CoreMocks(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			SetupTime();
		}

		private void SetupTime()
		{
			Time.Current = resolver.RegisterMock(new MockTime());
		}

		[IgnoreForResolver]
		private class MockTime : Time
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
}