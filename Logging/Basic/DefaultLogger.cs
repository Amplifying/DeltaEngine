using DeltaEngine.Networking;
using DeltaEngine.Platforms;

namespace DeltaEngine.Logging.Basic
{
	public class DefaultLogger : Logger
	{
		public DefaultLogger(Client client, Settings settings)
			: base(
				new ConsoleLogProvider(), new TextFileLogProvider(),
				new NetworkClientLogProvider(client, settings.LogServerIp, settings.LogServerPort)) {}
	}
}