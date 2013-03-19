using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Behaves like QuadraticScreenSpace but can also pan and zoom
	/// </summary>
	public class Camera2DControlledQuadraticScreenSpace : QuadraticScreenSpace
	{
		public Camera2DControlledQuadraticScreenSpace(Window window)
			: base(window) { }

		public override Point ToPixelSpace(Point currentScreenSpacePos)
		{
			return base.ToPixelSpace(Transform(currentScreenSpacePos));
		}

		public Point Transform(Point position)
		{
			return (position - LookAt) * Zoom + Point.Half;
		}

		public Point LookAt = Point.Half;

		public float Zoom
		{
			get { return zoom; }
			set
			{
				zoom = value;
				inverseZoom = 1.0f / zoom;
			}
		}

		private float zoom = 1.0f;
		private float inverseZoom = 1.0f;

		public override Size ToPixelSpace(Size currentScreenSpaceSize)
		{
			return base.ToPixelSpace(currentScreenSpaceSize) * Zoom;
		}

		public override Point FromPixelSpace(Point pixelPosition)
		{
			return InverseTransform(base.FromPixelSpace(pixelPosition));
		}

		public Point InverseTransform(Point position)
		{
			return (position - Point.Half) * inverseZoom + LookAt;
		}

		public override Size FromPixelSpace(Size pixelSize)
		{
			return base.FromPixelSpace(pixelSize) * inverseZoom;
		}

		public override Point TopLeft
		{
			get { return InverseTransform(base.TopLeft); }
		}

		public override Point BottomRight
		{
			get { return InverseTransform(base.BottomRight); }
		}

		public override float Left
		{
			get { return InverseTransformX(base.Left); }
		}

		private float InverseTransformX(float x)
		{
			return (x - 0.5f) * inverseZoom + LookAt.X;
		}

		public override float Top
		{
			get { return InverseTransformY(base.Top); }
		}

		private float InverseTransformY(float y)
		{
			return (y - 0.5f) * inverseZoom + LookAt.Y;
		}

		public override float Right
		{
			get { return InverseTransformX(base.Right); }
		}

		public override float Bottom
		{
			get { return InverseTransformY(base.Bottom); }
		}
	}
}