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
		public Sprite(Image image, Rectangle initialDrawArea)
			: this(image, initialDrawArea, Color.White) { }

		public Sprite(Image image, Rectangle initialDrawArea, Color color)
		{
			Image = image;
			DrawArea = initialDrawArea;
			Color = color;
		}

		public Image Image { get; private set; }
		public Rectangle DrawArea;
		public Color Color;

		public void SetImage(Image image)
		{
			Image = image;
		}

		protected override void Render(Renderer renderer, Time time)
		{
			screen = renderer.Screen;
			if (Rotation == 0.0f)
				DrawImage();
			else
				DrawImageWithRotation();
		}

		public float Rotation;
		protected ScreenSpace screen;

		private void DrawImage()
		{
			var vertices = new[]
			{
				GetVertex(DrawArea.TopLeft, Point.Zero), 
				GetVertex(DrawArea.TopRight, Point.UnitX),
				GetVertex(DrawArea.BottomRight, Point.One),
				GetVertex(DrawArea.BottomLeft, Point.UnitY)
			};
			Image.Draw(vertices);
		}

		protected VertexPositionColorTextured GetVertex(Point position, Point uv)
		{
			if (Flip == FlipMode.Horizontal)
				uv.X = 1.0f - uv.X;
			if (Flip == FlipMode.Vertical)
				uv.Y = 1.0f - uv.Y;
			return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position), Color, uv);
		}

		public FlipMode Flip = FlipMode.None;

		private void DrawImageWithRotation()
		{
			var vertices = new[]
			{
				GetVertex(Rotate(DrawArea.TopLeft), Point.Zero),
				GetVertex(Rotate(DrawArea.TopRight), Point.UnitX),
				GetVertex(Rotate(DrawArea.BottomRight), Point.One),
				GetVertex(Rotate(DrawArea.BottomLeft), Point.UnitY)
			};
			Image.Draw(vertices);
		}

		protected Point Rotate(Point point)
		{
			var rotationCenter = RotationCenter == RotateAroundCenter ? DrawArea.Center : RotationCenter;
			point -= rotationCenter;
			float sin = MathExtensions.Sin(Rotation);
			float cos = MathExtensions.Cos(Rotation);
			point = new Point(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
			return rotationCenter + point;
		}

		public Point RotationCenter = RotateAroundCenter;
		private static readonly Point RotateAroundCenter = new Point(-1, -1);
	}
}