using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Remote.Packets;

namespace DeltaEngine.Input.Remote
{
	/// <summary>
	/// Remote Touch implementation receiving input information from a networking client.
	/// </summary>
	public class RemoteTouch : Touch
	{
		public RemoteTouch()
		{
			states = new State[TouchPacket.MaxNumberOfTouches];
			positions = new Point[TouchPacket.MaxNumberOfTouches];
		}

		public bool IsAvailable { get; private set; }

		public Point GetPosition(int touchIndex)
		{
			return positions[touchIndex];
		}

		public State GetState(int touchIndex)
		{
			return states[touchIndex];
		}

		public void Run() {}

		internal void HandleNewPacket(TouchPacket packet)
		{
			IsAvailable = packet.IsAvailable;
			for (int index = 0; index < TouchPacket.MaxNumberOfTouches; index++)
			{
				states[index] = packet.States[index];
				positions[index] = new Point(packet.Positions[index * 2], packet.Positions[index * 2 + 1]);
			}
		}

		private readonly State[] states;
		private readonly Point[] positions;

		public void Dispose() {}
	}
}