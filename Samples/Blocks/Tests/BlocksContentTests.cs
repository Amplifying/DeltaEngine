using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace Blocks.Tests
{
	public class BlocksContentTests : TestWithAllFrameworks
	{
		[Test]
		public void DoBricksSplitInHalfOnExit()
		{
			Start(typeof(MockResolver), (JewelBlocksContent content) =>
			{
				Assert.IsFalse(content.DoBricksSplitInHalfWhenRowFull);
				content.DoBricksSplitInHalfWhenRowFull = true;
				Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
			});
		}

		[Test]
		public void AreFiveBrickBlocksAllowed()
		{
			Start(typeof(MockResolver), (JewelBlocksContent content) =>
			{
				Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
				content.AreFiveBrickBlocksAllowed = false;
				Assert.IsFalse(content.AreFiveBrickBlocksAllowed);
			});
		}

		[Test]
		public void DoBlocksStartInARandomColumn()
		{
			Start(typeof(MockResolver), (JewelBlocksContent content) =>
			{
				Assert.IsFalse(content.DoBlocksStartInARandomColumn);
				content.DoBlocksStartInARandomColumn = true;
				Assert.IsTrue(content.DoBlocksStartInARandomColumn);
			});
		}

		[Test]
		public void GetFilenameWithoutPrefix()
		{
			Start(typeof(MockResolver), (JewelBlocksContent content) =>
			{
				content.Prefix = "ABC";
				Assert.AreEqual("DEF", content.GetFilenameWithoutPrefix("ABCDEF"));
				Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
					() => content.GetFilenameWithoutPrefix("ADEF"));
				Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
					() => content.GetFilenameWithoutPrefix("AAADEF"));
			});
		}

		[VisualTest]
		public void LoadContentWithNoPrefixSet(Type resolver)
		{
			Start(resolver, (JewelBlocksContent content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				Assert.AreEqual(new Size(128), image.PixelSize);
				new Sprite(image, new Rectangle(0.45f, 0.45f, 0.1f, 0.1f));
			});
		}

		[VisualTest]
		public void LoadContentWithPrefixSet(Type resolver)
		{
			Start(resolver, (JewelBlocksContent content) =>
			{
				content.Prefix = "Mod1_";
				var image = content.Load<Image>("DeltaEngineLogo");
				if (resolver != typeof(MockResolver))
					Assert.AreEqual(new Size(64), image.PixelSize); //ncrunch: no coverage
				new Sprite(image, new Rectangle(0.3f, 0.45f, 0.1f, 0.1f));

				content.Prefix = "Mod2_";
				image = content.Load<Image>("DeltaEngineLogo");
				if (resolver != typeof(MockResolver))
					Assert.AreEqual(new Size(256), image.PixelSize); //ncrunch: no coverage
				new Sprite(image, new Rectangle(0.6f, 0.45f, 0.1f, 0.1f));
			});
		}
	}
}