using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Converts to and from quadratic space. Must be created whenever the viewport size changes.
	/// </summary>
	public class ScreenSpace
	{
		public ScreenSpace(Window window)
		{
			Update(window.ViewportPixelSize);
			window.ViewportSizeChanged += Update;
			window.OrientationChanged += orientation => Update(window.TotalPixelSize);
		}
		
		private void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			aspectRatio = newViewportSize.Width / newViewportSize.Height;
			quadraticToPixelScale = CalculateToPixelScale();
			quadraticToPixelOffset = CalculateToPixelOffset();
			pixelToQuadraticScale = CalculateToQuadraticScale();
			pixelToQuadraticOffset = CalculateToQuadraticOffset();
			if (ViewportSizeChanged != null)
				ViewportSizeChanged();
		}

		public event Action ViewportSizeChanged;

		private Size viewportPixelSize;
		private float aspectRatio;
		private Size quadraticToPixelScale;
		private Point quadraticToPixelOffset;
		private Size pixelToQuadraticScale;
		private Point pixelToQuadraticOffset;

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
			if (aspectRatio < 1.0f)
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
			var pixelPos =
				new Point(quadraticToPixelScale.Width * quadraticPos.X + quadraticToPixelOffset.X,
					quadraticToPixelScale.Height * quadraticPos.Y + quadraticToPixelOffset.Y);
			return new Point((float)Math.Round(pixelPos.X, 2), (float)Math.Round(pixelPos.Y, 2));
		}

		/// <summary>
		/// The rounded version of ToPixelSpace is used for lines, boxes and fonts where it matters to
		/// actually render at exact pixel positions to get sharp lines, boxes or font rendering.
		/// </summary>
		public Point ToPixelSpaceRounded(Point quadraticPos)
		{
			var pixelPos =
				new Point(quadraticToPixelScale.Width * quadraticPos.X + quadraticToPixelOffset.X,
					quadraticToPixelScale.Height * quadraticPos.Y + quadraticToPixelOffset.Y);
			return new Point((float)Math.Round(pixelPos.X + Epsilon),
				(float)Math.Round(pixelPos.Y + Epsilon));
		}

		/// <summary>
		/// Small value to make sure we always round up in ToPixelSpaceRounded for 0.5f or 0.499999f.
		/// </summary>
		private const float Epsilon = 0.001f;

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

		public float Left
		{
			get { return pixelToQuadraticOffset.X; }
		}

		public float Top
		{
			get { return pixelToQuadraticOffset.Y; }
		}

		public float Right
		{
			get { return 1 - pixelToQuadraticOffset.X; }
		}

		public float Bottom
		{
			get { return 1 - pixelToQuadraticOffset.Y; }
		}

		public Size Area
		{
			get { return new Size(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y); }
		}

		public Rectangle Viewport
		{
			get { return new Rectangle(TopLeft, Area); }
		}

		public Point GetInnerPoint(float x, float y)
		{
			return GetInnerPoint(new Point(x, y));
		}

		public Point GetInnerPoint(Point relativePoint)
		{
			return new Point(Left + Area.Width * relativePoint.X, Top + Area.Height * relativePoint.Y);
		}

		public Size ViewportPixelSize
		{
			get { return viewportPixelSize; }
		}
	}
}