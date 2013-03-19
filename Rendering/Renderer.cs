using System.Linq;
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
			ResortRenderablesIfAnyHaveChangedRenderLayer();
			var objectsToRender = sortedRenderables.Where(t => t.IsVisible).ToList();
			foreach (var renderObject in objectsToRender)
				renderObject.InternalRender(this, time);

			RemoveDisposedObjects();
		}

		private void ResortRenderablesIfAnyHaveChangedRenderLayer()
		{
			if (NoRenderableHasChangedRenderLayer())
				return;

			sortedRenderables.Resort();
		}

		private bool NoRenderableHasChangedRenderLayer()
		{
			for (int i = 0; i < sortedRenderables.Count; i++)
				if (sortedRenderables[i].HasRenderLayerChanged)
					return false;

			return true;
		}

		private void RemoveDisposedObjects()
		{
			var objectsToRemove = sortedRenderables.Where(t => t.markForDisposal).ToList();
			foreach (var renderObject in objectsToRemove)
				Remove(renderObject);
		}

		public void Remove(Renderable renderable)
		{
			if (sortedRenderables.Contains(renderable))
				sortedRenderables.Remove(renderable);
		}

		private readonly BubbleSortedList sortedRenderables = new BubbleSortedList();

		public int NumberOfActiveRenderableObjects
		{
			get { return sortedRenderables.Count(renderable => !renderable.markForDisposal); }
		}

		public void Add(Renderable renderable)
		{
			if (sortedRenderables.Contains(renderable))
				return;

			renderable.markForDisposal = false;
			sortedRenderables.Add(renderable);
		}

		public void RemoveAll()
		{
			sortedRenderables.Clear();
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

		public void DrawTriangle(Triangle2D triangle, Color color)
		{
			draw.DisableTexturing();
			draw.SetIndices(TriangleIndices, TriangleIndices.Length);
			var vertices = new[]
			{
				new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner1), color),
				new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner2), color),
				new VertexPositionColor(screen.ToPixelSpaceRounded(triangle.Corner3), color)
			};
			draw.DrawVertices(VerticesMode.Triangles, vertices);
		}

		private static readonly short[] TriangleIndices = new short[] { 0, 1, 2 };

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