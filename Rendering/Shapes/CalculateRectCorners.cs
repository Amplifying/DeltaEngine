using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Updates the corners of the Rect based on its DrawArea
	/// </summary>
	public class CalculateRectCorners : EntityHandler
	{
		public void Handle(List<Entity> entities)
		{
			foreach (Entity2D entity in
				entities.OfType<Entity2D>().Where(e => e.Contains<List<Point>>() && HasChanged(e)))
				UpdateCorners(entity);
		}

		private static bool HasChanged(Entity2D entity)
		{
			return true;
		}

		private void UpdateCorners(Entity2D entity)
		{
			var points = entity.Get<List<Point>>();
			points.Clear();
			Rectangle drawArea = entity.DrawArea;
			points.Add(drawArea.TopLeft);
			points.Add(drawArea.TopRight);
			points.Add(drawArea.BottomRight);
			points.Add(drawArea.BottomLeft);
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}