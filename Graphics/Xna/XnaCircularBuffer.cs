using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	public struct CircularBuffer
	{
		public CircularBuffer(int numberOfChunks)
		{
			Handle = null;
			NumberOfChunks = numberOfChunks;
			CurrentChunk = 0;
		}

		public DynamicVertexBuffer Handle;
		public int NumberOfChunks;
		public int CurrentChunk;

		public void SelectNextChunk()
		{
			CurrentChunk++;
			if (CurrentChunk == NumberOfChunks)
				CurrentChunk = 0;
		}
	}
}
