using System;
using DeltaEngine.Networking;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockClient : Client
	{
		public void Dispose()
		{
			if (Disconnected != null)
				Disconnected();
		}

		public void Connect(string targetAddress, int targetPort)
		{
			IsConnected = true;
			TargetAddress = targetAddress;
			if (Connected != null)
				Connected();
		}

		public bool IsConnected { get; private set; }
		public string TargetAddress { get; private set; }

		public void Send(object message)
		{
			if (DataReceived != null)
				DataReceived(message);
		}

		public event Action<object> DataReceived;
		public event Action Connected;
		public event Action Disconnected;
	}
}
