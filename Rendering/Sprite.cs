using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Sprites are 2D quads rendered automatically on the specified location, etc.
	/// </summary>
	public class Sprite : Renderable
	{
		public Sprite(Image image, Rectangle initalDrawArea, Color color)
		{
			this.image = image;
			DrawArea = initalDrawArea;
			Color = color;
		}

		public Sprite(Image image, Rectangle initalDrawArea)
			: this(image, initalDrawArea, Color.White) {}

		protected Image image;
		public Rectangle DrawArea;
		public Color Color;
		public float Rotation;
		public FlipMode Flip = FlipMode.None;
		public Point RotationCenter = RotateAroundCenter;
		private static readonly Point RotateAroundCenter = new Point(-1, -1);

		protected override void Render(Renderer renderer, Time time)
		{
			screen = renderer.Screen;
			if (Rotation == 0.0f)
				DrawImage();
			else
				DrawImageWithRotation();
		}

		private ScreenSpace screen;

		private void DrawImage()
		{
			var vertices = new[]
			{
				GetVertex(DrawArea.TopLeft, Point.Zero),
				GetVertex(DrawArea.TopRight, Point.UnitX),
				GetVertex(DrawArea.BottomRight, Point.One),
				GetVertex(DrawArea.BottomLeft, Point.UnitY)
			};
			image.Draw(vertices);
		}

		private VertexPositionColorTextured GetVertex(Point position, Point uv)
		{
			if (Flip == FlipMode.Horizontal)
				uv.X = 1.0f - uv.X;
			if (Flip == FlipMode.Vertical)
				uv.Y = 1.0f - uv.Y;
			return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position), Color, uv);
		}

		private void DrawImageWithRotation()
		{
			var vertices = new[]
			{
				GetVertex(Rotate(DrawArea.TopLeft), Point.Zero),
				GetVertex(Rotate(DrawArea.TopRight), Point.UnitX),
				GetVertex(Rotate(DrawArea.BottomRight), Point.One),
				GetVertex(Rotate(DrawArea.BottomLeft), Point.UnitY)
			};
			image.Draw(vertices);
		}
		
		private Point Rotate(Point point)
		{
			var rotationCenter = RotationCenter == RotateAroundCenter ? DrawArea.Center : RotationCenter;
			point -= rotationCenter;
			float sin = MathExtensions.Sin(Rotation);
			float cos = MathExtensions.Cos(Rotation);
			point = new Point(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
			return rotationCenter + point;
		}
	}
}