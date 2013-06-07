using System;
using System.IO.Abstractions;
using System.Windows;
using DeltaEngine.Editor.Common;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Networking;

namespace DeltaEngine.Editor
{
	public class OnlineService : Service
	{
		public OnlineService(Client connection)
		{
			Connection = connection;
			connection.Connect(ServerAddress, ServerListeningPort);
			Content = new ContentServiceFiles(new FileSystem());
			connection.DataReceived += OnDataReceived;
		}

		public Client Connection { get; private set; }
		private const string ServerAddress = "deltaengine.net";//"DeltaEngine.net"
		private const int ServerListeningPort = 800;
		public ContentService Content { get; private set; }
		public Window PluginHostWindow { get; private set; }

		//TODO: this is duplicate, same as BuildServiceConnection
		private void OnDataReceived(object message)
		{
			var user = message as BuildServiceUser;
			if (user != null)
			{
				AllowedPlatforms = user.AllowedPlaforms;
				isApiKeyValidCallback(user.IsLoggedIn);
			}
			else if (MessageReceived != null)
				MessageReceived(message);
			else
				throw new Exception("No one is listening to the service messages!");
		}

		public PlatformName[] AllowedPlatforms { get; private set; }
		private Action<bool> isApiKeyValidCallback;
		public event Action<object> MessageReceived;

		public void Login(string apiKey, Action<bool> isApiKeyValid)
		{
			isApiKeyValidCallback = isApiKeyValid;
			Connection.Send(new ApiKeyLogin { ApiKey = apiKey });
		}
		
		public void SendMessage(object message)
		{
			Connection.Send(message);
		}
	}
}