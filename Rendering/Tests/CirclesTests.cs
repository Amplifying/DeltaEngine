using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering.Tests
{
	public class ColoredCircleTests : TestStarter
	{
		[Test]
		public void ConstructorWithColor()
		{
			var circle = new Circle(Point.Half, 0.1f, Color.Blue);
			Assert.AreEqual(Point.Half, circle.Center);
			Assert.AreEqual(0.1f, circle.Radius);
			Assert.AreEqual(Color.Blue, circle.Color);
		}

		[Test]
		public void ConstructorWithoutColor()
		{
			var circle = new Circle(Point.Half, 0.1f);
			Assert.AreEqual(Point.Half, circle.Center);
			Assert.AreEqual(0.1f, circle.Radius);
		}

		[Test]
		public void SetRadius()
		{
			var circle = new Circle(Point.Half, 0.1f);
			Assert.AreEqual(0.1f, circle.Radius);
			circle.Radius = 0.7f;
			Assert.AreEqual(0.7f, circle.Radius);
			circle.Radius = 0.7f;
			Assert.AreEqual(0.7f, circle.Radius);
		}

		[Test]
		public void BorderColor()
		{
			var circle = new Circle(Point.Half, 0.1f) { BorderColor = Color.Cyan };
			Assert.AreEqual(Color.Cyan, circle.BorderColor);
		}

		[VisualTest]
		public void DrawLotsOfColoredBorderlessCircles(Type resolver)
		{
			Start(resolver, (Renderer renderer, Randomizer r) =>
			{
				for (int i = 0; i < 10; i++)
					renderer.Add(new Circle(new Point(r.Get(), r.Get()), r.Get(0.02f, 0.4f),
						Color.GetRandomColor()));
			});
		}

		[VisualTest]
		public void DrawLotsOfColoredBorderedCircles(Type resolver)
		{
			Start(resolver, (Renderer renderer, Randomizer r) =>
			{
				for (int i = 0; i < 10; i++)
					renderer.Add(new Circle(new Point(r.Get(), r.Get()), r.Get(0.02f, 0.4f),
						Color.GetRandomColor()) { BorderColor = Color.GetRandomColor() });
			});
		}
	}
}