using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// This recalculates the points of an Ellipse if they change
	/// </summary>
	public class CalculateEllipsePoints : EntityHandler
	{
		public void Handle(List<Entity> entities)
		{
			foreach (var entity in entities.Where(HasChanged))
				UpdateEllipsePoints(entity);
		}

		private static bool HasChanged(Entity entity)
		{
			return true;
		}

		private void UpdateEllipsePoints(Entity entity)
		{
			InitializeVariables(entity);
			FormEllipsePoints(entity);
		}

		private void InitializeVariables(Entity entity)
		{
			float rotation = entity.Get<Rotation>().Value;
			rotationSin = MathExtensions.Sin(rotation);
			rotationCos = MathExtensions.Cos(rotation);
			var drawArea = entity.Get<Rectangle>();
			center = drawArea.Center;
			radiusX = drawArea.Width / 2.0f;
			radiusY = drawArea.Height / 2.0f;
			float maxRadius = MathExtensions.Max(radiusX, radiusY);
			pointsCount = GetPointsCount(maxRadius);
			theta = 360.0f / (pointsCount - 1);
		}

		private float rotationSin;
		private float rotationCos;
		private Point center;
		private float radiusX;
		private float radiusY;
		private int pointsCount;
		private float theta;

		private static int GetPointsCount(float maxRadius)
		{
			var pointsCount = (int)(MaxPoints * MathExtensions.Max(0.22f + maxRadius / 2, maxRadius));
			return MathExtensions.Max(pointsCount, MinPoints);
		}

		private const int MinPoints = 5;
		private const int MaxPoints = 96;

		private void FormEllipsePoints(Entity entity)
		{
			ellipsePoints = entity.Get<List<Point>>();
			ellipsePoints.Clear();
			for (int i = 0; i < pointsCount; i++)
				FormRotatedEllipsePoint(i);

			entity.Set(ellipsePoints);
		}

		private List<Point> ellipsePoints;

		private void FormRotatedEllipsePoint(int i)
		{
			var ellipsePoint = new Point(radiusX * MathExtensions.Sin(i * theta),
				radiusY * MathExtensions.Cos(i * theta));
			ellipsePoint.RotateAround(Point.Zero, rotationSin, rotationCos);
			ellipsePoints.Add(center + ellipsePoint);
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}