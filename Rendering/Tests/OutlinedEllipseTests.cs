using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering.Tests
{
	public class OutlinedEllipseTests : TestStarter
	{
		[Test]
		public void Constructor()
		{
			var ellipse = new OutlinedEllipse(Point.Half, 0.1f, 0.2f);
			Assert.AreEqual(Point.Half, ellipse.Center);
			Assert.AreEqual(0.1f, ellipse.RadiusX);
			Assert.AreEqual(0.2f, ellipse.RadiusY);
		}

		[Test]
		public void Center()
		{
			var ellipse = new OutlinedEllipse(Point.Half, 0.1f, 0.2f) { Center = Point.One };
			Assert.AreEqual(Point.One, ellipse.Center);
		}

		[Test]
		public void RadiusX()
		{
			var ellipse = new OutlinedEllipse(Point.Half, 0.1f, 0.2f) { RadiusX = 0.3f };
			ellipse.RadiusX = 0.3f;
			Assert.AreEqual(0.3f, ellipse.RadiusX);
			ellipse.RadiusY = 0.2f;
			Assert.AreEqual(0.2f, ellipse.RadiusY);
			Assert.AreEqual(0.3f, ellipse.MaxRadius);
		}

		[Test]
		public void RadiusY()
		{
			var ellipse = new OutlinedEllipse(Point.Half, 0.1f, 0.2f) { RadiusY = 0.3f, };
			Assert.AreEqual(0.3f, ellipse.RadiusY);
		}

		[Test]
		public void ColorProperty()
		{
			var ellipse = new OutlinedEllipse(Point.Half, 0.1f, 0.2f) { Color = Color.Green };
			Assert.AreEqual(Color.Green, ellipse.Color);
		}

		[Test]
		public void Rotation()
		{
			var ellipse = new OutlinedEllipse(Point.Half, 0.1f, 0.2f) { Rotation = 100 };
			ellipse.Rotation = 100;
			Assert.AreEqual(100.0f, ellipse.Rotation);
		}

		[VisualTest]
		public void DrawRotatingEllipse(Type resolver)
		{
			OutlinedEllipse ellipse = null;
			Start(resolver,
				(Renderer r) => r.Add(ellipse = new OutlinedEllipse(Point.Half, 0.4f, 0.2f) { Rotation = 45 }),
				(Time time) => { ellipse.Rotation += 45 * time.CurrentDelta; });
		}

		[VisualTest]
		public void DrawLotsOfEllipses(Type resolver)
		{
			Start(resolver, (Renderer renderer, Randomizer r) =>
			{
				for (int i = 0; i < 10; i++)
					renderer.Add(new OutlinedEllipse(new Point(r.Get(), r.Get()), r.Get(0.02f, 0.4f),
						r.Get(0.02f, 0.4f)) { Color = Color.GetRandomColor(), Rotation = r.Get(0, 360) });
			});
		}
	}
}