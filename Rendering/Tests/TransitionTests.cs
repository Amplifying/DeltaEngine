using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class TransitionTests : TestWithMocksOrVisually
	{
		[Test]
		public void SetTransitionColor()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.Color(Color.White, Color.Blue));
			Assert.AreEqual(Color.Blue, sprite.Get<Transition.Color>().End);
			sprite.Get<Transition.Color>().End = Color.Green;
			Assert.AreEqual(Color.Green, sprite.Get<Transition.Color>().End);
			Window.CloseAfterFrame();
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[Test]
		public void RenderFadingLogo()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.FadingColor());
			sprite.Add(new Transition.Duration(5.0f));
		}

		[Test]
		public void SetTransitionOutlineColor()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.OutlineColor(Color.White, Color.Blue));
			Assert.AreEqual(Color.Blue, sprite.Get<Transition.OutlineColor>().End);
			sprite.Get<Transition.OutlineColor>().End = Color.Green;
			Assert.AreEqual(Color.Green, sprite.Get<Transition.OutlineColor>().End);
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetTransitionPosition()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.Position(sprite.Center, Point.One));
			Assert.AreEqual(Point.One, sprite.Get<Transition.Position>().End);
			sprite.Get<Transition.Position>().End = Point.Half;
			Assert.AreEqual(Point.Half, sprite.Get<Transition.Position>().End);
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetTransitionSize()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.Size(sprite.Size, Size.One));
			Assert.AreEqual(Size.One, sprite.Get<Transition.Size>().End);
			sprite.Get<Transition.Size>().End = Size.Half;
			Assert.AreEqual(Size.Half, sprite.Get<Transition.Size>().End);
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetTransitionRotation()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.Rotation(0, 360));
			Assert.AreEqual(360, sprite.Get<Transition.Rotation>().End);
			sprite.Get<Transition.Rotation>().End = 180;
			Assert.AreEqual(180, sprite.Get<Transition.Rotation>().End);
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetTransitionDuration()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			sprite.Start<Transition>().Add(new Transition.Duration(2.0f) { Elapsed = 1.0f });
			Assert.AreEqual(2.0f, sprite.Get<Transition.Duration>().Value);
			Assert.AreEqual(1.0f, sprite.Get<Transition.Duration>().Elapsed);
			Window.CloseAfterFrame();
		}

		[Test]
		public void TransitionSpriteColor()
		{
			Sprite sprite = CreateSpriteWithChangingColor();
			bool transitionEnded = false;
			sprite.Messaged += message =>
			{
				if (message is Transition.TransitionEnded)
					transitionEnded = true;
			};
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(0.5f, sprite.AlphaValue, 0.1f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(sprite.AlphaValue < 0.1f);
			Assert.IsTrue(transitionEnded);
			Window.CloseAfterFrame();
		}

		private static Sprite CreateSpriteWithChangingColor()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"), Rectangle.One);
			sprite.Start<Transition>().Add(new Transition.Duration(1.0f)).Add(
				new Transition.FadingColor(Color.Red));
			return sprite;
		}

		[Test]
		public void WhenTransitionEndsSpriteStaysActive()
		{
			Sprite sprite = CreateSpriteWithChangingColor();
			Console.WriteLine(sprite.ToString());
			resolver.AdvanceTimeAndExecuteRunners(2.0f);
			Console.WriteLine(sprite.ToString());
			Assert.IsTrue(sprite.IsActive);
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
			Window.CloseAfterFrame();
		}

		[Test]
		public void WhenTransitionEndsSpriteIsRemoved()
		{
			Sprite sprite = CreateSpriteWithChangingColor();
			sprite.Stop<Transition>();
			sprite.Start<FinalTransition>();
			resolver.AdvanceTimeAndExecuteRunners(2.0f);
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			Window.CloseAfterFrame();
		}

		[Test]
		public void TransitionRectColor()
		{
			FilledRect rect = CreateRectWithChangingColor();
			bool transitionEnded = false;
			rect.Messaged += message =>
			{
				if (message is Transition.TransitionEnded)
					transitionEnded = true;
			};
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(0.5f, rect.AlphaValue, 0.1f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(rect.AlphaValue < 0.1f);
			Assert.IsTrue(transitionEnded);
			Window.CloseAfterFrame();
		}

		private static FilledRect CreateRectWithChangingColor()
		{
			var rect = new FilledRect(Rectangle.One, Color.Blue);
			rect.Add(new Transition.Duration(1.0f)).Add(new Transition.FadingColor(Color.Red));
			rect.Start<Transition>();
			return rect;
		}

		[Test]
		public void TransitionSpriteDrawArea()
		{
			var sprite = CreateSpriteWithChangingDrawArea();
			bool transitionEnded = false;
			sprite.Messaged += message =>
			{
				if (message is Transition.TransitionEnded)
					transitionEnded = true;
			};
			resolver.AdvanceTimeAndExecuteRunners(0.75f);
			Assert.AreEqual(0.75f, sprite.DrawArea.Center.X, 0.05f);
			Assert.AreEqual(0.75f, sprite.DrawArea.Center.Y, 0.05f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(sprite.DrawArea.Center.X > 0.9f);
			Assert.IsTrue(sprite.DrawArea.Center.Y > 0.9f);
			Assert.IsTrue(transitionEnded);
			Window.CloseAfterFrame();
		}

		private static Sprite CreateSpriteWithChangingDrawArea()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"), Rectangle.Zero);
			sprite.Add(new Transition.Position(Point.Zero, Point.One));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<Transition>();
			return sprite;
		}

		[Test]
		public void RenderZoomingLogo()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"),
				Rectangle.FromCenter(Point.Half, new Size(0.1f, 0.1f)));
			sprite.Add(new Transition.Size(Size.Zero, Size.One));
			sprite.Add(new Transition.Duration(4.0f));
			sprite.Start<Transition>();
		}

		[Test]
		public void TransitionSpriteRotation()
		{
			var sprite = CreateRotatingSprite();
			bool transitionEnded = false;
			sprite.Messaged += message =>
			{
				if (message is Transition.TransitionEnded)
					transitionEnded = true;
			};
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(180, sprite.Rotation, 10);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreEqual(360, sprite.Rotation, 10);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(transitionEnded);
			Window.CloseAfterFrame();
		}

		private static Sprite CreateRotatingSprite()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"), Rectangle.One);
			sprite.Add(new Transition.Rotation(0, 360));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<Transition>();
			return sprite;
		}

		[Test]
		public void ChangeOutlineColorSprite()
		{
			var sprite = CreateOutlineColorSprite();
			bool transitionEnded = false;
			sprite.Messaged += message =>
			{
				if (message is Transition.TransitionEnded)
					transitionEnded = true;
			};
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(130, sprite.Get<OutlineColor>().Value.B, 10f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreEqual(0, sprite.Get<OutlineColor>().Value.B, 10f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(transitionEnded);
			Window.CloseAfterFrame();
		}

		private static Sprite CreateOutlineColorSprite()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"), Rectangle.One);
			sprite.Add(new OutlineColor(Color.Blue));
			sprite.Add(new Transition.OutlineColor(Color.Blue, Color.Red));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<Transition>();
			return sprite;
		}

		[Test]
		public void ChangeSizeSprite()
		{
			var sprite = CreateSizeSprite();
			bool transitionEnded = false;
			sprite.Messaged += message =>
			{
				if (message is Transition.TransitionEnded)
					transitionEnded = true;
			};
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.AreEqual(0.5f, sprite.Size.Height, 0.02f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreEqual(1.0f, sprite.Size.Height, 0.02f);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.IsTrue(transitionEnded);
			Window.CloseAfterFrame();
		}

		private static Sprite CreateSizeSprite()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"), Rectangle.One);
			sprite.Add(new Transition.Size(Size.Zero, Size.One));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<Transition>();
			return sprite;
		}

		[Test]
		public void RenderRotatingLogo()
		{
			var sprite = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"), screenCenter);
			sprite.Add(new Transition.Rotation(0, 360));
			sprite.Add(new Transition.Duration(4.0f));
			sprite.Start<Transition>();
		}

		[Test]
		public void RenderingFadingLine()
		{
			var sprite = new Line2D(Point.Zero, Point.One, Color.White);
			sprite.Add(new Transition.FadingColor(Color.Red));
			sprite.Add(new Transition.Duration(4.0f));
			sprite.Start<Transition>();
		}

		[Test]
		public void RenderingChangingOutlineColor()
		{
			var rect = new FilledRect(screenCenter, Color.Blue);
			rect.Stop<Polygon.Render>();
			rect.Start<Polygon.RenderOutline>();
			rect.Add(new OutlineColor(Color.Yellow));
			rect.Add(new Transition.OutlineColor(Color.Yellow, Color.Red));
			rect.Add(new Transition.Duration(4.0f));
			rect.Start<Transition>();
		}
	}
}