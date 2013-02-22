using System;
using System.IO;

namespace Mp3Reader
{
	internal class InputBitStream
	{
		private int currentBitPosition;
		private Stream stream;
		private byte currentByte;

		public void Rewind(Stream setStream)
		{
			stream = setStream;
			currentBitPosition = 0;
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
				if (currentBitPosition > stream.Length * 8)
					throw new EndOfStreamException();

				if (currentBitPosition % 8 == 0)
					currentByte = (byte)stream.ReadByte();

				int bitIndexInCurrentByte = 1 << (7 - (currentBitPosition & 7));
				currentBitPosition++;

				if ((currentByte & bitIndexInCurrentByte) != 0)
					returnValue |= 1 << (bits - 1 - bitIndex);
			}

			return returnValue;
		}

		public byte[] ReadBytes(int bytes)
		{
			if (currentBitPosition % 8 != 0)
				throw new InvalidOperationException("The current bit index is not byte aligned!");

			var result = new byte[bytes];
			stream.Read(result, 0, result.Length);
			currentBitPosition += bytes * 8;
			return result;
		}

		public void SeekBits(int length)
		{
			currentBitPosition += length;
			if (currentBitPosition > stream.Length * 8)
				throw new EndOfStreamException();
		}
	}
}