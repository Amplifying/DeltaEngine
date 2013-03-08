using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	public class TestRenderingResolver : TestModuleResolver
	{
		public TestRenderingResolver(TestResolver testResolver) 
			: base(testResolver)
		{
			SetupWindow();
			SetupGraphics();
			SetupRenderer();
		}

		private Window window;
		private Device device;
		private Drawing drawing;
		private QuadraticScreenSpace screen;

		private void SetupWindow()
		{
			var windowMock = testResolver.RegisterMock<Window>();
			windowMock.Setup(w => w.IsVisible).Returns(true);
			windowMock.Setup(w => w.IsClosing).Returns(true);
			windowMock.SetupProperty(w => w.Title, "WindowMock");
			SetupWindowSizeProperties(windowMock);
			window = windowMock.Object;
		}

		private void SetupWindowSizeProperties(Mock<Window> windowMock)
		{
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

		private void SetupGraphics()
		{
			SetupGraphicsDevice();
			SetupDrawing();
			SetupImage();
		}

		private void SetupGraphicsDevice()
		{
			device = new Mock<Device>().Object;
			testResolver.RegisterMock(device);
		}

		private void SetupDrawing()
		{
			var mockDrawing = new Mock<Drawing>(device);
			mockDrawing.Setup(
				d => d.DrawVertices(It.IsAny<VerticesMode>(), It.IsAny<VertexPositionColor[]>())).Callback(
					(VerticesMode mode, VertexPositionColor[] vertices) =>
						testResolver.NumberOfVerticesDrawn += vertices.Length);
			drawing = testResolver.RegisterMock(mockDrawing.Object);
		}

		private void SetupImage()
		{
			var mockImage = new Mock<Image>("dummy", drawing);
			mockImage.SetupGet(i => i.PixelSize).Returns(new Size(128, 128));
			mockImage.CallBase = true;
			testResolver.RegisterMock(mockImage.Object);
		}

		private void SetupRenderer()
		{
			screen = testResolver.RegisterMock(new QuadraticScreenSpace(window));
			testResolver.RegisterMock(new Mock<Renderer>(drawing, screen).Object);
		}
	}
}
