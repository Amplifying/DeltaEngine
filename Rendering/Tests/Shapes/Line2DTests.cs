using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class Line2DTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedLine()
		{
			new Line2D(Point.UnitX, Point.UnitY, Color.Red);
		}

		[Test]
		public void RenderLineAndSprite()
		{
			new Line2D(Point.Zero, Point.One, Color.Red);
			new Sprite("DeltaEngineLogo", Rectangle.FromCenter(Point.Half, new Size(0.1f)));
		}

		[Test]
		public void RenderManyRotatingLines()
		{
			for (int num = 0; num < 100; num++)
				AddRotatingLine(num);
		}

		private static void AddRotatingLine(int num)
		{
			var line = new Line2D(Point.Half, new Point(0.5f, 0.0f), Color.Orange);
			line.Rotation = num * 360 / 100.0f;
			line.Start<Rotate>();
		}

		public class Rotate : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				entity.Set(entity.Get<float>() + 10 * Time.Current.Delta);
				var line = entity as Line2D;
				if (line != null)
					line.EndPoint = Point.Half + new Point(0.5f, 0).RotateAround(Point.Zero, line.Rotation);
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		[Test]
		public void CreateLine()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red);
			Assert.AreEqual(Point.Zero, line.StartPoint);
			Assert.AreEqual(Point.One, line.EndPoint);
			Assert.AreEqual(Color.Red, line.Color);
			Assert.AreEqual(2, line.Points.Count);
			Window.CloseAfterFrame();
		}

		[Test]
		public void MoveLineViaItsEndPoints()
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red)
			{
				StartPoint = Point.Half,
				EndPoint = Point.One
			};
			Assert.AreEqual(Point.Half, line.StartPoint);
			Assert.AreEqual(Point.One, line.EndPoint);
			Window.CloseAfterFrame();
		}

		[Test]
		public void MoveLineByAssigningListOfPoints()
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red);
			line.Points = new List<Point> { Point.Half, Point.One };
			Assert.AreEqual(Point.Half, line.StartPoint);
			Assert.AreEqual(Point.One, line.EndPoint);
			Window.CloseAfterFrame();
		}

		[Test]
		public void AddLineToEntitySystem()
		{
			new Line2D(Point.Zero, Point.One, Color.Red);
			RunCode = () => Assert.AreEqual(2, Resolve<Drawing>().NumberOfVerticesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeColor()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red) { Color = Color.Green };
			Assert.AreEqual(Color.Green, line.Color);
			Assert.AreEqual(Color.Green, line.Get<Color>());
			Window.CloseAfterFrame();
		}

		[Test]
		public void RenderRedSquareViaAddingLines()
		{
			var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.4f), Color.Red);
			line.AddLine(new Point(0.6f, 0.4f), new Point(0.6f, 0.6f));
			line.AddLine(new Point(0.6f, 0.6f), new Point(0.4f, 0.6f));
			line.AddLine(new Point(0.4f, 0.6f), new Point(0.4f, 0.4f));
		}

		[Test]
		public void RenderRedSquareViaExtendingLine()
		{
			CreateRedBox();
		}

		private static Line2D CreateRedBox()
		{
			var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.4f), Color.Red);
			line.ExtendLine(new Point(0.6f, 0.6f));
			line.ExtendLine(new Point(0.4f, 0.6f));
			line.ExtendLine(new Point(0.4f, 0.4f));
			return line;
		}

		[Test]
		public void RenderRedSquareWithMissingTop()
		{
			var line = CreateRedBox();
			var points = line.Points;
			points.RemoveAt(0);
			points.RemoveAt(0);
		}

		[Test]
		public void RenderRedLineOverBlue()
		{
			new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.6f), Color.Red) { RenderLayer = 1 };
			new Line2D(new Point(0.6f, 0.4f), new Point(0.4f, 0.6f), Color.Blue) { RenderLayer = 0 };
		}

		[Test]
		public void RenderRedLineOverBlueRectOverYellowEllipseOverGreenLine()
		{
			new FilledRect(new Rectangle(0.48f, 0.48f, 0.04f, 0.04f), Color.Blue) { RenderLayer = 1 };
			new Line2D(new Point(0.2f, 0.2f), new Point(0.8f, 0.8f), Color.Red) { RenderLayer = 2 };
			new Ellipse(Point.Half, 0.1f, 0.1f, Color.Yellow) { RenderLayer = 0 };
			new Line2D(new Point(0.8f, 0.2f), new Point(0.2f, 0.8f), Color.Green) { RenderLayer = -1 };
		}

		[Test]
		public void DrawingTwoLinesWithTheSameRenderLayerOnlyIssuesOneDrawCall()
		{
			new Line2D(new Point(0.2f, 0.2f), new Point(0.8f, 0.8f), Color.Red);
			new Line2D(new Point(0.8f, 0.2f), new Point(0.2f, 0.8f), Color.Green);
			RunCode = () => Assert.AreEqual(1, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void DrawingTwoLinesWithDifferentRenderLayersIssuesTwoDrawCalls()
		{
			new Line2D(new Point(0.2f, 0.2f), new Point(0.8f, 0.8f), Color.Red) { RenderLayer = 1 };
			new Line2D(new Point(0.8f, 0.2f), new Point(0.2f, 0.8f), Color.Green) { RenderLayer = -1 };
			RunCode = () => Assert.AreEqual(2, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineInsideViewportIsNotClipped()
		{
			var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(0.4f, 0.4f), line.StartPoint);
			Assert.AreEqual(new Point(0.6f, 0.5f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineAboveViewportIsHidden()
		{
			var line = new Line2D(new Point(0.2f, -1.0f), new Point(0.6f, -1.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(0.2f, -1.0f), line.StartPoint);
			Assert.AreEqual(new Point(0.6f, -1.5f), line.EndPoint);
			Assert.AreEqual(Visibility.Hide, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineBelowViewportIsHidden()
		{
			var line = new Line2D(new Point(-0.2f, 2.0f), new Point(2.6f, 2.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(-0.2f, 2.0f), line.StartPoint);
			Assert.AreEqual(new Point(2.6f, 2.5f), line.EndPoint);
			Assert.AreEqual(Visibility.Hide, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineLeftOfViewportIsHidden()
		{
			var line = new Line2D(new Point(-0.2f, 0.0f), new Point(-0.6f, 1.0f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(-0.2f, 0.0f), line.StartPoint);
			Assert.AreEqual(new Point(-0.6f, 1.0f), line.EndPoint);
			Assert.AreEqual(Visibility.Hide, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineRightOfViewportIsHidden()
		{
			var line = new Line2D(new Point(1.2f, 0.5f), new Point(1.6f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(1.2f, 0.5f), line.StartPoint);
			Assert.AreEqual(new Point(1.6f, 0.5f), line.EndPoint);
			Assert.AreEqual(Visibility.Hide, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineNotCrossingViewportIsHidden()
		{
			var line = new Line2D(new Point(-1.0f, 0.2f), new Point(0.2f, -1.0f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(-1.0f, 0.2f), line.StartPoint);
			Assert.AreEqual(new Point(0.2f, -1.0f), line.EndPoint);
			Assert.AreEqual(Visibility.Hide, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineEnteringLeftEdgeIsClipped()
		{
			var line = new Line2D(new Point(-0.5f, 0.1f), new Point(0.5f, 0.2f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(0.0f, 0.15f), line.StartPoint);
			Assert.AreEqual(new Point(0.5f, 0.2f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineExitingLeftEdgeIsClipped()
		{
			var line = new Line2D(new Point(0.5f, 0.4f), new Point(-0.5f, 0.4f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Point(0.5f, 0.4f), line.StartPoint);
			Assert.AreEqual(new Point(0.25f, 0.4f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineEnteringRightEdgeIsClipped()
		{
			var line = new Line2D(new Point(0.5f, 0.4f), new Point(1.5f, 0.3f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Point(0.5f, 0.4f), line.StartPoint);
			Assert.AreEqual(new Point(0.75f, 0.375f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineExitingRightEdgeIsClipped()
		{
			var line = new Line2D(new Point(1.5f, 0.1f), new Point(0.5f, 0.2f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(1.0f, 0.15f), line.StartPoint);
			Assert.AreEqual(new Point(0.5f, 0.2f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineEnteringTopEdgeIsClipped()
		{
			var line = new Line2D(new Point(0.1f, -0.5f), new Point(0.2f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(0.15f, 0.0f), line.StartPoint);
			Assert.AreEqual(new Point(0.2f, 0.5f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineExitingTopEdgeIsClipped()
		{
			var line = new Line2D(new Point(0.4f, 0.5f), new Point(0.3f, -0.5f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Point(0.4f, 0.5f), line.StartPoint);
			Assert.AreEqual(new Point(0.375f, 0.25f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineEnteringBottomEdgeIsClipped()
		{
			var line = new Line2D(new Point(0.4f, 0.5f), new Point(0.3f, 1.5f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Point(0.4f, 0.5f), line.StartPoint);
			Assert.AreEqual(new Point(0.375f, 0.75f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineExitingBottomEdgeIsClipped()
		{
			var line = new Line2D(new Point(0.1f, 1.5f), new Point(0.2f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Point(0.15f, 1.0f), line.StartPoint);
			Assert.AreEqual(new Point(0.2f, 0.5f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LineCrossingViewportIsClippedAtStartAndEnd()
		{
			var line = new Line2D(new Point(-1.0f, 0.4f), new Point(2.0f, 0.6f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Point(0.25f, 0.4833f), line.StartPoint);
			Assert.AreEqual(new Point(0.75f, 0.5167f), line.EndPoint);
			Assert.AreEqual(Visibility.Show, line.Visibility);
			Window.CloseAfterFrame();
		}
	}
}