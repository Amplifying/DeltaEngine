using DeltaEngine.Networking;

namespace DeltaEngine.Logging.Basic
{
	public class DefaultLogger : Logger
	{
		public DefaultLogger(Client client)
			: base(new ConsoleLogProvider(), new TextFileLogProvider(),
				new NetworkClientLogProvider(client, "deltaengine.net", 777)) {}
	}
}