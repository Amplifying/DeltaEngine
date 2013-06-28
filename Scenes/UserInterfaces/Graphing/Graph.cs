using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Renders a graph with one or more lines at a given scale.
	/// Various behaviours can be added including autogrowing, autopruning and rendering axes
	/// and percentiles
	/// </summary>
	public class Graph : Entity2D
	{
		public Graph()
			: base(Rectangle.Zero)
		{
			Add(new ObserveEntity2D.SavedProperties());
			Add(new FilledRect(Rectangle.Zero, HalfBlack));
			Add(new Data());
			Start<ObserveEntity2D, UpdateGraphics, RenderKey>();
			Activated += Refresh;
			Inactivated += InactivateEverything;
		}

		internal static readonly Color HalfBlack = new Color(0, 0, 0, 0.75f);

		public class Data
		{
			public Data()
			{
				XAxis = new Line2D(Point.Zero, Point.Zero, Color.White) { Visibility = Visibility.Hide };
				YAxis = new Line2D(Point.Zero, Point.Zero, Color.White) { Visibility = Visibility.Hide };
				Lines = new List<GraphLine>();
				Percentiles = new List<Line2D>();
				PercentileColor = Color.Gray;
				PercentilePrefix = "";
				PercentileSuffix = "";
				PercentileLabels = new List<FontText>();
				PercentileLabelColor = Color.White;
				KeyLabels = new List<FontText>();
			}

			public Rectangle Viewport { get; set; }
			public Line2D XAxis { get; set; }
			public Line2D YAxis { get; set; }
			public List<GraphLine> Lines { get; private set; }
			public int MaximumNumberOfPoints { get; set; }
			public int NumberOfPercentiles { get; set; }
			public List<Line2D> Percentiles { get; private set; }
			public Color PercentileColor { get; set; }
			public string PercentilePrefix { get; set; }
			public string PercentileSuffix { get; set; }
			public List<FontText> PercentileLabels { get; private set; }
			public Color PercentileLabelColor { get; set; }
			public List<FontText> KeyLabels { get; private set; }
			public bool ArePercentileLabelsInteger { get; set; }
		}

		public void Refresh()
		{
			Get<ObserveEntity2D.SavedProperties>().IsRefreshRequired = true;
		}

		//TODO: Rethink InactivateEverything; Probably needs to return everything to its pools
		private void InactivateEverything()
		{
			var data = Get<Data>();
			InactivateAxes(data);
			foreach (GraphLine line in data.Lines)
				line.ClearGraphics();

			foreach (Line2D line in data.Percentiles)
				line.IsActive = false;

			foreach (FontText percentileLabel in data.PercentileLabels)
				percentileLabel.IsActive = false;

			foreach (FontText keyLabel in data.KeyLabels)
				keyLabel.IsActive = false;
		}

		private static void InactivateAxes(Data data)
		{
			data.XAxis.IsActive = false;
			data.YAxis.IsActive = false;
		}

		private class UpdateGraphics : EventListener2D
		{
			public override void ReceiveMessage(Entity2D entity, object message)
			{
				if (!(message is ObserveEntity2D.HasChanged))
					return;

				graph = entity as Graph;
				UpdateBackground();
				UpdateLines();
			}

			private Graph graph;

			private void UpdateBackground()
			{
				var background = graph.Get<FilledRect>();
				background.DrawArea = graph.DrawArea;
				background.RenderLayer = graph.RenderLayer;
				background.Visibility = graph.Visibility;
			}

			private void UpdateLines()
			{
				var data = graph.Get<Data>();
				foreach (GraphLine line in data.Lines)
					line.ClearGraphics();

				if (graph.Visibility == Visibility.Show)
					foreach (GraphLine line in data.Lines)
						line.Refresh();
			}
		}

		internal const float Border = 0.025f;

		public Rectangle Viewport
		{
			get { return Get<Data>().Viewport; }
			set
			{
				var data = Get<Data>();
				if (data.Viewport == value)
					return;

				Get<Data>().Viewport = value;
				Refresh();
			}
		}

		public GraphLine CreateLine(string key, Color color)
		{
			var line = new GraphLine(this) { Key = key, Color = color };
			Get<Data>().Lines.Add(line);
			Refresh();
			return line;
		}

		public void RemoveLine(GraphLine line)
		{
			line.Clear();
			Get<Data>().Lines.Remove(line);
			Refresh();
		}

		public Color AxisColor
		{
			get { return Get<Data>().XAxis.Color; }
			set
			{
				var data = Get<Data>();
				data.XAxis.Color = value;
				data.YAxis.Color = value;
				Refresh();
			}
		}

		public Color BackgroundColor
		{
			get { return Get<FilledRect>().Color; }
			set
			{
				Get<FilledRect>().Color = value;
				Refresh();
			}
		}

		public Point Origin { get; set; }

		internal void AddPoint(Point point)
		{
			MessageAllListeners(new PointAdded(point));
		}

		public class PointAdded
		{
			public PointAdded(Point point)
			{
				Point = point;
			}

			public Point Point { get; private set; }
		}

		public int MaximumNumberOfPoints
		{
			get { return Get<Data>().MaximumNumberOfPoints; }
			set
			{
				Get<Data>().MaximumNumberOfPoints = value;
				Start<RemoveOldestPoints>();
				Refresh();
			}
		}

		public int NumberOfPercentiles
		{
			get { return Get<Data>().NumberOfPercentiles; }
			set
			{
				Get<Data>().NumberOfPercentiles = value;
				Start<RenderPercentiles, RenderPercentileLabels>();
				Refresh();
			}
		}

		public Color PercentileColor
		{
			get { return Get<Data>().PercentileColor; }
			set
			{
				Get<Data>().PercentileColor = value;
				Refresh();
			}
		}

		public string PercentilePrefix
		{
			get { return Get<Data>().PercentilePrefix; }
			set
			{
				Get<Data>().PercentilePrefix = value;
				Refresh();
			}
		}

		public string PercentileSuffix
		{
			get { return Get<Data>().PercentileSuffix; }
			set
			{
				Get<Data>().PercentileSuffix = value;
				Refresh();
			}
		}

		public Color PercentileLabelColor
		{
			get { return Get<Data>().PercentileLabelColor; }
			set
			{
				Get<Data>().PercentileLabelColor = value;
				Refresh();
			}
		}

		public bool ArePercentileLabelsInteger
		{
			get { return Get<Data>().ArePercentileLabelsInteger; }
			set { Get<Data>().ArePercentileLabelsInteger = value; }
		}

		protected List<GraphLine> Lines
		{
			get { return Get<Data>().Lines; }
		}
	}
}