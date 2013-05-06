using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// A shape to be rendered defined by its border points
	/// </summary>
	public class Polygon : Entity2D
	{
		public Polygon()
			: this(Color.White) {}

		public Polygon(Color color)
			: base(Rectangle.Zero, color)
		{
			Add(new List<Point>());
			Add(new OutlineColor(color));
			Add<RenderPolygon>();
			Add<RenderPolygonOutline>(); //TODO: why render both the outline and the filled shape?
		}

		public Color OutlineColor
		{
			get { return Get<OutlineColor>().Value; }
			set { Set(new OutlineColor(value)); }
		}

		public List<Point> Points
		{
			get { return Get<List<Point>>(); }
			set { Set(value); }
		}
	}
}