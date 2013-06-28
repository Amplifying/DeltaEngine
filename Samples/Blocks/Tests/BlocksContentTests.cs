using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace Blocks.Tests
{
	public class BlocksContentTests : TestWithMocksOrVisually
	{
		[Test]
		public void DoBricksSplitInHalfOnExit()
		{
			var content = new JewelBlocksContent();
			Assert.IsFalse(content.DoBricksSplitInHalfWhenRowFull);
			content.DoBricksSplitInHalfWhenRowFull = true;
			Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
		}

		[Test]
		public void AreFiveBrickBlocksAllowed()
		{
			var content = new JewelBlocksContent();
			Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
			content.AreFiveBrickBlocksAllowed = false;
			Assert.IsFalse(content.AreFiveBrickBlocksAllowed);
		}

		[Test]
		public void DoBlocksStartInARandomColumn()
		{
			var content = new JewelBlocksContent();
			Assert.IsFalse(content.DoBlocksStartInARandomColumn);
			content.DoBlocksStartInARandomColumn = true;
			Assert.IsTrue(content.DoBlocksStartInARandomColumn);
		}

		[Test]
		public void GetFilenameWithoutPrefix()
		{
			var content = new JewelBlocksContent();
			content.Prefix = "ABC";
			Assert.AreEqual("DEF", content.GetFilenameWithoutPrefix("ABCDEF"));
			Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
				() => content.GetFilenameWithoutPrefix("ADEF"));
			Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
				() => content.GetFilenameWithoutPrefix("AAADEF"));
		}

		[Test]
		public void LoadContentWithNoPrefixSet()
		{
			var content = new JewelBlocksContent();
			content.Prefix = "";
			var image = content.Load<Image>("DeltaEngineLogo");
			Assert.AreEqual(new Size(128), image.PixelSize);
			new Sprite(image, new Rectangle(0.45f, 0.45f, 0.1f, 0.1f));
		}

		[Test]
		public void LoadContentWithPrefixSet()
		{
			var content = new JewelBlocksContent();
			content.Prefix = "Mod1_";
			var image = content.Load<Image>("DeltaEngineLogo");
			new Sprite(image, new Rectangle(0.3f, 0.45f, 0.1f, 0.1f));

			content.Prefix = "Mod2_";
			image = content.Load<Image>("DeltaEngineLogo");
			new Sprite(image, new Rectangle(0.6f, 0.45f, 0.1f, 0.1f));
		}

		[Test]
		public void ContentWithPrefixSet()
		{
			var content = new JewelBlocksContent();
			content.Prefix = "Mod1_";
			var image = content.Load<Image>("DeltaEngineLogo");
			Assert.AreEqual(new Size(64), image.PixelSize);

			content.Prefix = "Mod2_";
			image = content.Load<Image>("DeltaEngineLogo");
			Assert.AreEqual(new Size(256), image.PixelSize);
		}
	}
}