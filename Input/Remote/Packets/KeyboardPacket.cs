using System;
using System.Collections.Generic;
using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Remote.Packets
{
	/// <summary>
	/// The KeyboardPacket is the implementation of an InputPacket containing Keyboard information.
	/// </summary>
	[Serializable]
	public class KeyboardPacket : InputPacket
	{
		public KeyboardPacket()
			: base(InputDeviceType.Keyboard)
		{
			Data = new byte[KeyNames.Length];
		}

		private static readonly Type KeyType = typeof(Key);
		private static readonly string[] KeyNames = Enum.GetNames(KeyType);

		public void UpdateStateBytes(Keyboard keyboard)
		{
			for (int index = 0; index < KeyNames.Length; index++)
				Data[index] = (byte)keyboard.GetKeyState((Key)Enum.Parse(KeyType, KeyNames[index]));
		}

		public void UpdateStatesFromDataBytes(Dictionary<Key, State> stateArray)
		{
			for (int index = 0; index < KeyNames.Length; index++)
			{
				Key key = (Key)Enum.Parse(KeyType, KeyNames[index]);
				if (stateArray.ContainsKey(key) == false)
					stateArray.Add(key, State.NotPressed);

				stateArray[key] = (State)Data[index];
			}
		}

		public byte[] Data;
	}
}