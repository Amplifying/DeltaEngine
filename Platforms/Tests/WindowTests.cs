using System;
using System.Windows.Forms;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateWindow()
		{
			Assert.IsTrue(Window.Visibility);
		}

		[Test]
		public void GetTitle()
		{
			Window.Title = "TestTitle";
			Assert.AreEqual("TestTitle", Window.Title);
		}

		[Test]
		public void ChangeTotalSize()
		{
			Assert.AreEqual(new Size(640, 360), Window.ViewportPixelSize);
			Size changedSize = Window.TotalPixelSize;
			Window.ViewportSizeChanged += size => changedSize = size;
			Window.ViewportPixelSize = new Size(200, 200);
			Assert.AreEqual(new Size(200, 200), Window.ViewportPixelSize);
			Assert.IsTrue(Window.ViewportPixelSize.Width <= 200);
			Assert.IsTrue(Window.ViewportPixelSize.Height <= 200);
			Assert.IsTrue(changedSize.Width <= 200);
			Assert.IsTrue(changedSize.Height <= 200);
			Window.CloseAfterFrame();
		}

		/// <summary>
		/// Use the DeviceTests.SetFullscreenResolution to see the real resolution switching
		/// </summary>
		[Test]
		public void SetFullscreenMode()
		{
			var newFullscreenSize = new Size(Screen.PrimaryScreen.Bounds.Width,
				Screen.PrimaryScreen.Bounds.Height);
			Assert.IsFalse(Window.IsFullscreen);
			Window.SetFullscreen(newFullscreenSize);
			Assert.IsTrue(Window.IsFullscreen);
			Assert.AreEqual(newFullscreenSize, Window.TotalPixelSize);
		}

		[Test]
		public void SwitchToFullscreenAndWindowedMode()
		{
			Size sizeBeforeFullscreen = Window.TotalPixelSize;
			Window.SetFullscreen(new Size(1024, 768));
			Window.SetWindowed();
			Assert.IsFalse(Window.IsFullscreen);
			Assert.AreEqual(sizeBeforeFullscreen, Window.TotalPixelSize);
		}

		[Test]
		public void ShowColoredEllipse()
		{
			new Ellipse(new Rectangle(Point.Half, Size.Half), Color.Red);
		}

		[Test]
		public void ShowCursorAndToggleHideWhenClicking()
		{
			bool showCursor = true;
			Input.Add(MouseButton.Left, mouse => Window.ShowCursor = showCursor = !showCursor);
		}
	}
}