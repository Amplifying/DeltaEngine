namespace DeltaEngine.Logging.Basic
{
	public class BasicLogger : Logger
	{
		public BasicLogger()
			: base(new ConsoleLogProvider(), new NetworkClientLogProvider("deltaengine.net", 777)) {}
	}
}