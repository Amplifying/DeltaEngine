using System;

namespace DeltaEngine.Input.Remote.Packets
{
	/// <summary>
	/// The TouchPacket is the implementation of an InputPacket containing Touch information.
	/// </summary>
	[Serializable]
	public class TouchPacket : InputPacket
	{
		public TouchPacket()
			: base(InputDeviceType.Touch)
		{
			States = new State[MaxNumberOfTouches];
			Positions = new float[MaxNumberOfTouches * 2];
		}

		public State[] States;
		public float[] Positions;
		public const int MaxNumberOfTouches = 10;
	}
}