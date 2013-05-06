using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Rendering.Transitions;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	[Category("Slow")]
	public class TransitionTests : TestWithAllFrameworks
	{
		[Test]
		public void SetTransitionColor()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);

				sprite.Add<PerformTransition>().Add(new TransitionColor
				{
					End = Color.Blue,
					Start = sprite.Color
				});
				Assert.AreEqual(Color.Blue, sprite.Get<TransitionColor>().End);
				sprite.Get<TransitionColor>().End = Color.Green;
				Assert.AreEqual(Color.Green, sprite.Get<TransitionColor>().End);
			});
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[VisualTest]
		public void RenderFadingLogo(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Add<PerformTransition>().Add(new TransitionColor
				{
					Start = Color.White,
					End = Color.TransparentWhite,
				});
				sprite.Add(new Transition
				{
					Duration = 5,
					Elapsed = 0,
					IsEntityToBeRemovedWhenFinished = true
				});

				entitySystem.Add(sprite);
			});
		}

		[Test]
		public void SetTransitionOutlineColor()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);

				sprite.Add<PerformTransition>().Add(new TransitionOutlineColor
				{
					End = Color.Blue,
					Start = sprite.Color
				});
				Assert.AreEqual(Color.Blue, sprite.Get<TransitionOutlineColor>().End);
				sprite.Get<TransitionOutlineColor>().End = Color.Green;
				Assert.AreEqual(Color.Green, sprite.Get<TransitionOutlineColor>().End);
			});
		}

		[Test]
		public void SetTransitionPosition()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<PerformTransition>().Add(new TransitionPosition
				{
					End = Point.One,
					Start = sprite.DrawArea.Center
				});
				Assert.AreEqual(Point.One, sprite.Get<TransitionPosition>().End);
				sprite.Get<TransitionPosition>().End = Point.Half;
				Assert.AreEqual(Point.Half, sprite.Get<TransitionPosition>().End);
			});
		}

		[Test]
		public void SetTransitionSize()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<PerformTransition>().Add(new TransitionSize
				{
					End = Size.One,
					Start = sprite.Size
				});
				Assert.AreEqual(Size.One, sprite.Get<TransitionSize>().End);
				sprite.Get<TransitionSize>().End = Size.Half;
				Assert.AreEqual(Size.Half, sprite.Get<TransitionSize>().End);
			});
		}

		[Test]
		public void SetTransitionRotation()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<PerformTransition>().Add(new TransitionRotation
				{
					End = 360,
					Start = sprite.Rotation
				});
				Assert.AreEqual(360, sprite.Get<TransitionRotation>().End);
				sprite.Get<TransitionRotation>().End = 180;
				Assert.AreEqual(180, sprite.Get<TransitionRotation>().End);
			});
		}

		[Test]
		public void SetTransitionDuration()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<PerformTransition>().Add(new Transition { Duration = 1.0f, Elapsed = 1.0f });
				Assert.AreEqual(1.0f, sprite.Get<Transition>().Duration);
				Assert.AreEqual(1.0f, sprite.Get<Transition>().Elapsed);
			});
		}

		[Test]
		public void TransitionSpriteColor()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				Sprite sprite = CreateSpriteWithChangingColor(content, entitySystem);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is PerformTransition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(0.5f, sprite.AlphaValue, 0.1f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(sprite.AlphaValue < 0.1f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Sprite CreateSpriteWithChangingColor(ContentLoader content,
			EntitySystem entitySystem)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add<PerformTransition>().Add(new Transition { Duration = 1.0f }).Add(
				new TransitionColor { End = Color.TransparentWhite, Start = Color.Red });
			entitySystem.Add(sprite);
			return sprite;
		}

		[Test]
		public void WhenTransitionEndsSpriteStaysActive()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				Sprite sprite = CreateSpriteWithChangingColor(content, entitySystem);
				mockResolver.AdvanceTimeAndExecuteRunners(2.0f);
				Assert.IsTrue(sprite.IsActive);
				Assert.AreEqual(1, entitySystem.NumberOfEntities);
			});
		}

		[Test]
		public void WhenTransitionEndsSpriteIsRemoved()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				Sprite sprite = CreateSpriteWithChangingColor(content, entitySystem);
				sprite.Set(new Transition
				{
					Duration = 1,
					Elapsed = 0,
					IsEntityToBeRemovedWhenFinished = true
				});
				mockResolver.AdvanceTimeAndExecuteRunners(2.0f);
				Assert.AreEqual(0, entitySystem.NumberOfEntities);
			});
		}

		[Test]
		public void TransitionRectColor()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem) =>
			{
				Rect rect = CreateRectWithChangingColor(entitySystem);
				bool transitionEnded = false;
				rect.Messaged += message =>
				{
					if (message is PerformTransition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(0.5f, rect.AlphaValue, 0.1f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(rect.AlphaValue < 0.1f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Rect CreateRectWithChangingColor(EntitySystem entitySystem)
		{
			var rect = new Rect(Rectangle.One, Color.Blue);
			rect.Add(new Transition { Duration = 1, Elapsed = 0 }).Add(new TransitionColor
			{
				Start = Color.Red,
				End = Color.TransparentWhite
			});
			rect.Add<PerformTransition>();
			entitySystem.Add(rect);
			return rect;
		}

		[Test]
		public void TransitionSpriteDrawArea()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = CreateSpriteWithChangingDrawArea(content, entitySystem);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is PerformTransition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.75f);
				Assert.AreEqual(0.75f, sprite.DrawArea.Center.X, 0.05f);
				Assert.AreEqual(0.75f, sprite.DrawArea.Center.Y, 0.05f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(sprite.DrawArea.Center.X > 0.9f);
				Assert.IsTrue(sprite.DrawArea.Center.Y > 0.9f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Sprite CreateSpriteWithChangingDrawArea(ContentLoader content,
			EntitySystem entitySystem)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.Zero);
			sprite.Add(new TransitionPosition { End = Point.One, Start = Point.Zero });
			sprite.Add(new Transition { Duration = 1, Elapsed = 0 });
			sprite.Add<PerformTransition>();
			entitySystem.Add(sprite);
			return sprite;
		}

		[VisualTest]
		public void RenderZoomingLogo(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"),
					Rectangle.FromCenter(Point.Half, new Size(0.1f, 0.1f)));
				sprite.Add(new TransitionSize { Start = Size.Zero, End = Size.One });
				sprite.Add(new Transition { Duration = 4, Elapsed = 0 });
				sprite.Add<PerformTransition>();
				entitySystem.Add(sprite);
			});
		}

		[Test]
		public void TransitionSpriteRotation()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = CreateRotatingSprite(content, entitySystem);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is PerformTransition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(180, sprite.Rotation, 10);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.AreEqual(360, sprite.Rotation, 10);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Sprite CreateRotatingSprite(ContentLoader content, EntitySystem entitySystem)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new TransitionRotation { End = 360, Start = 0 });
			sprite.Add(new Transition
			{
				Duration = 1,
				Elapsed = 0,
				IsEntityToBeRemovedWhenFinished = true
			});
			sprite.Add<PerformTransition>();
			entitySystem.Add(sprite);
			return sprite;
		}

		[Test, Category("Slow")]
		public void ChangeOutlineColorSprite()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = CreateOutlineColorSprite(content, entitySystem);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is PerformTransition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(130, sprite.Get<OutlineColor>().Value.B, 10f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.AreEqual(0, sprite.Get<OutlineColor>().Value.B, 10f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Sprite CreateOutlineColorSprite(ContentLoader content,
			EntitySystem entitySystem)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new OutlineColor(Color.Yellow));
			sprite.Add(new TransitionOutlineColor { End = Color.Red, Start = Color.Blue, });
			sprite.Add(new Transition
			{
				Duration = 1,
				Elapsed = 0,
				IsEntityToBeRemovedWhenFinished = true
			});
			sprite.Add<PerformTransition>();
			entitySystem.Add(sprite);
			return sprite;
		}

		[Test, Category("Slow")]
		public void ChangeSizeSprite()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = CreateSizeSprite(content, entitySystem);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is PerformTransition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(0.5f, sprite.Size.Height, 0.02f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.AreEqual(1.0f, sprite.Size.Height, 0.02f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Sprite CreateSizeSprite(ContentLoader content,
			EntitySystem entitySystem)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new TransitionSize(){End = Size.One, Start = Size.Zero});
			sprite.Add(new Transition
			{
				Duration = 1,
				Elapsed = 0,
				IsEntityToBeRemovedWhenFinished = true
			});
			sprite.Add<PerformTransition>();
			entitySystem.Add(sprite);
			return sprite;
		}

		[VisualTest]
		public void RenderRotatingLogo(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Add(new TransitionRotation { End = 360, Start = 0 });
				sprite.Add(new Transition
				{
					Duration = 4,
					Elapsed = 0,
					IsEntityToBeRemovedWhenFinished = true
				});
				sprite.Add<PerformTransition>();
				entitySystem.Add(sprite);
			});
		}

		[VisualTest]
		public void RenderingFadingLine(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = new Line2D(Point.Zero, Point.One, Color.White);
				sprite.Add(new TransitionColor { End = Color.TransparentWhite, Start = Color.Red });
				sprite.Add(new Transition { Duration = 4, Elapsed = 0 });
				sprite.Add<PerformTransition>();
				entitySystem.Add(sprite);
			});
		}

		[VisualTest]
		public void RenderingChangingOutlineColor(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Add(new TransitionOutlineColor { End = Color.Blue, Start = Color.Red });
				sprite.Add(new OutlineColor(Color.Yellow));
				sprite.Add(new Transition
				{
					Duration = 4,
					Elapsed = 0,
					IsEntityToBeRemovedWhenFinished = true
				});
				sprite.Add<PerformTransition>();
				entitySystem.Add(sprite);
			});
		}
	}
}