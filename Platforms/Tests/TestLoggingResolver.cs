using DeltaEngine.Logging;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	public class TestLoggingResolver : TestModuleResolver
	{
		public TestLoggingResolver(TestResolver testResolver)
			: base(testResolver)
		{
			SetupLogProvider();
			SetupLogger();
		}

		private void SetupLogProvider()
		{
			provider = testResolver.RegisterMock<LogProvider>();
			provider.Setup(s => s.Log(It.IsAny<Info>()));
			provider.Setup(s => s.Log(It.IsAny<Warning>()));
			provider.Setup(s => s.Log(It.IsAny<Error>()));
		}

		private static Mock<LogProvider> provider;

		private class MockLogger : Logger
		{
			public MockLogger()
				: base(provider.Object) {}
		}

		private void SetupLogger()
		{
			testResolver.RegisterMock(new MockLogger());
		}
	}
}