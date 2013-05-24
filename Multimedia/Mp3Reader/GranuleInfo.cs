namespace Mp3Reader
{
	internal class GranuleInfo
	{
		private readonly int[,] sfComTable = new[,]
		{
			{ 0, 0, 0, 0, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4 },
			{ 0, 1, 2, 3, 0, 1, 2, 3, 1, 2, 3, 1, 2, 3, 2, 3 }
		};

		public readonly int[] TableSelect = new[] { 0, 0, 0 };
		public readonly int[] SubblockGain = new[] { 0, 0, 0 };
		public int Part23Length = 0;
		public int BigValues = 0;
		public int GlobalGain = 0;
		public int ScalefacCompress = 0;
		public int Region0Count = 0;
		public int Region1Count = 0;
		public int Count1TableSelect = 0;
		public int BlockType = 0;
		public bool WindowSwitchingFlag;
		public bool MixedBlockFlag;
		public bool PreFlag;
		public int ScalefacScale;

		public int[] Slen
		{
			get { return new[] { sfComTable[0, ScalefacCompress], sfComTable[1, ScalefacCompress] }; }
		}

		public bool IsMixedBlock
		{
			get { return BlockType == 2 && MixedBlockFlag; }
		}

		public bool IsLongBlock
		{
			get { return BlockType != 2; }
		}

		public bool IsShortBlock
		{
			get { return BlockType == 2 && MixedBlockFlag == false; }
		}
	}
}