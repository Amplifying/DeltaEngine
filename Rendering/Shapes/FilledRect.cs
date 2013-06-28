using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// A filled solid color rectangle to be rendered
	/// </summary>
	public class FilledRect : Polygon
	{
		public FilledRect(Rectangle drawArea, Color color)
			: base(drawArea, color)
		{
			UpdateCorners(0.0f);
		}

		private void UpdateCorners(float rotation)
		{
			var existingPoints = Points;
			existingPoints.Clear();
			existingPoints.AddRange(DrawArea.GetRotatedRectangleCorners(Center, rotation));
		}

		public override float Rotation
		{
			get { return GetWithDefault(0.0f); }
			set
			{
				Set(value);
				UpdateCorners(value);
			}
		}

		public override Point Center
		{
			get { return base.Center; }
			set
			{
				base.Center = value;
				UpdateCorners(Rotation);
			}
		}

		public override Rectangle DrawArea
		{
			get { return base.DrawArea; }
			set
			{
				base.DrawArea = value;
				UpdateCorners(Rotation);
			}
		}
	}
}