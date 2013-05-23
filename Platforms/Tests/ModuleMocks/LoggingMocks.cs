using System.Collections.Generic;
using DeltaEngine.Logging;
using DeltaEngine.Networking;
using Moq;

namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	public class LoggingMocks : BaseMocks
	{
		internal LoggingMocks(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			SetupLogProvider();
			SetupLogger();
			resolver.RegisterMock<Client>();
		}

		private void SetupLogProvider()
		{
			provider = resolver.RegisterMock<LogProvider>();
			provider.Setup(s => s.Log(It.IsAny<Info>())).Callback((Info info) => lines.Add(info.Text));
			provider.Setup(s => s.Log(It.IsAny<Warning>())).Callback(
				(Warning warning) => lines.Add(warning.Text));
			provider.Setup(s => s.Log(It.IsAny<Error>())).Callback(
				(Error error) => lines.Add(error.Text));
		}

		private static Mock<LogProvider> provider;
		public readonly List<string> lines = new List<string>();

		/// <summary>
		/// Needed because Autofac is unable to resolve the constructor parameter for a Moq Logger class
		/// </summary>
		private class MockLogger : Logger
		{
			public MockLogger()
				: base(provider.Object) {}
		}

		private void SetupLogger()
		{
			resolver.RegisterMock(new MockLogger());
		}
	}
}