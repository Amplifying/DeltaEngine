using System.IO;

namespace Mp3Reader
{
	/// <summary>
	/// Based on:
	/// 
	///  original C++ code by:
	/// Gustav "Grim Reaper" Munkby
	/// http://floach.pimpin.net/grd/
	/// grimreaperdesigns@gmx.net
	/// 
	/// modified and converted to C# by:
	/// Robert A. Wlodarczyk
	/// http://rob.wincereview.com:8080
	/// rwlodarc@hotmail.com
	/// </summary>
	internal class Mp3LengthReader
	{
		public bool ReadMp3Information(Stream stream)
		{
			filesize = stream.Length;

			byte[] header = new byte[4];
			byte[] vbrHeader = new byte[12];
			int currentPosition = 0;

			// Keep reading 4 bytes from the header until we know for sure that in 
			// fact it's an MP3
			do
			{
				stream.Position = currentPosition;
				stream.Read(header, 0, 4);
				currentPosition++;
				LoadMp3Header(header);
			} while (!IsValidHeader() && !IsEndOfStream(stream));

			if (IsEndOfStream(stream))
				return false;

			currentPosition += 3;
			currentPosition += GetVersionIndex() == 3
				? (GetModeIndex() == 3 ? 17 : 32) : (GetModeIndex() == 3 ? 9 : 17);

			// Check to see if the MP3 has a variable bitrate
			stream.Position = currentPosition;
			stream.Read(vbrHeader, 0, 12);
			isVariableBitrate = LoadVbrHeader(vbrHeader);
			LengthInSeconds = GetLengthInSeconds() + 0.002f;
			return true;
		}

		private long filesize;
		private bool isVariableBitrate;
		public float LengthInSeconds { get; private set; }

		private bool IsEndOfStream(Stream stream)
		{
			return stream.Position >= stream.Length;
		}

		private void LoadMp3Header(byte[] header)
		{
			bitHeader = CombineBytes(header, 0);
		}

		private int bitHeader;

		private int CombineBytes(byte[] data, int index)
		{
			return (data[index] << 24) | (data[index + 1] << 16) | (data[index + 2] << 8) |
				(data[index + 3]);
		}

		private bool LoadVbrHeader(byte[] vbrHeader)
		{
			// If it's a variable bitrate MP3, the first 4 bytes will read 'Xing'
			// since they're the ones who added variable bitrate-edness to MP3s
			if (vbrHeader[0] != 88 || vbrHeader[1] != 105 || vbrHeader[2] != 110 || vbrHeader[3] != 103)
				return false;

			int flags = CombineBytes(vbrHeader, 4);
			numberOfVariableFrames = ((flags & 0x0001) == 1) ? CombineBytes(vbrHeader, 8) : -1;
			return true;
		}

		private int numberOfVariableFrames;

		private bool IsValidHeader()
		{
			return IsFrameSyncValid() && (GetVersionIndex() != 1) && (GetLayerIndex() != 0) &&
				(GetBitrateIndex() != 0) && (GetBitrateIndex() != 15) && (GetFrequencyIndex() != 3) &&
				(GetEmphasisIndex() != 2);
		}

		private bool IsFrameSyncValid()
		{
			return ((bitHeader >> 21) & 2047) == 2047;
		}

		private int GetVersionIndex()
		{
			return (bitHeader >> 19) & 3;
		}

		private int GetLayerIndex()
		{
			return (bitHeader >> 17) & 3;
		}

		private int GetBitrateIndex()
		{
			return (bitHeader >> 12) & 15;
		}

		private int GetFrequencyIndex()
		{
			return (bitHeader >> 10) & 3;
		}

		private int GetModeIndex()
		{
			return (bitHeader >> 6) & 3;
		}

		private int GetEmphasisIndex()
		{
			return bitHeader & 3;
		}

		private int GetBitrate()
		{
			return isVariableBitrate ? GetVariableBitrate() : GetConstantBitrate();
		}

		private int GetVariableBitrate()
		{
			double medFrameSize = filesize / (double)GetNumberOfFrames();
			double layerFactor = (GetLayerIndex() == 3) ? 12000.0 : 144000.0;
			return (int)((medFrameSize * GetFrequency()) / layerFactor);
		}

		private int GetConstantBitrate()
		{
			int[,,] table =
				{
					{
						// MPEG 2 & 2.5
						{ 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0 }, // Layer III
						{ 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0 }, // Layer II
						{ 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 0 } // Layer I
					},
					{
						// MPEG 1
						{ 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0 }, // Layer III
						{ 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 0 }, // Layer II
						{ 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, 0 } // Layer I
					}
				};

			return table[GetVersionIndex() & 1, GetLayerIndex() - 1, GetBitrateIndex()];
		}

		private int GetFrequency()
		{
			int[,] table =
				{
					{ 32000, 16000, 8000 }, // MPEG 2.5
					{ 0, 0, 0 }, // reserved
					{ 22050, 24000, 16000 }, // MPEG 2
					{ 44100, 48000, 32000 } // MPEG 1
				};

			return table[GetVersionIndex(), GetFrequencyIndex()];
		}

		private float GetLengthInSeconds()
		{
			return (8 * filesize) / 1000f / GetBitrate();
		}

		private int GetNumberOfFrames()
		{
			if (isVariableBitrate)
				return numberOfVariableFrames;

			double medFrameSize = ((GetLayerIndex() == 3) ? 12 : 144) *
				((1000.0 * GetBitrate()) / GetFrequency());
			return (int)(filesize / medFrameSize);
		}
	}
}