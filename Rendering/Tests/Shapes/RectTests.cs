using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RectTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderRect()
		{
			new FilledRect(new Rectangle(0.3f, 0.3f, 0.4f, 0.4f), Color.Blue);
		}

		[Test]
		public void CollidingRectanglesChangeColor()
		{
			var r1 = new FilledRect(new Rectangle(0.2f, 0.2f, 0.1f, 0.1f), Color.Red) { Rotation = 45 };
			var r2 = new FilledRect(new Rectangle(0.6f, 0.6f, 0.1f, 0.2f), Color.Red) { Rotation = 70 };
			r1.Start<CollidingChangesColor>();
			r2.Start<CollidingChangesColor>();
			Input.Add(MouseButton.Left, State.Pressed, mouse => r1.Center = mouse.Position);
			Input.Add(MouseButton.Right, State.Pressed, mouse => r2.Center = mouse.Position);
		}

		private class CollidingChangesColor : Behavior2D
		{
			public CollidingChangesColor()
			{
				Filter = entity => entity is FilledRect;
			}

			public override void Handle(Entity2D entity)
			{
				foreach (
					var otherEntity in EntitySystem.Current.GetEntitiesOfType<FilledRect>())
					if (entity != otherEntity)
						UpdateColorIfColliding(entity as FilledRect, otherEntity);
			}

			private static void UpdateColorIfColliding(FilledRect entity, FilledRect otherEntity)
			{
				entity.Color = entity.DrawArea.IsColliding(entity.Rotation, otherEntity.DrawArea,
					otherEntity.Rotation) ? Color.Yellow : Color.Red;
			}
		}

		[Test]
		public void DefaultRectIsRectangleZeroAndWhite()
		{
			var rect = new FilledRect(Rectangle.One, Color.White);
			Assert.AreEqual(Rectangle.One, rect.DrawArea);
			Assert.AreEqual(Color.White, rect.Color);
			Window.CloseAfterFrame();
		}

		[Test]
		public void RectangleCornersAreCorrect()
		{
			var rect = new FilledRect(Rectangle.One, Color.White);
			var corners = new List<Point> { Point.Zero, Point.UnitX, Point.One, Point.UnitY };
			Assert.AreEqual(corners, rect.Points);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ToggleRectVisibilityOnClick()
		{
			var rect = new FilledRect(Rectangle.FromCenter(Point.Half, new Size(0.2f)), Color.Orange);
			Input.Add(MouseButton.Left, State.Releasing, mouse => ChangeVisibility(rect));
		}

		private static void ChangeVisibility(FilledRect rect)
		{
			rect.Visibility = rect.Visibility == Visibility.Show ? Visibility.Hide : Visibility.Show;
		}

		[Test]
		public void RenderRedRectOverBlue()
		{
			new FilledRect(new Rectangle(0.3f, 0.3f, 0.4f, 0.4f), Color.Red) { RenderLayer = 1 };
			new FilledRect(new Rectangle(0.4f, 0.4f, 0.4f, 0.4f), Color.Blue) { RenderLayer = 0 };
		}
	}
}