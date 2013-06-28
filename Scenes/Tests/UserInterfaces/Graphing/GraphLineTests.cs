using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Scenes.UserInterfaces.Graphing;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces.Graphing
{
	public class GraphLineTests
	{
		[Test]
		public void ChangeLineColor()
		{
			var graph = new Graph();
			var line = graph.CreateLine("", Color.Blue);
			Assert.AreEqual(Color.Blue, line.Color);
			line.Color = Color.Green;
			Assert.AreEqual(Color.Green, line.Color);
		}

		[Test]
		public void ChangeLineKey()
		{
			var graph = new Graph();
			var line = graph.CreateLine("ABC", Color.Blue);
			Assert.AreEqual("ABC", line.Key);
			line.Key = "DEF";
			Assert.AreEqual("DEF", line.Key);
		}

		[Test]
		public void AddingFirstPointDoesntCreateALine()
		{
			var graph = new Graph();
			var line = graph.CreateLine("", Color.Blue);
			Assert.AreEqual(0, line.points.Count);
			Assert.AreEqual(0, line.lines.Count);
			line.AddPoint(Point.Zero);
			Assert.AreEqual(1, line.points.Count);
			Assert.AreEqual(0, line.lines.Count);
		}

		[Test]
		public void TwoPointsDrawALine()
		{
			GraphLine line = CreateLineWithTwoPoints();
			Assert.AreEqual(2, line.points.Count);
			Assert.AreEqual(1, line.lines.Count);
			Line2D line2D = line.lines[0];
			Assert.AreEqual(new Point(0.462f, 0.5f), line2D.StartPoint);
			Assert.AreEqual(new Point(0.538f, 0.462f), line2D.EndPoint);
			Assert.AreEqual(LineColor, line.Color);
		}

		private static GraphLine CreateLineWithTwoPoints()
		{
			var graph = new Graph { DrawArea = Center, Viewport = Rectangle.One };
			var line = graph.CreateLine("", LineColor);
			line.AddPoint(new Point(0.4f, 0.5f));
			line.AddPoint(new Point(0.6f, 0.7f));
			return line;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.2f);
		private static readonly Color LineColor = Color.Blue;

		[Test]
		public void AddThirdPointAtTheEnd()
		{
			GraphLine line = CreateLineWithTwoPoints();
			line.AddPoint(new Point(0.8f, 0.5f));
			Assert.AreEqual(3, line.points.Count);
			Assert.AreEqual(2, line.lines.Count);
			Line2D line2D = line.lines[1];
			Assert.AreEqual(new Point(0.538f, 0.462f), line2D.StartPoint);
			Assert.AreEqual(new Point(0.6143f, 0.5f), line2D.EndPoint);
		}

		[Test]
		public void AddThirdPointAtTheStart()
		{
			GraphLine line = CreateLineWithTwoPoints();
			line.AddPoint(new Point(0.2f, 0.5f));
			Assert.AreEqual(3, line.points.Count);
			Assert.AreEqual(2, line.lines.Count);
			Line2D line0 = line.lines[0];
			Assert.AreEqual(new Point(0.3857f, 0.5f), line0.StartPoint);
			Assert.AreEqual(new Point(0.462f, 0.5f), line0.EndPoint);
			Line2D line1 = line.lines[1];
			Assert.AreEqual(new Point(0.462f, 0.5f), line1.StartPoint);
			Assert.AreEqual(new Point(0.538f, 0.462f), line1.EndPoint);
		}

		[Test]
		public void AddThirdPointInTheMiddle()
		{
			GraphLine line = CreateLineWithTwoPoints();
			line.AddPoint(new Point(0.5f, 0.4f));
			Assert.AreEqual(3, line.points.Count);
			Assert.AreEqual(2, line.lines.Count);
			Line2D line0 = line.lines[0];
			Assert.AreEqual(new Point(0.462f, 0.5f), line0.StartPoint);
			Assert.AreEqual(new Point(0.5f, 0.519f), line0.EndPoint);
			Line2D line1 = line.lines[1];
			Assert.AreEqual(new Point(0.5f, 0.519f), line1.StartPoint);
			Assert.AreEqual(new Point(0.538f, 0.462f), line1.EndPoint);
		}

		[Test]
		public void RemoveFirstPoint()
		{
			GraphLine line = CreateLineWithThreePoints();
			line.RemoveAt(0);
			Assert.AreEqual(2, line.points.Count);
			Assert.AreEqual(1, line.lines.Count);
			Assert.AreEqual(new Point(0.6f, 0.7f), line.points[0]);
			Assert.AreEqual(new Point(0.8f, 0.4f), line.points[1]);
			Assert.AreEqual(new Point(0.538f, 0.462f), line.lines[0].StartPoint);
			Assert.AreEqual(new Point(0.6143f, 0.519f), line.lines[0].EndPoint);
		}

		private static GraphLine CreateLineWithThreePoints()
		{
			var graph = new Graph { DrawArea = Center, Viewport = Rectangle.One };
			var line = graph.CreateLine("", LineColor);
			line.AddPoint(new Point(0.4f, 0.5f));
			line.AddPoint(new Point(0.6f, 0.7f));
			line.AddPoint(new Point(0.8f, 0.4f));
			return line;
		}

		[Test]
		public void RemoveMiddlePoint()
		{
			GraphLine line = CreateLineWithThreePoints();
			line.RemoveAt(1);
			Assert.AreEqual(2, line.points.Count);
			Assert.AreEqual(1, line.lines.Count);
			Assert.AreEqual(new Point(0.4f, 0.5f), line.points[0]);
			Assert.AreEqual(new Point(0.8f, 0.4f), line.points[1]);
			Assert.AreEqual(new Point(0.462f, 0.5f), line.lines[0].StartPoint);
			Assert.AreEqual(new Point(0.6143f, 0.519f), line.lines[0].EndPoint);
		}

		[Test]
		public void RemoveLastPoint()
		{
			GraphLine line = CreateLineWithThreePoints();
			line.RemoveAt(2);
			Assert.AreEqual(2, line.points.Count);
			Assert.AreEqual(1, line.lines.Count);
			Assert.AreEqual(new Point(0.462f, 0.5f), line.lines[0].StartPoint);
			Assert.AreEqual(new Point(0.538f, 0.462f), line.lines[0].EndPoint);
		}

		[Test]
		public void ClearGraphicsRemovesAllLines()
		{
			GraphLine line = CreateLineWithTwoPoints();
			Line2D line2D = line.lines[0];
			Assert.IsTrue(line2D.IsActive);
			line.ClearGraphics();
			Assert.AreEqual(0, line.lines.Count);
			Assert.AreEqual(2, line.points.Count);
		}

		[Test]
		public void ClearRemovesAllLinesAndClearsAllPoints()
		{
			GraphLine line = CreateLineWithTwoPoints();
			Line2D line2D = line.lines[0];
			Assert.IsTrue(line2D.IsActive);
			line.Clear();
			Assert.AreEqual(0, line.lines.Count);
			Assert.AreEqual(0, line.points.Count);
		}

		[Test]
		public void RefreshDoesNothingIfViewportDidntChange()
		{
			GraphLine line = CreateLineWithTwoPoints();
			Point start = line.lines[0].StartPoint;
			Point end = line.lines[0].EndPoint;
			line.Refresh();
			Assert.AreEqual(start, line.lines[0].StartPoint);
			Assert.AreEqual(end, line.lines[0].EndPoint);
		}

		[Test]
		public void RefreshUpdatesLinesIfViewportChanged()
		{
			GraphLine line = CreateLineWithTwoPoints();
			line.graph.Viewport = Rectangle.FromCenter(0.4f, 0.4f, 0.8f, 0.8f);
			line.Refresh();
			Assert.AreEqual(new Point(0.5f, 0.4762f), line.lines[0].StartPoint);
			Assert.AreEqual(new Point(0.5952f, 0.4286f), line.lines[0].EndPoint);
		}

		[Test]
		public void AddValueAddsToTheEnd()
		{
			GraphLine line = CreateLineWithTwoPoints();
			line.AddValue(0.4f, 4.0f);
			Assert.AreEqual(new Point(1.0f, 4.0f), line.points[2]);
			line.AddValue(3.0f);
			Assert.AreEqual(new Point(2.0f, 3.0f), line.points[3]);
		}
	}
}