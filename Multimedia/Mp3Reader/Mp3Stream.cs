using System;
using System.IO;

namespace Mp3Reader
{
	public class Mp3Stream
	{
		public Mp3Stream(Stream setInput)
		{
			input = setInput;
			CalculateLengthInSeconds();

			frame = new Mp3Frame();
			decoder = new Mp3Decoder();
			Rewind();
			frame.SeekMp3Frame();
		}

		private readonly Stream input;
		private readonly Mp3Frame frame;
		private readonly Mp3Decoder decoder;

		private void CalculateLengthInSeconds()
		{
			var lengthReader = new Mp3LengthReader();
			lengthReader.ReadMp3Information(input);
			LengthInSeconds = (float)Math.Round(lengthReader.LengthInSeconds, 2);
			input.Position = 0;
		}

		public int Read(byte[] buffer, int length)
		{
			int readLength = 0;
			while (readLength < length)
			{
				int bytesRead = TryReadNextData(buffer, readLength, length);
				if (bytesRead <= 0)
					break;

				readLength += bytesRead;
			}

			return readLength;
		}

		private int TryReadNextData(byte[] buffer, int offset, int length)
		{
			if (IsBufferPositionInitial())
				RequestNewData();
			else if (IsMoreDataNeeded())
			{
				if (!frame.SeekMp3Frame())
					return 0;

				RequestNewData();
			}

			int maxLengthToRead = Math.Min(length - offset, decoder.Pcm.Length - bufferPosition);
			Array.Copy(decoder.Pcm, bufferPosition, buffer, offset, maxLengthToRead);
			bufferPosition += maxLengthToRead;
			return maxLengthToRead;
		}

		private bool IsBufferPositionInitial()
		{
			return bufferPosition == InitialPosition;
		}

		private void RequestNewData()
		{
			decoder.DecodeFrame(frame);
			bufferPosition = 0;
		}

		private bool IsMoreDataNeeded()
		{
			return bufferPosition >= decoder.Pcm.Length;
		}

		private int bufferPosition = InitialPosition;
		private const int InitialPosition = -1;

		public void Rewind()
		{
			bufferPosition = InitialPosition;
			input.Position = 0;
			frame.Rewind(input);
		}

		public int Channels
		{
			get { return frame.Channels; }
		}

		public int Samplerate
		{
			get { return frame.SampleRate; }
		}

		public float LengthInSeconds { get; private set; }
	}
}