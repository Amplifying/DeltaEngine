namespace DeltaEngine.Logging.Basic
{
	public class OfflineLogger : Logger
	{
		public OfflineLogger()
			: base(new ConsoleLogProvider(), new TextFileLogProvider()) {}
	}
}