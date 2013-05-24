using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// A rectangle to be rendered
	/// </summary>
	public class Rect : Polygon
	{
		public Rect()
			: this(Rectangle.Zero, Color.White) {}

		public Rect(Rectangle drawArea, Color color)
			: base(color)
		{
			DrawArea = drawArea;
			Add<UpdateCorners>();
		}

		/// <summary>
		/// Updates the corners of the Rect based on its DrawArea
		/// </summary>
		public class UpdateCorners : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (Entity2D entity in
					entities.OfType<Entity2D>().Where(e => e.Contains<List<Point>>() && HasChanged(e)))
					UpdateRectCorners(entity);
			}

			private static bool HasChanged(Entity2D entity)
			{
				return true;
			}

			private static void UpdateRectCorners(Entity2D entity)
			{
				var points = entity.Get<List<Point>>();
				points.Clear();
				Rectangle drawArea = entity.DrawArea;
				points.Add(drawArea.TopLeft);
				points.Add(drawArea.TopRight);
				points.Add(drawArea.BottomRight);
				points.Add(drawArea.BottomLeft);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}
	}
}