using System;
using System.Drawing;
using System.IO;
using DeltaEngine.Core;
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

		//ncrunch: no coverage start
		[IntegrationTest]
		public void CreateBitmapAndLoadItAsAnImage(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				const string TestFilePath = "testfile";
				if (resolver != typeof(TestResolver))
					using (var bitmap = new Bitmap(128, 128))
						bitmap.Save(TestFilePath);
				var image = content.Load<Image>(TestFilePath);
				Assert.IsNotNull(image);
				Assert.AreEqual(new Size(128, 128), image.PixelSize);
				if (resolver != typeof(TestResolver))
					File.Delete(TestFilePath);
			});
		}
	}
}