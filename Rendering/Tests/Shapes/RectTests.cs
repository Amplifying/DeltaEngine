using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RectTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderRect(Type resolver)
		{
			Start(resolver, () => new Rect(new Rectangle(0.3f, 0.3f, 0.4f, 0.4f), Color.Blue));
		}

		[VisualTest]
		public void CollidingRectanglesChangeColor(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var r1 = new Rect(new Rectangle(0.2f, 0.2f, 0.1f, 0.1f), Color.Red);
				r1.Rotation = 45;
				var r2 = new Rect(new Rectangle(0.6f, 0.6f, 0.1f, 0.2f), Color.Red);
				r2.Rotation = 70;
				r1.Add<CollidingChangesColor>();
				r2.Add<CollidingChangesColor>();
				input.Add(MouseButton.Left, State.Pressed, mouse => r1.Center = mouse.Position);
				input.Add(MouseButton.Right, State.Pressed, mouse => r2.Center = mouse.Position);
			});
		}

		private class CollidingChangesColor : EntityHandler
		{
			public CollidingChangesColor()
			{
				Filter = entity => entity is Rect;
			}

			public override void Handle(Entity entity)
			{
				foreach (var otherEntity in EntitySystem.Current.GetEntitiesWithTag(null).OfType<Rect>())
					if (entity != otherEntity)
						UpdateColorIfColliding(entity as Rect, otherEntity);
			}

			private static void UpdateColorIfColliding(Rect entity, Rect otherEntity)
			{
				entity.Color = entity.DrawArea.IsColliding(entity.Rotation, otherEntity.DrawArea,
					otherEntity.Rotation) ? Color.Yellow : Color.Red;
			}
		}

		[Test]
		public void DefaultRectIsRectangleZeroAndWhite()
		{
			var rect = new Rect();
			Assert.AreEqual(Rectangle.Zero, rect.DrawArea);
			Assert.AreEqual(Color.White, rect.Color);
		}

		[IntegrationTest]
		public void RectangleCornersAreCorrect(Type resolver)
		{
			Start(resolver, () =>
			{
				var rect = new Rect(Rectangle.One, Color.White);
				EntitySystem.Current.Run();
				var corners = new List<Point> { Point.Zero, Point.UnitX, Point.One, Point.UnitY };
				Assert.AreEqual(corners, rect.Points);
			});
		}

		[VisualTest]
		public void MakeRectInvisibleOnClick(Type resolver)
		{
			Start(resolver, (InputCommands commands) =>
			{
				var rect = new Rect(Rectangle.FromCenter(Point.Half, new Size(0.2f)), Color.Orange);
				rect.Add<Rect.Render>();
				commands.Add(MouseButton.Left, State.Releasing, mouse => { ChangeVisibility(rect); });
			});
		}

		private static void ChangeVisibility(Rect rect)
		{
			rect.Visibility = rect.Visibility == Visibility.Show ? Visibility.Hide : Visibility.Show;
		}
	}
}