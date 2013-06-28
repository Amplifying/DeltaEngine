using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Scenes.UserInterfaces.Graphing;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Scenes.Tests.UserInterfaces.Graphing
{
	public class RemoveOldestPointsTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderTenPointRandomScrollingGraphOfDollars()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			CreateGraphAndLine();
			graph.PercentilePrefix = "$";
			Assert.AreEqual(10, graph.MaximumNumberOfPoints);
			RunCode = () =>
			{
				if (Time.Current.Delta > 0.1f || !Time.Current.CheckEvery(0.1f))
					return;

				line.AddValue(0.1f, Randomizer.Current.Get());
			};
		}

		private void CreateGraphAndLine()
		{
			graph = new Graph
			{
				DrawArea = Center,
				Viewport = Rectangle.One,
				MaximumNumberOfPoints = 10,
				NumberOfPercentiles = 2
			};
			graph.Start<RenderAxes>();
			line = graph.CreateLine("", LineColor);
			EntitySystem.Current.Run();
		}

		private Graph graph;
		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.3f);
		private GraphLine line;
		private static readonly Color LineColor = Color.Blue;

		[Test]
		public void AddingPointDoesNotRemoveFirstPointIfUnderTheLimit()
		{
			CreateGraphAndLine();
			graph.MaximumNumberOfPoints = 3;
			line.AddPoint(new Point(1, 0));
			line.AddPoint(new Point(2, 0));
			Assert.AreEqual(2, line.points.Count);
			line.AddPoint(new Point(3, 0));
			Assert.AreEqual(3, line.points.Count);
			Window.CloseAfterFrame();
		}

		[Test]
		public void AddingPointRemovesFirstPointIfOverTheLimit()
		{
			CreateGraphAndLine();
			graph.MaximumNumberOfPoints = 3;
			line.AddPoint(new Point(1, 0));
			line.AddPoint(new Point(2, 0));
			line.AddPoint(new Point(3, 0));
			Assert.AreEqual(3, line.points.Count);
			line.AddPoint(new Point(4, 0));
			Assert.AreEqual(3, line.points.Count);
			Window.CloseAfterFrame();
		}
	}
}