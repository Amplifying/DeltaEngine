using System.Threading;
using DeltaEngine.Networking;
using DeltaEngine.Networking.Sockets;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuildServiceConnectionViaLAN : BuildServiceConnection
	{
		public BuildServiceConnectionViaLAN()
			: base(new TcpSocket())
		{
			Client.Connect(ServerAddress, ServerListeningPort);
			Login("abc", loginResult => IsLoggedIn = loginResult);
			WaitForServerResponse();
		}

		//private const string ServerAddress = "WIN7-VM";
		private const string ServerAddress = "JudgeWork-PC";
		private new const int ServerListeningPort = 800;

		public bool IsLoggedIn { get; set; }

		private static void WaitForServerResponse()
		{
			Thread.Sleep(100);
		}
	}
}