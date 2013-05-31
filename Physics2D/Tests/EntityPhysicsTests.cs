using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class EntityPhysicsTests : TestWithAllFrameworks
	{
		[Test]
		public void FallingEffectIsRemovedAfterOneSecond()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = CreateFallingSpriteWhichExpires(content);
				CheckSpriteAfterHalfASecond(sprite);
				CheckFallingEffectStateAfterOneSecond(sprite);
			});
		}

		private static Sprite CreateFallingSpriteWhichExpires(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new SimplePhysics.Data
			{
				Velocity = Point.Half,
				Gravity = new Point(1.0f, 2.0f),
				RotationSpeed = 100.0f,
				Duration = 1.0f
			});
			sprite.Add<SimplePhysics.Fall>();
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
			Start(resolver, (ContentLoader content) => { CreateFallingSprite(content); });
		}

		private Sprite CreateFallingSprite(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
			sprite.Add(new SimplePhysics.Data
			{
				Velocity = new Point(0.0f, -0.3f),
				RotationSpeed = 100.0f,
				Gravity = new Point(0.0f, 0.1f)
			});
			sprite.Add<SimplePhysics.Fall>();
			return sprite;
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half,
			new Size(0.2f, 0.2f));

		[VisualTest]
		public void RenderFallingLogoBouncingUsingTrigger(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				// When the sprite hits the bottom of the screen
				// - Bounce up
				// - Change color
				var trigger = new Trigger(entity => entity.Get<Rectangle>().Bottom > 0.75f);
				trigger.Fired += entity =>
				{
					var velocity = entity.Get<SimplePhysics.Data>().Velocity;
					entity.Get<SimplePhysics.Data>().Velocity = new Point(velocity.X, -velocity.Y.Abs());
				};
				trigger.Fired += entity => entity.Set(Color.GetRandomBrightColor());
				CreateFallingSprite(content).AddTrigger(trigger);
				if (resolver == typeof(MockResolver))
				{
					mockResolver.AdvanceTimeAndExecuteRunners(8);
				}
			});
		}

		[VisualTest]
		public void RenderFallingCircle(Type resolver)
		{
			Start(resolver, () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.1f, 0.1f, Color.Blue);
				ellipse.Add(new SimplePhysics.Data
				{
					Velocity = new Point(0.1f, -0.1f),
					Gravity = new Point(0.0f, 0.1f)
				});
				ellipse.Add<SimplePhysics.Fall>();
			});
		}

		[VisualTest]
		public void RenderEllipsemovingInScreenSpace(Type resolver)
		{
			Start(resolver, () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.1f, 0.06f, Color.Green);
				ellipse.Add(new SimplePhysics.Data
				{
					Velocity = new Point(0.4f, -0.4f),
					Gravity = new Point(0.0f, 0.0f)
				});
				ellipse.Add<SimplePhysics.BounceOffScreenEdges>();
			});
		}
	}
}