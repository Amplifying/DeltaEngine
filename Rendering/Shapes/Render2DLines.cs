using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Responsible for rendering all kinds of 2D lines (Line2D, Circle, etc)
	/// </summary>
	public class Render2DLines : EntityListener
	{
		public Render2DLines(Drawing draw, ScreenSpace screen)
		{
			this.draw = draw;
			this.screen = screen;
		}

		private readonly Drawing draw;
		private readonly ScreenSpace screen;

		public void Handle(List<Entity> entities) { }

		public void ReceiveMessage(Entity entity, object message)
		{
			if (message is SortAndRenderEntity2D.TimeToRender)
				Render(entity);
		}

		private void Render(Entity entity)
		{
			var color = entity.Get<Color>();
			var points = entity.Get<List<Point>>();
			var vertices = new VertexPositionColor[points.Count];
			for (int num = 0; num < points.Count; num++)
				vertices[num] = new VertexPositionColor(screen.ToPixelSpaceRounded(points[num]), color);
			
			draw.DrawVertices(VerticesMode.Lines, vertices);
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.Last; }
		}
	}
}