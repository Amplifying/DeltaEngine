using System;
using DeltaEngine.Input.Remote.Packets;

namespace DeltaEngine.Input.Remote
{
	public class InputPacketsAnalyser
	{
		public void HandleNewMessage(InputPacket packet)
		{
			switch (packet.Type)
			{
			case InputDeviceType.Mouse:
				AnalyseMousePacket(packet as MousePacket);
				break;

			case InputDeviceType.Keyboard:
				AnalyseKeyboardPacket(packet as KeyboardPacket);
				break;

			case InputDeviceType.Touch:
				AnalyseTouchPacket(packet as TouchPacket);
				break;

			default:
				throw new UnknownInputPacket();
			}
		}

		private class UnknownInputPacket : Exception {}

		private void AnalyseMousePacket(MousePacket packet)
		{
			if (packet != null && Mouse != null)
				Mouse.HandleNewPacket(packet);
		}

		public RemoteMouse Mouse { private get; set; }

		private void AnalyseKeyboardPacket(KeyboardPacket packet)
		{
			if (packet != null && Keyboard != null)
				Keyboard.HandleNewPacket(packet);
		}

		public RemoteKeyboard Keyboard { private get; set; }

		private void AnalyseTouchPacket(TouchPacket packet)
		{
			if (packet != null && Touch != null)
				Touch.HandleNewPacket(packet);
		}

		public RemoteTouch Touch { private get; set; }
	}
}