namespace DeltaEngine.Input.Remote.Packets
{
	/// <summary>
	/// The MousePacket is the implementation of an InputPacket containing Mouse information.
	/// </summary>
	public class MousePacket : InputPacket
	{
		public MousePacket()
			: base(InputDeviceType.Mouse) {}

		public float X;
		public float Y;
		public int ScrollWheelValue;
		public State LeftButtonState;
		public State RightButtonState;
		public State MiddleButtonState;
		public State X1ButtonState;
		public State X2ButtonState;
	}
}