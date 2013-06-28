using DeltaEngine.Datatypes;
using DeltaEngine.Input.Remote.Packets;

namespace DeltaEngine.Input.Remote
{
	/// <summary>
	/// Remote Mouse implementation receiving input information from a networking client.
	/// </summary>
	public class RemoteMouse : Mouse
	{
		public override void SetPosition(Point position) {}

		public override void Run() {}

		internal void HandleNewPacket(MousePacket packet)
		{
			isAvailable = packet.IsAvailable;
			Position = new Point(packet.X, packet.Y);
			ScrollWheelValue = packet.ScrollWheelValue;
			LeftButton = packet.LeftButtonState;
			RightButton = packet.RightButtonState;
			MiddleButton = packet.MiddleButtonState;
			X1Button = packet.X1ButtonState;
			X2Button = packet.X2ButtonState;
		}

		public override bool IsAvailable
		{
			get { return isAvailable; }
		}

		private bool isAvailable;

		public override void Dispose()
		{
		}
	}
}