using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class ZoomingEffectTests : TestStarter
	{
		[Test]
		public void CreateZoomingEffect()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new ZoomingEffect(content.Load<Image>("test"), Rectangle.Zero, Rectangle.One);
			Assert.AreEqual(Point.Zero, effect.DrawArea.Center);
		}

		[Test]
		public void EmulateRunZoomingEffectForOneSecondToBeRemoved()
		{
			Start(typeof(TestResolver), (Content content, Renderer renderer) =>
			{
				var effect = new ZoomingEffect(content.Load<Image>("test"), Rectangle.Zero, Rectangle.One);
				renderer.Add(effect);
				Assert.AreEqual(1, renderer.NumberOfActiveRenderableObjects);
				testResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(0.25f, effect.DrawArea.Center.X, 0.1f);
				Assert.AreEqual(0.25f, effect.DrawArea.Center.Y, 0.1f);
				testResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.AreEqual(0.5f, effect.DrawArea.Center.X, 0.1f);
				Assert.AreEqual(0.5f, effect.DrawArea.Center.Y, 0.1f);
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[VisualTest]
		public void RenderZoomingLogo(Type resolver)
		{
			Start(resolver,
				(Content content, Renderer renderer) =>
					renderer.Add(new ZoomingEffect(content.Load<Image>("DeltaEngineLogo"),
						new Rectangle(Point.Half, Size.Zero), Rectangle.One, 2.0f)));
		}
	}
}