namespace DeltaEngine.Logging
{
	public class ConsoleLogger : Logger
	{
		public ConsoleLogger()
			: base(new ConsoleLogProvider()) {}
	}
}