using System;
using System.Collections.Generic;

namespace DeltaEngine.Networking.Sockets
{
	internal class DataCollector
	{
		public DataCollector()
		{
			availableData = new Queue<byte>();
			currentContainerToFill = null;
		}

		private readonly Queue<byte> availableData;
		private MessageData currentContainerToFill;

		public void ReadBytes(byte[] data, int startOffset, int numberOfBytesToRead)
		{
			for (int index = startOffset; index < numberOfBytesToRead; index++)
				availableData.Enqueue(data[index]);
			ReadBytes();
		}

		public void ReadBytes()
		{
			if (currentContainerToFill == null)
			{
				if (availableData.Count < NumberOfReservedBytesForMessageLength)
					return;
				currentContainerToFill = new MessageData(ReadLength());
			}

			currentContainerToFill.ReadData(availableData);
			if (currentContainerToFill.IsDataComplete)
				TriggerObjectFinishedAndResetCurrentContainer();

			if (availableData.Count > 0)
				ReadBytes();
		}
		
		private const int NumberOfReservedBytesForMessageLength = sizeof(int);

		private int ReadLength()
		{
			var lengthBuffer = new byte[NumberOfReservedBytesForMessageLength];
			for (int index = 0; index < NumberOfReservedBytesForMessageLength; index++)
				lengthBuffer[index] = availableData.Dequeue();

			return BitConverter.ToInt32(lengthBuffer, 0);
		}

		private void TriggerObjectFinishedAndResetCurrentContainer()
		{
			if (ObjectFinished != null)
				ObjectFinished(currentContainerToFill);
			currentContainerToFill = null;
		}

		public event Action<MessageData> ObjectFinished;

		public static byte[] GetTestBytesWithLengthHeader(int numberOfWishedTestBytes)
		{
			var generatedBytes = new List<byte>();
			generatedBytes.AddRange(BitConverter.GetBytes(numberOfWishedTestBytes));
			for (int num = 0; num < numberOfWishedTestBytes; num++)
				generatedBytes.Add((byte)(100 + num));

			return generatedBytes.ToArray();
		}
	}
}