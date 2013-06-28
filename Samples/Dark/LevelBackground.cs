using System.Drawing;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Sprites;
using Color = System.Drawing.Color;
using Image = DeltaEngine.Graphics.Image;
using Point = DeltaEngine.Datatypes.Point;
using Rectangle = DeltaEngine.Datatypes.Rectangle;

namespace Dark
{
	public class LevelBackground : Sprite
	{
		public LevelBackground(Image image, Rectangle drawArea)
			: base(image, drawArea)
		{
			Center = Point.Zero;
			RenderLayer = -512;
			//bitmap = new Bitmap(@".\..\..\Content\AsylumFull.png");
		}

		private readonly Bitmap bitmap;

		public Color GetBitmapPixel(Vector worldPosition)
		{
			var pixelPosition = Coordinates.WorldToPixel(worldPosition);
			return bitmap.GetPixel((int)pixelPosition.X, (int)pixelPosition.Y);
		}
	}
}