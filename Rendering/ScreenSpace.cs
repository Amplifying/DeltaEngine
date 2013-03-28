using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Converts to and from some kind of screen space like Quadratic, Pixel, etc.
	/// </summary>
	public abstract class ScreenSpace
	{
		protected ScreenSpace(Window window)
		{
			Window = window;
			viewportPixelSize = window.ViewportPixelSize;
			window.ViewportSizeChanged += Update;
			window.OrientationChanged += orientation => Update(window.ViewportPixelSize);
		}

		public Window Window { get; private set; }
		protected Size viewportPixelSize;

		protected virtual void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			if (ViewportSizeChanged != null)
				ViewportSizeChanged();
		}

		public event Action ViewportSizeChanged;

		/// <summary>
		/// The rounded version of ToPixelSpace is used for lines, boxes and fonts where it matters to
		/// actually render at exact pixel positions to get sharp lines, boxes or font rendering.
		/// </summary>
		public Point ToPixelSpaceRounded(Point quadraticPos)
		{
			Point pixelPos = ToPixelSpace(quadraticPos);
			return new Point((float)Math.Round(pixelPos.X + Epsilon),
				(float)Math.Round(pixelPos.Y + Epsilon));
		}

		/// <summary>
		/// Small value to make sure we always round up in ToPixelSpaceRounded for 0.5f or 0.499999f.
		/// </summary>
		private const float Epsilon = 0.001f;

		public abstract Point ToPixelSpace(Point currentScreenSpacePos);

		public abstract Size ToPixelSpace(Size currentScreenSpaceSize);

		public Rectangle ToPixelSpace(Rectangle quadraticRect)
		{
			return new Rectangle(ToPixelSpace(quadraticRect.TopLeft), ToPixelSpace(quadraticRect.Size));
		}

		public abstract Point FromPixelSpace(Point pixelPosition);

		public abstract Size FromPixelSpace(Size pixelSize);

		public Rectangle FromPixelSpace(Rectangle quadraticRect)
		{
			return new Rectangle(FromPixelSpace(quadraticRect.TopLeft),
				FromPixelSpace(quadraticRect.Size));
		}

		public abstract Point TopLeft { get; }

		public abstract Point BottomRight { get; }

		public abstract float Left { get; }

		public abstract float Top { get; }

		public abstract float Right { get; }

		public abstract float Bottom { get; }

		protected abstract Size GetSize { get; }

		public Rectangle Viewport
		{
			get { return new Rectangle(TopLeft, GetSize); }
		}

		public abstract Point GetInnerPoint(Point relativePoint);
	}
}