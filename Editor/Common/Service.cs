using System;
using System.Windows;

namespace DeltaEngine.Editor.Common
{
	public interface Service
	{
		PlatformName[] AllowedPlatforms { get; }
		ContentService Content { get; }
		Window PluginHostWindow { get; }
		event Action<object> MessageReceived;
		void SendMessage(object message);
	}
}