using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Renders a set of axes at the origin
	/// </summary>
	public class RenderAxes : EventListener2D
	{
		public override void ReceiveMessage(Entity2D entity, object message)
		{
			if (!(message is ObserveEntity2D.HasChanged))
				return;

			if (entity.Visibility == Visibility.Hide)
				HideAxes((Graph)entity);
			else
				ShowAxes((Graph)entity);
		}

		private static void HideAxes(Graph graph)
		{
			var data = graph.Get<Graph.Data>();
			data.XAxis.Visibility = Visibility.Hide;
			data.YAxis.Visibility = Visibility.Hide;
		}

		private void ShowAxes(Graph graph)
		{
			renderLayer = graph.RenderLayer + RenderLayerOffset;
			var data = graph.Get<Graph.Data>();
			viewport = data.Viewport;
			drawArea = graph.DrawArea;
			clippingBounds = Rectangle.FromCorners(
				ToQuadratic(viewport.BottomLeft, viewport, drawArea),
				ToQuadratic(viewport.TopRight, viewport, drawArea));
			Point origin = graph.Origin;
			SetAxis(data.XAxis, ToQuadratic(new Point(viewport.Left, origin.Y), viewport, drawArea),
				ToQuadratic(new Point(viewport.Right, origin.Y), viewport, drawArea));
			SetAxis(data.YAxis, ToQuadratic(new Point(origin.X, viewport.Top), viewport, drawArea),
				ToQuadratic(new Point(origin.X, viewport.Bottom), viewport, drawArea));
		}

		private int renderLayer;
		private const int RenderLayerOffset = 2;
		private Rectangle viewport;
		private Rectangle drawArea;
		private Rectangle clippingBounds;

		private static Point ToQuadratic(Point point, Rectangle viewport, Rectangle drawArea)
		{
			float borderWidth = viewport.Width * Graph.Border;
			float borderHeight = viewport.Height * Graph.Border;
			float x = (point.X - viewport.Left + borderWidth) / (viewport.Width + 2 * borderWidth);
			float y = (point.Y - viewport.Top + borderHeight) / (viewport.Height + 2 * borderHeight);
			return new Point(drawArea.Left + x * drawArea.Width, drawArea.Bottom - y * drawArea.Height);
		}

		private void SetAxis(Line2D axis, Point startPoint, Point endPoint)
		{
			axis.StartPoint = startPoint;
			axis.EndPoint = endPoint;
			axis.RenderLayer = renderLayer;
			axis.Clip(clippingBounds);
			axis.Visibility = Visibility.Show;
		}
	}
}