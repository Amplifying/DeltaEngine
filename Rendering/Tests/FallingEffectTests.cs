using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class FallingEffectTests : TestStarter
	{
		[Test]
		public void CreateFallingEffect()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FallingEffect(content.Load<Image>("test"), Point.Half, Size.One);
			Assert.AreEqual(Rectangle.One, effect.DrawArea);
		}

		[Test]
		public void CreateFallingEffectFromDrawArea()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FallingEffect(content.Load<Image>("test"), Rectangle.One);
			Assert.AreEqual(Rectangle.One, effect.DrawArea);
		}

		[Test]
		public void Velocity()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FallingEffect(content.Load<Image>("test"), Rectangle.One)
			{
				Velocity = Point.Half
			};
			Assert.AreEqual(Point.Half, effect.Velocity);
		}

		[Test]
		public void Gravity()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FallingEffect(content.Load<Image>("test"), Rectangle.One)
			{
				Gravity = Point.Half
			};
			Assert.AreEqual(Point.Half, effect.Gravity);
		}

		[Test]
		public void RotationSpeed()
		{
			var resolver = new TestResolver();
			var content = resolver.Resolve<Content>();
			var effect = new FallingEffect(content.Load<Image>("test"), Rectangle.One)
			{
				RotationSpeed = 1.0f
			};
			Assert.AreEqual(1.0f, effect.RotationSpeed);
		}

		[Test]
		public void FallingEffectIsRemovedAfterOneSecond()
		{
			Start(typeof(TestResolver), (Content content, Renderer renderer) =>
			{
				var effect = new FallingEffect(content.Load<Image>("test"), Rectangle.One)
				{
					Velocity = Point.Half,
					Gravity = new Point(1.0f, 2.0f),
					RotationSpeed = 100.0f
				};
				renderer.Add(effect);
				CheckFallingEffectStateAfterHalfASecond(effect, renderer);
				CheckFallingEffectStateAfterOneSecond(effect, renderer);
			});
		}

		private void CheckFallingEffectStateAfterHalfASecond(Sprite effect, Renderer renderer)
		{
			testResolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(0.879f, effect.DrawArea.Center.X, 0.01f);
			Assert.AreEqual(1.008f, effect.DrawArea.Center.Y, 0.01f);
			Assert.AreEqual(50.0f, effect.Rotation, 2.0f);
			Assert.AreEqual(1, renderer.NumberOfActiveRenderableObjects);
		}

		private void CheckFallingEffectStateAfterOneSecond(Sprite effect, Renderer renderer)
		{
			testResolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreEqual(1.534f, effect.DrawArea.Center.X, 0.01f);
			Assert.AreEqual(2.059f, effect.DrawArea.Center.Y, 0.01f);
			Assert.AreEqual(100.0f, effect.Rotation, 5.0f);
			Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
		}

		[VisualTest]
		public void RenderFallingLogo(Type resolver)
		{
			Start(resolver,
				(Content content, Renderer renderer) =>
					renderer.Add(new FallingEffect(content.Load<Image>("DeltaEngineLogo"), Point.Half,
						Size.Half, 2.0f) { RotationSpeed = 100.0f }));
		}
	}
}