using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class ConsoleLoggerTests
	{
		[Test, Category("Slow")]
		public void CreateLogger()
		{
			var logger = new ConsoleLogger();
			Assert.IsNotNull(logger);
		}

		[Test, Category("Slow")]
		public void ConsoleLogger()
		{
			var logger = new ConsoleLogger();
			logger.Info("Test");
		}
	}
}