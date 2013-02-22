using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering.Tests
{
	public class ColoredEllipseTests : TestStarter
	{
		[Test]
		public void Constructor()
		{
			var circle = new ColoredEllipse(Point.Half, 0.1f, 0.2f);
			Assert.AreEqual(Point.Half, circle.Center);
			Assert.AreEqual(0.1f, circle.RadiusX);
			Assert.AreEqual(0.2f, circle.RadiusY);
		}

		[Test]
		public void BorderColor()
		{
			var circle = new ColoredEllipse(Point.Half, 0.1f, 0.2f) { BorderColor = Color.Cyan };
			Assert.AreEqual(Color.Cyan, circle.BorderColor);
		}

		[VisualTest]
		public void DrawColoredBorderedEllipse(Type resolver)
		{
			Start(resolver,
				(Renderer r) =>
					r.Add(new ColoredEllipse(Point.Half, 0.4f, 0.2f)
					{
						Rotation = 45,
						Color = Color.Blue,
						BorderColor = Color.Red
					}));
		}

		[VisualTest]
		public void DrawLotsOfColoredBorderlessEllipses(Type resolver)
		{
			Start(resolver, (Renderer renderer, Randomizer r) =>
			{
				for (int i = 0; i < 10; i++)
					renderer.Add(new ColoredEllipse(new Point(r.Get(), r.Get()), r.Get(0.02f, 0.4f),
						r.Get(0.02f, 0.4f)) { Color = Color.GetRandomColor() });
			});
		}

		[VisualTest]
		public void DrawLotsOfColoredBorderedEllipses(Type resolver)
		{
			Start(resolver, (Renderer renderer, Randomizer r) =>
			{
				for (int i = 0; i < 10; i++)
					renderer.Add(new ColoredEllipse(new Point(r.Get(), r.Get()), r.Get(0.02f, 0.4f),
						r.Get(0.02f, 0.4f))
					{
						Color = Color.GetRandomColor(),
						BorderColor = Color.GetRandomColor()
					});
			});
		}
	}
}