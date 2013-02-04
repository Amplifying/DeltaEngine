using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Facilitates the rendering of shapes, meshes or images on screen. Uses drawings, which needs a
	/// graphic device. Most importantly this class does sort and optimize all rendering each frame.
	/// </summary>
	public class Renderer : Runner<Time>
	{
		public Renderer(Drawing draw, ScreenSpace screen)
		{
			this.draw = draw;
			this.screen = screen;
		}

		private readonly Drawing draw;
		private readonly ScreenSpace screen;

		public ScreenSpace Screen
		{
			get { return screen; }
		}

		public void Run(Time time)
		{
			List<Renderable> objectsToRender = new List<Renderable>();
			foreach (var renderObject in visibles)
				if (renderObject.Value.IsVisible)
					objectsToRender.Add(renderObject.Value);

			foreach (var renderObject in objectsToRender)
				renderObject.InternalRender(this, time);

			RemoveDisposedObjects();
		}

		private void RemoveDisposedObjects()
		{
			List<Renderable> objectsToRemove = new List<Renderable>();
			foreach (var renderObject in visibles)
				if (renderObject.Value.markForDisposal)
					objectsToRemove.Add(renderObject.Value);

			foreach (var renderObject in objectsToRemove)
				Remove(renderObject);
		}

		private readonly SortedList<int, Renderable> visibles = new SortedList<int, Renderable>();

		public int NumberOfActiveRenderableObjects
		{
			get { return visibles.Count; }
		}

		public void Add(Renderable renderable)
		{
			if (visibles.ContainsValue(renderable))
				return;

			renderable.markForDisposal = false;
			visibles.Add(renderable.SortKey, renderable);
		}

		public void Remove(Renderable renderable)
		{
			foreach (var pair in visibles)
				if (pair.Value == renderable)
				{
					visibles.Remove(pair.Key);
					break;
				}
		}

		public void RemoveAll()
		{
			visibles.Clear();
		}

		public void DrawLine(Point startPosition, Point endPosition, Color color)
		{
			draw.DisableTexturing();
			draw.DrawVertices(VerticesMode.Lines,
				new[]
				{
					new VertexPositionColor(screen.ToPixelSpaceRounded(startPosition), color),
					new VertexPositionColor(screen.ToPixelSpaceRounded(endPosition), color)
				});
		}

		public void DrawRectangle(Rectangle rect, Color color)
		{
			draw.DisableTexturing();
			draw.SetIndices(QuadIndices, QuadIndices.Length);
			var vertices = new[]
			{
				new VertexPositionColor(screen.ToPixelSpaceRounded(rect.TopLeft), color),
				new VertexPositionColor(screen.ToPixelSpaceRounded(rect.TopRight), color),
				new VertexPositionColor(screen.ToPixelSpaceRounded(rect.BottomRight), color),
				new VertexPositionColor(screen.ToPixelSpaceRounded(rect.BottomLeft), color)
			};
			draw.DrawVertices(VerticesMode.Triangles, vertices);
		}

		private static readonly short[] QuadIndices = new short[] { 0, 1, 2, 0, 2, 3 };
	}
}