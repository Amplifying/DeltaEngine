using System;
using System.Drawing;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Graphics.OpenTK;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Graphics.Tests
{
	/// <summary>
	/// The image tests here are limited to loading and integration tests, not visual tests, which
	/// you can find in DeltaEngine.Rendering.Tests.SpriteTests.
	/// </summary>
	public class ImageTests : TestStarter
	{
		[IntegrationTest]
		public void LoadExistingImage(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				Assert.AreEqual(new Size(128, 128), image.PixelSize);
			});
		}

		[IntegrationTest]
		public void DrawImage(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				image.Draw(new VertexPositionColorTextured[4]);
			});
		}

		[VisualTest]
		public void DrawImageVisualTest(Type resolver)
		{
			Image remimage = null;
			Start(resolver, (Content content, Window window, Drawing drawing) =>
			{
				remimage = content.Load<Image>("DeltaEngineLogo");
			}, () => remimage.Draw(new VertexPositionColorTextured[4]));
		}

		
		[IntegrationTest]
		public void CreateBitmapAndLoadItAsAnImage(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				const string TestFilePath = "testfile";
				if (resolver == typeof(TestResolver))
				{
					var image = content.Load<Image>(TestFilePath);
					Assert.IsNotNull(image);
					Assert.AreEqual(new Size(128, 128), image.PixelSize);
				}
				else //ncrunch: no coverage start
					NonTestResolverCreateBitmap(content, TestFilePath);
			});
		}

		private static void NonTestResolverCreateBitmap(Content content, String testFilePath)
		{
			using (var bitmap = new Bitmap(128, 128))
				bitmap.Save(testFilePath);
			var image = content.Load<Image>(testFilePath);
			Assert.IsNotNull(image);
			Assert.AreEqual(new Size(4, 4), image.PixelSize);
			File.Delete(testFilePath);
		}
	}
}