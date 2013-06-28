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
	public class AutogrowViewportTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderGraph()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			CreateGraphAndLine();
			line.AddPoint(new Point(-1.0f, -1.0f));
			line.AddPoint(new Point(0.0f, 0.5f));
			line.AddPoint(new Point(1.0f, 1.0f));
			line.AddPoint(new Point(1.5f, -2.0f));
		}

		private void CreateGraphAndLine()
		{
			graph = new Graph { DrawArea = Center };
			graph.Start<RenderAxes, AutogrowViewport>();
			line = graph.CreateLine("Line 1", LineColor);
			EntitySystem.Current.Run();
		}

		private Graph graph;
		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.3f);
		private GraphLine line;
		private static readonly Color LineColor = Color.Blue;

		[Test]
		public void RenderRandomGraph()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			CreateGraphAndLine();
			RunCode = () =>
			{
				if (Time.Current.Delta < 0.1f && Time.Current.CheckEvery(0.1f))
					line.AddValue(Randomizer.Current.Get());
			};
		}

		[Test]
		public void RenderFps()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			CreateGraphAndLine();
			RunCode = () =>
			{
				if (Time.Current.Delta > 0.1f || !Time.Current.CheckEvery(0.1f))
					return;

				line.AddValue(Time.Current.Fps);
				Window.Title = "Render Fps: " + Time.Current.Fps + " fps";
			};
		}

		[Test]
		public void RenderNumberOfEntities()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			CreateGraphAndLine();
			graph.Viewport = new Rectangle(-1, -1, 1, 1);
			RunCode = () =>
			{
				if (Time.Current.Delta > 0.1f || !Time.Current.CheckEvery(0.1f))
					return;

				line.AddValue(EntitySystem.Current.NumberOfEntities);
				Window.Title = "Render Number Of Entities: " + EntitySystem.Current.NumberOfEntities;
			};
		}

		[Test]
		public void RenderTwoRandomLinesWithKey()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			CreateGraphAndLine();
			GraphLine line2 = graph.CreateLine("Line 2", Color.Red);
			RunCode = () =>
			{
				if (Time.Current.Delta > 0.1f || !Time.Current.CheckEvery(0.1f))
					return;

				line.AddValue(Randomizer.Current.Get());
				line2.AddValue(Randomizer.Current.Get());
			};
		}

		[Test]
		public void ViewportStartsZero()
		{
			CreateGraphAndLine();
			EntitySystem.Current.Run();
			Assert.AreEqual(Rectangle.Zero, graph.Viewport);
			Window.CloseAfterFrame();
		}

		[Test]
		public void WhenOnlyOnePointHasBeenAddedViewportPositionsToThatPoint()
		{
			CreateGraphAndLine();
			line.AddPoint(new Point(1.0f, -2.5f));
			EntitySystem.Current.Run();
			Assert.AreEqual(new Rectangle(1.0f, -2.5f, 0.0f, 0.0f), graph.Viewport);
			Window.CloseAfterFrame();
		}

		[Test]
		public void WhenMultiplePointsHaveBeenAddedViewportPositionsAndSizesToContainThem()
		{
			CreateGraphAndLine();
			line.AddPoint(new Point(0.0f, -2.5f));
			line.AddPoint(new Point(2.0f, -1.0f));
			line.AddPoint(new Point(-1.0f, -3.0f));
			EntitySystem.Current.Run();
			Assert.AreEqual(new Rectangle(-1.15f, -3.1f, 3.3f, 2.2f), graph.Viewport);
			Window.CloseAfterFrame();
		}

		[Test]
		public void IfViewportDoesNotNeedToChangeToAccomodateNewPointsItDoesnt()
		{
			CreateGraphAndLine();
			line.AddPoint(new Point(0, -2.5f));
			line.AddPoint(new Point(2, -1.0f));
			line.AddPoint(new Point(1, -1.5f));
			EntitySystem.Current.Run();
			Assert.AreEqual(new Rectangle(-0.1f, -2.575f, 2.2f, 1.65f), graph.Viewport);
			Window.CloseAfterFrame();
		}
	}
}