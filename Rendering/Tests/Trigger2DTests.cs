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
				trigger.Add<Trigger2D>();
				Assert.AreEqual(Point.Zero, trigger.Get<Rectangle>().TopLeft);
				Assert.AreEqual(Point.One, trigger.Get<Rectangle>().BottomRight);
			});
		}

		[VisualTest]
		public void ChangeColorIfTwoRectanglesCollide(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, Window window) =>
			{
				window.BackgroundColor = Color.Red;
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, new Rectangle(0.25f, 0.2f, 0.1f, 0.5f), Color.White);
				sprite.Add<CollisionTrigger>().Add<Rotate>().Add(new CollisionTriggerData(Color.Yellow,
					Color.Blue));
				sprite.Get<CollisionTriggerData>().SearchTags.Add("Creep");
				entitySystem.Add(sprite);
				var sprite2 = new Sprite(image, new Rectangle(0.5f, 0.2f, 0.1f, 0.5f), Color.White);
				sprite2.Tag = "Creep";
				entitySystem.Add(sprite2);
			});
		}

		public class Rotate : EntityHandler
		{
			public void Handle(List<Entity> entities)
			{
				foreach (var sprite in entities)
				{
					float angle = sprite.Get<Rotation>().Value;
					angle = angle + 50 * Time.Current.Delta;
					var rotation = new Rotation(angle);
					sprite.Set(rotation);
				}
			}

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[VisualTest]
		public void ChangeColorOverTime(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, Window window) =>
			{
				window.BackgroundColor = Color.Red;
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, new Rectangle(0.25f, 0.2f, 0.5f, 0.5f), Color.Green);
				sprite.Add<TimeTrigger>().Add(new TimeTriggerData(Color.Green, Color.Gold, 0.1f));
				entitySystem.Add(sprite);
			});
		}
	}
}