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
			RenderLayer = (byte)Blocks.RenderLayer.Grid;
		}

		public Point Offset;

		internal void Render(Renderer renderer)
		{
			Render(renderer, null);
		}

		protected override void Render(Renderer renderer, Time time)
		{
			DrawArea = new Rectangle(RenderOffset + (Position - Point.UnitY) * RenderZoom, Size);
			base.Render(renderer, time);
		}

		public Point Position
		{
			get { return TopLeft + Offset; }
		}

		public Point TopLeft;
		public static readonly Point RenderOffset = new Point(0.2f, 0.3f);
		public static readonly float RenderZoom = 0.02f;
		public static readonly Size Size = new Size(RenderZoom);
	}
}