using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Sets up an Entity that can be used in line rendering
	/// </summary>
	public class Line2D : Entity2D
	{
		public Line2D(Point start, Point end, Color color)
			: base(new Rectangle(start, (Size)(end - start)), color)
		{
			Add(new List<Point> { start, end });
			Add<Render>();
		}

		public List<Point> Points
		{
			get { return Get<List<Point>>(); }
			set { Set(value); }
		}

		public Point Start
		{
			get { return Points[0]; }
			set { Points[0] = value; }
		}

		public Point End
		{
			get
			{
				var points = Points;
				return points[points.Count - 1];
			}
			set
			{
				var points = Points;
				points[points.Count - 1] = value;
			}
		}

		public void AddLine(Point start, Point end)
		{
			var points = Points;
			points.Add(start);
			points.Add(end);
		}

		public void ExtendLine(Point nextPoint)
		{
			var points = Points;
			points.Add(points[points.Count - 1]);
			points.Add(nextPoint);
		}

		/// <summary>
		/// Responsible for rendering all kinds of 2D lines (Line2D, Circle, etc)
		/// </summary>
		public class Render : EntityListener
		{
			public Render(Drawing draw, ScreenSpace screen)
			{
				this.draw = draw;
				this.screen = screen;
			}

			private readonly Drawing draw;
			private readonly ScreenSpace screen;

			public override void ReceiveMessage(Entity entity, object message)
			{
				if (message is SortAndRender.TimeToRender)
					RenderLine(entity);
			}

			private void RenderLine(Entity entity)
			{
				var color = entity.Get<Color>();
				var points = entity.Get<List<Point>>();
				var vertices = new VertexPositionColor[points.Count];
				for (int num = 0; num < points.Count; num++)
					vertices[num] = new VertexPositionColor(screen.ToPixelSpaceRounded(points[num]), color);

				draw.DisableTexturing();
				draw.DrawVertices(VerticesMode.Lines, vertices);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Last; }
			}
		}
	}
}