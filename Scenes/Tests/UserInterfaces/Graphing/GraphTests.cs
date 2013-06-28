using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Scenes.UserInterfaces.Graphing;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces.Graphing
{
	public class GraphTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderGraphWithFourLines()
		{
			CreateGraphWithFourLines();
		}

		private static Graph CreateGraphWithFourLines()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			var graph = new Graph { DrawArea = Center, Viewport = LargeViewport };
			GraphLine line = graph.CreateLine("", LineColor);
			line.AddPoint(new Point(-1.0f, -1.0f));
			line.AddPoint(new Point(0.1f, 0.5f));
			line.AddPoint(new Point(0.5f, 0.2f));
			line.AddPoint(new Point(0.9f, 1.0f));
			line.AddPoint(new Point(1.5f, -2.0f));
			return graph;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.2f);
		private static readonly Rectangle LargeViewport = new Rectangle(-2.0f, -2.0f, 4.0f, 4.0f);

		[Test]
		public void RenderGraphWithAxes()
		{
			var graph = CreateGraphWithFourLines();
			graph.Start<RenderAxes>();
		}

		[Test]
		public void RenderOffCenterGraphWithAxesAndClipping()
		{
			var graph = CreateGraphWithFourLines();
			graph.Start<RenderAxes>();
			graph.Origin = Point.Half;
			graph.Viewport = new Rectangle(0.2f, 0.3f, 1.0f, 1.0f);
		}

		[Test]
		public void RenderFpsWithFivePercentiles()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			var viewport = new Rectangle(0.0f, 0.0f, 10.0f, 60.0f);
			var graph = CreateGraphWithFivePercentiles(viewport);
			GraphLine line = graph.CreateLine("", LineColor);
			RunCode = () =>
			{
				if (Time.Current.Delta > 0.1f || !Time.Current.CheckEvery(0.1f))
					return;

				line.AddValue(0.1f, Time.Current.Fps);
				Window.Title = "Render Fps: " + Time.Current.Fps + " fps";
			};
		}

		private static Graph CreateGraphWithFivePercentiles(Rectangle viewport)
		{
			var graph = new Graph
			{
				DrawArea = Center,
				Viewport = viewport,
				NumberOfPercentiles = 5,
				PercentileSuffix = "%"
			};
			graph.Start<RenderAxes>();
			return graph;
		}

		[Test]
		public void ChangeOrigin()
		{
			var graph = new Graph();
			Assert.AreEqual(Point.Zero, graph.Origin);
			graph.Origin = Origin;
			Assert.AreEqual(Origin, graph.Origin);
			Window.CloseAfterFrame();
		}

		private static readonly Point Origin = new Point(1.5f, -1.5f);

		[Test]
		public void ChangeAxisColor()
		{
			var graph = new Graph();
			Assert.AreEqual(Color.White, graph.AxisColor);
			graph.AxisColor = Color.Blue;
			Assert.AreEqual(Color.Blue, graph.AxisColor);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeViewport()
		{
			var graph = new Graph();
			Assert.AreEqual(Rectangle.Zero, graph.Viewport);
			graph.Viewport = Rectangle.One;
			Assert.AreEqual(Rectangle.One, graph.Viewport);
			graph.Viewport = Rectangle.One;
			Assert.AreEqual(Rectangle.One, graph.Viewport);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeBackgroundColor()
		{
			var graph = new Graph();
			Assert.AreEqual(Graph.HalfBlack, graph.BackgroundColor);
			graph.BackgroundColor = Color.White;
			Assert.AreEqual(Color.White, graph.BackgroundColor);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeNumberOfPercentiles()
		{
			var graph = new Graph();
			Assert.AreEqual(0, graph.NumberOfPercentiles);
			graph.NumberOfPercentiles = 2;
			Assert.AreEqual(2, graph.NumberOfPercentiles);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangePercentileColor()
		{
			var graph = new Graph();
			Assert.AreEqual(Color.Gray, graph.PercentileColor);
			graph.PercentileColor = Color.White;
			Assert.AreEqual(Color.White, graph.PercentileColor);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangePercentileSuffix()
		{
			var graph = new Graph();
			Assert.AreEqual("", graph.PercentileSuffix);
			graph.PercentileSuffix = "%";
			Assert.AreEqual("%", graph.PercentileSuffix);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangePercentilePrefix()
		{
			var graph = new Graph();
			Assert.AreEqual("", graph.PercentilePrefix);
			graph.PercentilePrefix = "$";
			Assert.AreEqual("$", graph.PercentilePrefix);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangePercentileLabelColor()
		{
			var graph = new Graph();
			Assert.AreEqual(Color.White, graph.PercentileLabelColor);
			graph.PercentileLabelColor = Color.Gray;
			Assert.AreEqual(Color.Gray, graph.PercentileLabelColor);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeArePercentileLabelsInteger()
		{
			var graph = new Graph();
			Assert.IsFalse(graph.ArePercentileLabelsInteger);
			graph.ArePercentileLabelsInteger = true;
			Assert.IsTrue(graph.ArePercentileLabelsInteger);
			Window.CloseAfterFrame();
		}

		[Test]
		public void InactivatingGraphClearsLines()
		{
			var graph = new Graph { NumberOfPercentiles = 2 };
			graph.Start<RenderKey>();
			GraphLine line = graph.CreateLine("Line 1", LineColor);
			line.AddPoint(new Point(-1.0f, -1.0f));
			line.AddPoint(new Point(0.1f, 0.5f));
			EntitySystem.Current.Run();
			graph.IsActive = false;
			Assert.AreEqual(2, line.points.Count);
			Assert.AreEqual(0, line.lines.Count);
			Window.CloseAfterFrame();
		}

		private static readonly Color LineColor = Color.Blue;

		[Test]
		public void ReactivatingGraphRecreatesLines()
		{
			var graph = new Graph { NumberOfPercentiles = 5, ArePercentileLabelsInteger = true };
			GraphLine line = graph.CreateLine("", LineColor);
			EntitySystem.Current.Run();
			line.AddPoint(new Point(-1.0f, -1.0f));
			line.AddPoint(new Point(0.1f, 0.5f));
			graph.IsActive = false;
			Assert.AreEqual(0, line.lines.Count);
			graph.IsActive = true;
			EntitySystem.Current.Run();
			Assert.AreEqual(1, line.lines.Count);
			Window.CloseAfterFrame();
		}

		[Test]
		public void HideGraph()
		{
			var graph = new Graph { NumberOfPercentiles = 5 };
			graph.Start<RenderAxes>();
			GraphLine line = graph.CreateLine("", LineColor);
			line.AddPoint(new Point(-1.0f, -1.0f));
			line.AddPoint(new Point(0.1f, 0.5f));
			graph.Visibility = Visibility.Hide;
			EntitySystem.Current.Run();
			Window.CloseAfterFrame();
		}

		[Test]
		public void RemovingGraphLineRemovesItsLines()
		{
			var graph = new Graph();
			GraphLine line = graph.CreateLine("", LineColor);
			line.AddPoint(new Point(-1.0f, -1.0f));
			line.AddPoint(new Point(0.1f, 0.5f));
			Assert.AreEqual(1, graph.Get<Graph.Data>().Lines.Count);
			Assert.AreEqual(1, line.lines.Count);
			graph.RemoveLine(line);
			Assert.AreEqual(0, graph.Get<Graph.Data>().Lines.Count);
			Assert.AreEqual(0, line.lines.Count);
		}
	}
}