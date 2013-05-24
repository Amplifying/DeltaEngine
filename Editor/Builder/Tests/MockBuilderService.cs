using System;
using System.Windows;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class MockBuilderService : Service
	{
		public MockBuilderService()
		{
			AllowedPlatforms = new[] { PlatformName.WindowsPhone7, PlatformName.Android, };
			Content = null;
		}

		public PlatformName[] AllowedPlatforms { get; private set; }
		public ContentService Content { get; private set; }
		public Window PluginHostWindow { get; set; }
		public event Action<object> MessageReceived;
		public void SendMessage(object message)
		{
		}

		public void ReceiveSomeTestMessages()
		{
			MessageReceived("A BuildInfo".AsBuildTestInfoMessage());
			MessageReceived("A BuildWarning".AsBuildTestWarning());
			MessageReceived("A BuildError".AsBuildTestError());
		}
	}
}