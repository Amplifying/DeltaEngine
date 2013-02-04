using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowTests : TestStarter
	{
		[VisualTest]
		public void CreateWindow(Type resolver)
		{
			Start(resolver, (Window window) => Assert.IsTrue(window.IsVisible));
		}

		[VisualTest]
		public void GetTitle(Type resolver)
		{
			Window remWindow = null;
			Start(resolver, (Window window) =>
			{
				remWindow = window;
				remWindow.Title = "TestTitle";
			}, () => Assert.AreEqual("TestTitle", remWindow.Title));
		}

		[IntegrationTest]
		public void ChangeTotalSize(Type resolver)
		{
			Start(resolver, (Window window) =>
			{
				Assert.AreEqual(new Size(800, 600), window.TotalPixelSize);
				Size changedSize = window.TotalPixelSize;
				window.ViewportSizeChanged += size => changedSize = size;
				window.TotalPixelSize = new Size(200, 200);
				Assert.AreEqual(new Size(200, 200), window.TotalPixelSize);
				Assert.IsTrue(window.ViewportPixelSize.Width <= 200);
				Assert.IsTrue(window.ViewportPixelSize.Height <= 200);
				Assert.IsTrue(changedSize.Width <= 200);
				Assert.IsTrue(changedSize.Height <= 200);
			});
		}

		[VisualTest]
		public void ShowColoredRectangle(Type resolver)
		{
			Start(resolver,
				(Renderer r) => r.Add(new ColoredRectangle(Point.Half, Size.Half, Color.Red)));
		}
	}
}