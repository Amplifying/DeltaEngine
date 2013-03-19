using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking
{
	/// <summary>
	/// Servers listen on a specific port and accept multiple clients.
	/// </summary>
	public abstract class Server : IDisposable
	{
		public virtual bool IsRunning { get; protected set; }

		public int ListenPort { get; protected set; }

		public int NumberOfConnectedClients
		{
			get { return connectedClients.Count; }
		}

		protected readonly List<ClientConnection> connectedClients = new List<ClientConnection>();

		protected void OnClientConnected(ClientConnection client)
		{
			client.Disconnected += OnClientDisconnected;
			client.DataReceived += OnClientDataReceived;

			lock (connectedClients)
				connectedClients.Add(client);

			if (ClientConnected != null)
				ClientConnected(client);
		}

		protected virtual void OnClientDisconnected(ClientConnection client)
		{
			lock (connectedClients)
				connectedClients.Remove(client);

			if (ClientDisconnected != null)
				ClientDisconnected(client);
		}

		public event Action<ClientConnection> ClientDisconnected;
		public event Action<ClientConnection> ClientConnected;

		protected virtual void OnClientDataReceived(ClientConnection client, BinaryData message)
		{
			if (ClientDataReceived != null)
				ClientDataReceived(client, message);
		}

		public event Action<ClientConnection, BinaryData> ClientDataReceived;

		public virtual void Dispose()
		{
			lock (connectedClients)
			{
				var closingConnections = new List<ClientConnection>(connectedClients);
				foreach (var connection in closingConnections)
					connection.Dispose();
			}
		}
	}
}