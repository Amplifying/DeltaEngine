using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class ConsoleLoggerTests
	{
		[Test]
		public void CreateLogger()
		{
			var logger = new ConsoleLogger();
			Assert.IsNotNull(logger);
		}

		[Test]
		public void ConsoleLogger()
		{
			var logger = new ConsoleLogger();
			logger.Info("Test");
		}
	}
}