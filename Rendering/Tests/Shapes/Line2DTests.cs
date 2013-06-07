using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class Line2DTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void RenderRedLine(Type resolver)
		{
			Start(resolver, () => new Line2D(Point.UnitX, Point.UnitY, Color.Red));
		}

		[VisualTest]
		public void RenderLineAndSprite(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new Line2D(Point.Zero, Point.One, Color.Red);
				new Sprite(content.Load<Image>("DeltaEngineLogo"),
					Rectangle.FromCenter(Point.Half, new Size(0.1f)));
			});
		}

		[VisualTest]
		public void RenderManyRotatingLines(Type resolver)
		{
			Start(resolver, () =>
			{
				for (int num = 0; num < 100; num++)
					AddRotatingLine(num);
			});
		}

		private static void AddRotatingLine(int num)
		{
			var line = new Line2D(Point.Half, new Point(0.5f, 0.0f), Color.Orange);
			line.Rotation = num * 360 / 100.0f;
			line.Add<Rotate>();
		}

		public class Rotate : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				entity.Set(entity.Get<float>() + 10 * Time.Current.Delta);
				var line = entity as Line2D;
				if (line != null)
					line.End = Point.Half + new Point(0.5f, 0).RotateAround(Point.Zero, line.Rotation);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[Test]
		public void CreateLine()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red);
			Assert.AreEqual(Point.Zero, line.Start);
			Assert.AreEqual(Point.One, line.End);
			Assert.AreEqual(Color.Red, line.Color);
			Assert.AreEqual(2, line.Points.Count);
		}

		[Test]
		public void MoveLineViaItsEndPoints()
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red)
			{
				Start = Point.Half,
				End = Point.One
			};
			Assert.AreEqual(Point.Half, line.Start);
			Assert.AreEqual(Point.One, line.End);
		}

		[Test]
		public void MoveLineByAssigningListOfPoints()
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red);
			line.Points = new List<Point> { Point.Half, Point.One };
			Assert.AreEqual(Point.Half, line.Start);
			Assert.AreEqual(Point.One, line.End);
		}

		[Test]
		public void AddLineToEntitySystem()
		{
			Start(typeof(MockResolver), () =>
			{
				new Line2D(Point.Zero, Point.One, Color.Red);
				EntitySystem.Current.Run();
				Assert.AreEqual(3, mockResolver.rendering.NumberOfVerticesDrawn);
			});
		}

		[Test]
		public void ChangeColor()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red) { Color = Color.Green };
			Assert.AreEqual(Color.Green, line.Color);
			Assert.AreEqual(Color.Green, line.Get<Color>());
		}

		[VisualTest]
		public void RenderRedSquareViaAddingLines(Type resolver)
		{
			Start(resolver, () =>
			{
				var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.4f), Color.Red);
				line.AddLine(new Point(0.6f, 0.4f), new Point(0.6f, 0.6f));
				line.AddLine(new Point(0.6f, 0.6f), new Point(0.4f, 0.6f));
				line.AddLine(new Point(0.4f, 0.6f), new Point(0.4f, 0.4f));
			});
		}

		[VisualTest]
		public void RenderRedSquareViaExtendingLine(Type resolver)
		{
			Start(resolver, () => CreateRedBox());
		}

		private static Line2D CreateRedBox()
		{
			var line = new Line2D(new Point(0.4f, 0.4f), new Point(0.6f, 0.4f), Color.Red);
			line.ExtendLine(new Point(0.6f, 0.6f));
			line.ExtendLine(new Point(0.4f, 0.6f));
			line.ExtendLine(new Point(0.4f, 0.4f));
			return line;
		}

		[VisualTest]
		public void RenderRedSquareWithMissingTop(Type resolver)
		{
			Start(resolver, () =>
			{
				var line = CreateRedBox();
				var points = line.Points;
				points.RemoveAt(0);
				points.RemoveAt(0);
			});
		}
	}
}