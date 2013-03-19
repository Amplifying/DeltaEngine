using DeltaEngine.Core;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Mocks Core objects for testing
	/// </summary>
	public class TestCoreResolver : TestModuleResolver
	{
		public TestCoreResolver(TestResolver testResolver) 
			: base(testResolver)
		{
			SetupTime();
			SetupContentAndRandom();
		}

		private void SetupTime()
		{
			var elapsedTimeMock = new Mock<ElapsedTime>();
			int ticks = 0;
			elapsedTimeMock.Setup(e => e.GetTicks()).Returns(() => ++ticks);
			elapsedTimeMock.SetupGet(e => e.TicksPerSecond).Returns(60);
			testResolver.RegisterMock(elapsedTimeMock.Object);
			testResolver.RegisterMock(new Mock<Time>(elapsedTimeMock.Object).Object);
		}

		private void SetupContentAndRandom()
		{
			testResolver.RegisterSingleton<Content>();
			testResolver.RegisterSingleton<PseudoRandom>();
		}
	}
}
