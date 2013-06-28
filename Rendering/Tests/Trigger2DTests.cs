using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Rendering.Triggers;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	internal class Trigger2DTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateTrigger()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var trigger = new Sprite(image, new Rectangle(Point.Zero, (Size)Point.One))
			{
				Color = Color.Red
			};
			trigger.Add(new TimeTriggerData(Color.Red, Color.Gray, 1));
			trigger.Start<CollisionTrigger>().Add(new CollisionTriggerData(Color.White, Color.Red));
			Assert.AreEqual(Point.Zero, trigger.Get<Rectangle>().TopLeft);
			Assert.AreEqual(Point.One, trigger.Get<Rectangle>().BottomRight);
		}

		[Test]
		public void ChangeColorIfTwoRectanglesCollide()
		{
			Window.BackgroundColor = Color.Red;
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var sprite = new Sprite(image, new Rectangle(0.25f, 0.2f, 0.1f, 0.5f));
			sprite.Start<CollisionTrigger, Rotate>().Add(new CollisionTriggerData(Color.Yellow,
				Color.Blue));
			sprite.Get<CollisionTriggerData>().SearchTags.Add("Creep");
			var sprite2 = new Sprite(image, new Rectangle(0.5f, 0.2f, 0.1f, 0.5f));
			sprite2.AddTag("Creep");
		}

		public class Rotate : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				var angle = entity.Rotation;
				angle = angle + 50 * Time.Current.Delta;
				entity.Rotation = angle;
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		[Test]
		public void ChangeColorOverTime()
		{
			Window.BackgroundColor = Color.Red;
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var sprite = new Sprite(image, new Rectangle(0.25f, 0.2f, 0.5f, 0.5f))
			{
				Color = Color.Green
			};
			sprite.Start<TimeTrigger>().Add(new TimeTriggerData(Color.Green, Color.Gold, 0.1f));
		}
	}
}