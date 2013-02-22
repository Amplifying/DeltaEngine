using System;
using System.Collections.Generic;
using System.IO;

namespace Mp3Reader
{
	internal class BitStream
	{
		private int currentBitPosition;
		private readonly List<byte> data = new List<byte>();

		public int LengthInBytes
		{
			get { return data.Count; }
		}

		public int LengthInBits
		{
			get { return data.Count * 8; }
		}

		public int PositionInBits
		{
			get { return currentBitPosition; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "Position can't be less than 0.");

				currentBitPosition = value;
			}
		}

		public int ReadBits(int bits)
		{
			if (bits < 0 || bits > 32)
				throw new ArgumentOutOfRangeException("bits",
					"Only bit numbers between 0 and 32 are allowed!");

			if (bits == 0)
				return 0;

			int returnValue = 0;
			for (int bitIndex = 0; bitIndex < bits; bitIndex++)
			{
				if (currentBitPosition > LengthInBits)
					throw new EndOfStreamException();

				int positionAsByteIndex = currentBitPosition >> 3;
				int bitIndexInCurrentByte = 1 << (7 - (currentBitPosition & 7));
				currentBitPosition++;

				if ((data[positionAsByteIndex] & bitIndexInCurrentByte) != 0)
					returnValue |= 1 << (bits - 1 - bitIndex);
			}

			return returnValue;
		}

		public void AddBytes(Mp3Frame frame)
		{
			data.AddRange(frame.MainData);
			data.RemoveRange(0, data.Count - (frame.MainData.Length + frame.MainDataBegin));
		}
	}
}