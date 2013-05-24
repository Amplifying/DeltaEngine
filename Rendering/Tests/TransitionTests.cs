using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class TransitionTests : TestWithAllFrameworks
	{
		[Test]
		public void SetTransitionColor()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<Transition>().Add(new Transition.Color(Color.White, Color.Blue));
				Assert.AreEqual(Color.Blue, sprite.Get<Transition.Color>().End);
				sprite.Get<Transition.Color>().End = Color.Green;
				Assert.AreEqual(Color.Green, sprite.Get<Transition.Color>().End);
			});
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[VisualTest]
		public void RenderFadingLogo(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Add<Transition>().Add(new Transition.FadingColor(Color.White));
				sprite.Add(new Transition.Duration(5.0f));
			});
		}

		[Test]
		public void SetTransitionOutlineColor()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);

				sprite.Add<Transition>().Add(new Transition.OutlineColor(Color.White, Color.Blue));
				Assert.AreEqual(Color.Blue, sprite.Get<Transition.OutlineColor>().End);
				sprite.Get<Transition.OutlineColor>().End = Color.Green;
				Assert.AreEqual(Color.Green, sprite.Get<Transition.OutlineColor>().End);
			});
		}

		[Test]
		public void SetTransitionPosition()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<Transition>().Add(new Transition.Position(sprite.Center, Point.One));
				Assert.AreEqual(Point.One, sprite.Get<Transition.Position>().End);
				sprite.Get<Transition.Position>().End = Point.Half;
				Assert.AreEqual(Point.Half, sprite.Get<Transition.Position>().End);
			});
		}

		[Test]
		public void SetTransitionSize()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<Transition>().Add(new Transition.Size(sprite.Size, Size.One));
				Assert.AreEqual(Size.One, sprite.Get<Transition.Size>().End);
				sprite.Get<Transition.Size>().End = Size.Half;
				Assert.AreEqual(Size.Half, sprite.Get<Transition.Size>().End);
			});
		}

		[Test]
		public void SetTransitionRotation()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<Transition>().Add(new Transition.Rotation(0, 360));
				Assert.AreEqual(360, sprite.Get<Transition.Rotation>().End);
				sprite.Get<Transition.Rotation>().End = 180;
				Assert.AreEqual(180, sprite.Get<Transition.Rotation>().End);
			});
		}

		[Test]
		public void SetTransitionDuration()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				sprite.Add<Transition>().Add(new Transition.Duration(2.0f) { Elapsed = 1.0f });
				Assert.AreEqual(2.0f, sprite.Get<Transition.Duration>().Value);
				Assert.AreEqual(1.0f, sprite.Get<Transition.Duration>().Elapsed);
			});
		}

		[Test]
		public void TransitionSpriteColor()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				Sprite sprite = CreateSpriteWithChangingColor(content);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is Transition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(0.5f, sprite.AlphaValue, 0.1f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(sprite.AlphaValue < 0.1f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Sprite CreateSpriteWithChangingColor(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add<Transition>().Add(new Transition.Duration(1.0f)).Add(
				new Transition.FadingColor(Color.Red));
			return sprite;
		}

		[Test]
		public void WhenTransitionEndsSpriteStaysActive()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				Sprite sprite = CreateSpriteWithChangingColor(content);
				Console.WriteLine(sprite.ToString());
				mockResolver.AdvanceTimeAndExecuteRunners(2.0f);
				Console.WriteLine(sprite.ToString());
				Assert.IsTrue(sprite.IsActive);
				Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
			});
		}

		[Test]
		public void WhenTransitionEndsSpriteIsRemoved()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				Sprite sprite = CreateSpriteWithChangingColor(content);
				sprite.Remove<Transition>();
				sprite.Add<FinalTransition>();
				mockResolver.AdvanceTimeAndExecuteRunners(2.0f);
				Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			});
		}

		[Test]
		public void TransitionRectColor()
		{
			Start(typeof(MockResolver), () =>
			{
				Rect rect = CreateRectWithChangingColor();
				bool transitionEnded = false;
				rect.Messaged += message =>
				{
					if (message is Transition.TransitionEnded)
						transitionEnded = true;
				};
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
				Assert.AreEqual(0.5f, rect.AlphaValue, 0.1f);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.IsTrue(rect.AlphaValue < 0.1f);
				Assert.IsTrue(transitionEnded);
			});
		}

		private static Rect CreateRectWithChangingColor()
		{
			var rect = new Rect(Rectangle.One, Color.Blue);
			rect.Add(new Transition.Duration(1.0f)).Add(new Transition.FadingColor(Color.Red));
			rect.Add<Transition>();
			return rect;
		}

		[Test]
		public void TransitionSpriteDrawArea()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = CreateSpriteWithChangingDrawArea(content);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is Transition.TransitionEnded)
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

		private static Sprite CreateSpriteWithChangingDrawArea(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.Zero);
			sprite.Add(new Transition.Position(Point.Zero, Point.One));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Add<Transition>();
			return sprite;
		}

		[VisualTest]
		public void RenderZoomingLogo(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"),
					Rectangle.FromCenter(Point.Half, new Size(0.1f, 0.1f)));
				sprite.Add(new Transition.Size(Size.Zero, Size.One));
				sprite.Add(new Transition.Duration(4.0f));
				sprite.Add<Transition>();
			});
		}

		[Test]
		public void TransitionSpriteRotation()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = CreateRotatingSprite(content);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is Transition.TransitionEnded)
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

		private static Sprite CreateRotatingSprite(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new Transition.Rotation(0, 360));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Add<Transition>();
			return sprite;
		}

		[Test]
		public void ChangeOutlineColorSprite()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = CreateOutlineColorSprite(content);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is Transition.TransitionEnded)
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

		private static Sprite CreateOutlineColorSprite(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new OutlineColor(Color.Blue));
			sprite.Add(new Transition.OutlineColor(Color.Blue, Color.Red));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Add<Transition>();
			return sprite;
		}

		[Test]
		public void ChangeSizeSprite()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var sprite = CreateSizeSprite(content);
				bool transitionEnded = false;
				sprite.Messaged += message =>
				{
					if (message is Transition.TransitionEnded)
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

		private static Sprite CreateSizeSprite(ContentLoader content)
		{
			var sprite = new Sprite(content.Load<Image>("test"), Rectangle.One);
			sprite.Add(new Transition.Size(Size.Zero, Size.One));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Add<Transition>();
			return sprite;
		}

		[VisualTest]
		public void RenderRotatingLogo(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Add(new Transition.Rotation(0, 360));
				sprite.Add(new Transition.Duration(4.0f));
				sprite.Add<Transition>();
			});
		}

		[VisualTest]
		public void RenderingFadingLine(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var sprite = new Line2D(Point.Zero, Point.One, Color.White);
				sprite.Add(new Transition.FadingColor(Color.Red));
				sprite.Add(new Transition.Duration(4.0f));
				sprite.Add<Transition>();
			});
		}

		[VisualTest]
		public void RenderingChangingOutlineColor(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var rect = new Rect(screenCenter, Color.Blue);
				rect.Remove<Polygon.Render>();
				rect.Add<Polygon.RenderOutline>();
				rect.Add(new OutlineColor(Color.Yellow));
				rect.Add(new Transition.OutlineColor(Color.Yellow, Color.Red));
				rect.Add(new Transition.Duration(4.0f));
				rect.Add<Transition>();
			});
		}
	}
}