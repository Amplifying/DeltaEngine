using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class FadeoutEffectTests : TestStarter
	{
		[Test]
		public void CreateFadeoutEffect()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FadeoutEffect(content.Load<Image>("test"), Point.Half, Size.One);
			Assert.IsTrue(effect.IsVisible);
			Assert.AreEqual(Point.Half, effect.DrawArea.Center);
			Assert.AreEqual(Color.White, effect.Color);
		}

		[Test]
		public void EmulateRunFadeoutEffectForOneSecondToFadeoutAndBeRemoved()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FadeoutEffect(content.Load<Image>("test"), Point.Half, Size.One);
			var renderer = resolver.Resolve<Renderer>();
			renderer.Add(effect);
			Assert.AreEqual(1, renderer.NumberOfActiveRenderableObjects);
			Assert.AreEqual(Color.White, effect.Color);
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.IsTrue(effect.Color.AlphaValue.IsNearlyEqual(0.5f, 0.1f),
				"effect alpha should be 0.5, but is " + effect.Color.AlphaValue);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(effect.Color.A < 10);
			Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
		}

		[VisualTest]
		public void RenderLogoAndFadeout(Type resolver)
		{
			Start(resolver,
				(Content content, Renderer renderer) =>
					renderer.Add(new FadeoutEffect(content.Load<Image>("DeltaEngineLogo"), Point.Half,
						Size.Half, 2.0f)));
		}
	}
}