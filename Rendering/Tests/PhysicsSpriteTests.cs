using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class PhysicsSpriteTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateFallingEffect()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var effect = new PhysicsSprite(content.Load<Image>("test"), Rectangle.One);
				Assert.AreEqual(Rectangle.One, effect.DrawArea);
			});
		}

		[Test]
		public void SetVelocity()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var effect = new PhysicsSprite(content.Load<Image>("test"), Rectangle.One)
				{
					Velocity = Point.Half
				};
				Assert.AreEqual(Point.Half, effect.Velocity);
			});
		}

		[Test]
		public void SetGravity()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var effect = new PhysicsSprite(content.Load<Image>("test"), Rectangle.One)
				{
					Gravity = Point.Half
				};
				Assert.AreEqual(Point.Half, effect.Gravity);
			});
		}

		[Test]
		public void SetRotationSpeed()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var effect = new PhysicsSprite(content.Load<Image>("test"), Rectangle.One)
				{
					RotationSpeed = 1.0f
				};
				Assert.AreEqual(1.0f, effect.RotationSpeed);
			});
		}

		[Test]
		public void SetPhysicsDuration()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var effect = new PhysicsSprite(content.Load<Image>("test"), Rectangle.One)
				{
					PhysicsDuration = 1.0f
				};
				Assert.AreEqual(1.0f, effect.PhysicsDuration);
			});
		}

		[Test]
		public void FallingEffectIsRemovedAfterOneSecond()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = CreateFallingSprite(content, entitySystem);
				CheckSpriteAfterHalfASecond(sprite);
				CheckFallingEffectStateAfterOneSecond(sprite);
			});
		}

		private static PhysicsSprite CreateFallingSprite(ContentLoader content,
			EntitySystem entitySystem)
		{
			var sprite = new PhysicsSprite(content.Load<Image>("test"), Rectangle.One)
			{
				Velocity = Point.Half,
				Gravity = new Point(1.0f, 2.0f),
				RotationSpeed = 100.0f,
				PhysicsDuration = 1.0f
			};
			sprite.Add<Fall>();
			entitySystem.Add(sprite);
			return sprite;
		}

		private void CheckSpriteAfterHalfASecond(Entity2D entity)
		{
			mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(0.879f, entity.DrawArea.Center.X, 0.01f);
			Assert.AreEqual(1.008f, entity.DrawArea.Center.Y, 0.01f);
			Assert.AreEqual(50.0f, entity.Rotation, 2.0f);
		}

		private void CheckFallingEffectStateAfterOneSecond(Entity2D entity)
		{
			mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreEqual(1.534f, entity.DrawArea.Center.X, 0.01f);
			Assert.AreEqual(2.059f, entity.DrawArea.Center.Y, 0.01f);
			Assert.AreEqual(100.0f, entity.Rotation, 5.0f);
		}

		[VisualTest]
		public void RenderFallingLogo(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = new PhysicsSprite(content.Load<Image>("DeltaEngineLogo"), ScreenCenter)
				{
					Velocity = new Point(0.0f, -1.0f),
					RotationSpeed = 100.0f
				};
				sprite.Add<Fall>();
				entitySystem.Add(sprite);
			});
		}

		private static readonly Rectangle ScreenCenter = Rectangle.FromCenter(Point.Half,
			new Size(0.2f, 0.2f));
	}
}