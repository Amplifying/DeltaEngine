using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Mocks common Rendering and Graphics objects for testing
	/// </summary>
	public class TestRenderingResolver : TestModuleResolver
	{
		public TestRenderingResolver(TestResolver testResolver)
			: base(testResolver)
		{
			Window window = SetupWindow();
			Drawing drawing = SetupGraphics();
			SetupRenderer(window, drawing);
		}

		private Window SetupWindow()
		{
			var windowMock = testResolver.RegisterMock<Window>();
			windowMock.Setup(w => w.IsVisible).Returns(true);
			windowMock.Setup(w => w.IsClosing).Returns(true);
			windowMock.SetupProperty(w => w.Title, "WindowMock");
			SetupWindowSizeProperties(windowMock);
			return windowMock.Object;
		}

		private static void SetupWindowSizeProperties(Mock<Window> windowMock)
		{
			Window window = windowMock.Object;
			var currentSize = new Size(1024, 640);
			windowMock.SetupGet(w => w.TotalPixelSize).Returns(() => currentSize);
#pragma warning disable 0618
			windowMock.SetupSet(w => w.TotalPixelSize).Callback(s =>
			{
				currentSize = s;
				windowMock.Raise(w => w.ViewportSizeChanged += null, s);
			});
			windowMock.SetupGet(w => w.ViewportPixelSize).Returns(() => currentSize);
			bool isFullscreen = false;
			var rememberSizeBeforeFullscreen = new Size();
			windowMock.Setup(w => w.SetFullscreen(It.IsAny<Size>())).Callback((Size displaySize) =>
			{
				isFullscreen = true;
				rememberSizeBeforeFullscreen = window.TotalPixelSize;
				window.TotalPixelSize = displaySize;
			});
			windowMock.Setup(w => w.SetWindowed()).Callback(() =>
			{
				isFullscreen = false;
				window.TotalPixelSize = rememberSizeBeforeFullscreen;
			});
			windowMock.SetupGet(w => w.IsFullscreen).Returns(() => isFullscreen);
		}

		private Drawing SetupGraphics()
		{
			Device device = SetupGraphicsDevice();
			Drawing drawing = SetupDrawing(device);
			SetupImage(drawing);
			return drawing;
		}

		private Device SetupGraphicsDevice()
		{
			Device device = new Mock<Device>().Object;
			testResolver.RegisterMock(device);
			return device;
		}

		private Drawing SetupDrawing(Device device)
		{
			var mockDrawing = new Mock<Drawing>(device);
			mockDrawing.Setup(
				d => d.DrawVertices(It.IsAny<VerticesMode>(), It.IsAny<VertexPositionColor[]>())).Callback(
					(VerticesMode mode, VertexPositionColor[] vertices) =>
						testResolver.NumberOfVerticesDrawn += vertices.Length);
			return testResolver.RegisterMock(mockDrawing.Object);
		}

		private void SetupImage(Drawing drawing)
		{
			MockImage = new Mock<Image>("dummy", drawing);
			MockImage.SetupGet(i => i.PixelSize).Returns(new Size(128, 128));
			MockImage.CallBase = true;
			testResolver.RegisterMock(MockImage.Object);
		}

		internal Mock<Image> MockImage { get; private set; }

		private void SetupRenderer(Window window, Drawing drawing)
		{
			ScreenSpace screen = testResolver.RegisterMock(new QuadraticScreenSpace(window));
			testResolver.RegisterMock(new Mock<Renderer>(drawing, screen).Object);
		}
	}
}