using System;

namespace DeltaEngine.Editor.Common
{
	public interface Service
	{
		PlatformName[] AllowedPlatforms { get; }
		ContentService Content { get; }
		event Action<object> MessageReceived;
		void SendMessage(object message);
	}
}