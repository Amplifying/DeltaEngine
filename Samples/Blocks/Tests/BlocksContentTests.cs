using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Blocks.Tests
{
	public class BlocksContentTests : TestStarter
	{
		[Test]
		public void DoBricksSplitInHalfOnExit()
		{
			var content = new TestResolver().Resolve<BlocksContent>();
			Assert.IsFalse(content.DoBricksSplitInHalfWhenRowFull);
			content.DoBricksSplitInHalfWhenRowFull = true;
			Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
		}

		[Test]
		public void AreFiveBrickBlocksAllowed()
		{
			var content = new TestResolver().Resolve<BlocksContent>();
			Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
			content.AreFiveBrickBlocksAllowed = false;
			Assert.IsFalse(content.AreFiveBrickBlocksAllowed);
		}

		[Test]
		public void DoBlocksStartInARandomColumn()
		{
			var content = new TestResolver().Resolve<BlocksContent>();
			Assert.IsFalse(content.DoBlocksStartInARandomColumn);
			content.DoBlocksStartInARandomColumn = true;
			Assert.IsTrue(content.DoBlocksStartInARandomColumn);
		}

		[Test]
		public void GetFilenameWithoutPrefix()
		{
			var content = new TestResolver().Resolve<BlocksContent>();
			content.Prefix = "ABC";
			Assert.AreEqual("DEF", content.GetFilenameWithoutPrefix("ABCDEF"));
			Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
				() => content.GetFilenameWithoutPrefix("ADEF"));
			Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
				() => content.GetFilenameWithoutPrefix("AAADEF"));
		}

		[VisualTest]
		public void LoadContentWithNoPrefixSet(Type resolver)
		{
			Start(resolver, (BlocksContent content, Renderer renderer) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				Assert.AreEqual(new Size(128), image.PixelSize);
				renderer.Add(new Sprite(image, new Rectangle(0.45f, 0.45f, 0.1f, 0.1f)));
			});
		}

		[VisualTest]
		public void LoadContentWithPrefixSet(Type resolver)
		{
			Start(resolver, (BlocksContent content, Renderer renderer) =>
			{
				content.Prefix = "Mod1_";
				var image = content.Load<Image>("DeltaEngineLogo");
				if (resolver != typeof(TestResolver))
					Assert.AreEqual(new Size(64), image.PixelSize);
				renderer.Add(new Sprite(image, new Rectangle(0.3f, 0.45f, 0.1f, 0.1f)));

				content.Prefix = "Mod2_";
				image = content.Load<Image>("DeltaEngineLogo");
				if (resolver != typeof(TestResolver))
					Assert.AreEqual(new Size(256), image.PixelSize);
				renderer.Add(new Sprite(image, new Rectangle(0.6f, 0.45f, 0.1f, 0.1f)));
			});
		}
	}
}