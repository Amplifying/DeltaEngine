using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace Blocks
{
	/// <summary>
	/// This represents a brick within a block
	/// </summary>
	public class Brick : Sprite
	{
		public Brick(Image image, Point offset, Constants.DisplayMode displayMode)
			: base(image, Rectangle.Zero)
		{
			Offset = offset;
			RenderLayer = (int)Blocks.RenderLayer.Grid;
			this.displayMode = displayMode;
		}

		public Point Offset;
		private readonly Constants.DisplayMode displayMode;

		public void UpdateDrawArea()
		{
			if (displayMode == Constants.DisplayMode.LandScape)
				DrawArea = new Rectangle(OffsetLandscape + (Position - Point.UnitY) * ZoomLandscape,
					SizeLandscape);
			else
				DrawArea = new Rectangle(OffsetPortrait + (Position - Point.UnitY) * ZoomPortrait,
					SizePortrait); 
		}

		public Point Position
		{
			get { return TopLeftGridCoord + Offset; }
		}

		public Point TopLeftGridCoord;
		public static readonly Point OffsetLandscape = new Point(0.38f, 0.385f);
		public static readonly Point OffsetPortrait = new Point(0.38f, 0.385f);
		public const float ZoomLandscape = 0.02f;
		public const float ZoomPortrait = 0.02f;
		private static readonly Size SizeLandscape = new Size(ZoomLandscape);
		private static readonly Size SizePortrait = new Size(ZoomPortrait);
	}
}