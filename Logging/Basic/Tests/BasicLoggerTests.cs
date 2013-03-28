using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class BasicLoggerTests
	{
		[Test, Category("Slow")]
		public void CreateLogger()
		{
			var logger = new BasicLogger();
			Assert.IsNotNull(logger);
		}

		[Test, Category("Slow")]
		public void LogTestInfoMessage()
		{
			var logger = new BasicLogger();
			logger.Info("Test");
		}
	}
}