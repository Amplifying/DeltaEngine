using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
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
				Assert.AreEqual(new Size(1024, 640), window.TotalPixelSize);
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

		/// <summary>
		/// Use the DeviceTests.SetFullscreenResolution to see the real resolution switching
		/// </summary>
		/// <param name="resolver"></param>
		[VisualTest]
		public void SetFullscreenMode(Type resolver)
		{
			Start(resolver, (Window window) =>
			{
				var newFullscreenSize = new Size(800, 600);
				Assert.IsFalse(window.IsFullscreen);
				window.SetFullscreen(newFullscreenSize);
				Assert.IsTrue(window.IsFullscreen);
				Assert.AreEqual(newFullscreenSize, window.TotalPixelSize);
			});
		}

		[IntegrationTest]
		public void SwitchToFullscreenAndWindowedMode(Type resolver)
		{
			Start(resolver, (Window window) =>
			{
				Size sizeBeforeFullscreen = window.TotalPixelSize;
				var newFullscreenSize = new Size(800, 600);
				window.SetFullscreen(newFullscreenSize);
				window.SetWindowed();
				Assert.IsFalse(window.IsFullscreen);
				Assert.AreEqual(sizeBeforeFullscreen, window.TotalPixelSize);
			});
		}

		[VisualTest]
		public void ShowColoredRectangle(Type resolver)
		{
			Start(resolver, (Renderer r) => r.Add(new Rect(Point.Half, Size.Half, Color.Red)));
		}

		[VisualTest]
		public void ShowCursor(Type resolver)
		{
			bool showCursor = true;
			Start(resolver,
				(Window window, InputCommands input) =>
				{ input.Add(MouseButton.Left, mouse => window.ShowCursor = showCursor = !showCursor); });
		}
	}
}