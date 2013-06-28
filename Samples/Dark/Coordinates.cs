using DeltaEngine.Datatypes;

namespace Dark
{
	public static class Coordinates
	{
		public static Point WorldToPixel(Vector worldPosition)
		{
			return new Point((worldPosition.X * PixelScale) + PixelOffsetX,
				(worldPosition.Y * PixelScale) + PixelOffsetY);
		}

		private const float PixelScale = 500.0f;
		private const int PixelOffsetX = 2041;
		private const int PixelOffsetY = 1352;

		public static Vector PixelToWorld(Point pixelPosition)
		{
			return new Vector((pixelPosition.X - PixelOffsetX) * WorldScale,
				(pixelPosition.Y - PixelOffsetY) * WorldScale, 0.0f);
		}

		private const float WorldScale = 1.0f / PixelScale;
	}
}