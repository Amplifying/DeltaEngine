namespace DeltaEngine.Graphics.OpenTK
{
	public struct CircularBuffer
	{
		public CircularBuffer(int handle, int numberOfChunks)
		{
			Handle = handle;
			NumberOfChunks = numberOfChunks;
			CurrentChunk = 0;
		}

		public int Handle;
		public int NumberOfChunks;
		public int CurrentChunk;
		//public int ChunkSize;

		public void SelectNextChunk()
		{
			//this is completely broken for just 1 draw call per frame
			CurrentChunk++;
			if (CurrentChunk == NumberOfChunks)
				CurrentChunk = 0;
		}
	}
}
