using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class Camera2DControlledQuadraticScreenSpaceTests : TestStarter
	{
		[Test]
		public void LookAt()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new TestResolver().Resolve<Window>());
			Assert.AreEqual(Point.Half, camera.LookAt);
			camera.LookAt = Point.One;
			Assert.AreEqual(Point.One, camera.LookAt);
		}

		[Test]
		public void Zoom()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new TestResolver().Resolve<Window>());
			Assert.AreEqual(1.0f, camera.Zoom);
			camera.Zoom = 2.5f;
			Assert.AreEqual(2.5f, camera.Zoom);
		}

		[Test]
		public void IfCameraNotAdjustedItBehavesIdenticallyToQuadraticScreenSpace()
		{
			Start(typeof(TestResolver),
				(QuadraticScreenSpace q, Camera2DControlledQuadraticScreenSpace c) =>
				{
					Assert.AreEqual(q.FromPixelSpace(new Point(1, 2)), c.FromPixelSpace(new Point(1, 2)));
					Assert.AreEqual(q.FromPixelSpace(new Size(-3, 4)), c.FromPixelSpace(new Size(-3, 4)));
					Assert.AreEqual(q.ToPixelSpace(new Point(2, 6)), c.ToPixelSpace(new Point(2, 6)));
					Assert.AreEqual(q.ToPixelSpace(new Size(-2, 0)), c.ToPixelSpace(new Size(-2, 0)));
				});
		}

		[Test]
		public void IfCameraNotAdjustedEdgesAreIdenticalToQuadraticScreenSpace()
		{
			Start(typeof(TestResolver),
				(QuadraticScreenSpace q, Camera2DControlledQuadraticScreenSpace c) =>
				{
					Assert.AreEqual(q.TopLeft, c.TopLeft);
					Assert.AreEqual(q.BottomRight, c.BottomRight);
					Assert.AreEqual(q.Top, c.Top, 0.0001f);
					Assert.AreEqual(q.Left, c.Left, 0.0001f);
					Assert.AreEqual(q.Bottom, c.Bottom, 0.0001f);
					Assert.AreEqual(q.Right, c.Right, 0.0001f);
				});
		}

		[Test]
		public void EdgesAfterZoomingIn()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new TestResolver().Resolve<Window>())
			{
				Zoom = 2.0f
			};

			Assert.AreEqual(new Point(0.25f, 0.34375f), camera.TopLeft);
			Assert.AreEqual(new Point(0.75f, 0.65625f), camera.BottomRight);
			Assert.AreEqual(0.34375f, camera.Top, 0.0001f);
			Assert.AreEqual(0.25f, camera.Left, 0.0001f);
			Assert.AreEqual(0.65625f, camera.Bottom, 0.0001f);
			Assert.AreEqual(0.75f, camera.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterPanning()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new TestResolver().Resolve<Window>())
			{
				LookAt = new Point(0.75f, 0.6f)
			};

			Assert.AreEqual(new Point(0.25f, 0.2875f), camera.TopLeft);
			Assert.AreEqual(new Point(1.25f, 0.9125f), camera.BottomRight);
			Assert.AreEqual(0.2875f, camera.Top, 0.0001f);
			Assert.AreEqual(0.25f, camera.Left, 0.0001f);
			Assert.AreEqual(0.9125f, camera.Bottom, 0.0001f);
			Assert.AreEqual(1.25f, camera.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterPanningAndZooming()
		{
			var camera = new Camera2DControlledQuadraticScreenSpace(new TestResolver().Resolve<Window>())
			{
				LookAt = new Point(0.4f, 0.5f),
				Zoom = 0.5f
			};

			Assert.AreEqual(new Point(-0.6f, -0.125f), camera.TopLeft);
			Assert.AreEqual(new Point(1.4f, 1.125f), camera.BottomRight);
			Assert.AreEqual(-0.125f, camera.Top, 0.0001f);
			Assert.AreEqual(-0.6f, camera.Left, 0.0001f);
			Assert.AreEqual(1.125f, camera.Bottom, 0.0001f);
			Assert.AreEqual(1.4f, camera.Right, 0.0001f);
		}

		[Test]
		public void LoopingToAndFromPixelSpaceLeavesAPointUnchanged()
		{
			var window = new TestResolver().Resolve<Window>();
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(-0.5f, 0.6f),
				Zoom = 3.0f
			};

			Assert.AreEqual(new Point(1.2f, 3.4f),
				camera.ToPixelSpace(camera.FromPixelSpace(new Point(1.2f, 3.4f))));
			Assert.AreEqual(new Point(1.2f, 3.4f),
				camera.FromPixelSpace(camera.ToPixelSpace(new Point(1.2f, 3.4f))));
		}

		[Test]
		public void ToPixelSpace()
		{
			var window = new TestResolver().Resolve<Window>();
			var quadraticSize = new Size(window.ViewportPixelSize.Width);
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(-0.5f, 0.6f),
				Zoom = 2.0f
			};

			Assert.AreEqual(quadraticSize.Width * 1.5f, camera.ToPixelSpace(Point.Zero).X);
			Assert.AreEqual(new Point(2560, 115.2f), camera.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Point(3584, 1139.2f), camera.ToPixelSpace(Point.One));
			Assert.AreEqual(quadraticSize, camera.ToPixelSpace(Size.Half));
		}

		[Test]
		public void FromPixelSpace()
		{
			var window = new TestResolver().Resolve<Window>();
			var camera = new Camera2DControlledQuadraticScreenSpace(window)
			{
				LookAt = new Point(-0.5f, 0.6f),
				Zoom = 2.0f
			};

			Assert.AreEqual(new Point(-0.75f, 0.44375f), camera.FromPixelSpace(Point.Zero));
			Assert.AreEqual(new Point(-0.25f, 0.75625f),
				camera.FromPixelSpace((Point)window.ViewportPixelSize));
			Assert.AreEqual(camera.LookAt, camera.FromPixelSpace((Point)window.ViewportPixelSize / 2));
		}

		[VisualTest]
		public void RenderPanAndZoomIntoLogo(Type resolver)
		{
			Camera2DControlledQuadraticScreenSpace camera = null;
			float elapsed = 0.0f;

			Start(resolver,
				(Camera2DControlledQuadraticScreenSpace cam, Content content, Renderer renderer) =>
				{
					camera = cam;
					renderer.Add(new Sprite(content.Load<Image>("DeltaEngineLogo"),
						Rectangle.FromCenter(Point.One, new Size(0.25f))));
				}, (Time time) =>
				{
					elapsed += time.CurrentDelta;
					camera.LookAt = Point.Lerp(Point.Half, Point.One, elapsed / 2);
					camera.Zoom = MathExtensions.Lerp(1.0f, 2.0f, elapsed / 4);
				});
		}
	}
}