using System;
using System.IO.Abstractions;
using System.Windows;
using DeltaEngine.Editor.Common;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Networking;

namespace DeltaEngine.Editor.Builder
{
	public abstract class BuildServiceConnection : Service
	{
		protected BuildServiceConnection(Client client)
		{
			Client = client;
			Content = new ContentServiceFiles(new FileSystem());
			Client.DataReceived += OnDataReceived;
		}

		public Client Client { get; private set; }
		public ContentService Content { get; private set; }
		public Window PluginHostWindow { get; private set; }

		private void OnDataReceived(object message)
		{
			var user = message as BuildServiceUser;
			if (user != null)
			{
				AllowedPlatforms = user.AllowedPlaforms;
				isApiKeyValidCallback(user.IsLoggedIn);
			}
			else if (message is BuildMessage && BuildMessageRecieved != null)
				BuildMessageRecieved((BuildMessage)message);
			else if (message is BuildResult && BuildResultRecieved != null)
				BuildResultRecieved((BuildResult)message);
			else if (MessageReceived != null)
				MessageReceived(message);
			else
				throw new Exception("No one is listening to the service messages!");
		}

		public PlatformName[] AllowedPlatforms { get; protected set; }
		private Action<bool> isApiKeyValidCallback;
		public event Action<BuildMessage> BuildMessageRecieved;
		public event Action<BuildResult> BuildResultRecieved;
		public event Action<object> MessageReceived;
		public const int ServerListeningPort = 800;

		public void Login(string apiKey, Action<bool> isApiKeyValid)
		{
			isApiKeyValidCallback = isApiKeyValid;
			SendMessage(new ApiKeyLogin { ApiKey = apiKey });
		}

		public void SendMessage(object message)
		{
			Client.Send(message);
		}
	}
}