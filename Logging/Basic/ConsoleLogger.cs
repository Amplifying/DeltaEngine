namespace DeltaEngine.Logging.Basic
{
	public class ConsoleLogger : Logger
	{
		public ConsoleLogger()
			: base(new ConsoleLogProvider()) {}
	}
}