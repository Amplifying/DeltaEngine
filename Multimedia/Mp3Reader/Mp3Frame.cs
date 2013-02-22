using System.IO;

// Based on the toyMp3 library released under the New BSD License.
// For more information visit: http://code.google.com/p/toymp3/

namespace Mp3Reader
{
	internal class Mp3Frame
	{
		private const int FrameSeekLimit = 40960;

		private readonly int[] bitrateTable = new[]
		{ 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 };

		private readonly int[] frequencyTable = new[] { 44100, 48000, 32000, 0 };

		public readonly InputBitStream Stream = new InputBitStream();

		private int protectionBit;
		private int paddingBit;
		private int emphasis;
		private int id;
		private int layer;
		private int bitrateIndex;

		public readonly int[,] Scfsi = new int[2,4];
		public readonly GranuleInfo[,] Granule = new GranuleInfo[2,2];
		public int FrequencyIndex { get; private set; }
		public int ModeExtention { get; private set; }
		public int Mode { get; private set; }
		public int Channels { get; private set; }
		public int MainDataSize { get; private set; }
		public int SampleRate { get; private set; }
		public int MainDataBegin { get; private set; }
		public byte[] MainData { get; private set; }

		public Mp3Frame()
		{
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 2; j++)
					Granule[i, j] = new GranuleInfo();
		}

		public void Rewind(Stream input)
		{
			Stream.Rewind(input);
		}

		public bool SeekMp3Frame()
		{
			for (;;)
			{
				if (SeekToFrameStart() == false)
					return false;

				if (ParseData())
					break;

				Stream.SeekBits(-24);
			}

			ParseCrc();

			if (DecodeSideTableInformation() == false)
				return false;

			if (id != 1 || layer != 1 || bitrateIndex == 0 || MainDataSize < 0)
				return false;

			MainData = Stream.ReadBytes(MainDataSize);
			return true;
		}

		private bool SeekToFrameStart()
		{
			for (int index = 0;; index++)
			{
				if (Stream.ReadBits(12) == 4095)
					break;

				if (index > FrameSeekLimit)
					return false;

				Stream.SeekBits(-4);
			}

			return true;
		}

		private bool ParseData()
		{
			id = Stream.ReadBits(1);
			layer = Stream.ReadBits(2);
			protectionBit = Stream.ReadBits(1);
			bitrateIndex = Stream.ReadBits(4);
			FrequencyIndex = Stream.ReadBits(2);
			paddingBit = Stream.ReadBits(1);
			// private bit
			Stream.ReadBits(1);
			Mode = Stream.ReadBits(2);
			ModeExtention = Stream.ReadBits(2);
			// copyright
			Stream.ReadBits(1);
			// original
			Stream.ReadBits(1);
			emphasis = Stream.ReadBits(2);

			SampleRate = frequencyTable[FrequencyIndex];
			Channels = Mode == 3 ? 1 : 2;

			int frameSize = (int)(144.0 * bitrateTable[bitrateIndex] * 1000 / SampleRate) + paddingBit;
			// FrameSize - (FrameHeader(4 bytes) | CRC(1 byte) if exists | SideTable size)
			MainDataSize = frameSize - 4 - (protectionBit == 0 ? 2 : 0) - (Channels == 1 ? 17 : 32);

			return id == 1 && layer == 1 && bitrateIndex >= 1 && bitrateIndex <= 14 &&
				FrequencyIndex >= 0 && FrequencyIndex <= 2 && emphasis != 2;
		}

		private void ParseCrc()
		{
			if (protectionBit == 0)
				Stream.ReadBits(16);
		}

		private bool DecodeSideTableInformation()
		{
			MainDataBegin = Stream.ReadBits(9);
			Stream.SeekBits(Channels == 1 ? 5 : 3);

			for (int channel = 0; channel < Channels; channel++)
				for (int index = 0; index < 4; index++)
					Scfsi[channel, index] = Stream.ReadBits(1);

			for (int granule = 0; granule < 2; granule++)
				for (int channel = 0; channel < Channels; channel++)
					if (DecodeGranuleChannel(granule, channel) == false)
						return false;

			return true;
		}

		private bool DecodeGranuleChannel(int granule, int channel)
		{
			Granule[channel, granule].Part23Length = Stream.ReadBits(12);
			Granule[channel, granule].BigValues = Stream.ReadBits(9);
			Granule[channel, granule].GlobalGain = Stream.ReadBits(8);
			Granule[channel, granule].ScalefacCompress = Stream.ReadBits(4);
			Granule[channel, granule].WindowSwitchingFlag = Stream.ReadBits(1) == 1;

			if (Granule[channel, granule].WindowSwitchingFlag)
			{
				Granule[channel, granule].BlockType = Stream.ReadBits(2);
				Granule[channel, granule].MixedBlockFlag = Stream.ReadBits(1) == 1;

				if (Granule[channel, granule].BlockType == 0)
					return false;

				for (int w = 0; w < 2; w++)
					Granule[channel, granule].TableSelect[w] = Stream.ReadBits(5);

				for (int w = 0; w < 3; w++)
					Granule[channel, granule].SubblockGain[w] = Stream.ReadBits(3);

				Granule[channel, granule].Region0Count = (Granule[channel, granule].BlockType == 2 &&
					Granule[channel, granule].MixedBlockFlag == false) ? 8 : 7;

				Granule[channel, granule].Region1Count = 36;
			}
			else
			{
				for (int w = 0; w < 3; w++)
					Granule[channel, granule].TableSelect[w] = Stream.ReadBits(5);

				Granule[channel, granule].Region0Count = Stream.ReadBits(4);
				Granule[channel, granule].Region1Count = Stream.ReadBits(3);
				Granule[channel, granule].MixedBlockFlag = false;
				Granule[channel, granule].BlockType = 0;
			}

			Granule[channel, granule].PreFlag = Stream.ReadBits(1) == 1;
			Granule[channel, granule].ScalefacScale = Stream.ReadBits(1);
			Granule[channel, granule].Count1TableSelect = Stream.ReadBits(1);

			return true;
		}
	}
}