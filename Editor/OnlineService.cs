using System;
using System.IO.Abstractions;
using DeltaEngine.Editor.Common;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Networking;
using DeltaEngine.Platforms;
using Window = System.Windows.Window;

namespace DeltaEngine.Editor
{
	public class OnlineService : Service
	{
		public OnlineService(Client connection)
		{
			Connection = connection;
			AllowedPlatforms = new[] { PlatformName.Windows };
			var settings = new FileSettings(false);
			connection.Connect(settings.ContentServerIp, settings.ContentServerPort);
			Content = new ContentServiceFiles(new FileSystem());
			connection.DataReceived += OnDataReceived;
		}

		public Client Connection { get; protected set; }
		public ContentService Content { get; private set; }
		public Window PluginHostWindow { get; internal set; }

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