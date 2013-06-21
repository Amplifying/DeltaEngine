using System;

// Based on the toyMp3 library released under the New BSD License.
// For more information visit: http://code.google.com/p/toymp3/

namespace Mp3Reader
{
	internal struct Scalefactor
	{
		public int[] LongBlock;
		public int[,] ShortBlock;
	}

	internal struct ScalefactorBandIndex
	{
		public int[] LongBlock;
		public int[] ShortBlock;
	}

	internal class Sample : IComparable
	{
		public int BlockType { get; set; }
		public int Subblock { get; set; }
		public double Scale { get; set; }
		public int OrderIndex { get; set; }

		public int CompareTo(object obj)
		{
			return OrderIndex - ((Sample)obj).OrderIndex;
		}
	}

	partial class Mp3Decoder
	{
		private readonly int[] linbits;
		private readonly double[,] polyphaseCoefficient = new[,]
		{
			{
				0.000000000, -0.000015259, -0.000015259, -0.000015259,
				-0.000015259, -0.000015259, -0.000015259, -0.000030518,
				-0.000030518, -0.000030518, -0.000030518, -0.000045776,
				-0.000045776, -0.000061035, -0.000061035, -0.000076294,
				-0.000076294, -0.000091553, -0.000106812, -0.000106812,
				-0.000122070, -0.000137329, -0.000152588, -0.000167847,
				-0.000198364, -0.000213623, -0.000244141, -0.000259399,
				-0.000289917, -0.000320435, -0.000366211, -0.000396729,
				-0.000442505, -0.000473022, -0.000534058, -0.000579834,
				-0.000625610, -0.000686646, -0.000747681, -0.000808716,
				-0.000885010, -0.000961304, -0.001037598, -0.001113892,
				-0.001205444, -0.001296997, -0.001388550, -0.001480103,
				-0.001586914, -0.001693726, -0.001785278, -0.001907349,
				-0.002014160, -0.002120972, -0.002243042, -0.002349854,
				-0.002456665, -0.002578735, -0.002685547, -0.002792358,
				-0.002899170, -0.002990723, -0.003082275, -0.003173828
			},
			{
				0.003250122, 0.003326416, 0.003387451, 0.003433228,
				0.003463745, 0.003479004, 0.003479004, 0.003463745,
				0.003417969, 0.003372192, 0.003280640, 0.003173828,
				0.003051758, 0.002883911, 0.002700806, 0.002487183,
				0.002227783, 0.001937866, 0.001617432, 0.001266479,
				0.000869751, 0.000442505, -0.000030518, -0.000549316,
				-0.001098633, -0.001693726, -0.002334595, -0.003005981,
				-0.003723145, -0.004486084, -0.005294800, -0.006118774,
				-0.007003784, -0.007919312, -0.008865356, -0.009841919,
				-0.010848999, -0.011886597, -0.012939453, -0.014022827,
				-0.015121460, -0.016235352, -0.017349243, -0.018463135,
				-0.019577026, -0.020690918, -0.021789551, -0.022857666,
				-0.023910522, -0.024932861, -0.025909424, -0.026840210,
				-0.027725220, -0.028533936, -0.029281616, -0.029937744,
				-0.030532837, -0.031005859, -0.031387329, -0.031661987,
				-0.031814575, -0.031845093, -0.031738281, -0.031478882
			},
			{
				0.031082153, 0.030517578, 0.029785156, 0.028884888,
				0.027801514, 0.026535034, 0.025085449, 0.023422241,
				0.021575928, 0.019531250, 0.017257690, 0.014801025,
				0.012115479, 0.009231567, 0.006134033, 0.002822876,
				-0.000686646, -0.004394531, -0.008316040, -0.012420654,
				-0.016708374, -0.021179199, -0.025817871, -0.030609131,
				-0.035552979, -0.040634155, -0.045837402, -0.051132202,
				-0.056533813, -0.061996460, -0.067520142, -0.073059082,
				-0.078628540, -0.084182739, -0.089706421, -0.095169067,
				-0.100540161, -0.105819702, -0.110946655, -0.115921021,
				-0.120697021, -0.125259399, -0.129562378, -0.133590698,
				-0.137298584, -0.140670776, -0.143676758, -0.146255493,
				-0.148422241, -0.150115967, -0.151306152, -0.151962280,
				-0.152069092, -0.151596069, -0.150497437, -0.148773193,
				-0.146362305, -0.143264771, -0.139450073, -0.134887695,
				-0.129577637, -0.123474121, -0.116577148, -0.108856201
			},
			{
				0.100311279, 0.090927124, 0.080688477, 0.069595337,
				0.057617187, 0.044784546, 0.031082153, 0.016510010,
				0.001068115, -0.015228271, -0.032379150, -0.050354004,
				-0.069168091, -0.088775635, -0.109161377, -0.130310059,
				-0.152206421, -0.174789429, -0.198059082, -0.221984863,
				-0.246505737, -0.271591187, -0.297210693, -0.323318481,
				-0.349868774, -0.376800537, -0.404083252, -0.431655884,
				-0.459472656, -0.487472534, -0.515609741, -0.543823242,
				-0.572036743, -0.600219727, -0.628295898, -0.656219482,
				-0.683914185, -0.711318970, -0.738372803, -0.765029907,
				-0.791213989, -0.816864014, -0.841949463, -0.866363525,
				-0.890090942, -0.913055420, -0.935195923, -0.956481934,
				-0.976852417, -0.996246338, -1.014617920, -1.031936646,
				-1.048156738, -1.063217163, -1.077117920, -1.089782715,
				-1.101211548, -1.111373901, -1.120223999, -1.127746582,
				-1.133926392, -1.138763428, -1.142211914, -1.144287109
			},
			{
				1.144989014, 1.144287109, 1.142211914, 1.138763428,
				1.133926392, 1.127746582, 1.120223999, 1.111373901,
				1.101211548, 1.089782715, 1.077117920, 1.063217163,
				1.048156738, 1.031936646, 1.014617920, 0.996246338,
				0.976852417, 0.956481934, 0.935195923, 0.913055420,
				0.890090942, 0.866363525, 0.841949463, 0.816864014,
				0.791213989, 0.765029907, 0.738372803, 0.711318970,
				0.683914185, 0.656219482, 0.628295898, 0.600219727,
				0.572036743, 0.543823242, 0.515609741, 0.487472534,
				0.459472656, 0.431655884, 0.404083252, 0.376800537,
				0.349868774, 0.323318481, 0.297210693, 0.271591187,
				0.246505737, 0.221984863, 0.198059082, 0.174789429,
				0.152206421, 0.130310059, 0.109161377, 0.088775635,
				0.069168091, 0.050354004, 0.032379150, 0.015228271,
				-0.001068115, -0.016510010, -0.031082153, -0.044784546,
				-0.057617187, -0.069595337, -0.080688477, -0.090927124
			},
			{
				0.100311279, 0.108856201, 0.116577148, 0.123474121,
				0.129577637, 0.134887695, 0.139450073, 0.143264771,
				0.146362305, 0.148773193, 0.150497437, 0.151596069,
				0.152069092, 0.151962280, 0.151306152, 0.150115967,
				0.148422241, 0.146255493, 0.143676758, 0.140670776,
				0.137298584, 0.133590698, 0.129562378, 0.125259399,
				0.120697021, 0.115921021, 0.110946655, 0.105819702,
				0.100540161, 0.095169067, 0.089706421, 0.084182739,
				0.078628540, 0.073059082, 0.067520142, 0.061996460,
				0.056533813, 0.051132202, 0.045837402, 0.040634155,
				0.035552979, 0.030609131, 0.025817871, 0.021179199,
				0.016708374, 0.012420654, 0.008316040, 0.004394531,
				0.000686646, -0.002822876, -0.006134033, -0.009231567,
				-0.012115479, -0.014801025, -0.017257690, -0.019531250,
				-0.021575928, -0.023422241, -0.025085449, -0.026535034,
				-0.027801514, -0.028884888, -0.029785156, -0.030517578
			},
			{
				0.031082153, 0.031478882, 0.031738281, 0.031845093,
				0.031814575, 0.031661987, 0.031387329, 0.031005859,
				0.030532837, 0.029937744, 0.029281616, 0.028533936,
				0.027725220, 0.026840210, 0.025909424, 0.024932861,
				0.023910522, 0.022857666, 0.021789551, 0.020690918,
				0.019577026, 0.018463135, 0.017349243, 0.016235352,
				0.015121460, 0.014022827, 0.012939453, 0.011886597,
				0.010848999, 0.009841919, 0.008865356, 0.007919312,
				0.007003784, 0.006118774, 0.005294800, 0.004486084,
				0.003723145, 0.003005981, 0.002334595, 0.001693726,
				0.001098633, 0.000549316, 0.000030518, -0.000442505,
				-0.000869751, -0.001266479, -0.001617432, -0.001937866,
				-0.002227783, -0.002487183, -0.002700806, -0.002883911,
				-0.003051758, -0.003173828, -0.003280640, -0.003372192,
				-0.003417969, -0.003463745, -0.003479004, -0.003479004,
				-0.003463745, -0.003433228, -0.003387451, -0.003326416
			},
			{
				0.003250122, 0.003173828, 0.003082275, 0.002990723,
				0.002899170, 0.002792358, 0.002685547, 0.002578735,
				0.002456665, 0.002349854, 0.002243042, 0.002120972,
				0.002014160, 0.001907349, 0.001785278, 0.001693726,
				0.001586914, 0.001480103, 0.001388550, 0.001296997,
				0.001205444, 0.001113892, 0.001037598, 0.000961304,
				0.000885010, 0.000808716, 0.000747681, 0.000686646,
				0.000625610, 0.000579834, 0.000534058, 0.000473022,
				0.000442505, 0.000396729, 0.000366211, 0.000320435,
				0.000289917, 0.000259399, 0.000244141, 0.000213623,
				0.000198364, 0.000167847, 0.000152588, 0.000137329,
				0.000122070, 0.000106812, 0.000106812, 0.000091553,
				0.000076294, 0.000076294, 0.000061035, 0.000061035,
				0.000045776, 0.000045776, 0.000030518, 0.000030518,
				0.000030518, 0.000030518, 0.000015259, 0.000015259,
				0.000015259, 0.000015259, 0.000015259, 0.000015259
			}
		};

		public Mp3Decoder()
		{
			Reset();
			stream = new BitStream();
			scf = new Scalefactor[2, 2];
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					scf[i, j].LongBlock = new int[22];
					scf[i, j].ShortBlock = new int[3,14];
				}
			}

			scfBandIndex = new ScalefactorBandIndex[3];
			// fs = 32kHz
			scfBandIndex[0].LongBlock = new[]
			{
				0, 4, 8, 12, 16, 20, 24, 30, 36, 44, 52, 62, 74, 90, 110, 134, 162, 196, 238, 288, 342, 418, 576
			};
			scfBandIndex[0].ShortBlock = new[]
			{
				0, 4, 8, 12, 16, 20, 30, 40, 52, 66, 84, 106, 136, 192
			};
			// fs = 44.1kHz
			scfBandIndex[1].LongBlock = new[]
			{
				0, 4, 8, 12, 16, 20, 24, 30, 36, 42, 50, 60, 72, 88, 106, 128, 156, 190, 230, 276, 330, 384, 576
			};
			scfBandIndex[1].ShortBlock = new[]
			{
				0, 4, 8, 12, 16, 22, 28, 38, 50, 64, 80, 100, 126, 192
			};
			// fs = 48kHz
			scfBandIndex[2].LongBlock = new[]
			{
				0, 4, 8, 12, 16, 20, 24, 30, 36, 44, 54, 66, 82, 102, 126, 156, 194, 240, 296, 364, 448, 550, 576
			};
			scfBandIndex[2].ShortBlock = new[]
			{
				0, 4, 8, 12, 16, 22, 30, 42, 58, 78, 104, 138, 180, 192
			};

			for (int i = 0; i < 2; i++)
			{
				TableQ[i] = new HuffmanTable();
				TreeQ[i] = new HuffmanNodeQ();
			}

			for (int i = 0; i < 32; i++)
			{
				Table[i] = new HuffmanTable();
				Tree[i] = new HuffmanNode();
			}

			Tree[0].Data = new[] { 0, 0 };

			linbits = new[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				1, 2, 3, 4, 6, 8, 10, 13, 4, 5, 6, 7, 8, 9, 11, 13
			};

			int[][] numberBits =
				{
					new[] {0, 0, 0, 0},
					new[] {0, 0, 0, 1},
					new[] {0, 0, 1, 0},
					new[] {0, 0, 1, 1},
					new[] {0, 1, 0, 0},
					new[] {0, 1, 0, 1},
					new[] {0, 1, 1, 0},
					new[] {0, 1, 1, 1},
					new[] {1, 0, 0, 0},
					new[] {1, 0, 0, 1},
					new[] {1, 0, 1, 0},
					new[] {1, 0, 1, 1},
					new[] {1, 1, 0, 0},
					new[] {1, 1, 0, 1},
					new[] {1, 1, 1, 0},
					new[] {1, 1, 1, 1}
				};

			TableQ[0].Codes.Add(new HuffmanCode("1", numberBits[0]));
			TableQ[0].Codes.Add(new HuffmanCode("0101", numberBits[1]));
			TableQ[0].Codes.Add(new HuffmanCode("0100", numberBits[2]));
			TableQ[0].Codes.Add(new HuffmanCode("00101", numberBits[3]));
			TableQ[0].Codes.Add(new HuffmanCode("0110", numberBits[4]));
			TableQ[0].Codes.Add(new HuffmanCode("000101", numberBits[5]));
			TableQ[0].Codes.Add(new HuffmanCode("00100", numberBits[6]));
			TableQ[0].Codes.Add(new HuffmanCode("000100", numberBits[7]));
			TableQ[0].Codes.Add(new HuffmanCode("0111", numberBits[8]));
			TableQ[0].Codes.Add(new HuffmanCode("00011", numberBits[9]));
			TableQ[0].Codes.Add(new HuffmanCode("00110", numberBits[10]));
			TableQ[0].Codes.Add(new HuffmanCode("000000", numberBits[11]));
			TableQ[0].Codes.Add(new HuffmanCode("00111", numberBits[12]));
			TableQ[0].Codes.Add(new HuffmanCode("000010", numberBits[13]));
			TableQ[0].Codes.Add(new HuffmanCode("000011", numberBits[14]));
			TableQ[0].Codes.Add(new HuffmanCode("000001", numberBits[15]));

			TableQ[1].Codes.Add(new HuffmanCode("1111", numberBits[0]));
			TableQ[1].Codes.Add(new HuffmanCode("1110", numberBits[1]));
			TableQ[1].Codes.Add(new HuffmanCode("1101", numberBits[2]));
			TableQ[1].Codes.Add(new HuffmanCode("1100", numberBits[3]));
			TableQ[1].Codes.Add(new HuffmanCode("1011", numberBits[4]));
			TableQ[1].Codes.Add(new HuffmanCode("1010", numberBits[5]));
			TableQ[1].Codes.Add(new HuffmanCode("1001", numberBits[6]));
			TableQ[1].Codes.Add(new HuffmanCode("1000", numberBits[7]));
			TableQ[1].Codes.Add(new HuffmanCode("0111", numberBits[8]));
			TableQ[1].Codes.Add(new HuffmanCode("0110", numberBits[9]));
			TableQ[1].Codes.Add(new HuffmanCode("0101", numberBits[10]));
			TableQ[1].Codes.Add(new HuffmanCode("0100", numberBits[11]));
			TableQ[1].Codes.Add(new HuffmanCode("0011", numberBits[12]));
			TableQ[1].Codes.Add(new HuffmanCode("0010", numberBits[13]));
			TableQ[1].Codes.Add(new HuffmanCode("0001", numberBits[14]));
			TableQ[1].Codes.Add(new HuffmanCode("0000", numberBits[15]));

			ResetCounter(1);
			Table[1].Codes.Add(new HuffmanCode("1", GetNextCounter()));
			Table[1].Codes.Add(new HuffmanCode("001", GetNextCounter()));
			Table[1].Codes.Add(new HuffmanCode("01", GetNextCounter()));
			Table[1].Codes.Add(new HuffmanCode("000", GetNextCounter()));

			ResetCounter(2);
			Table[2].Codes.Add(new HuffmanCode("1", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("010", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("000001", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("011", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("001", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("00001", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("00011", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("00010", GetNextCounter()));
			Table[2].Codes.Add(new HuffmanCode("000000", GetNextCounter()));

			ResetCounter(2);
			Table[3].Codes.Add(new HuffmanCode("11", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("10", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("000001", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("001", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("01", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("00001", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("00011", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("00010", GetNextCounter()));
			Table[3].Codes.Add(new HuffmanCode("000000", GetNextCounter()));

			ResetCounter(3);
			Table[5].Codes.Add(new HuffmanCode("1", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("010", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("000110", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("0000101", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("011", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("001", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("000100", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("0000100", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("000111", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("000101", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("0000111", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("00000001", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("0000110", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("000001", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("0000001", GetNextCounter()));
			Table[5].Codes.Add(new HuffmanCode("00000000", GetNextCounter()));

			ResetCounter(3);
			Table[6].Codes.Add(new HuffmanCode("111", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("011", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("00101", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("0000001", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("110", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("10", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("0011", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("00010", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("0101", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("0100", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("00100", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("000001", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("000011", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("00011", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("000010", GetNextCounter()));
			Table[6].Codes.Add(new HuffmanCode("0000000", GetNextCounter()));

			ResetCounter(5);
			Table[7].Codes.Add(new HuffmanCode("1", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("001010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00010011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00010000", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000001010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000111", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0001010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000101", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00000011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("001011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00100", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0001101", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00010001", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00001000", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000000100", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0001100", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0001011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00010010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000001111", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000001011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000000010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000111", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000110", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00001001", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000001110", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000000011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000000001", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00000110", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("00000100", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("000000101", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000000011", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000000010", GetNextCounter()));
			Table[7].Codes.Add(new HuffmanCode("0000000000", GetNextCounter()));

			ResetCounter(5);
			Table[8].Codes.Add(new HuffmanCode("11", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("100", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000110", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00010010", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00001100", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000000101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("01", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("0010", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00010000", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00001001", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00000011", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000111", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("0011", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00001110", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00000111", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000000011", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00010011", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00010001", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00001111", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000001101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000001010", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("0000000100", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00001101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("0000101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00001000", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000001011", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("0000000101", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("0000000001", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000001100", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00000100", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000000100", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("000000001", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00000000001", GetNextCounter()));
			Table[8].Codes.Add(new HuffmanCode("00000000000", GetNextCounter()));

			ResetCounter(5);
			Table[9].Codes.Add(new HuffmanCode("111", new[] {0, 0}));
			Table[9].Codes.Add(new HuffmanCode("101", new[] {0, 1}));
			Table[9].Codes.Add(new HuffmanCode("01001", new[] {0, 2}));
			Table[9].Codes.Add(new HuffmanCode("001110", new[] {0, 3}));
			Table[9].Codes.Add(new HuffmanCode("00001111", new[] {0, 4}));
			Table[9].Codes.Add(new HuffmanCode("000000111", new[] {0, 5}));
			Table[9].Codes.Add(new HuffmanCode("110", new[] {1, 0}));
			Table[9].Codes.Add(new HuffmanCode("100", new[] {1, 1}));
			Table[9].Codes.Add(new HuffmanCode("0101", new[] {1, 2}));
			Table[9].Codes.Add(new HuffmanCode("00101", new[] {1, 3}));
			Table[9].Codes.Add(new HuffmanCode("000110", new[] {1, 4}));
			Table[9].Codes.Add(new HuffmanCode("00000111", new[] {1, 5}));
			Table[9].Codes.Add(new HuffmanCode("0111", new[] {2, 0}));
			Table[9].Codes.Add(new HuffmanCode("0110", new[] {2, 1}));
			Table[9].Codes.Add(new HuffmanCode("01000", new[] {2, 2}));
			Table[9].Codes.Add(new HuffmanCode("001000", new[] {2, 3}));
			Table[9].Codes.Add(new HuffmanCode("0001000", new[] {2, 4}));
			Table[9].Codes.Add(new HuffmanCode("00000101", new[] {2, 5}));
			Table[9].Codes.Add(new HuffmanCode("001111", new[] {3, 0}));
			Table[9].Codes.Add(new HuffmanCode("00110", new[] {3, 1}));
			Table[9].Codes.Add(new HuffmanCode("001001", new[] {3, 2}));
			Table[9].Codes.Add(new HuffmanCode("0001010", new[] {3, 3}));
			Table[9].Codes.Add(new HuffmanCode("0000101", new[] {3, 4}));
			Table[9].Codes.Add(new HuffmanCode("00000001", new[] {3, 5}));
			Table[9].Codes.Add(new HuffmanCode("0001011", new[] {4, 0}));
			Table[9].Codes.Add(new HuffmanCode("000111", new[] {4, 1}));
			Table[9].Codes.Add(new HuffmanCode("0001001", new[] {4, 2}));
			Table[9].Codes.Add(new HuffmanCode("0000110", new[] {4, 3}));
			Table[9].Codes.Add(new HuffmanCode("00000100", new[] {4, 4}));
			Table[9].Codes.Add(new HuffmanCode("000000001", new[] {4, 5}));
			Table[9].Codes.Add(new HuffmanCode("00001110", new[] {5, 0}));
			Table[9].Codes.Add(new HuffmanCode("0000100", new[] {5, 1}));
			Table[9].Codes.Add(new HuffmanCode("00000110", new[] {5, 2}));
			Table[9].Codes.Add(new HuffmanCode("00000010", new[] {5, 3}));
			Table[9].Codes.Add(new HuffmanCode("000000110", new[] {5, 4}));
			Table[9].Codes.Add(new HuffmanCode("000000000", new[] {5, 5}));

			ResetCounter(7);
			Table[10].Codes.Add(new HuffmanCode("1", new[] {0, 0}));
			Table[10].Codes.Add(new HuffmanCode("010", new[] {0, 1}));
			Table[10].Codes.Add(new HuffmanCode("001010", new[] {0, 2}));
			Table[10].Codes.Add(new HuffmanCode("00010111", new[] {0, 3}));
			Table[10].Codes.Add(new HuffmanCode("000100011", new[] {0, 4}));
			Table[10].Codes.Add(new HuffmanCode("000011110", new[] {0, 5}));
			Table[10].Codes.Add(new HuffmanCode("000001100", new[] {0, 6}));
			Table[10].Codes.Add(new HuffmanCode("0000010001", new[] {0, 7}));
			Table[10].Codes.Add(new HuffmanCode("011", new[] {1, 0}));
			Table[10].Codes.Add(new HuffmanCode("0011", new[] {1, 1}));
			Table[10].Codes.Add(new HuffmanCode("001000", new[] {1, 2}));
			Table[10].Codes.Add(new HuffmanCode("0001100", new[] {1, 3}));
			Table[10].Codes.Add(new HuffmanCode("00010010", new[] {1, 4}));
			Table[10].Codes.Add(new HuffmanCode("000010101", new[] {1, 5}));
			Table[10].Codes.Add(new HuffmanCode("00001100", new[] {1, 6}));
			Table[10].Codes.Add(new HuffmanCode("00000111", new[] {1, 7}));
			Table[10].Codes.Add(new HuffmanCode("001011", new[] {2, 0}));
			Table[10].Codes.Add(new HuffmanCode("001001", new[] {2, 1}));
			Table[10].Codes.Add(new HuffmanCode("0001111", new[] {2, 2}));
			Table[10].Codes.Add(new HuffmanCode("00010101", new[] {2, 3}));
			Table[10].Codes.Add(new HuffmanCode("000100000", new[] {2, 4}));
			Table[10].Codes.Add(new HuffmanCode("0000101000", new[] {2, 5}));
			Table[10].Codes.Add(new HuffmanCode("000010011", new[] {2, 6}));
			Table[10].Codes.Add(new HuffmanCode("000000110", new[] {2, 7}));
			Table[10].Codes.Add(new HuffmanCode("0001110", new[] {3, 0}));
			Table[10].Codes.Add(new HuffmanCode("0001101", new[] {3, 1}));
			Table[10].Codes.Add(new HuffmanCode("00010110", new[] {3, 2}));
			Table[10].Codes.Add(new HuffmanCode("000100010", new[] {3, 3}));
			Table[10].Codes.Add(new HuffmanCode("0000101110", new[] {3, 4}));
			Table[10].Codes.Add(new HuffmanCode("0000010111", new[] {3, 5}));
			Table[10].Codes.Add(new HuffmanCode("000010010", new[] {3, 6}));
			Table[10].Codes.Add(new HuffmanCode("0000000111", new[] {3, 7}));
			Table[10].Codes.Add(new HuffmanCode("00010100", new[] {4, 0}));
			Table[10].Codes.Add(new HuffmanCode("00010011", new[] {4, 1}));
			Table[10].Codes.Add(new HuffmanCode("000100001", new[] {4, 2}));
			Table[10].Codes.Add(new HuffmanCode("0000101111", new[] {4, 3}));
			Table[10].Codes.Add(new HuffmanCode("0000011011", new[] {4, 4}));
			Table[10].Codes.Add(new HuffmanCode("0000010110", new[] {4, 5}));
			Table[10].Codes.Add(new HuffmanCode("0000001001", new[] {4, 6}));
			Table[10].Codes.Add(new HuffmanCode("0000000011", new[] {4, 7}));
			Table[10].Codes.Add(new HuffmanCode("000011111", new[] {5, 0}));
			Table[10].Codes.Add(new HuffmanCode("000010110", new[] {5, 1}));
			Table[10].Codes.Add(new HuffmanCode("0000101001", new[] {5, 2}));
			Table[10].Codes.Add(new HuffmanCode("0000011010", new[] {5, 3}));
			Table[10].Codes.Add(new HuffmanCode("00000010101", new[] {5, 4}));
			Table[10].Codes.Add(new HuffmanCode("00000010100", new[] {5, 5}));
			Table[10].Codes.Add(new HuffmanCode("0000000101", new[] {5, 6}));
			Table[10].Codes.Add(new HuffmanCode("00000000011", new[] {5, 7}));
			Table[10].Codes.Add(new HuffmanCode("00001110", new[] {6, 0}));
			Table[10].Codes.Add(new HuffmanCode("00001101", new[] {6, 1}));
			Table[10].Codes.Add(new HuffmanCode("000001010", new[] {6, 2}));
			Table[10].Codes.Add(new HuffmanCode("0000001011", new[] {6, 3}));
			Table[10].Codes.Add(new HuffmanCode("0000010000", new[] {6, 4}));
			Table[10].Codes.Add(new HuffmanCode("0000000110", new[] {6, 5}));
			Table[10].Codes.Add(new HuffmanCode("00000000101", new[] {6, 6}));
			Table[10].Codes.Add(new HuffmanCode("00000000001", new[] {6, 7}));
			Table[10].Codes.Add(new HuffmanCode("000001001", new[] {7, 0}));
			Table[10].Codes.Add(new HuffmanCode("00001000", new[] {7, 1}));
			Table[10].Codes.Add(new HuffmanCode("000000111", new[] {7, 2}));
			Table[10].Codes.Add(new HuffmanCode("0000001000", new[] {7, 3}));
			Table[10].Codes.Add(new HuffmanCode("0000000100", new[] {7, 4}));
			Table[10].Codes.Add(new HuffmanCode("00000000100", new[] {7, 5}));
			Table[10].Codes.Add(new HuffmanCode("00000000010", new[] {7, 6}));
			Table[10].Codes.Add(new HuffmanCode("00000000000", new[] {7, 7}));

			ResetCounter(7);
			Table[11].Codes.Add(new HuffmanCode("11", new[] {0, 0}));
			Table[11].Codes.Add(new HuffmanCode("100", new[] {0, 1}));
			Table[11].Codes.Add(new HuffmanCode("01010", new[] {0, 2}));
			Table[11].Codes.Add(new HuffmanCode("0011000", new[] {0, 3}));
			Table[11].Codes.Add(new HuffmanCode("00100010", new[] {0, 4}));
			Table[11].Codes.Add(new HuffmanCode("000100001", new[] {0, 5}));
			Table[11].Codes.Add(new HuffmanCode("00010101", new[] {0, 6}));
			Table[11].Codes.Add(new HuffmanCode("000001111", new[] {0, 7}));
			Table[11].Codes.Add(new HuffmanCode("101", new[] {1, 0}));
			Table[11].Codes.Add(new HuffmanCode("011", new[] {1, 1}));
			Table[11].Codes.Add(new HuffmanCode("0100", new[] {1, 2}));
			Table[11].Codes.Add(new HuffmanCode("001010", new[] {1, 3}));
			Table[11].Codes.Add(new HuffmanCode("00100000", new[] {1, 4}));
			Table[11].Codes.Add(new HuffmanCode("00010001", new[] {1, 5}));
			Table[11].Codes.Add(new HuffmanCode("0001011", new[] {1, 6}));
			Table[11].Codes.Add(new HuffmanCode("00001010", new[] {1, 7}));
			Table[11].Codes.Add(new HuffmanCode("01011", new[] {2, 0}));
			Table[11].Codes.Add(new HuffmanCode("00111", new[] {2, 1}));
			Table[11].Codes.Add(new HuffmanCode("001101", new[] {2, 2}));
			Table[11].Codes.Add(new HuffmanCode("0010010", new[] {2, 3}));
			Table[11].Codes.Add(new HuffmanCode("00011110", new[] {2, 4}));
			Table[11].Codes.Add(new HuffmanCode("000011111", new[] {2, 5}));
			Table[11].Codes.Add(new HuffmanCode("00010100", new[] {2, 6}));
			Table[11].Codes.Add(new HuffmanCode("00000101", new[] {2, 7}));
			Table[11].Codes.Add(new HuffmanCode("0011001", new[] {3, 0}));
			Table[11].Codes.Add(new HuffmanCode("001011", new[] {3, 1}));
			Table[11].Codes.Add(new HuffmanCode("0010011", new[] {3, 2}));
			Table[11].Codes.Add(new HuffmanCode("000111011", new[] {3, 3}));
			Table[11].Codes.Add(new HuffmanCode("00011011", new[] {3, 4}));
			Table[11].Codes.Add(new HuffmanCode("0000010010", new[] {3, 5}));
			Table[11].Codes.Add(new HuffmanCode("00001100", new[] {3, 6}));
			Table[11].Codes.Add(new HuffmanCode("000000101", new[] {3, 7}));
			Table[11].Codes.Add(new HuffmanCode("00100011", new[] {4, 0}));
			Table[11].Codes.Add(new HuffmanCode("00100001", new[] {4, 1}));
			Table[11].Codes.Add(new HuffmanCode("00011111", new[] {4, 2}));
			Table[11].Codes.Add(new HuffmanCode("000111010", new[] {4, 3}));
			Table[11].Codes.Add(new HuffmanCode("000011110", new[] {4, 4}));
			Table[11].Codes.Add(new HuffmanCode("0000010000", new[] {4, 5}));
			Table[11].Codes.Add(new HuffmanCode("000000111", new[] {4, 6}));
			Table[11].Codes.Add(new HuffmanCode("0000000101", new[] {4, 7}));
			Table[11].Codes.Add(new HuffmanCode("00011100", new[] {5, 0}));
			Table[11].Codes.Add(new HuffmanCode("00011010", new[] {5, 1}));
			Table[11].Codes.Add(new HuffmanCode("000100000", new[] {5, 2}));
			Table[11].Codes.Add(new HuffmanCode("0000010011", new[] {5, 3}));
			Table[11].Codes.Add(new HuffmanCode("0000010001", new[] {5, 4}));
			Table[11].Codes.Add(new HuffmanCode("0000000111", new[] {5, 5}));
			Table[11].Codes.Add(new HuffmanCode("0000001000", new[] {5, 6}));
			Table[11].Codes.Add(new HuffmanCode("0000000111", new[] {5, 7}));
			Table[11].Codes.Add(new HuffmanCode("00001110", new[] {6, 0}));
			Table[11].Codes.Add(new HuffmanCode("0001100", new[] {6, 1}));
			Table[11].Codes.Add(new HuffmanCode("0001001", new[] {6, 2}));
			Table[11].Codes.Add(new HuffmanCode("00001101", new[] {6, 3}));
			Table[11].Codes.Add(new HuffmanCode("000001110", new[] {6, 4}));
			Table[11].Codes.Add(new HuffmanCode("0000001001", new[] {6, 5}));
			Table[11].Codes.Add(new HuffmanCode("0000000100", new[] {6, 6}));
			Table[11].Codes.Add(new HuffmanCode("0000000001", new[] {6, 7}));
			Table[11].Codes.Add(new HuffmanCode("00001011", new[] {7, 0}));
			Table[11].Codes.Add(new HuffmanCode("0000100", new[] {7, 1}));
			Table[11].Codes.Add(new HuffmanCode("00000110", new[] {7, 2}));
			Table[11].Codes.Add(new HuffmanCode("000000110", new[] {7, 3}));
			Table[11].Codes.Add(new HuffmanCode("0000000110", new[] {7, 4}));
			Table[11].Codes.Add(new HuffmanCode("0000000011", new[] {7, 5}));
			Table[11].Codes.Add(new HuffmanCode("0000000010", new[] {7, 6}));
			Table[11].Codes.Add(new HuffmanCode("0000000000", new[] {7, 7}));

			ResetCounter(7);
			Table[12].Codes.Add(new HuffmanCode("1001", new[] {0, 0}));
			Table[12].Codes.Add(new HuffmanCode("110", new[] {0, 1}));
			Table[12].Codes.Add(new HuffmanCode("10000", new[] {0, 2}));
			Table[12].Codes.Add(new HuffmanCode("0100001", new[] {0, 3}));
			Table[12].Codes.Add(new HuffmanCode("00101001", new[] {0, 4}));
			Table[12].Codes.Add(new HuffmanCode("000100111", new[] {0, 5}));
			Table[12].Codes.Add(new HuffmanCode("000100110", new[] {0, 6}));
			Table[12].Codes.Add(new HuffmanCode("000011010", new[] {0, 7}));
			Table[12].Codes.Add(new HuffmanCode("111", new[] {1, 0}));
			Table[12].Codes.Add(new HuffmanCode("101", new[] {1, 1}));
			Table[12].Codes.Add(new HuffmanCode("0110", new[] {1, 2}));
			Table[12].Codes.Add(new HuffmanCode("01001", new[] {1, 3}));
			Table[12].Codes.Add(new HuffmanCode("0010111", new[] {1, 4}));
			Table[12].Codes.Add(new HuffmanCode("0010000", new[] {1, 5}));
			Table[12].Codes.Add(new HuffmanCode("00011010", new[] {1, 6}));
			Table[12].Codes.Add(new HuffmanCode("00001011", new[] {1, 7}));
			Table[12].Codes.Add(new HuffmanCode("10001", new[] {2, 0}));
			Table[12].Codes.Add(new HuffmanCode("0111", new[] {2, 1}));
			Table[12].Codes.Add(new HuffmanCode("01011", new[] {2, 2}));
			Table[12].Codes.Add(new HuffmanCode("001110", new[] {2, 3}));
			Table[12].Codes.Add(new HuffmanCode("0010101", new[] {2, 4}));
			Table[12].Codes.Add(new HuffmanCode("00011110", new[] {2, 5}));
			Table[12].Codes.Add(new HuffmanCode("0001010", new[] {2, 6}));
			Table[12].Codes.Add(new HuffmanCode("00000111", new[] {2, 7}));
			Table[12].Codes.Add(new HuffmanCode("010001", new[] {3, 0}));
			Table[12].Codes.Add(new HuffmanCode("01010", new[] {3, 1}));
			Table[12].Codes.Add(new HuffmanCode("001111", new[] {3, 2}));
			Table[12].Codes.Add(new HuffmanCode("001100", new[] {3, 3}));
			Table[12].Codes.Add(new HuffmanCode("0010010", new[] {3, 4}));
			Table[12].Codes.Add(new HuffmanCode("00011100", new[] {3, 5}));
			Table[12].Codes.Add(new HuffmanCode("00001110", new[] {3, 6}));
			Table[12].Codes.Add(new HuffmanCode("00000101", new[] {3, 7}));
			Table[12].Codes.Add(new HuffmanCode("0100000", new[] {4, 0}));
			Table[12].Codes.Add(new HuffmanCode("001101", new[] {4, 1}));
			Table[12].Codes.Add(new HuffmanCode("0010110", new[] {4, 2}));
			Table[12].Codes.Add(new HuffmanCode("0010011", new[] {4, 3}));
			Table[12].Codes.Add(new HuffmanCode("00010010", new[] {4, 4}));
			Table[12].Codes.Add(new HuffmanCode("00010000", new[] {4, 5}));
			Table[12].Codes.Add(new HuffmanCode("00001001", new[] {4, 6}));
			Table[12].Codes.Add(new HuffmanCode("000000101", new[] {4, 7}));
			Table[12].Codes.Add(new HuffmanCode("00101000", new[] {5, 0}));
			Table[12].Codes.Add(new HuffmanCode("0010001", new[] {5, 1}));
			Table[12].Codes.Add(new HuffmanCode("00011111", new[] {5, 2}));
			Table[12].Codes.Add(new HuffmanCode("00011101", new[] {5, 3}));
			Table[12].Codes.Add(new HuffmanCode("00010001", new[] {5, 4}));
			Table[12].Codes.Add(new HuffmanCode("000001101", new[] {5, 5}));
			Table[12].Codes.Add(new HuffmanCode("00000100", new[] {5, 6}));
			Table[12].Codes.Add(new HuffmanCode("000000010", new[] {5, 7}));
			Table[12].Codes.Add(new HuffmanCode("00011011", new[] {6, 0}));
			Table[12].Codes.Add(new HuffmanCode("0001100", new[] {6, 1}));
			Table[12].Codes.Add(new HuffmanCode("0001011", new[] {6, 2}));
			Table[12].Codes.Add(new HuffmanCode("00001111", new[] {6, 3}));
			Table[12].Codes.Add(new HuffmanCode("00001010", new[] {6, 4}));
			Table[12].Codes.Add(new HuffmanCode("000000111", new[] {6, 5}));
			Table[12].Codes.Add(new HuffmanCode("000000100", new[] {6, 6}));
			Table[12].Codes.Add(new HuffmanCode("0000000001", new[] {6, 7}));
			Table[12].Codes.Add(new HuffmanCode("000011011", new[] {7, 0}));
			Table[12].Codes.Add(new HuffmanCode("00001100", new[] {7, 1}));
			Table[12].Codes.Add(new HuffmanCode("00001000", new[] {7, 2}));
			Table[12].Codes.Add(new HuffmanCode("000001100", new[] {7, 3}));
			Table[12].Codes.Add(new HuffmanCode("000000110", new[] {7, 4}));
			Table[12].Codes.Add(new HuffmanCode("000000011", new[] {7, 5}));
			Table[12].Codes.Add(new HuffmanCode("000000001", new[] {7, 6}));
			Table[12].Codes.Add(new HuffmanCode("0000000000", new[] {7, 7}));

			ResetCounter(15);
			Table[13].Codes.Add(new HuffmanCode("1", new[] {0, 0}));
			Table[13].Codes.Add(new HuffmanCode("0101", new[] {0, 1}));
			Table[13].Codes.Add(new HuffmanCode("001110", new[] {0, 2}));
			Table[13].Codes.Add(new HuffmanCode("0010101", new[] {0, 3}));
			Table[13].Codes.Add(new HuffmanCode("00100010", new[] {0, 4}));
			Table[13].Codes.Add(new HuffmanCode("000110011", new[] {0, 5}));
			Table[13].Codes.Add(new HuffmanCode("000101110", new[] {0, 6}));
			Table[13].Codes.Add(new HuffmanCode("0001000111", new[] {0, 7}));
			Table[13].Codes.Add(new HuffmanCode("000101010", new[] {0, 8}));
			Table[13].Codes.Add(new HuffmanCode("0000110100", new[] {0, 9}));
			Table[13].Codes.Add(new HuffmanCode("00001000100", new[] {0, 10}));
			Table[13].Codes.Add(new HuffmanCode("00000110100", new[] {0, 11}));
			Table[13].Codes.Add(new HuffmanCode("000001000011", new[] {0, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000101100", new[] {0, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000101011", new[] {0, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000010011", new[] {0, 15}));
			Table[13].Codes.Add(new HuffmanCode("011", new[] {1, 0}));
			Table[13].Codes.Add(new HuffmanCode("0100", new[] {1, 1}));
			Table[13].Codes.Add(new HuffmanCode("001100", new[] {1, 2}));
			Table[13].Codes.Add(new HuffmanCode("0010011", new[] {1, 3}));
			Table[13].Codes.Add(new HuffmanCode("00011111", new[] {1, 4}));
			Table[13].Codes.Add(new HuffmanCode("00011010", new[] {1, 5}));
			Table[13].Codes.Add(new HuffmanCode("000101100", new[] {1, 6}));
			Table[13].Codes.Add(new HuffmanCode("000100001", new[] {1, 7}));
			Table[13].Codes.Add(new HuffmanCode("000011111", new[] {1, 8}));
			Table[13].Codes.Add(new HuffmanCode("000011000", new[] {1, 9}));
			Table[13].Codes.Add(new HuffmanCode("0000100000", new[] {1, 10}));
			Table[13].Codes.Add(new HuffmanCode("0000011000", new[] {1, 11}));
			Table[13].Codes.Add(new HuffmanCode("00000011111", new[] {1, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000100011", new[] {1, 13}));
			Table[13].Codes.Add(new HuffmanCode("000000010110", new[] {1, 14}));
			Table[13].Codes.Add(new HuffmanCode("000000001110", new[] {1, 15}));
			Table[13].Codes.Add(new HuffmanCode("001111", new[] {2, 0}));
			Table[13].Codes.Add(new HuffmanCode("001101", new[] {2, 1}));
			Table[13].Codes.Add(new HuffmanCode("0010111", new[] {2, 2}));
			Table[13].Codes.Add(new HuffmanCode("00100100", new[] {2, 3}));
			Table[13].Codes.Add(new HuffmanCode("000111011", new[] {2, 4}));
			Table[13].Codes.Add(new HuffmanCode("000110001", new[] {2, 5}));
			Table[13].Codes.Add(new HuffmanCode("0001001101", new[] {2, 6}));
			Table[13].Codes.Add(new HuffmanCode("0001000001", new[] {2, 7}));
			Table[13].Codes.Add(new HuffmanCode("000011101", new[] {2, 8}));
			Table[13].Codes.Add(new HuffmanCode("0000101000", new[] {2, 9}));
			Table[13].Codes.Add(new HuffmanCode("0000011110", new[] {2, 10}));
			Table[13].Codes.Add(new HuffmanCode("00000101000", new[] {2, 11}));
			Table[13].Codes.Add(new HuffmanCode("00000011011", new[] {2, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000100001", new[] {2, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000101010", new[] {2, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000010000", new[] {2, 15}));
			Table[13].Codes.Add(new HuffmanCode("0010110", new[] {3, 0}));
			Table[13].Codes.Add(new HuffmanCode("0010100", new[] {3, 1}));
			Table[13].Codes.Add(new HuffmanCode("00100101", new[] {3, 2}));
			Table[13].Codes.Add(new HuffmanCode("000111101", new[] {3, 3}));
			Table[13].Codes.Add(new HuffmanCode("000111000", new[] {3, 4}));
			Table[13].Codes.Add(new HuffmanCode("0001001111", new[] {3, 5}));
			Table[13].Codes.Add(new HuffmanCode("0001001001", new[] {3, 6}));
			Table[13].Codes.Add(new HuffmanCode("0001000000", new[] {3, 7}));
			Table[13].Codes.Add(new HuffmanCode("0000101011", new[] {3, 8}));
			Table[13].Codes.Add(new HuffmanCode("00001001100", new[] {3, 9}));
			Table[13].Codes.Add(new HuffmanCode("00000111000", new[] {3, 10}));
			Table[13].Codes.Add(new HuffmanCode("00000100101", new[] {3, 11}));
			Table[13].Codes.Add(new HuffmanCode("00000011010", new[] {3, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000011111", new[] {3, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000011001", new[] {3, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000001110", new[] {3, 15}));
			Table[13].Codes.Add(new HuffmanCode("00100011", new[] {4, 0}));
			Table[13].Codes.Add(new HuffmanCode("0010000", new[] {4, 1}));
			Table[13].Codes.Add(new HuffmanCode("000111100", new[] {4, 2}));
			Table[13].Codes.Add(new HuffmanCode("000111001", new[] {4, 3}));
			Table[13].Codes.Add(new HuffmanCode("0001100001", new[] {4, 4}));
			Table[13].Codes.Add(new HuffmanCode("0001001011", new[] {4, 5}));
			Table[13].Codes.Add(new HuffmanCode("00001110010", new[] {4, 6}));
			Table[13].Codes.Add(new HuffmanCode("00001011011", new[] {4, 7}));
			Table[13].Codes.Add(new HuffmanCode("0000110110", new[] {4, 8}));
			Table[13].Codes.Add(new HuffmanCode("00001001001", new[] {4, 9}));
			Table[13].Codes.Add(new HuffmanCode("00000110111", new[] {4, 10}));
			Table[13].Codes.Add(new HuffmanCode("000000101001", new[] {4, 11}));
			Table[13].Codes.Add(new HuffmanCode("000000110000", new[] {4, 12}));
			Table[13].Codes.Add(new HuffmanCode("0000000110101", new[] {4, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000010111", new[] {4, 14}));
			Table[13].Codes.Add(new HuffmanCode("00000000011000", new[] {4, 15}));
			Table[13].Codes.Add(new HuffmanCode("000111010", new[] {5, 0}));
			Table[13].Codes.Add(new HuffmanCode("00011011", new[] {5, 1}));
			Table[13].Codes.Add(new HuffmanCode("000110010", new[] {5, 2}));
			Table[13].Codes.Add(new HuffmanCode("0001100000", new[] {5, 3}));
			Table[13].Codes.Add(new HuffmanCode("0001001100", new[] {5, 4}));
			Table[13].Codes.Add(new HuffmanCode("0001000110", new[] {5, 5}));
			Table[13].Codes.Add(new HuffmanCode("00001011101", new[] {5, 6}));
			Table[13].Codes.Add(new HuffmanCode("00001010100", new[] {5, 7}));
			Table[13].Codes.Add(new HuffmanCode("00001001101", new[] {5, 8}));
			Table[13].Codes.Add(new HuffmanCode("00000111010", new[] {5, 9}));
			Table[13].Codes.Add(new HuffmanCode("000001001111", new[] {5, 10}));
			Table[13].Codes.Add(new HuffmanCode("00000011101", new[] {5, 11}));
			Table[13].Codes.Add(new HuffmanCode("0000001001010", new[] {5, 12}));
			Table[13].Codes.Add(new HuffmanCode("0000000110001", new[] {5, 13}));
			Table[13].Codes.Add(new HuffmanCode("00000000101001", new[] {5, 14}));
			Table[13].Codes.Add(new HuffmanCode("00000000010001", new[] {5, 15}));
			Table[13].Codes.Add(new HuffmanCode("000101111", new[] {6, 0}));
			Table[13].Codes.Add(new HuffmanCode("000101101", new[] {6, 1}));
			Table[13].Codes.Add(new HuffmanCode("0001001110", new[] {6, 2}));
			Table[13].Codes.Add(new HuffmanCode("0001001010", new[] {6, 3}));
			Table[13].Codes.Add(new HuffmanCode("00001110011", new[] {6, 4}));
			Table[13].Codes.Add(new HuffmanCode("00001011110", new[] {6, 5}));
			Table[13].Codes.Add(new HuffmanCode("00001011010", new[] {6, 6}));
			Table[13].Codes.Add(new HuffmanCode("00001001111", new[] {6, 7}));
			Table[13].Codes.Add(new HuffmanCode("00001000101", new[] {6, 8}));
			Table[13].Codes.Add(new HuffmanCode("000001010011", new[] {6, 9}));
			Table[13].Codes.Add(new HuffmanCode("000001000111", new[] {6, 10}));
			Table[13].Codes.Add(new HuffmanCode("000000110010", new[] {6, 11}));
			Table[13].Codes.Add(new HuffmanCode("0000000111011", new[] {6, 12}));
			Table[13].Codes.Add(new HuffmanCode("0000000100110", new[] {6, 13}));
			Table[13].Codes.Add(new HuffmanCode("00000000100100", new[] {6, 14}));
			Table[13].Codes.Add(new HuffmanCode("00000000001111", new[] {6, 15}));
			Table[13].Codes.Add(new HuffmanCode("0001001000", new[] {7, 0}));
			Table[13].Codes.Add(new HuffmanCode("000100010", new[] {7, 1}));
			Table[13].Codes.Add(new HuffmanCode("0000111000", new[] {7, 2}));
			Table[13].Codes.Add(new HuffmanCode("00001011111", new[] {7, 3}));
			Table[13].Codes.Add(new HuffmanCode("00001011100", new[] {7, 4}));
			Table[13].Codes.Add(new HuffmanCode("00001010101", new[] {7, 5}));
			Table[13].Codes.Add(new HuffmanCode("000001011011", new[] {7, 6}));
			Table[13].Codes.Add(new HuffmanCode("000001011010", new[] {7, 7}));
			Table[13].Codes.Add(new HuffmanCode("000001010110", new[] {7, 8}));
			Table[13].Codes.Add(new HuffmanCode("000001001001", new[] {7, 9}));
			Table[13].Codes.Add(new HuffmanCode("0000001001101", new[] {7, 10}));
			Table[13].Codes.Add(new HuffmanCode("0000001000001", new[] {7, 11}));
			Table[13].Codes.Add(new HuffmanCode("0000000110011", new[] {7, 12}));
			Table[13].Codes.Add(new HuffmanCode("00000000101100", new[] {7, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000101011", new[] {7, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000000101010", new[] {7, 15}));
			Table[13].Codes.Add(new HuffmanCode("000101011", new[] {8, 0}));
			Table[13].Codes.Add(new HuffmanCode("00010100", new[] {8, 1}));
			Table[13].Codes.Add(new HuffmanCode("000011110", new[] {8, 2}));
			Table[13].Codes.Add(new HuffmanCode("0000101100", new[] {8, 3}));
			Table[13].Codes.Add(new HuffmanCode("0000110111", new[] {8, 4}));
			Table[13].Codes.Add(new HuffmanCode("00001001110", new[] {8, 5}));
			Table[13].Codes.Add(new HuffmanCode("00001001000", new[] {8, 6}));
			Table[13].Codes.Add(new HuffmanCode("000001010111", new[] {8, 7}));
			Table[13].Codes.Add(new HuffmanCode("000001001110", new[] {8, 8}));
			Table[13].Codes.Add(new HuffmanCode("000000111101", new[] {8, 9}));
			Table[13].Codes.Add(new HuffmanCode("000000101110", new[] {8, 10}));
			Table[13].Codes.Add(new HuffmanCode("0000000110110", new[] {8, 11}));
			Table[13].Codes.Add(new HuffmanCode("0000000100101", new[] {8, 12}));
			Table[13].Codes.Add(new HuffmanCode("00000000011110", new[] {8, 13}));
			Table[13].Codes.Add(new HuffmanCode("000000000010100", new[] {8, 14}));
			Table[13].Codes.Add(new HuffmanCode("000000000010000", new[] {8, 15}));
			Table[13].Codes.Add(new HuffmanCode("0000110101", new[] {9, 0}));
			Table[13].Codes.Add(new HuffmanCode("000011001", new[] {9, 1}));
			Table[13].Codes.Add(new HuffmanCode("0000101001", new[] {9, 2}));
			Table[13].Codes.Add(new HuffmanCode("0000100101", new[] {9, 3}));
			Table[13].Codes.Add(new HuffmanCode("00000101100", new[] {9, 4}));
			Table[13].Codes.Add(new HuffmanCode("00000111011", new[] {9, 5}));
			Table[13].Codes.Add(new HuffmanCode("00000110110", new[] {9, 6}));
			Table[13].Codes.Add(new HuffmanCode("0000001010001", new[] {9, 7}));
			Table[13].Codes.Add(new HuffmanCode("000001000010", new[] {9, 8}));
			Table[13].Codes.Add(new HuffmanCode("0000001001100", new[] {9, 9}));
			Table[13].Codes.Add(new HuffmanCode("0000000111001", new[] {9, 10}));
			Table[13].Codes.Add(new HuffmanCode("00000000110110", new[] {9, 11}));
			Table[13].Codes.Add(new HuffmanCode("00000000100101", new[] {9, 12}));
			Table[13].Codes.Add(new HuffmanCode("00000000010010", new[] {9, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000100111", new[] {9, 14}));
			Table[13].Codes.Add(new HuffmanCode("000000000001011", new[] {9, 15}));
			Table[13].Codes.Add(new HuffmanCode("0000100011", new[] {10, 0}));
			Table[13].Codes.Add(new HuffmanCode("0000100001", new[] {10, 1}));
			Table[13].Codes.Add(new HuffmanCode("0000011111", new[] {10, 2}));
			Table[13].Codes.Add(new HuffmanCode("00000111001", new[] {10, 3}));
			Table[13].Codes.Add(new HuffmanCode("00000101010", new[] {10, 4}));
			Table[13].Codes.Add(new HuffmanCode("000001010010", new[] {10, 5}));
			Table[13].Codes.Add(new HuffmanCode("000001001000", new[] {10, 6}));
			Table[13].Codes.Add(new HuffmanCode("0000001010000", new[] {10, 7}));
			Table[13].Codes.Add(new HuffmanCode("000000101111", new[] {10, 8}));
			Table[13].Codes.Add(new HuffmanCode("0000000111010", new[] {10, 9}));
			Table[13].Codes.Add(new HuffmanCode("00000000110111", new[] {10, 10}));
			Table[13].Codes.Add(new HuffmanCode("0000000010101", new[] {10, 11}));
			Table[13].Codes.Add(new HuffmanCode("00000000010110", new[] {10, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000000011010", new[] {10, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000100110", new[] {10, 14}));
			Table[13].Codes.Add(new HuffmanCode("00000000000010110", new[] {10, 15}));
			Table[13].Codes.Add(new HuffmanCode("00000110101", new[] {11, 0}));
			Table[13].Codes.Add(new HuffmanCode("0000011001", new[] {11, 1}));
			Table[13].Codes.Add(new HuffmanCode("0000010111", new[] {11, 2}));
			Table[13].Codes.Add(new HuffmanCode("00000100110", new[] {11, 3}));
			Table[13].Codes.Add(new HuffmanCode("000001000110", new[] {11, 4}));
			Table[13].Codes.Add(new HuffmanCode("000000111100", new[] {11, 5}));
			Table[13].Codes.Add(new HuffmanCode("000000110011", new[] {11, 6}));
			Table[13].Codes.Add(new HuffmanCode("000000100100", new[] {11, 7}));
			Table[13].Codes.Add(new HuffmanCode("0000000110111", new[] {11, 8}));
			Table[13].Codes.Add(new HuffmanCode("0000000011010", new[] {11, 9}));
			Table[13].Codes.Add(new HuffmanCode("0000000100010", new[] {11, 10}));
			Table[13].Codes.Add(new HuffmanCode("00000000010111", new[] {11, 11}));
			Table[13].Codes.Add(new HuffmanCode("000000000011011", new[] {11, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000000001110", new[] {11, 13}));
			Table[13].Codes.Add(new HuffmanCode("000000000001001", new[] {11, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000111", new[] {11, 15}));
			Table[13].Codes.Add(new HuffmanCode("00000100010", new[] {12, 0}));
			Table[13].Codes.Add(new HuffmanCode("00000100000", new[] {12, 1}));
			Table[13].Codes.Add(new HuffmanCode("00000011100", new[] {12, 2}));
			Table[13].Codes.Add(new HuffmanCode("000000100111", new[] {12, 3}));
			Table[13].Codes.Add(new HuffmanCode("000000110001", new[] {12, 4}));
			Table[13].Codes.Add(new HuffmanCode("0000001001011", new[] {12, 5}));
			Table[13].Codes.Add(new HuffmanCode("000000011110", new[] {12, 6}));
			Table[13].Codes.Add(new HuffmanCode("0000000110100", new[] {12, 7}));
			Table[13].Codes.Add(new HuffmanCode("00000000110000", new[] {12, 8}));
			Table[13].Codes.Add(new HuffmanCode("00000000101000", new[] {12, 9}));
			Table[13].Codes.Add(new HuffmanCode("000000000110100", new[] {12, 10}));
			Table[13].Codes.Add(new HuffmanCode("000000000011100", new[] {12, 11}));
			Table[13].Codes.Add(new HuffmanCode("000000000010010", new[] {12, 12}));
			Table[13].Codes.Add(new HuffmanCode("0000000000010001", new[] {12, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000001001", new[] {12, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000101", new[] {12, 15}));
			Table[13].Codes.Add(new HuffmanCode("000000101101", new[] {13, 0}));
			Table[13].Codes.Add(new HuffmanCode("00000010101", new[] {13, 1}));
			Table[13].Codes.Add(new HuffmanCode("000000100010", new[] {13, 2}));
			Table[13].Codes.Add(new HuffmanCode("0000001000000", new[] {13, 3}));
			Table[13].Codes.Add(new HuffmanCode("0000000111000", new[] {13, 4}));
			Table[13].Codes.Add(new HuffmanCode("0000000110010", new[] {13, 5}));
			Table[13].Codes.Add(new HuffmanCode("00000000110001", new[] {13, 6}));
			Table[13].Codes.Add(new HuffmanCode("00000000101101", new[] {13, 7}));
			Table[13].Codes.Add(new HuffmanCode("00000000011111", new[] {13, 8}));
			Table[13].Codes.Add(new HuffmanCode("00000000010011", new[] {13, 9}));
			Table[13].Codes.Add(new HuffmanCode("00000000001100", new[] {13, 10}));
			Table[13].Codes.Add(new HuffmanCode("000000000001111", new[] {13, 11}));
			Table[13].Codes.Add(new HuffmanCode("0000000000001010", new[] {13, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000000000111", new[] {13, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000110", new[] {13, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000011", new[] {13, 15}));
			Table[13].Codes.Add(new HuffmanCode("0000000110000", new[] {14, 0}));
			Table[13].Codes.Add(new HuffmanCode("000000010111", new[] {14, 1}));
			Table[13].Codes.Add(new HuffmanCode("000000010100", new[] {14, 2}));
			Table[13].Codes.Add(new HuffmanCode("0000000100111", new[] {14, 3}));
			Table[13].Codes.Add(new HuffmanCode("0000000100100", new[] {14, 4}));
			Table[13].Codes.Add(new HuffmanCode("0000000100011", new[] {14, 5}));
			Table[13].Codes.Add(new HuffmanCode("000000000110101", new[] {14, 6}));
			Table[13].Codes.Add(new HuffmanCode("00000000010101", new[] {14, 7}));
			Table[13].Codes.Add(new HuffmanCode("00000000010000", new[] {14, 8}));
			Table[13].Codes.Add(new HuffmanCode("00000000000010111", new[] {14, 9}));
			Table[13].Codes.Add(new HuffmanCode("000000000001101", new[] {14, 10}));
			Table[13].Codes.Add(new HuffmanCode("000000000001010", new[] {14, 11}));
			Table[13].Codes.Add(new HuffmanCode("000000000000110", new[] {14, 12}));
			Table[13].Codes.Add(new HuffmanCode("00000000000000001", new[] {14, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000100", new[] {14, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000010", new[] {14, 15}));
			Table[13].Codes.Add(new HuffmanCode("000000010000", new[] {15, 0}));
			Table[13].Codes.Add(new HuffmanCode("000000001111", new[] {15, 1}));
			Table[13].Codes.Add(new HuffmanCode("0000000010001", new[] {15, 2}));
			Table[13].Codes.Add(new HuffmanCode("00000000011011", new[] {15, 3}));
			Table[13].Codes.Add(new HuffmanCode("00000000011001", new[] {15, 4}));
			Table[13].Codes.Add(new HuffmanCode("00000000010100", new[] {15, 5}));
			Table[13].Codes.Add(new HuffmanCode("000000000011101", new[] {15, 6}));
			Table[13].Codes.Add(new HuffmanCode("00000000001011", new[] {15, 7}));
			Table[13].Codes.Add(new HuffmanCode("000000000010001", new[] {15, 8}));
			Table[13].Codes.Add(new HuffmanCode("000000000001100", new[] {15, 9}));
			Table[13].Codes.Add(new HuffmanCode("0000000000010000", new[] {15, 10}));
			Table[13].Codes.Add(new HuffmanCode("0000000000001000", new[] {15, 11}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000000001", new[] {15, 12}));
			Table[13].Codes.Add(new HuffmanCode("000000000000000001", new[] {15, 13}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000000000", new[] {15, 14}));
			Table[13].Codes.Add(new HuffmanCode("0000000000000001", new[] {15, 15}));

			ResetCounter(15);
			Table[15].Codes.Add(new HuffmanCode("111", new[] {0, 0}));
			Table[15].Codes.Add(new HuffmanCode("1100", new[] {0, 1}));
			Table[15].Codes.Add(new HuffmanCode("10010", new[] {0, 2}));
			Table[15].Codes.Add(new HuffmanCode("0110101", new[] {0, 3}));
			Table[15].Codes.Add(new HuffmanCode("0101111", new[] {0, 4}));
			Table[15].Codes.Add(new HuffmanCode("01001100", new[] {0, 5}));
			Table[15].Codes.Add(new HuffmanCode("001111100", new[] {0, 6}));
			Table[15].Codes.Add(new HuffmanCode("001101100", new[] {0, 7}));
			Table[15].Codes.Add(new HuffmanCode("001011001", new[] {0, 8}));
			Table[15].Codes.Add(new HuffmanCode("0001111011", new[] {0, 9}));
			Table[15].Codes.Add(new HuffmanCode("0001101100", new[] {0, 10}));
			Table[15].Codes.Add(new HuffmanCode("00001110111", new[] {0, 11}));
			Table[15].Codes.Add(new HuffmanCode("00001101011", new[] {0, 12}));
			Table[15].Codes.Add(new HuffmanCode("00001010001", new[] {0, 13}));
			Table[15].Codes.Add(new HuffmanCode("000001111010", new[] {0, 14}));
			Table[15].Codes.Add(new HuffmanCode("0000000111111", new[] {0, 15}));
			Table[15].Codes.Add(new HuffmanCode("1101", new[] {1, 0}));
			Table[15].Codes.Add(new HuffmanCode("101", new[] {1, 1}));
			Table[15].Codes.Add(new HuffmanCode("10000", new[] {1, 2}));
			Table[15].Codes.Add(new HuffmanCode("011011", new[] {1, 3}));
			Table[15].Codes.Add(new HuffmanCode("0101110", new[] {1, 4}));
			Table[15].Codes.Add(new HuffmanCode("0100100", new[] {1, 5}));
			Table[15].Codes.Add(new HuffmanCode("00111101", new[] {1, 6}));
			Table[15].Codes.Add(new HuffmanCode("00110011", new[] {1, 7}));
			Table[15].Codes.Add(new HuffmanCode("00101010", new[] {1, 8}));
			Table[15].Codes.Add(new HuffmanCode("001000110", new[] {1, 9}));
			Table[15].Codes.Add(new HuffmanCode("000110100", new[] {1, 10}));
			Table[15].Codes.Add(new HuffmanCode("0001010011", new[] {1, 11}));
			Table[15].Codes.Add(new HuffmanCode("0001000001", new[] {1, 12}));
			Table[15].Codes.Add(new HuffmanCode("0000101001", new[] {1, 13}));
			Table[15].Codes.Add(new HuffmanCode("00000111011", new[] {1, 14}));
			Table[15].Codes.Add(new HuffmanCode("00000100100", new[] {1, 15}));
			Table[15].Codes.Add(new HuffmanCode("10011", new[] {2, 0}));
			Table[15].Codes.Add(new HuffmanCode("10001", new[] {2, 1}));
			Table[15].Codes.Add(new HuffmanCode("01111", new[] {2, 2}));
			Table[15].Codes.Add(new HuffmanCode("011000", new[] {2, 3}));
			Table[15].Codes.Add(new HuffmanCode("0101001", new[] {2, 4}));
			Table[15].Codes.Add(new HuffmanCode("0100010", new[] {2, 5}));
			Table[15].Codes.Add(new HuffmanCode("00111011", new[] {2, 6}));
			Table[15].Codes.Add(new HuffmanCode("00110000", new[] {2, 7}));
			Table[15].Codes.Add(new HuffmanCode("00101000", new[] {2, 8}));
			Table[15].Codes.Add(new HuffmanCode("001000000", new[] {2, 9}));
			Table[15].Codes.Add(new HuffmanCode("000110010", new[] {2, 10}));
			Table[15].Codes.Add(new HuffmanCode("0001001110", new[] {2, 11}));
			Table[15].Codes.Add(new HuffmanCode("0000111110", new[] {2, 12}));
			Table[15].Codes.Add(new HuffmanCode("00001010000", new[] {2, 13}));
			Table[15].Codes.Add(new HuffmanCode("00000111000", new[] {2, 14}));
			Table[15].Codes.Add(new HuffmanCode("00000100001", new[] {2, 15}));
			Table[15].Codes.Add(new HuffmanCode("011101", new[] {3, 0}));
			Table[15].Codes.Add(new HuffmanCode("011100", new[] {3, 1}));
			Table[15].Codes.Add(new HuffmanCode("011001", new[] {3, 2}));
			Table[15].Codes.Add(new HuffmanCode("0101011", new[] {3, 3}));
			Table[15].Codes.Add(new HuffmanCode("0100111", new[] {3, 4}));
			Table[15].Codes.Add(new HuffmanCode("00111111", new[] {3, 5}));
			Table[15].Codes.Add(new HuffmanCode("00110111", new[] {3, 6}));
			Table[15].Codes.Add(new HuffmanCode("001011101", new[] {3, 7}));
			Table[15].Codes.Add(new HuffmanCode("001001100", new[] {3, 8}));
			Table[15].Codes.Add(new HuffmanCode("000111011", new[] {3, 9}));
			Table[15].Codes.Add(new HuffmanCode("0001011101", new[] {3, 10}));
			Table[15].Codes.Add(new HuffmanCode("0001001000", new[] {3, 11}));
			Table[15].Codes.Add(new HuffmanCode("0000110110", new[] {3, 12}));
			Table[15].Codes.Add(new HuffmanCode("00001001011", new[] {3, 13}));
			Table[15].Codes.Add(new HuffmanCode("00000110010", new[] {3, 14}));
			Table[15].Codes.Add(new HuffmanCode("00000011101", new[] {3, 15}));
			Table[15].Codes.Add(new HuffmanCode("0110100", new[] {4, 0}));
			Table[15].Codes.Add(new HuffmanCode("010110", new[] {4, 1}));
			Table[15].Codes.Add(new HuffmanCode("0101010", new[] {4, 2}));
			Table[15].Codes.Add(new HuffmanCode("0101000", new[] {4, 3}));
			Table[15].Codes.Add(new HuffmanCode("01000011", new[] {4, 4}));
			Table[15].Codes.Add(new HuffmanCode("00111001", new[] {4, 5}));
			Table[15].Codes.Add(new HuffmanCode("001011111", new[] {4, 6}));
			Table[15].Codes.Add(new HuffmanCode("001001111", new[] {4, 7}));
			Table[15].Codes.Add(new HuffmanCode("001001000", new[] {4, 8}));
			Table[15].Codes.Add(new HuffmanCode("000111001", new[] {4, 9}));
			Table[15].Codes.Add(new HuffmanCode("0001011001", new[] {4, 10}));
			Table[15].Codes.Add(new HuffmanCode("0001000101", new[] {4, 11}));
			Table[15].Codes.Add(new HuffmanCode("0000110001", new[] {4, 12}));
			Table[15].Codes.Add(new HuffmanCode("00001000010", new[] {4, 13}));
			Table[15].Codes.Add(new HuffmanCode("00000101110", new[] {4, 14}));
			Table[15].Codes.Add(new HuffmanCode("00000011011", new[] {4, 15}));
			Table[15].Codes.Add(new HuffmanCode("01001101", new[] {5, 0}));
			Table[15].Codes.Add(new HuffmanCode("0100101", new[] {5, 1}));
			Table[15].Codes.Add(new HuffmanCode("0100011", new[] {5, 2}));
			Table[15].Codes.Add(new HuffmanCode("01000010", new[] {5, 3}));
			Table[15].Codes.Add(new HuffmanCode("00111010", new[] {5, 4}));
			Table[15].Codes.Add(new HuffmanCode("00110100", new[] {5, 5}));
			Table[15].Codes.Add(new HuffmanCode("001011011", new[] {5, 6}));
			Table[15].Codes.Add(new HuffmanCode("001001010", new[] {5, 7}));
			Table[15].Codes.Add(new HuffmanCode("000111110", new[] {5, 8}));
			Table[15].Codes.Add(new HuffmanCode("000110000", new[] {5, 9}));
			Table[15].Codes.Add(new HuffmanCode("0001001111", new[] {5, 10}));
			Table[15].Codes.Add(new HuffmanCode("0000111111", new[] {5, 11}));
			Table[15].Codes.Add(new HuffmanCode("00001011010", new[] {5, 12}));
			Table[15].Codes.Add(new HuffmanCode("00000111110", new[] {5, 13}));
			Table[15].Codes.Add(new HuffmanCode("00000101000", new[] {5, 14}));
			Table[15].Codes.Add(new HuffmanCode("000000100110", new[] {5, 15}));
			Table[15].Codes.Add(new HuffmanCode("001111101", new[] {6, 0}));
			Table[15].Codes.Add(new HuffmanCode("0100000", new[] {6, 1}));
			Table[15].Codes.Add(new HuffmanCode("00111100", new[] {6, 2}));
			Table[15].Codes.Add(new HuffmanCode("00111000", new[] {6, 3}));
			Table[15].Codes.Add(new HuffmanCode("00110010", new[] {6, 4}));
			Table[15].Codes.Add(new HuffmanCode("001011100", new[] {6, 5}));
			Table[15].Codes.Add(new HuffmanCode("001001110", new[] {6, 6}));
			Table[15].Codes.Add(new HuffmanCode("001000001", new[] {6, 7}));
			Table[15].Codes.Add(new HuffmanCode("000110111", new[] {6, 8}));
			Table[15].Codes.Add(new HuffmanCode("0001010111", new[] {6, 9}));
			Table[15].Codes.Add(new HuffmanCode("0001000111", new[] {6, 10}));
			Table[15].Codes.Add(new HuffmanCode("0000110011", new[] {6, 11}));
			Table[15].Codes.Add(new HuffmanCode("00001001001", new[] {6, 12}));
			Table[15].Codes.Add(new HuffmanCode("00000110011", new[] {6, 13}));
			Table[15].Codes.Add(new HuffmanCode("000001000110", new[] {6, 14}));
			Table[15].Codes.Add(new HuffmanCode("000000011110", new[] {6, 15}));
			Table[15].Codes.Add(new HuffmanCode("001101101", new[] {7, 0}));
			Table[15].Codes.Add(new HuffmanCode("00110101", new[] {7, 1}));
			Table[15].Codes.Add(new HuffmanCode("00110001", new[] {7, 2}));
			Table[15].Codes.Add(new HuffmanCode("001011110", new[] {7, 3}));
			Table[15].Codes.Add(new HuffmanCode("001011000", new[] {7, 4}));
			Table[15].Codes.Add(new HuffmanCode("001001011", new[] {7, 5}));
			Table[15].Codes.Add(new HuffmanCode("001000010", new[] {7, 6}));
			Table[15].Codes.Add(new HuffmanCode("0001111010", new[] {7, 7}));
			Table[15].Codes.Add(new HuffmanCode("0001011011", new[] {7, 8}));
			Table[15].Codes.Add(new HuffmanCode("0001001001", new[] {7, 9}));
			Table[15].Codes.Add(new HuffmanCode("0000111000", new[] {7, 10}));
			Table[15].Codes.Add(new HuffmanCode("0000101010", new[] {7, 11}));
			Table[15].Codes.Add(new HuffmanCode("00001000000", new[] {7, 12}));
			Table[15].Codes.Add(new HuffmanCode("00000101100", new[] {7, 13}));
			Table[15].Codes.Add(new HuffmanCode("00000010101", new[] {7, 14}));
			Table[15].Codes.Add(new HuffmanCode("000000011001", new[] {7, 15}));
			Table[15].Codes.Add(new HuffmanCode("001011010", new[] {8, 0}));
			Table[15].Codes.Add(new HuffmanCode("00101011", new[] {8, 1}));
			Table[15].Codes.Add(new HuffmanCode("00101001", new[] {8, 2}));
			Table[15].Codes.Add(new HuffmanCode("001001101", new[] {8, 3}));
			Table[15].Codes.Add(new HuffmanCode("001001001", new[] {8, 4}));
			Table[15].Codes.Add(new HuffmanCode("000111111", new[] {8, 5}));
			Table[15].Codes.Add(new HuffmanCode("000111000", new[] {8, 6}));
			Table[15].Codes.Add(new HuffmanCode("0001011100", new[] {8, 7}));
			Table[15].Codes.Add(new HuffmanCode("0001001101", new[] {8, 8}));
			Table[15].Codes.Add(new HuffmanCode("0001000010", new[] {8, 9}));
			Table[15].Codes.Add(new HuffmanCode("0000101111", new[] {8, 10}));
			Table[15].Codes.Add(new HuffmanCode("00001000011", new[] {8, 11}));
			Table[15].Codes.Add(new HuffmanCode("00000110000", new[] {8, 12}));
			Table[15].Codes.Add(new HuffmanCode("000000110101", new[] {8, 13}));
			Table[15].Codes.Add(new HuffmanCode("000000100100", new[] {8, 14}));
			Table[15].Codes.Add(new HuffmanCode("000000010100", new[] {8, 15}));
			Table[15].Codes.Add(new HuffmanCode("001000111", new[] {9, 0}));
			Table[15].Codes.Add(new HuffmanCode("00100010", new[] {9, 1}));
			Table[15].Codes.Add(new HuffmanCode("001000011", new[] {9, 2}));
			Table[15].Codes.Add(new HuffmanCode("000111100", new[] {9, 3}));
			Table[15].Codes.Add(new HuffmanCode("000111010", new[] {9, 4}));
			Table[15].Codes.Add(new HuffmanCode("000110001", new[] {9, 5}));
			Table[15].Codes.Add(new HuffmanCode("0001011000", new[] {9, 6}));
			Table[15].Codes.Add(new HuffmanCode("0001001100", new[] {9, 7}));
			Table[15].Codes.Add(new HuffmanCode("0001000011", new[] {9, 8}));
			Table[15].Codes.Add(new HuffmanCode("00001101010", new[] {9, 9}));
			Table[15].Codes.Add(new HuffmanCode("00001000111", new[] {9, 10}));
			Table[15].Codes.Add(new HuffmanCode("00000110110", new[] {9, 11}));
			Table[15].Codes.Add(new HuffmanCode("00000100110", new[] {9, 12}));
			Table[15].Codes.Add(new HuffmanCode("000000100111", new[] {9, 13}));
			Table[15].Codes.Add(new HuffmanCode("000000010111", new[] {9, 14}));
			Table[15].Codes.Add(new HuffmanCode("000000001111", new[] {9, 15}));
			Table[15].Codes.Add(new HuffmanCode("0001101101", new[] {10, 0}));
			Table[15].Codes.Add(new HuffmanCode("000110101", new[] {10, 1}));
			Table[15].Codes.Add(new HuffmanCode("000110011", new[] {10, 2}));
			Table[15].Codes.Add(new HuffmanCode("000101111", new[] {10, 3}));
			Table[15].Codes.Add(new HuffmanCode("0001011010", new[] {10, 4}));
			Table[15].Codes.Add(new HuffmanCode("0001010010", new[] {10, 5}));
			Table[15].Codes.Add(new HuffmanCode("0000111010", new[] {10, 6}));
			Table[15].Codes.Add(new HuffmanCode("0000111001", new[] {10, 7}));
			Table[15].Codes.Add(new HuffmanCode("0000110000", new[] {10, 8}));
			Table[15].Codes.Add(new HuffmanCode("00001001000", new[] {10, 9}));
			Table[15].Codes.Add(new HuffmanCode("00000111001", new[] {10, 10}));
			Table[15].Codes.Add(new HuffmanCode("00000101001", new[] {10, 11}));
			Table[15].Codes.Add(new HuffmanCode("00000010111", new[] {10, 12}));
			Table[15].Codes.Add(new HuffmanCode("000000011011", new[] {10, 13}));
			Table[15].Codes.Add(new HuffmanCode("0000000111110", new[] {10, 14}));
			Table[15].Codes.Add(new HuffmanCode("000000001001", new[] {10, 15}));
			Table[15].Codes.Add(new HuffmanCode("0001010110", new[] {11, 0}));
			Table[15].Codes.Add(new HuffmanCode("000101010", new[] {11, 1}));
			Table[15].Codes.Add(new HuffmanCode("000101000", new[] {11, 2}));
			Table[15].Codes.Add(new HuffmanCode("000100101", new[] {11, 3}));
			Table[15].Codes.Add(new HuffmanCode("0001000110", new[] {11, 4}));
			Table[15].Codes.Add(new HuffmanCode("0001000000", new[] {11, 5}));
			Table[15].Codes.Add(new HuffmanCode("0000110100", new[] {11, 6}));
			Table[15].Codes.Add(new HuffmanCode("0000101011", new[] {11, 7}));
			Table[15].Codes.Add(new HuffmanCode("00001000110", new[] {11, 8}));
			Table[15].Codes.Add(new HuffmanCode("00000110111", new[] {11, 9}));
			Table[15].Codes.Add(new HuffmanCode("00000101010", new[] {11, 10}));
			Table[15].Codes.Add(new HuffmanCode("00000011001", new[] {11, 11}));
			Table[15].Codes.Add(new HuffmanCode("000000011101", new[] {11, 12}));
			Table[15].Codes.Add(new HuffmanCode("000000010010", new[] {11, 13}));
			Table[15].Codes.Add(new HuffmanCode("000000001011", new[] {11, 14}));
			Table[15].Codes.Add(new HuffmanCode("0000000001011", new[] {11, 15}));
			Table[15].Codes.Add(new HuffmanCode("00001110110", new[] {12, 0}));
			Table[15].Codes.Add(new HuffmanCode("0001000100", new[] {12, 1}));
			Table[15].Codes.Add(new HuffmanCode("000011110", new[] {12, 2}));
			Table[15].Codes.Add(new HuffmanCode("0000110111", new[] {12, 3}));
			Table[15].Codes.Add(new HuffmanCode("0000110010", new[] {12, 4}));
			Table[15].Codes.Add(new HuffmanCode("0000101110", new[] {12, 5}));
			Table[15].Codes.Add(new HuffmanCode("00001001010", new[] {12, 6}));
			Table[15].Codes.Add(new HuffmanCode("00001000001", new[] {12, 7}));
			Table[15].Codes.Add(new HuffmanCode("00000110001", new[] {12, 8}));
			Table[15].Codes.Add(new HuffmanCode("00000100111", new[] {12, 9}));
			Table[15].Codes.Add(new HuffmanCode("00000011000", new[] {12, 10}));
			Table[15].Codes.Add(new HuffmanCode("00000010000", new[] {12, 11}));
			Table[15].Codes.Add(new HuffmanCode("000000010110", new[] {12, 12}));
			Table[15].Codes.Add(new HuffmanCode("000000001101", new[] {12, 13}));
			Table[15].Codes.Add(new HuffmanCode("0000000001110", new[] {12, 14}));
			Table[15].Codes.Add(new HuffmanCode("0000000000111", new[] {12, 15}));
			Table[15].Codes.Add(new HuffmanCode("00001011011", new[] {13, 0}));
			Table[15].Codes.Add(new HuffmanCode("0000101100", new[] {13, 1}));
			Table[15].Codes.Add(new HuffmanCode("0000100111", new[] {13, 2}));
			Table[15].Codes.Add(new HuffmanCode("0000100110", new[] {13, 3}));
			Table[15].Codes.Add(new HuffmanCode("0000100010", new[] {13, 4}));
			Table[15].Codes.Add(new HuffmanCode("00000111111", new[] {13, 5}));
			Table[15].Codes.Add(new HuffmanCode("00000110100", new[] {13, 6}));
			Table[15].Codes.Add(new HuffmanCode("00000101101", new[] {13, 7}));
			Table[15].Codes.Add(new HuffmanCode("00000011111", new[] {13, 8}));
			Table[15].Codes.Add(new HuffmanCode("000000110100", new[] {13, 9}));
			Table[15].Codes.Add(new HuffmanCode("000000011100", new[] {13, 10}));
			Table[15].Codes.Add(new HuffmanCode("000000010011", new[] {13, 11}));
			Table[15].Codes.Add(new HuffmanCode("000000001110", new[] {13, 12}));
			Table[15].Codes.Add(new HuffmanCode("000000001000", new[] {13, 13}));
			Table[15].Codes.Add(new HuffmanCode("0000000001001", new[] {13, 14}));
			Table[15].Codes.Add(new HuffmanCode("0000000000011", new[] {13, 15}));
			Table[15].Codes.Add(new HuffmanCode("000001111011", new[] {14, 0}));
			Table[15].Codes.Add(new HuffmanCode("00000111100", new[] {14, 1}));
			Table[15].Codes.Add(new HuffmanCode("00000111010", new[] {14, 2}));
			Table[15].Codes.Add(new HuffmanCode("00000110101", new[] {14, 3}));
			Table[15].Codes.Add(new HuffmanCode("00000101111", new[] {14, 4}));
			Table[15].Codes.Add(new HuffmanCode("00000101011", new[] {14, 5}));
			Table[15].Codes.Add(new HuffmanCode("00000100000", new[] {14, 6}));
			Table[15].Codes.Add(new HuffmanCode("00000010110", new[] {14, 7}));
			Table[15].Codes.Add(new HuffmanCode("000000100101", new[] {14, 8}));
			Table[15].Codes.Add(new HuffmanCode("000000011000", new[] {14, 9}));
			Table[15].Codes.Add(new HuffmanCode("000000010001", new[] {14, 10}));
			Table[15].Codes.Add(new HuffmanCode("000000001100", new[] {14, 11}));
			Table[15].Codes.Add(new HuffmanCode("0000000001111", new[] {14, 12}));
			Table[15].Codes.Add(new HuffmanCode("0000000001010", new[] {14, 13}));
			Table[15].Codes.Add(new HuffmanCode("000000000010", new[] {14, 14}));
			Table[15].Codes.Add(new HuffmanCode("0000000000001", new[] {14, 15}));
			Table[15].Codes.Add(new HuffmanCode("000001000111", new[] {15, 0}));
			Table[15].Codes.Add(new HuffmanCode("00000100101", new[] {15, 1}));
			Table[15].Codes.Add(new HuffmanCode("00000100010", new[] {15, 2}));
			Table[15].Codes.Add(new HuffmanCode("00000011110", new[] {15, 3}));
			Table[15].Codes.Add(new HuffmanCode("00000011100", new[] {15, 4}));
			Table[15].Codes.Add(new HuffmanCode("00000010100", new[] {15, 5}));
			Table[15].Codes.Add(new HuffmanCode("00000010001", new[] {15, 6}));
			Table[15].Codes.Add(new HuffmanCode("000000011010", new[] {15, 7}));
			Table[15].Codes.Add(new HuffmanCode("000000010101", new[] {15, 8}));
			Table[15].Codes.Add(new HuffmanCode("000000010000", new[] {15, 9}));
			Table[15].Codes.Add(new HuffmanCode("000000001010", new[] {15, 10}));
			Table[15].Codes.Add(new HuffmanCode("000000000110", new[] {15, 11}));
			Table[15].Codes.Add(new HuffmanCode("0000000001000", new[] {15, 12}));
			Table[15].Codes.Add(new HuffmanCode("0000000000110", new[] {15, 13}));
			Table[15].Codes.Add(new HuffmanCode("0000000000010", new[] {15, 14}));
			Table[15].Codes.Add(new HuffmanCode("0000000000000", new[] {15, 15}));

			ResetCounter(15);
			Table[16].Codes.Add(new HuffmanCode("1", new[] {0, 0}));
			Table[16].Codes.Add(new HuffmanCode("0101", new[] {0, 1}));
			Table[16].Codes.Add(new HuffmanCode("001110", new[] {0, 2}));
			Table[16].Codes.Add(new HuffmanCode("00101100", new[] {0, 3}));
			Table[16].Codes.Add(new HuffmanCode("001001010", new[] {0, 4}));
			Table[16].Codes.Add(new HuffmanCode("000111111", new[] {0, 5}));
			Table[16].Codes.Add(new HuffmanCode("0001101110", new[] {0, 6}));
			Table[16].Codes.Add(new HuffmanCode("0001011101", new[] {0, 7}));
			Table[16].Codes.Add(new HuffmanCode("00010101100", new[] {0, 8}));
			Table[16].Codes.Add(new HuffmanCode("00010010101", new[] {0, 9}));
			Table[16].Codes.Add(new HuffmanCode("00010001010", new[] {0, 10}));
			Table[16].Codes.Add(new HuffmanCode("000011110010", new[] {0, 11}));
			Table[16].Codes.Add(new HuffmanCode("000011100001", new[] {0, 12}));
			Table[16].Codes.Add(new HuffmanCode("000011000011", new[] {0, 13}));
			Table[16].Codes.Add(new HuffmanCode("0000101111000", new[] {0, 14}));
			Table[16].Codes.Add(new HuffmanCode("000010001", new[] {0, 15}));
			Table[16].Codes.Add(new HuffmanCode("011", new[] {1, 0}));
			Table[16].Codes.Add(new HuffmanCode("0100", new[] {1, 1}));
			Table[16].Codes.Add(new HuffmanCode("001100", new[] {1, 2}));
			Table[16].Codes.Add(new HuffmanCode("0010100", new[] {1, 3}));
			Table[16].Codes.Add(new HuffmanCode("00100011", new[] {1, 4}));
			Table[16].Codes.Add(new HuffmanCode("000111110", new[] {1, 5}));
			Table[16].Codes.Add(new HuffmanCode("000110101", new[] {1, 6}));
			Table[16].Codes.Add(new HuffmanCode("000101111", new[] {1, 7}));
			Table[16].Codes.Add(new HuffmanCode("0001010011", new[] {1, 8}));
			Table[16].Codes.Add(new HuffmanCode("0001001011", new[] {1, 9}));
			Table[16].Codes.Add(new HuffmanCode("0001000100", new[] {1, 10}));
			Table[16].Codes.Add(new HuffmanCode("00001110111", new[] {1, 11}));
			Table[16].Codes.Add(new HuffmanCode("000011001001", new[] {1, 12}));
			Table[16].Codes.Add(new HuffmanCode("00001101011", new[] {1, 13}));
			Table[16].Codes.Add(new HuffmanCode("000011001111", new[] {1, 14}));
			Table[16].Codes.Add(new HuffmanCode("00001001", new[] {1, 15}));
			Table[16].Codes.Add(new HuffmanCode("001111", new[] {2, 0}));
			Table[16].Codes.Add(new HuffmanCode("001101", new[] {2, 1}));
			Table[16].Codes.Add(new HuffmanCode("0010111", new[] {2, 2}));
			Table[16].Codes.Add(new HuffmanCode("00100110", new[] {2, 3}));
			Table[16].Codes.Add(new HuffmanCode("001000011", new[] {2, 4}));
			Table[16].Codes.Add(new HuffmanCode("000111010", new[] {2, 5}));
			Table[16].Codes.Add(new HuffmanCode("0001100111", new[] {2, 6}));
			Table[16].Codes.Add(new HuffmanCode("0001011010", new[] {2, 7}));
			Table[16].Codes.Add(new HuffmanCode("00010100001", new[] {2, 8}));
			Table[16].Codes.Add(new HuffmanCode("0001001000", new[] {2, 9}));
			Table[16].Codes.Add(new HuffmanCode("00001111111", new[] {2, 10}));
			Table[16].Codes.Add(new HuffmanCode("00001110101", new[] {2, 11}));
			Table[16].Codes.Add(new HuffmanCode("00001101110", new[] {2, 12}));
			Table[16].Codes.Add(new HuffmanCode("000011010001", new[] {2, 13}));
			Table[16].Codes.Add(new HuffmanCode("000011001110", new[] {2, 14}));
			Table[16].Codes.Add(new HuffmanCode("000010000", new[] {2, 15}));
			Table[16].Codes.Add(new HuffmanCode("00101101", new[] {3, 0}));
			Table[16].Codes.Add(new HuffmanCode("0010101", new[] {3, 1}));
			Table[16].Codes.Add(new HuffmanCode("00100111", new[] {3, 2}));
			Table[16].Codes.Add(new HuffmanCode("001000101", new[] {3, 3}));
			Table[16].Codes.Add(new HuffmanCode("001000000", new[] {3, 4}));
			Table[16].Codes.Add(new HuffmanCode("0001110010", new[] {3, 5}));
			Table[16].Codes.Add(new HuffmanCode("0001100011", new[] {3, 6}));
			Table[16].Codes.Add(new HuffmanCode("0001010111", new[] {3, 7}));
			Table[16].Codes.Add(new HuffmanCode("00010011110", new[] {3, 8}));
			Table[16].Codes.Add(new HuffmanCode("00010001100", new[] {3, 9}));
			Table[16].Codes.Add(new HuffmanCode("000011111100", new[] {3, 10}));
			Table[16].Codes.Add(new HuffmanCode("000011010100", new[] {3, 11}));
			Table[16].Codes.Add(new HuffmanCode("000011000111", new[] {3, 12}));
			Table[16].Codes.Add(new HuffmanCode("0000110000011", new[] {3, 13}));
			Table[16].Codes.Add(new HuffmanCode("0000101101101", new[] {3, 14}));
			Table[16].Codes.Add(new HuffmanCode("0000011010", new[] {3, 15}));
			Table[16].Codes.Add(new HuffmanCode("001001011", new[] {4, 0}));
			Table[16].Codes.Add(new HuffmanCode("00100100", new[] {4, 1}));
			Table[16].Codes.Add(new HuffmanCode("001000100", new[] {4, 2}));
			Table[16].Codes.Add(new HuffmanCode("001000001", new[] {4, 3}));
			Table[16].Codes.Add(new HuffmanCode("0001110011", new[] {4, 4}));
			Table[16].Codes.Add(new HuffmanCode("0001100101", new[] {4, 5}));
			Table[16].Codes.Add(new HuffmanCode("00010110011", new[] {4, 6}));
			Table[16].Codes.Add(new HuffmanCode("00010100100", new[] {4, 7}));
			Table[16].Codes.Add(new HuffmanCode("00010011011", new[] {4, 8}));
			Table[16].Codes.Add(new HuffmanCode("000100001000", new[] {4, 9}));
			Table[16].Codes.Add(new HuffmanCode("000011110110", new[] {4, 10}));
			Table[16].Codes.Add(new HuffmanCode("000011100010", new[] {4, 11}));
			Table[16].Codes.Add(new HuffmanCode("0000110001011", new[] {4, 12}));
			Table[16].Codes.Add(new HuffmanCode("0000101111110", new[] {4, 13}));
			Table[16].Codes.Add(new HuffmanCode("0000101101010", new[] {4, 14}));
			Table[16].Codes.Add(new HuffmanCode("000001001", new[] {4, 15}));
			Table[16].Codes.Add(new HuffmanCode("001000010", new[] {5, 0}));
			Table[16].Codes.Add(new HuffmanCode("00011110", new[] {5, 1}));
			Table[16].Codes.Add(new HuffmanCode("000111011", new[] {5, 2}));
			Table[16].Codes.Add(new HuffmanCode("000111000", new[] {5, 3}));
			Table[16].Codes.Add(new HuffmanCode("0001100110", new[] {5, 4}));
			Table[16].Codes.Add(new HuffmanCode("00010111001", new[] {5, 5}));
			Table[16].Codes.Add(new HuffmanCode("00010101101", new[] {5, 6}));
			Table[16].Codes.Add(new HuffmanCode("000100001001", new[] {5, 7}));
			Table[16].Codes.Add(new HuffmanCode("00010001110", new[] {5, 8}));
			Table[16].Codes.Add(new HuffmanCode("000011111101", new[] {5, 9}));
			Table[16].Codes.Add(new HuffmanCode("000011101000", new[] {5, 10}));
			Table[16].Codes.Add(new HuffmanCode("0000110010000", new[] {5, 11}));
			Table[16].Codes.Add(new HuffmanCode("0000110000100", new[] {5, 12}));
			Table[16].Codes.Add(new HuffmanCode("0000101111010", new[] {5, 13}));
			Table[16].Codes.Add(new HuffmanCode("00000110111101", new[] {5, 14}));
			Table[16].Codes.Add(new HuffmanCode("0000010000", new[] {5, 15}));
			Table[16].Codes.Add(new HuffmanCode("0001101111", new[] {6, 0}));
			Table[16].Codes.Add(new HuffmanCode("000110110", new[] {6, 1}));
			Table[16].Codes.Add(new HuffmanCode("000110100", new[] {6, 2}));
			Table[16].Codes.Add(new HuffmanCode("0001100100", new[] {6, 3}));
			Table[16].Codes.Add(new HuffmanCode("00010111000", new[] {6, 4}));
			Table[16].Codes.Add(new HuffmanCode("00010110010", new[] {6, 5}));
			Table[16].Codes.Add(new HuffmanCode("00010100000", new[] {6, 6}));
			Table[16].Codes.Add(new HuffmanCode("00010000101", new[] {6, 7}));
			Table[16].Codes.Add(new HuffmanCode("000100000001", new[] {6, 8}));
			Table[16].Codes.Add(new HuffmanCode("000011110100", new[] {6, 9}));
			Table[16].Codes.Add(new HuffmanCode("000011100100", new[] {6, 10}));
			Table[16].Codes.Add(new HuffmanCode("000011011001", new[] {6, 11}));
			Table[16].Codes.Add(new HuffmanCode("0000110000001", new[] {6, 12}));
			Table[16].Codes.Add(new HuffmanCode("0000101101110", new[] {6, 13}));
			Table[16].Codes.Add(new HuffmanCode("00001011001011", new[] {6, 14}));
			Table[16].Codes.Add(new HuffmanCode("0000001010", new[] {6, 15}));
			Table[16].Codes.Add(new HuffmanCode("0001100010", new[] {7, 0}));
			Table[16].Codes.Add(new HuffmanCode("000110000", new[] {7, 1}));
			Table[16].Codes.Add(new HuffmanCode("0001011011", new[] {7, 2}));
			Table[16].Codes.Add(new HuffmanCode("0001011000", new[] {7, 3}));
			Table[16].Codes.Add(new HuffmanCode("00010100101", new[] {7, 4}));
			Table[16].Codes.Add(new HuffmanCode("00010011101", new[] {7, 5}));
			Table[16].Codes.Add(new HuffmanCode("00010010100", new[] {7, 6}));
			Table[16].Codes.Add(new HuffmanCode("000100000101", new[] {7, 7}));
			Table[16].Codes.Add(new HuffmanCode("000011111000", new[] {7, 8}));
			Table[16].Codes.Add(new HuffmanCode("0000110010111", new[] {7, 9}));
			Table[16].Codes.Add(new HuffmanCode("0000110001101", new[] {7, 10}));
			Table[16].Codes.Add(new HuffmanCode("0000101110100", new[] {7, 11}));
			Table[16].Codes.Add(new HuffmanCode("0000101111100", new[] {7, 12}));
			Table[16].Codes.Add(new HuffmanCode("000001101111001", new[] {7, 13}));
			Table[16].Codes.Add(new HuffmanCode("000001101110100", new[] {7, 14}));
			Table[16].Codes.Add(new HuffmanCode("0000001000", new[] {7, 15}));
			Table[16].Codes.Add(new HuffmanCode("0001010101", new[] {8, 0}));
			Table[16].Codes.Add(new HuffmanCode("0001010100", new[] {8, 1}));
			Table[16].Codes.Add(new HuffmanCode("0001010001", new[] {8, 2}));
			Table[16].Codes.Add(new HuffmanCode("00010011111", new[] {8, 3}));
			Table[16].Codes.Add(new HuffmanCode("00010011100", new[] {8, 4}));
			Table[16].Codes.Add(new HuffmanCode("00010001111", new[] {8, 5}));
			Table[16].Codes.Add(new HuffmanCode("000100000100", new[] {8, 6}));
			Table[16].Codes.Add(new HuffmanCode("000011111001", new[] {8, 7}));
			Table[16].Codes.Add(new HuffmanCode("0000110101011", new[] {8, 8}));
			Table[16].Codes.Add(new HuffmanCode("0000110010001", new[] {8, 9}));
			Table[16].Codes.Add(new HuffmanCode("0000110001000", new[] {8, 10}));
			Table[16].Codes.Add(new HuffmanCode("0000101111111", new[] {8, 11}));
			Table[16].Codes.Add(new HuffmanCode("00001011010111", new[] {8, 12}));
			Table[16].Codes.Add(new HuffmanCode("00001011001001", new[] {8, 13}));
			Table[16].Codes.Add(new HuffmanCode("00001011000100", new[] {8, 14}));
			Table[16].Codes.Add(new HuffmanCode("0000000111", new[] {8, 15}));
			Table[16].Codes.Add(new HuffmanCode("00010011010", new[] {9, 0}));
			Table[16].Codes.Add(new HuffmanCode("0001001100", new[] {9, 1}));
			Table[16].Codes.Add(new HuffmanCode("0001001001", new[] {9, 2}));
			Table[16].Codes.Add(new HuffmanCode("00010001101", new[] {9, 3}));
			Table[16].Codes.Add(new HuffmanCode("00010000011", new[] {9, 4}));
			Table[16].Codes.Add(new HuffmanCode("000100000000", new[] {9, 5}));
			Table[16].Codes.Add(new HuffmanCode("000011110101", new[] {9, 6}));
			Table[16].Codes.Add(new HuffmanCode("0000110101010", new[] {9, 7}));
			Table[16].Codes.Add(new HuffmanCode("0000110010110", new[] {9, 8}));
			Table[16].Codes.Add(new HuffmanCode("0000110001010", new[] {9, 9}));
			Table[16].Codes.Add(new HuffmanCode("0000110000000", new[] {9, 10}));
			Table[16].Codes.Add(new HuffmanCode("00001011011111", new[] {9, 11}));
			Table[16].Codes.Add(new HuffmanCode("0000101100111", new[] {9, 12}));
			Table[16].Codes.Add(new HuffmanCode("00001011000110", new[] {9, 13}));
			Table[16].Codes.Add(new HuffmanCode("0000101100000", new[] {9, 14}));
			Table[16].Codes.Add(new HuffmanCode("00000001011", new[] {9, 15}));
			Table[16].Codes.Add(new HuffmanCode("00010001011", new[] {10, 0}));
			Table[16].Codes.Add(new HuffmanCode("00010000001", new[] {10, 1}));
			Table[16].Codes.Add(new HuffmanCode("0001000011", new[] {10, 2}));
			Table[16].Codes.Add(new HuffmanCode("00001111101", new[] {10, 3}));
			Table[16].Codes.Add(new HuffmanCode("000011110111", new[] {10, 4}));
			Table[16].Codes.Add(new HuffmanCode("000011101001", new[] {10, 5}));
			Table[16].Codes.Add(new HuffmanCode("000011100101", new[] {10, 6}));
			Table[16].Codes.Add(new HuffmanCode("000011011011", new[] {10, 7}));
			Table[16].Codes.Add(new HuffmanCode("0000110001001", new[] {10, 8}));
			Table[16].Codes.Add(new HuffmanCode("00001011100111", new[] {10, 9}));
			Table[16].Codes.Add(new HuffmanCode("00001011100001", new[] {10, 10}));
			Table[16].Codes.Add(new HuffmanCode("00001011010000", new[] {10, 11}));
			Table[16].Codes.Add(new HuffmanCode("000001101110101", new[] {10, 12}));
			Table[16].Codes.Add(new HuffmanCode("000001101110010", new[] {10, 13}));
			Table[16].Codes.Add(new HuffmanCode("00000110110111", new[] {10, 14}));
			Table[16].Codes.Add(new HuffmanCode("0000000100", new[] {10, 15}));
			Table[16].Codes.Add(new HuffmanCode("000011110011", new[] {11, 0}));
			Table[16].Codes.Add(new HuffmanCode("00001111000", new[] {11, 1}));
			Table[16].Codes.Add(new HuffmanCode("00001110110", new[] {11, 2}));
			Table[16].Codes.Add(new HuffmanCode("00001110011", new[] {11, 3}));
			Table[16].Codes.Add(new HuffmanCode("000011100011", new[] {11, 4}));
			Table[16].Codes.Add(new HuffmanCode("000011011111", new[] {11, 5}));
			Table[16].Codes.Add(new HuffmanCode("0000110001100", new[] {11, 6}));
			Table[16].Codes.Add(new HuffmanCode("00001011101010", new[] {11, 7}));
			Table[16].Codes.Add(new HuffmanCode("00001011100110", new[] {11, 8}));
			Table[16].Codes.Add(new HuffmanCode("00001011100000", new[] {11, 9}));
			Table[16].Codes.Add(new HuffmanCode("00001011010001", new[] {11, 10}));
			Table[16].Codes.Add(new HuffmanCode("00001011001000", new[] {11, 11}));
			Table[16].Codes.Add(new HuffmanCode("00001011000010", new[] {11, 12}));
			Table[16].Codes.Add(new HuffmanCode("0000011011111", new[] {11, 13}));
			Table[16].Codes.Add(new HuffmanCode("00000110110100", new[] {11, 14}));
			Table[16].Codes.Add(new HuffmanCode("00000000110", new[] {11, 15}));
			Table[16].Codes.Add(new HuffmanCode("000011001010", new[] {12, 0}));
			Table[16].Codes.Add(new HuffmanCode("000011100000", new[] {12, 1}));
			Table[16].Codes.Add(new HuffmanCode("000011011110", new[] {12, 2}));
			Table[16].Codes.Add(new HuffmanCode("000011011010", new[] {12, 3}));
			Table[16].Codes.Add(new HuffmanCode("000011011000", new[] {12, 4}));
			Table[16].Codes.Add(new HuffmanCode("0000110000101", new[] {12, 5}));
			Table[16].Codes.Add(new HuffmanCode("0000110000010", new[] {12, 6}));
			Table[16].Codes.Add(new HuffmanCode("0000101111101", new[] {12, 7}));
			Table[16].Codes.Add(new HuffmanCode("0000101101100", new[] {12, 8}));
			Table[16].Codes.Add(new HuffmanCode("000001101111000", new[] {12, 9}));
			Table[16].Codes.Add(new HuffmanCode("00000110111011", new[] {12, 10}));
			Table[16].Codes.Add(new HuffmanCode("00001011000011", new[] {12, 11}));
			Table[16].Codes.Add(new HuffmanCode("00000110111000", new[] {12, 12}));
			Table[16].Codes.Add(new HuffmanCode("00000110110101", new[] {12, 13}));
			Table[16].Codes.Add(new HuffmanCode("0000011011000000", new[] {12, 14}));
			Table[16].Codes.Add(new HuffmanCode("00000000100", new[] {12, 15}));
			Table[16].Codes.Add(new HuffmanCode("00001011101011", new[] {13, 0}));
			Table[16].Codes.Add(new HuffmanCode("000011010011", new[] {13, 1}));
			Table[16].Codes.Add(new HuffmanCode("000011010010", new[] {13, 2}));
			Table[16].Codes.Add(new HuffmanCode("000011010000", new[] {13, 3}));
			Table[16].Codes.Add(new HuffmanCode("0000101110010", new[] {13, 4}));
			Table[16].Codes.Add(new HuffmanCode("0000101111011", new[] {13, 5}));
			Table[16].Codes.Add(new HuffmanCode("00001011011110", new[] {13, 6}));
			Table[16].Codes.Add(new HuffmanCode("00001011010011", new[] {13, 7}));
			Table[16].Codes.Add(new HuffmanCode("00001011001010", new[] {13, 8}));
			Table[16].Codes.Add(new HuffmanCode("0000011011000111", new[] {13, 9}));
			Table[16].Codes.Add(new HuffmanCode("000001101110011", new[] {13, 10}));
			Table[16].Codes.Add(new HuffmanCode("000001101101101", new[] {13, 11}));
			Table[16].Codes.Add(new HuffmanCode("000001101101100", new[] {13, 12}));
			Table[16].Codes.Add(new HuffmanCode("00000110110000011", new[] {13, 13}));
			Table[16].Codes.Add(new HuffmanCode("000001101100001", new[] {13, 14}));
			Table[16].Codes.Add(new HuffmanCode("00000000010", new[] {13, 15}));
			Table[16].Codes.Add(new HuffmanCode("0000101111001", new[] {14, 0}));
			Table[16].Codes.Add(new HuffmanCode("0000101110001", new[] {14, 1}));
			Table[16].Codes.Add(new HuffmanCode("00001100110", new[] {14, 2}));
			Table[16].Codes.Add(new HuffmanCode("000010111011", new[] {14, 3}));
			Table[16].Codes.Add(new HuffmanCode("00001011010110", new[] {14, 4}));
			Table[16].Codes.Add(new HuffmanCode("00001011010010", new[] {14, 5}));
			Table[16].Codes.Add(new HuffmanCode("0000101100110", new[] {14, 6}));
			Table[16].Codes.Add(new HuffmanCode("00001011000111", new[] {14, 7}));
			Table[16].Codes.Add(new HuffmanCode("00001011000101", new[] {14, 8}));
			Table[16].Codes.Add(new HuffmanCode("000001101100010", new[] {14, 9}));
			Table[16].Codes.Add(new HuffmanCode("0000011011000110", new[] {14, 10}));
			Table[16].Codes.Add(new HuffmanCode("000001101100111", new[] {14, 11}));
			Table[16].Codes.Add(new HuffmanCode("00000110110000010", new[] {14, 12}));
			Table[16].Codes.Add(new HuffmanCode("000001101100110", new[] {14, 13}));
			Table[16].Codes.Add(new HuffmanCode("00000110110010", new[] {14, 14}));
			Table[16].Codes.Add(new HuffmanCode("00000000000", new[] {14, 15}));
			Table[16].Codes.Add(new HuffmanCode("000001100", new[] {15, 0}));
			Table[16].Codes.Add(new HuffmanCode("00001010", new[] {15, 1}));
			Table[16].Codes.Add(new HuffmanCode("00000111", new[] {15, 2}));
			Table[16].Codes.Add(new HuffmanCode("000001011", new[] {15, 3}));
			Table[16].Codes.Add(new HuffmanCode("000001010", new[] {15, 4}));
			Table[16].Codes.Add(new HuffmanCode("0000010001", new[] {15, 5}));
			Table[16].Codes.Add(new HuffmanCode("0000001011", new[] {15, 6}));
			Table[16].Codes.Add(new HuffmanCode("0000001001", new[] {15, 7}));
			Table[16].Codes.Add(new HuffmanCode("00000001101", new[] {15, 8}));
			Table[16].Codes.Add(new HuffmanCode("00000001100", new[] {15, 9}));
			Table[16].Codes.Add(new HuffmanCode("00000001010", new[] {15, 10}));
			Table[16].Codes.Add(new HuffmanCode("00000000111", new[] {15, 11}));
			Table[16].Codes.Add(new HuffmanCode("00000000101", new[] {15, 12}));
			Table[16].Codes.Add(new HuffmanCode("00000000011", new[] {15, 13}));
			Table[16].Codes.Add(new HuffmanCode("00000000001", new[] {15, 14}));
			Table[16].Codes.Add(new HuffmanCode("00000011", new[] {15, 15}));

			for (int index = 17; index <= 23; index++)
				Tree[index] = Tree[16];

			#region Table 24
			ResetCounter(15);
			Table[24].Codes.Add(new HuffmanCode("1111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("101110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1010000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10010010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("100000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011111000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0110110010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0110101010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01010011101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01010001101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01010001001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01001101101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000000101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010000001000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("001011000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("100110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10000010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011011000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101011001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("101111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("101001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1001010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1000100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10000000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011011101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101010100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000011101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0010010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1010001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("100111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1001011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011011100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101000101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100001111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0010000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10010011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1001000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1000101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101100000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101000011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000011100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("100000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("1000010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("10000001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101010101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011111001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01111001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101011011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101001010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000001000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0110110011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01110011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01101111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01101101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101100001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101001100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000010011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00010001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0110101011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011010000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011001001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010101001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101000000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100001100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000000010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00010000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101001111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("011000101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010101110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101001101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101000001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000001001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00001011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01010011100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010111000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010110011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010101111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101011000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101001011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000010010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00001010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01010001100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101011010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010101011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010101000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010100100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000000001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01010001000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0101000010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100111000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100110011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100001101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100000101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000000000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101100111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01001101100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100101000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100100000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100001010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("01000000011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101100101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("010000001001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100011000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100010010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100001011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100001000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0100000011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101111010", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101110100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101101000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101100110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101100100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000000", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00101011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0010100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0010011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0010001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0001001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0000110", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0000100", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000111", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000101", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000011", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("00000001", GetNextCounter()));
			Table[24].Codes.Add(new HuffmanCode("0011", GetNextCounter()));
			#endregion

			for (int index = 25; index <= 31; index++)
				Tree[index] = Tree[24];

			InitHuffmanBinaryTreeQ(TableQ[0], TreeQ[0]);
			InitHuffmanBinaryTreeQ(TableQ[1], TreeQ[1]);

			for (int index = 1; index <= 24; index++)
				InitHuffmanBinaryTree(Table[index], Tree[index]);
		}

		private int xCounter;
		private int yCounter;
		private int counterSize;

		private void ResetCounter(int size)
		{
			xCounter = 0;
			yCounter = 0;
			counterSize = size;
		}

		private int[] GetNextCounter()
		{
			int[] result = new[] {xCounter, yCounter};
			yCounter++;
			if (yCounter > counterSize)
			{
				yCounter = 0;
				xCounter++;
			}

			return result;
		}

		private void InitHuffmanBinaryTreeQ(HuffmanTable table, HuffmanNodeQ node)
		{
			var w = node;
			foreach (HuffmanCode hc in table.Codes)
			{
				int tmp;
				for (int i = 0; i < hc.Hcode.Length; i++)
				{
					tmp = int.Parse(hc.Hcode.Substring(i, 1));
					if (w.Nodes == null)
						w.Add();
					
					w = w.Nodes[tmp];
				}
				w.Data = hc.Data;
				// return to root node.
				w = node;
			}
		}

		private void InitHuffmanBinaryTree(HuffmanTable table, HuffmanNode node)
		{
			var w = node;
			foreach (HuffmanCode hc in table.Codes)
			{
				int tmp;
				for (int i = 0; i < hc.Hcode.Length; i++)
				{
					tmp = int.Parse(hc.Hcode.Substring(i, 1));
					if (w.Nodes == null)
						w.Add();
					
					w = w.Nodes[tmp];
				}
				w.Data = hc.Data;
				// return to root node.
				w = node;
			}
		}
	}
}
