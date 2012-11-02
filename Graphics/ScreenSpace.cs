using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Converts to and from quadratic space. Must be created whenever the viewport size changes.
	/// </summary>
	public class ScreenSpace
	{
		public ScreenSpace(Size viewportPixelSize)
		{
			this.viewportPixelSize = viewportPixelSize;
			aspectRatio = viewportPixelSize.Width / viewportPixelSize.Height;
			quadraticToPixelScale = CalculateToPixelScale();
			quadraticToPixelOffset = CalculateToPixelOffset();
			pixelToQuadraticScale = CalculateToQuadraticScale();
			pixelToQuadraticOffset = CalculateToQuadraticOffset();
		}

		private readonly Size viewportPixelSize;
		private readonly float aspectRatio;
		private readonly Size quadraticToPixelScale;
		private readonly Point quadraticToPixelOffset;
		private readonly Size pixelToQuadraticScale;
		private readonly Point pixelToQuadraticOffset;

		private Size CalculateToPixelScale()
		{
			Size scale = viewportPixelSize;
			if (aspectRatio < 1f)
				scale.Width *= 1f / aspectRatio;
			else if (aspectRatio > 1f)
				scale.Height *= aspectRatio;
			return scale;
		}

		private Point CalculateToPixelOffset()
		{
			Point offset = Point.Zero;
			if (aspectRatio < 1f)
				offset.X = (viewportPixelSize.Width - quadraticToPixelScale.Width) * 0.5f;
			else
				offset.Y = (viewportPixelSize.Height - quadraticToPixelScale.Height) * 0.5f;
			return offset;
		}

		private Size CalculateToQuadraticScale()
		{
			return 1f / quadraticToPixelScale;
		}

		private Point CalculateToQuadraticOffset()
		{
			return new Point(-quadraticToPixelOffset.X / quadraticToPixelScale.Width,
				-quadraticToPixelOffset.Y / quadraticToPixelScale.Height);
		}

		public Point ToQuadraticSpace(Point pixelPosition)
		{
			var scaledPixelPosition = new Point(pixelToQuadraticScale.Width * pixelPosition.X,
				pixelToQuadraticScale.Height * pixelPosition.Y);
			return scaledPixelPosition + pixelToQuadraticOffset;
		}

		public Size ToQuadraticSpace(Size pixelSize)
		{
			return pixelToQuadraticScale * pixelSize;
		}

		public Rectangle ToQuadraticSpace(Rectangle quadraticRect)
		{
			return new Rectangle(ToQuadraticSpace(quadraticRect.TopLeft),
				ToQuadraticSpace(quadraticRect.Size));
		}

		public Point ToPixelSpace(Point quadraticPos)
		{
			var pixelPos = new Point(
				quadraticToPixelScale.Width * quadraticPos.X + quadraticToPixelOffset.X,
				quadraticToPixelScale.Height * quadraticPos.Y + quadraticToPixelOffset.Y);
			return new Point((float)Math.Round(pixelPos.X, 2), (float)Math.Round(pixelPos.Y, 2));
		}

		public Size ToPixelSpace(Size quadraticSize)
		{
			return quadraticToPixelScale * quadraticSize;
		}

		public Rectangle ToPixelSpace(Rectangle quadraticRect)
		{
			return new Rectangle(ToPixelSpace(quadraticRect.TopLeft), ToPixelSpace(quadraticRect.Size));
		}

		public Point TopLeft
		{
			get { return pixelToQuadraticOffset; }
		}

		public Point BottomRight
		{
			get { return new Point(1 - pixelToQuadraticOffset.X, 1 - pixelToQuadraticOffset.Y); }
		}

		public Size Area
		{
			get { return new Size(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y); }
		}

		public Rectangle Viewport
		{
			get { return new Rectangle(TopLeft, Area); }
		}
	}
}