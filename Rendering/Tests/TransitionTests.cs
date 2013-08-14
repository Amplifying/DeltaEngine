using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class TransitionTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateSpriteWithHugeDuration()
		{
			sprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"),
				Rectangle.HalfCentered);
			sprite.Add(new Transition.Duration(200000));
		}

		private Sprite sprite;

		[Test]
		public void RenderFadingLogo()
		{
			sprite.Start<Transition>().Add(new Transition.FadingColor());
		}

		[Test, CloseAfterFirstFrame]
		public void SetTransitionColor()
		{
			sprite.Start<Transition>().Add(new Transition.ColorRange(Color.White, Color.Blue));
			Assert.AreEqual(Color.Blue, sprite.Get<Transition.ColorRange>().End);
			sprite.Set(new Transition.ColorRange(Color.White, Color.Green));
			Assert.AreEqual(Color.Green, sprite.Get<Transition.ColorRange>().End);
		}

		[Test, CloseAfterFirstFrame]
		public void SetTransitionOutlineColor()
		{
			sprite.Start<Transition>().Add(new Transition.OutlineColor(Color.White, Color.Blue));
			Assert.AreEqual(Color.Blue, sprite.Get<Transition.OutlineColor>().End);
			sprite.Set(new Transition.OutlineColor(Color.White, Color.Green));
			Assert.AreEqual(Color.Green, sprite.Get<Transition.OutlineColor>().End);
		}

		[Test, CloseAfterFirstFrame]
		public void SetTransitionPosition()
		{
			sprite.Start<Transition>().Add(new Transition.Position(sprite.Center, Point.One));
			Assert.AreEqual(Point.One, sprite.Get<Transition.Position>().End);
			sprite.Set(new Transition.Position(sprite.Center, Point.Half));
			Assert.AreEqual(Point.Half, sprite.Get<Transition.Position>().End);
		}

		[Test, CloseAfterFirstFrame]
		public void SetTransitionSize()
		{
			sprite.Start<Transition>().Add(new Transition.SizeRange(sprite.Size, Size.One));
			Assert.AreEqual(Size.One, sprite.Get<Transition.SizeRange>().End);
			sprite.Set(new Transition.SizeRange(sprite.Size, Size.Half));
			Assert.AreEqual(Size.Half, sprite.Get<Transition.SizeRange>().End);
		}

		[Test, CloseAfterFirstFrame]
		public void SetTransitionRotation()
		{
			sprite.Start<Transition>().Add(new Transition.Rotation(0, 360));
			Assert.AreEqual(360, sprite.Get<Transition.Rotation>().End);
			sprite.Set(new Transition.Rotation(0, 180));
			Assert.AreEqual(180, sprite.Get<Transition.Rotation>().End);
		}

		[Test, CloseAfterFirstFrame]
		public void SetTransitionDuration()
		{
			sprite.Start<Transition>().Set(new Transition.Duration(2.0f) { Elapsed = 1.0f });
			Assert.AreEqual(2.0f, sprite.Get<Transition.Duration>().Value);
			Assert.AreEqual(1.0f, sprite.Get<Transition.Duration>().Elapsed);
		}

		[Test]
		public void TransitionSpriteColor()
		{
			Sprite alphaSprite = CreateSpriteWithChangingColor();
			alphaSprite.Stop<Transition>();
			alphaSprite.Start<FinalTransition>();
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(0.5f, alphaSprite.Alpha, 0.1f);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsTrue(alphaSprite.Alpha < 0.1f);
			Assert.IsFalse(alphaSprite.IsActive);
		}

		private static Sprite CreateSpriteWithChangingColor()
		{
			var sprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"), Rectangle.One);
			sprite.Start<Transition>().Add(new Transition.Duration(1.0f)).Add(
				new Transition.FadingColor(Color.Red));
			return sprite;
		}

		[Test, CloseAfterFirstFrame]
		public void WhenTransitionEndsSpriteStaysActive()
		{
			var recoloredSprite = CreateSpriteWithChangingColor();
			Console.WriteLine(recoloredSprite.ToString());
			AdvanceTimeAndUpdateEntities(2.0f);
			Console.WriteLine(recoloredSprite.ToString());
			Assert.IsTrue(recoloredSprite.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void WhenTransitionEndsSpriteIsRemoved()
		{
			var recoloredSprite = CreateSpriteWithChangingColor();
			recoloredSprite.Stop<Transition>();
			recoloredSprite.Start<FinalTransition>();
			AdvanceTimeAndUpdateEntities(2.0f);
			Assert.IsFalse(recoloredSprite.IsActive);
		}

		[Test]
		public void TransitionRectColor()
		{
			var rect = CreateRectWithChangingColor();
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(0.5f, rect.Alpha, 0.1f);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsTrue(rect.Alpha < 0.1f);
			Assert.IsFalse(rect.IsActive);
		}

		private static FilledRect CreateRectWithChangingColor()
		{
			var rect = new FilledRect(Rectangle.One, Color.Blue);
			rect.Add(new Transition.Duration(1.0f)).Add(new Transition.FadingColor(Color.Red));
			rect.Start<FinalTransition>();
			return rect;
		}

		[Test, CloseAfterFirstFrame]
		public void TransitionSpriteDrawArea()
		{
			var moveableSprite = CreateSpriteWithChangingDrawArea();
			AdvanceTimeAndUpdateEntities(0.75f);
			Assert.AreEqual(0.75f, moveableSprite.DrawArea.Center.X, 0.05f);
			Assert.AreEqual(0.75f, moveableSprite.DrawArea.Center.Y, 0.05f);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsTrue(moveableSprite.DrawArea.Center.X > 0.9f);
			Assert.IsTrue(moveableSprite.DrawArea.Center.Y > 0.9f);
			Assert.IsFalse(moveableSprite.IsActive);
		}

		private static Sprite CreateSpriteWithChangingDrawArea()
		{
			var sprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"), Rectangle.Zero);
			sprite.Add(new Transition.Position(Point.Zero, Point.One));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<FinalTransition>();
			return sprite;
		}

		[Test]
		public void RenderZoomingLogo()
		{
			var logoSprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"),
				Rectangle.FromCenter(Point.Half, new Size(0.1f, 0.1f)));
			logoSprite.Add(new Transition.SizeRange(Size.Zero, Size.One));
			logoSprite.Add(new Transition.Duration(4.0f));
			logoSprite.Start<Transition>();
		}

		[Test, CloseAfterFirstFrame]
		public void TransitionSpriteRotation()
		{
			var rotatingSprite = CreateRotatingSprite();
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(180, rotatingSprite.Rotation, 20);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(360, rotatingSprite.Rotation, 20);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsFalse(rotatingSprite.IsActive);
		}

		private static Sprite CreateRotatingSprite()
		{
			var sprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"), Rectangle.One);
			sprite.Add(new Transition.Rotation(0, 360));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<FinalTransition>();
			return sprite;
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeOutlineColorSprite()
		{
			var outlineColorSprite = CreateOutlineColorSprite();
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(130, outlineColorSprite.Get<OutlineColor>().Value.B, 12);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(0, outlineColorSprite.Get<OutlineColor>().Value.B, 12);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsFalse(outlineColorSprite.IsActive);
		}

		private static Sprite CreateOutlineColorSprite()
		{
			var sprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"), Rectangle.One);
			sprite.Add(new OutlineColor(Color.Blue));
			sprite.Add(new Transition.OutlineColor(Color.Blue, Color.Red));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<FinalTransition>();
			return sprite;
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeSizeSprite()
		{
			var sizeSprite = CreateSizeSprite();
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(0.5f, sizeSprite.Size.Height, 0.05f);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(1.0f, sizeSprite.Size.Height, 0.05f);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsFalse(sizeSprite.IsActive);
		}

		private static Sprite CreateSizeSprite()
		{
			var sprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"), Rectangle.One);
			sprite.Add(new Transition.SizeRange(Size.Zero, Size.One));
			sprite.Add(new Transition.Duration(1.0f));
			sprite.Start<FinalTransition>();
			return sprite;
		}

		[Test]
		public void RenderRotatingLogo()
		{
			var logoSprite = new Sprite(new Material(Shader.Position2DUv, "DeltaEngineLogo"),
				Rectangle.HalfCentered);
			logoSprite.Add(new Transition.Rotation(0, 360));
			logoSprite.Add(new Transition.Duration(4.0f));
			logoSprite.Start<Transition>();
		}

		[Test]
		public void RenderingFadingLine()
		{
			var line2D = new Line2D(Point.Zero, Point.One, Color.White);
			line2D.Add(new Transition.FadingColor(Color.Red));
			line2D.Add(new Transition.Duration(4.0f));
			line2D.Start<Transition>();
		}

		[Test]
		public void RenderingChangingOutlineColor()
		{
			var rect = new FilledRect(Rectangle.HalfCentered, Color.Blue);
			rect.OnDraw<DrawPolygon2DOutlines>();
			rect.Add(new OutlineColor(Color.Yellow));
			rect.Add(new Transition.OutlineColor(Color.Yellow, Color.Red));
			rect.Add(new Transition.Duration(4.0f));
			rect.Start<Transition>();
		}

		[Test]
		public void FadingTransitionOfshortDuration()
		{
			var line2D = new Line2D(Point.Zero, Point.One, Color.White);
			line2D.Add(new Transition.FadingColor(Color.Red));
			line2D.Add(new Transition.Duration(0.2f));
			line2D.Start<Transition>();
		}
	}
}