using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// This represents a brick within a block
	/// </summary>
	public class Brick : Sprite
	{
		public Brick(Image image, Point offset)
			: base(image, Rectangle.Zero)
		{
			Offset = offset;
			RenderLayer = (int)Blocks.RenderLayer.Grid;
		}

		public Point Offset;

		internal void Render(Renderer renderer)
		{
			Render(renderer, null);
		}

		protected override void Render(Renderer renderer, Time time)
		{
			if (renderer.Screen.Viewport.Aspect >= 1.0f)
				DrawArea = new Rectangle(OffsetLandscape + (Position - Point.UnitY) * ZoomLandscape,
					SizeLandscape);
			else
				DrawArea = new Rectangle(OffsetPortrait + (Position - Point.UnitY) * ZoomPortrait,
					SizePortrait);

			base.Render(renderer, time);
		}

		public Point Position
		{
			get { return TopLeft + Offset; }
		}

		public Point TopLeft;
		public static readonly Point OffsetLandscape = new Point(0.38f, 0.385f);
		public static readonly Point OffsetPortrait = new Point(0.38f, 0.385f);
		public const float ZoomLandscape = 0.02f;
		public const float ZoomPortrait = 0.02f;
		private static readonly Size SizeLandscape = new Size(ZoomLandscape);
		private static readonly Size SizePortrait = new Size(ZoomPortrait);
	}
}