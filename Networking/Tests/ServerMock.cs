namespace DeltaEngine.Networking.Tests
{
	public sealed class ServerMock : Server
	{
		public ServerMock()
		{
			IsRunning = true;
		}

		public void ClientConnectedToServer(ClientMock client)
		{
			connectedClients.Add(client);
		}

		public void ClientDisconnectedFromServer(ClientMock client)
		{
			connectedClients.Remove(client);
		}

		public byte[] ReceivedMessage { get; set; }
	}
}