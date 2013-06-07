using System.Collections.Generic;
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
			public override void Handle(Entity entity)
			{
				var drawArea = entity.Get<Rectangle>();
				var existingPoints = entity.Get<List<Point>>();
				existingPoints.Clear();
				var rotation = entity.Get<float>();
				existingPoints.AddRange(drawArea.GetRotatedRectangleCorners(drawArea.Center, rotation));
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}
	}
}