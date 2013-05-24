using System;

// Based on the toyMp3 library released under the New BSD License.
// For more information visit: http://code.google.com/p/toymp3/

namespace Mp3Reader
{
	internal partial class Mp3Decoder
	{
		private const int BlockShort = 0;
		private const int BlockLong = 1;
		private const int SampleCount = 576;

		private readonly BitStream stream;

		private readonly ScalefactorBandIndex[] scfBandIndex;
		private readonly Scalefactor[,] scf;
		private Mp3Frame frame;
		private readonly int[,] _is = new int[2, SampleCount];
		public double[,] Xr = new double[2, SampleCount];
		private readonly double[] fCi = new[] { -0.6, -0.535, -0.33, -0.185, -0.095, -0.041, -0.0142, -0.0037 };

		public HuffmanTable[] TableQ = new HuffmanTable[4];
		public HuffmanNodeQ[] TreeQ = new HuffmanNodeQ[32];
		public HuffmanTable[] Table = new HuffmanTable[32];
		public HuffmanNode[] Tree = new HuffmanNode[32];

		private readonly Sample[,] sampleMap = new Sample[2, SampleCount];
		private readonly int[] preemphasisTable = new[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 1, 1, 1, 1, 2, 2, 3, 3, 3,
			2, 0
		};

		private bool inversedMdctInitializedFlag;
		private bool subbandSynthesysInitFlag;
		private bool antialiasInitializedFlag;

		private readonly double[] gCs = new double[8];
		private readonly double[] gCa = new double[8];
		private readonly double[,] sineWindow = new double[4, 36];
		private readonly double[,] pfb = new double[2, SampleCount];
		private readonly double[,] pfbOut = new double[2, SampleCount];
		private double[,] imdctPrevRawout = new double[2, SampleCount];
		private double[,,] subbandBuffer = new double[2, 16, 64];
		private readonly int[] subbandBufIndex = new int[2];
		private readonly double[,] cos = new double[64, 32];

		public readonly byte[] Pcm = new byte[1152 * 2 * 2];

		public bool DecodeFrame(Mp3Frame setFrame)
		{
			frame = setFrame;

			stream.AddBytes(frame);

			// Checking if position of the buffer to read exists.
			if (frame.MainDataBegin > (stream.LengthInBytes - frame.MainDataSize))
				return false;

			// Set reading location 
			stream.PositionInBits = stream.LengthInBits - ((frame.MainData.Length + frame.MainDataBegin) * 8);

			for (int granule = 0; granule < 2; granule++)
			{
				// Insie of granule-set, there exists glanule*frame.Channels
				for (int channel = 0; channel < frame.Channels; channel++)
				{
					// End position of "Part2~3" (Scalefactor + Huffmancode)
					int p23EndPos = stream.PositionInBits + frame.Granule[channel, granule].Part23Length;

					DecodeScalefactor(frame.Granule[channel, granule], channel, granule);
					if (DecodeHuffmanCode(channel, granule, p23EndPos) == false)
						return false;
					
					stream.PositionInBits = p23EndPos;
				}

				for (int channel = 0; channel < frame.Channels; channel++)
				{
					CreateSampleMap(channel, granule);
					Dequantize(channel, granule);
				}

				JointStereoDecode();

				for (int channel = 0; channel < frame.Channels; channel++)
				{
					Antialias(channel, granule);
					InverseMdctSynthesys(channel, granule);
					SubbandSynthesys(channel);
				}

				CreatePcm(granule);
			}

			return true;
		}

		private void DecodeScalefactor(GranuleInfo granuleInfo, int channel, int granule)
		{
			int slen1 = granuleInfo.Slen[0];
			int slen2 = granuleInfo.Slen[1];

			// Mixed Block
			if (granuleInfo.IsMixedBlock)
			{
				for (int sfb1 = 0; sfb1 < 8; sfb1++)
					scf[channel, granule].LongBlock[sfb1] = stream.ReadBits(slen1);

				int sfb = 3;
				for (; sfb < 6; sfb++)
					for (int w = 0; w < 3; w++)
						scf[channel, granule].ShortBlock[w, sfb] = stream.ReadBits(slen1);

				for (; sfb < 12; sfb++)
					for (int w = 0; w < 3; w++)
						scf[channel, granule].ShortBlock[w, sfb] = stream.ReadBits(slen2);
			}
				// Short Block
			else if (granuleInfo.IsShortBlock)
			{
				int sfb = 0;
				for (; sfb < 6; sfb++)
					for (int w = 0; w < 3; w++)
						scf[channel, granule].ShortBlock[w, sfb] = stream.ReadBits(slen1);

				for (; sfb < 12; sfb++)
					for (int w = 0; w < 3; w++)
						scf[channel, granule].ShortBlock[w, sfb] = stream.ReadBits(slen2);
			}
				// Long Block
			else if (granuleInfo.IsLongBlock)
			{
				// In case granule0,
				// without exception, data should be taken from bitstream.
				if (granule == 0)
				{
					int sfb = 0;
					for (; sfb < 11; sfb++)
						scf[channel, granule].LongBlock[sfb] = stream.ReadBits(slen1);

					for (; sfb < 21; sfb++)
						scf[channel, granule].LongBlock[sfb] = stream.ReadBits(slen2);
				}
				else
				{
					// When processing granule1 and SCFSI is 1,
					// sdalefactor will shared with granule0.
					// Else, scalefactor should be got from bitstream.
					if (frame.Scfsi[channel, 0] == 1)
					{
						for (int sfb = 0; sfb < 6; sfb++)
							scf[channel, granule].LongBlock[sfb] = scf[channel, 0].LongBlock[sfb];
					}
					else
					{
						for (int sfb = 0; sfb < 6; sfb++)
							scf[channel, granule].LongBlock[sfb] = stream.ReadBits(slen1);
					}

					if (frame.Scfsi[channel, 1] == 1)
					{
						for (int sfb = 6; sfb < 11; sfb++)
							scf[channel, granule].LongBlock[sfb] = scf[channel, 0].LongBlock[sfb];
					}
					else
					{
						for (int sfb = 6; sfb < 11; sfb++)
							scf[channel, granule].LongBlock[sfb] = stream.ReadBits(slen1);
					}

					if (frame.Scfsi[channel, 2] == 1)
					{
						for (int sfb = 11; sfb < 16; sfb++)
							scf[channel, granule].LongBlock[sfb] = scf[channel, 0].LongBlock[sfb];
					}
					else
					{
						for (int sfb = 11; sfb < 16; sfb++)
							scf[channel, granule].LongBlock[sfb] = stream.ReadBits(slen2);
					}

					if (frame.Scfsi[channel, 3] == 1)
					{
						for (int sfb = 16; sfb < 21; sfb++)
							scf[channel, granule].LongBlock[sfb] = scf[channel, 0].LongBlock[sfb];
					}
					else
					{
						for (int sfb = 16; sfb < 21; sfb++)
							scf[channel, granule].LongBlock[sfb] = stream.ReadBits(slen2);
					}
				}
			}
		}

		private bool DecodeHuffmanCode(int channel, int granule, int endpos)
		{
			int idx = 0;
			// Reading and decoding BigValue.
			int region1Start;
			int region2Start;
			GranuleInfo fGranule = frame.Granule[channel, granule];

			if (fGranule.BlockType == 0)
			{
				region1Start = scfBandIndex[frame.FrequencyIndex].LongBlock[fGranule.Region0Count + 1];
				region2Start = scfBandIndex[frame.FrequencyIndex].LongBlock[fGranule.Region0Count +
					fGranule.Region1Count + 2];
			}
			else
			{
				region1Start = 36;
				region2Start = 576;
			}

			for (; idx <= 576 - 2 && idx < fGranule.BigValues * 2; idx += 2)
			{
				int tableIndex = fGranule.TableSelect[idx < region1Start ? 0 : (idx < region2Start ? 1 : 2)];
				LookupHuffman(channel, tableIndex, idx);
			}

			// Reading and decoding Count1
			for (; (idx <= 576 - 4) && (stream.PositionInBits < endpos); idx += 4)
				LookupHuffmanQ(channel, fGranule.Count1TableSelect, idx);

			for (; idx < 576; idx++)
				_is[channel, idx] = 0;

			return true;
		}

		private void LookupHuffman(int ch, int tbl, int idx)
		{
			var tmpX = 0;
			var tmpY = 0;
			var tmpL = 0;
			var node = Tree[tbl];

			if (tbl != 0)
			{
				// Lookup in Huffman table
				for (int i = 0; i < 18; i++)
				{
					if (i == 18)
						throw new Exception("Failed to decode huffman code.");

					int q = stream.ReadBits(1);
					if (node.Nodes[q].X != -1 && node.Nodes[q].Y != -1)
					{
						tmpX = node.Nodes[q].X;
						tmpY = node.Nodes[q].Y;
						tmpL = linbits[tbl];
						break;
					}

					node = node.Nodes[q];
				}
			}
			else
			{
				tmpX = 0;
				tmpY = 0;
				tmpL = 0;
			}

			if (tmpL > 0 && tmpX == 15)
				tmpX += stream.ReadBits(tmpL);

			if (tmpX > 0 && stream.ReadBits(1) != 0)
				tmpX = -tmpX;

			if (tmpL > 0 && tmpY == 15)
				tmpY += stream.ReadBits(tmpL);

			if (tmpY > 0 && stream.ReadBits(1) != 0)
				tmpY = -tmpY;

			_is[ch, idx] = tmpX;
			_is[ch, idx + 1] = tmpY;
		}

		private void LookupHuffmanQ(int ch, int tbl, int idx)
		{
			var tmpV = 0;
			var tmpW = 0;
			var tmpX = 0;
			var tmpY = 0;
			var node = TreeQ[tbl];

			for (int i = 0; i < 7; i++)
			{
				if (i == 7)
					throw new Exception("Failed to decode huffman code.");

				int q = stream.ReadBits(1);
				if (node.Nodes[q].X != -1)
				{
					tmpV = node.Nodes[q].V;
					tmpW = node.Nodes[q].W;
					tmpX = node.Nodes[q].X;
					tmpY = node.Nodes[q].Y;
					break;
				}

				node = node.Nodes[q];
			}

			if (tmpV > 0 && stream.ReadBits(1) != 0)
				tmpV = -tmpV;

			if (tmpW > 0 && stream.ReadBits(1) != 0)
				tmpW = -tmpW;

			if (tmpX > 0 && stream.ReadBits(1) != 0)
				tmpX = -tmpX;

			if (tmpY > 0 && stream.ReadBits(1) != 0)
				tmpY = -tmpY;

			_is[ch, idx] = tmpV;
			_is[ch, idx + 1] = tmpW;
			_is[ch, idx + 2] = tmpX;
			_is[ch, idx + 3] = tmpY;
		}

		private void CreateSampleMap(int ch, int gr)
		{
			int idx = 0;
			GranuleInfo granuleInfo = frame.Granule[ch, gr];
			// Short Block
			if (granuleInfo.IsShortBlock)
			{
				int[] scfbIdx = scfBandIndex[frame.FrequencyIndex].ShortBlock;

				for (int sbi = 0; sbi < 13; sbi++)
				{
					int idxS = idx;
					for (int sub = 0; sub < 3; sub++)
					{
						int criticalBandWidth = scfbIdx[sbi + 1] - scfbIdx[sbi];
						for (int i = 0; i < criticalBandWidth; i++)
						{
							sampleMap[ch, idx] = new Sample
							{
								BlockType = BlockShort,
								Subblock = sub,
								Scale = 0.5 * (granuleInfo.ScalefacScale + 1.0) * scf[ch, gr].ShortBlock[sub, sbi],
								OrderIndex = 3 * ((idx - idxS) % criticalBandWidth) +
									(idx - idxS) / criticalBandWidth + idxS
							};
							idx++;
						}
					}
				}
			}
				// Long Block
			else if (granuleInfo.IsLongBlock)
			{
				int[] scfbIdx = scfBandIndex[frame.FrequencyIndex].LongBlock;
				int scfb = 0;

				for (int i = 0; i < SampleCount; i++)
				{
					sampleMap[ch, idx] = new Sample {BlockType = BlockLong, Subblock = 0, OrderIndex = idx};

					if (idx >= scfbIdx[scfb + 1])
						scfb++;

					int scfLong = scf[ch, gr].LongBlock[scfb];
					if (granuleInfo.PreFlag)
						scfLong += preemphasisTable[scfb];

					sampleMap[ch, idx].Scale = 0.5 * (granuleInfo.ScalefacScale + 1.0) * scfLong;
					idx++;
				}
			}
				// MixedBlock
			else
			{
				// LongBlock sample
				int[] scfbIdx = scfBandIndex[frame.FrequencyIndex].LongBlock;
				int scfb = 0;
				for (int i = 0; i < 36; i++)
				{
					sampleMap[ch, idx] = new Sample {BlockType = BlockLong, Subblock = 0, OrderIndex = idx};

					if (idx >= scfbIdx[scfb + 1])
						scfb++;

					int scfLong = scf[ch, gr].LongBlock[scfb];
					if (granuleInfo.PreFlag)
						scfLong += preemphasisTable[scfb];

					sampleMap[ch, idx].Scale = 0.5 * (granuleInfo.ScalefacScale + 1.0) * scfLong;
					idx++;
				}

				// Short Block Samples
				scfbIdx = scfBandIndex[frame.FrequencyIndex].ShortBlock;

				for (scfb = 3; scfb < 13; scfb++)
				{
					int idxS = idx;
					for (int sub = 0; sub < 3; sub++)
					{
						int criticalBandWidth = scfbIdx[scfb + 1] - scfbIdx[scfb];
						for (int i = 0; i < criticalBandWidth; i++)
						{
							sampleMap[ch, idx].BlockType = BlockShort;
							sampleMap[ch, idx].Subblock = sub;
							int scfShort = scf[ch, gr].ShortBlock[sub, scfb];
							sampleMap[ch, idx].Scale = 0.5 * (granuleInfo.ScalefacScale + 1.0) * scfShort;
							sampleMap[ch, idx].OrderIndex = 3 * ((idx - idxS) % criticalBandWidth) +
								(idx - idxS) / criticalBandWidth + idxS;
							idx++;
						}
					}
				}
			}
		}

		private void Dequantize(int channel, int granule)
		{
			GranuleInfo granuleInfo = frame.Granule[channel, granule];
			double gain = 0.25 * (granuleInfo.GlobalGain - 210.0);
			for (int sample = 0; sample < SampleCount; sample++)
			{
				if (sampleMap[channel, sample].BlockType == BlockShort)
					gain -= -2.0 * granuleInfo.SubblockGain[sampleMap[channel, sample].Subblock];

				Xr[channel, sampleMap[channel, sample].OrderIndex] = _is[channel, sample] *
					Math.Pow(Math.Abs(_is[channel, sample]), 1.0 / 3.0) *
					Math.Pow(2.0, gain - sampleMap[channel, sample].Scale);
			}
		}

		private void JointStereoDecode()
		{
			bool isMsStereo = frame.Mode == 1 && frame.ModeExtention >= 2;
			bool isIStereo = frame.ModeExtention == 1 || frame.ModeExtention == 3;
			
			// I Stereo (is not implemented)
			if (isIStereo)
				throw new Exception("I-Stereo is not suported.");

			if (isMsStereo)
				ProcessMsStereo();
		}

		private void ProcessMsStereo()
		{
			for (int i = 0; i < SampleCount; i++)
			{
				double left = (Xr[0, i] + Xr[1, i]) / 1.41421356;
				double right = (Xr[0, i] - Xr[1, i]) / 1.41421356;
				Xr[0, i] = left;
				Xr[1, i] = right;
			}
		}

		private void Antialias(int channel, int granule)
		{
			if (antialiasInitializedFlag == false)
			{
				InitAntialias();
				antialiasInitializedFlag = true;
			}

			GranuleInfo granuleInfo = frame.Granule[channel, granule];
			if (granuleInfo.IsShortBlock)
				return;

			int nl = granuleInfo.IsLongBlock ? 31 : 1;

			for (int k = 0; k < nl; k++)
			{
				for (int index = 0; index < 8; index++)
				{
					int index1 = (k * 18) + 18 + index;
					int index2 = (k * 18) + 17 - index;

					double a0 = Xr[channel, index1];
					double b0 = Xr[channel, index2];
					Xr[channel, index1] = a0 * gCs[index] + b0 * gCa[index];
					Xr[channel, index2] = b0 * gCs[index] - a0 * gCa[index];
				}
			}
		}

		private void InitAntialias()
		{
			for (int index = 0; index < 8; index++)
			{
				double sq = Math.Sqrt(1.0 + fCi[index] * fCi[index]);
				gCs[index] = 1.0 / sq;
				gCa[index] = fCi[index] / sq;
			}
		}

		private void InverseMdctSynthesys(int channel, int granule)
		{
			if (inversedMdctInitializedFlag == false)
			{
				InitImdct();
				inversedMdctInitializedFlag = true;
			}

			GranuleInfo granuleInfo = frame.Granule[channel, granule];
			int longSubbandNum = 0;
			int windowTypeLong = 0;

			if (granuleInfo.IsLongBlock)
			{
				longSubbandNum = 32;
				windowTypeLong = granuleInfo.BlockType;
			}
			else if (granuleInfo.IsShortBlock)
			{
				longSubbandNum = 0;
				windowTypeLong = -1;
			}
			else if (granuleInfo.IsMixedBlock)
			{
				longSubbandNum = 2;
				windowTypeLong = 0;
			}

			int subband;
			int index = 0;

			// Long (and mixed) Block
			for (subband = 0; subband < longSubbandNum; subband++)
			{
				var rawout = new double[36];
				for (int i = 0; i < 36; i++)
				{
					double sum = 0.0;
					for (int j = 0; j < 18; j++)
						sum += Xr[channel, j + subband * 18] * Math.Cos(Math.PI / 72.0 * (2.0 * i + 19.0) * (2.0 * j + 1.0));

					rawout[i] = sum * sineWindow[windowTypeLong, i];
				}

				// Combine width last granule and saving;
				for (int ss = 0; ss < 18; ss++)
				{
					// First half of data + Second half of prev data.
					pfb[channel, index] = rawout[ss] + imdctPrevRawout[channel, index];
					// Keep second half of data for next granule.
					imdctPrevRawout[channel, index] = rawout[ss + 18];
					index++;
				}
			}

			// ShortBlock
			for (; subband < 32; subband++)
			{
				var rawout = new double[36];
				for (int subblock = 0; subblock < 3; subblock++)
				{
					var rawoutTmp = new double[12];

					for (int i = 0; i < 12; i++)
					{
						double sum = 0;
						for (int j = 0; j < 6; j++)
						{
							sum += Xr[channel, subband * 18 + j * 3 + subblock] *
								Math.Cos(Math.PI / 24.0 * (2.0 * i + 7.0) * (2.0 * j + 1.0));
						}
						rawoutTmp[i] = sum * sineWindow[2, i];
					}

					for (int i = 0; i < 12; i++)
						rawout[6 * subblock + i + 6] += rawoutTmp[i];
				}

				for (int ss = 0; ss < 18; ss++)
				{
					pfb[channel, index] = rawout[ss] + imdctPrevRawout[channel, index];
					imdctPrevRawout[channel, index] = rawout[ss + 18];

					index++;
				}
			}
		}

		private void InitImdct()
		{
			for (int i = 0; i < 36; i++)
			{
				for (int j = 0; j < 4; j++)
					sineWindow[j, i] = 0;
			}

			for (int i = 0; i < 36; i++)
				sineWindow[0, i] = Math.Sin(Math.PI / 36 * (i + 0.5));

			for (int i = 0; i < 18; i ++)
				sineWindow[1, i] = Math.Sin(Math.PI / 36 * (i + 0.5));

			for (int i = 18; i < 24; i++)
				sineWindow[1, i] = 1.0;

			for (int i = 24; i < 30; i++)
				sineWindow[1, i] = Math.Sin(Math.PI / 12 * (i + 0.5 - 18));

			for (int i = 30; i < 36; i++)
				sineWindow[1, i] = 0.0;

			// Longblock TYPE3
			for (int i = 0; i < 6; i++)
				sineWindow[3, i] = 0.0;

			for (int i = 6; i < 12; i++)
				sineWindow[3, i] = Math.Sin(Math.PI / 12 * (i + 0.5 - 6));

			for (int i = 12; i < 18; i++)
				sineWindow[3, i] = 1.0;

			for (int i = 18; i < 36; i++)
				sineWindow[3, i] = Math.Sin(Math.PI / 36 * (i + 0.5));

			// Short Block
			for (int i = 0; i < 12; i++)
				sineWindow[2, i] = Math.Sin(Math.PI / 12 * (i + 0.5));

			for (int i = 12; i < 36; i++)
				sineWindow[2, i] = 0.0;
		}

		private void SubbandSynthesys(int ch)
		{
			if (subbandSynthesysInitFlag == false)
			{
				InitSubbandSynthesys();
				subbandSynthesysInitFlag = true;
			}

			for (int ss = 0; ss < 18; ss++)
			{
				for (int i = 0; i < 64; i++)
				{
					double sum = 0.0;
					for (int j = 0; j < 32; j++)
					{
						double sig = (ss % 2 == 1) && (j % 2 == 1) ? -1.0 : 1.0;
						sum += sig * pfb[ch, ss + j * 18] * cos[i, j];
					}

					subbandBuffer[ch, subbandBufIndex[ch], i] = sum;
				}

				for (int i = 0; i < 32; i++)
				{
					double sum = 0.0;
					for (int j = 0; j < 16; j++)
					{
						int offset = (j % 2) == 0 ? 0 : 32;

						sum += polyphaseCoefficient[j / 2, offset + i] *
							subbandBuffer[ch, (subbandBufIndex[ch] + 16 - j) % 16, offset + i];
					}

					pfbOut[ch, ss * 32 + i] = sum;
				}

				subbandBufIndex[ch] = (subbandBufIndex[ch] + 1) % 16;
			}
		}

		private void InitSubbandSynthesys()
		{
			for (int i = 0; i < 64; i++)
				for (int j = 0; j < 32; j++)
					cos[i, j] = Math.Cos((2.0 * j + 1.0) * (i + 16.0) * Math.PI / 64);
		}

		private void CreatePcm(int granule)
		{
			for (int sample = 0; sample < SampleCount; sample++)
			{
				for (int channel = 0; channel < frame.Channels; channel++)
				{
					int newSample = (int)(pfbOut[channel, sample] * 32768);
					
					if (newSample > short.MaxValue)
						newSample = short.MaxValue;
					else if (newSample < short.MinValue)
						newSample = short.MinValue;

					// [<---granule_margine--->+<-sample_just_made->]
					int granuleMargin = 576 * granule * frame.Channels;
					int sampleOffset = (sample * frame.Channels) + channel;
					int offset = (granuleMargin + sampleOffset) * 2;
					byte[] newBytes = BitConverter.GetBytes((short)newSample);
					Pcm[offset] = newBytes[0];
					Pcm[offset + 1] = newBytes[1];
				}
			}
		}

		private void Reset()
		{
			imdctPrevRawout = new double[2, SampleCount];
			subbandBuffer = new double[2, 16, 64];
			subbandBufIndex[0] = 0;
			subbandBufIndex[1] = 0;
		}
	}
}
