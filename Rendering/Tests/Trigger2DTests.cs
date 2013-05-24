using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Rendering.Triggers;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	internal class Trigger2DTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateTrigger()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var trigger = new Sprite(image, new Rectangle(Point.Zero, (Size)Point.One), Color.Red);
				trigger.Add(new TimeTriggerData(Color.Red, Color.Gray, 1));
				trigger.Add<Trigger2D>();
				Assert.AreEqual(Point.Zero, trigger.Get<Rectangle>().TopLeft);
				Assert.AreEqual(Point.One, trigger.Get<Rectangle>().BottomRight);
			});
		}

		[VisualTest]
		public void ChangeColorIfTwoRectanglesCollide(Type resolver)
		{
			Start(resolver, (ContentLoader content, Window window) =>
			{
				window.BackgroundColor = Color.Red;
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, new Rectangle(0.25f, 0.2f, 0.1f, 0.5f), Color.White);
				sprite.Add<CollisionTrigger>().Add<Rotate>().Add(new CollisionTriggerData(Color.Yellow,
					Color.Blue));
				sprite.Get<CollisionTriggerData>().SearchTags.Add("Creep");
				var sprite2 = new Sprite(image, new Rectangle(0.5f, 0.2f, 0.1f, 0.5f), Color.White);
				sprite2.Tag = "Creep";
			});
		}

		public class Rotate : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (var sprite in entities)
				{
					var angle = sprite.Get<float>();
					angle = angle + 50 * Time.Current.Delta;
					sprite.Set(angle);
				}
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[VisualTest]
		public void ChangeColorOverTime(Type resolver)
		{
			Start(resolver, (ContentLoader content, Window window) =>
			{
				window.BackgroundColor = Color.Red;
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, new Rectangle(0.25f, 0.2f, 0.5f, 0.5f), Color.Green);
				sprite.Add<TimeTrigger>().Add(new TimeTriggerData(Color.Green, Color.Gold, 0.1f));
			});
		}
	}
}