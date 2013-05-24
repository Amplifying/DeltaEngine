using System;
using System.Windows;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Launcher.Tests
{
	internal class MockLauncherService : Service
	{
		public MockLauncherService()
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
			Console.WriteLine(message);
		}
	}
}