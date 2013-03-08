using DeltaEngine.Input.Remote.Packets;

namespace DeltaEngine.Input.Remote
{
	public class InputPacketsAnalyser
	{
		public void SetActiveMouse(RemoteMouse mouse)
		{
			activeMouse = mouse;
		}

		private RemoteMouse activeMouse;

		public void SetActiveKeyboard(RemoteKeyboard keyboard)
		{
			activeKeyboard = keyboard;
		}

		private RemoteKeyboard activeKeyboard;

		public void SetActiveTouch(RemoteTouch touch)
		{
			activeTouch = touch;
		}

		private RemoteTouch activeTouch;

		public void HandleNewMessage(Message message)
		{
			if (message is InputPacket)
				AnalyseInputPacket(message as InputPacket);
		}

		private void AnalyseInputPacket(InputPacket packet)
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
			}
		}

		private void AnalyseMousePacket(MousePacket packet)
		{
			if (packet != null && activeMouse != null)
				activeMouse.HandleNewPacket(packet);
		}

		private void AnalyseKeyboardPacket(KeyboardPacket packet)
		{
			if (packet != null && activeKeyboard != null)
				activeKeyboard.HandleNewPacket(packet);
		}

		private void AnalyseTouchPacket(TouchPacket packet)
		{
			if (packet != null && activeTouch != null)
				activeTouch.HandleNewPacket(packet);
		}
	}
}