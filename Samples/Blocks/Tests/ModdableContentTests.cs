using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Blocks.Tests
{
	public class ModdableContentTests : TestStarter
	{
		[VisualTest]
		public void LoadContentWithNoModDirectorySet(Type resolver)
		{
			Start(resolver, (ModdableContent content, Renderer renderer) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				Assert.AreEqual(new Size(128), image.PixelSize);
				renderer.Add(new Sprite(image, new Rectangle(0.45f, 0.45f, 0.1f, 0.1f)));
			});
		}

		[VisualTest]
		public void LoadContentWithModDirectorySet(Type resolver)
		{
			Start(resolver, (ModdableContent content, Renderer renderer) =>
			{
				content.Subdirectory = "Mod1";
				var image = content.Load<Image>("DeltaEngineLogo");
				if (resolver != typeof(TestResolver))
					Assert.AreEqual(new Size(64), image.PixelSize);
				renderer.Add(new Sprite(image, new Rectangle(0.3f, 0.45f, 0.1f, 0.1f)));

				content.Subdirectory = "Mod2";
				image = content.Load<Image>("DeltaEngineLogo");
				if (resolver != typeof(TestResolver))
					Assert.AreEqual(new Size(256), image.PixelSize);
				renderer.Add(new Sprite(image, new Rectangle(0.6f, 0.45f, 0.1f, 0.1f)));
			});
		}
	}
}