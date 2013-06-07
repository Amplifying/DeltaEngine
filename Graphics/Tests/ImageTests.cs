using System;
using System.Collections.Generic;
using System.Diagnostics;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	/// <summary>
	/// The image tests here are limited to loading and integration tests, not visual tests, which
	/// you can find in DeltaEngine.Rendering.Tests.SpriteTests.
	/// </summary>
	public class ImageTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawImage(Type resolver)
		{
			Image image = null;
			Start(resolver, (ContentLoader content, Window window) =>
			{
				window.BackgroundColor = Color.CornflowerBlue;
				image = content.Load<Image>("DeltaEngineLogo");
			},
				(Drawing drawing) =>
					drawing.DrawQuad(image,
						new List<VertexPositionColorTextured>
						{
							new VertexPositionColorTextured(new Point(175, 25), Color.Yellow, Point.Zero),
							new VertexPositionColorTextured(new Point(475, 25), Color.Red, Point.UnitX),
							new VertexPositionColorTextured(new Point(475, 325), Color.Blue, Point.One),
							new VertexPositionColorTextured(new Point(175, 325), Color.Teal, Point.UnitY)
						}, new List<short> { 0, 1, 2, 0, 2, 3 }));
		}

		[VisualTest]
		public void DrawImagesWithOneMillionPolygonsPerFrame(Type resolver)
		{
			Image image = null;
			var vertices = new VertexPositionColorTextured[40000];
			var indices = new short[60000];
			var indicesIndex = 0;
			for (int y = 0; y < 100; y++)
				for (int x = 0; x < 100; x++)
				{
					int quadIndex = (y * 100 + x) * 4;
					vertices[quadIndex + 0] = new VertexPositionColorTextured(new Point(x, y) * 10,
						Color.GetRandomColor(), Point.Zero);
					vertices[quadIndex + 1] = new VertexPositionColorTextured(new Point(x + 1, y) * 10,
						Color.GetRandomColor(), Point.UnitX);
					vertices[quadIndex + 2] = new VertexPositionColorTextured(new Point(x + 1, y + 1) * 10,
						Color.GetRandomColor(), Point.One);
					vertices[quadIndex + 3] = new VertexPositionColorTextured(new Point(x, y + 1) * 10,
						Color.GetRandomColor(), Point.UnitY);
					// 2 Polygons per quad
					indices[indicesIndex++] = (short)quadIndex;
					indices[indicesIndex++] = (short)(quadIndex + 1);
					indices[indicesIndex++] = (short)(quadIndex + 2);
					indices[indicesIndex++] = (short)quadIndex;
					indices[indicesIndex++] = (short)(quadIndex + 2);
					indices[indicesIndex++] = (short)(quadIndex + 3);
				}

			Start(resolver,
				(ContentLoader content) => { image = content.Load<Image>("DeltaEngineLogo"); },
				(Drawing drawing, Window window) =>
				{
					drawing.EnableTexturing(image);
					// Draw 50 times to reach 1 million polygons per frame
					drawing.SetIndices(indices, indices.Length);
					for (int num = 0; num < 50; num++)
						drawing.DrawVerticesForSprite(VerticesMode.Triangles, vertices);

					if (Time.Current.CheckEvery(1))
						window.Title = "Fps: " + Time.Current.Fps;
				});
		}

		[VisualTest]
		public void BlendModes(Type resolver)
		{
			Image image = null;
			Start(resolver, (ContentLoader content, Window window) =>
			{
				image = content.Load<Image>("DeltaEngineLogoAlpha");
				window.Title = "Blend modes: Opaque, Normal, Additive";
			}, (Drawing drawing) =>
			{
				drawing.SetBlending(BlendMode.Opaque);
				DrawAlphaImageTwice(image, drawing, new Point(25, 80));
				drawing.SetBlending(BlendMode.Normal);
				DrawAlphaImageTwice(image, drawing, new Point(225, 80));
				drawing.SetBlending(BlendMode.Additive);
				DrawAlphaImageTwice(image, drawing, new Point(425, 80));
			});
		}

		private static void DrawAlphaImageTwice(Image image, Drawing drawing, Point startPoint)
		{
			var x = (int)startPoint.X;
			var y = (int)startPoint.Y;
			const int Size = 120;
			for (int i = 1; i <= 2; i++)
			{
				drawing.DrawQuad(image,
					new List<VertexPositionColorTextured>
					{
						new VertexPositionColorTextured(new Point(x, y), Color.White, Point.Zero),
						new VertexPositionColorTextured(new Point(x + Size, y), Color.White, Point.UnitX),
						new VertexPositionColorTextured(new Point(x + Size, y + Size), Color.White, Point.One),
						new VertexPositionColorTextured(new Point(x, y + Size), Color.White, Point.UnitY)
					}, new List<short> { 0, 1, 2, 0, 2, 3 });
				x += Size / 2;
				y += Size / 2;
			}
		}

		[IntegrationTest]
		public void LoadExistingImage(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				Assert.AreEqual("DeltaEngineLogo", image.Name);
				Assert.IsFalse(image.IsDisposed);
				Assert.AreEqual(new Size(128, 128), image.PixelSize);
			});
		}

		[IntegrationTest]
		public void ShouldThrowIfImageNotLoadedWithDebuggerAttached(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				if (!Debugger.IsAttached || resolver.FullName.Contains("Mock"))
					return;
				//ncrunch: no coverage start
				Assert.Throws<ContentLoader.ContentNotFound>(() => content.Load<Image>("UnavailableImage"));
			});
		}
	}
}