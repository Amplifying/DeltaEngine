using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	public class TestRenderingResolver : TestModuleResolver
	{
		public TestRenderingResolver(TestResolver testResolver) 
			: base(testResolver) {}

		public override void Register()
		{
			SetupWindow();
			SetupGraphics();
			SetupRenderer();			
		}
		
		internal Drawing Drawing { get; private set; }
		internal Renderer Renderer { get; private set; }
		internal Mock<Image> MockImage { get; private set; }

		private void SetupWindow()
		{
			var windowMock = testResolver.RegisterMock<Window>();
			windowMock.Setup(w => w.IsVisible).Returns(true);
			windowMock.Setup(w => w.IsClosing).Returns(true);
			windowMock.SetupProperty(w => w.Title, "WindowMock");
			SetupWindowSizeProperties(windowMock);
			window = windowMock.Object;
		}

		private Window window;

		private void SetupWindowSizeProperties(Mock<Window> windowMock)
		{
			var currentSize = new Size(1024, 640);
			windowMock.SetupGet(w => w.TotalPixelSize).Returns(() => currentSize);
#pragma warning disable 612,618
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

		private Device device;

		private void SetupDrawing()
		{
			var mockDrawing = new Mock<Drawing>(device);
			mockDrawing.Setup(
				d => d.DrawVertices(It.IsAny<VerticesMode>(), It.IsAny<VertexPositionColor[]>())).Callback(
					(VerticesMode mode, VertexPositionColor[] vertices) =>
						testResolver.NumberOfVerticesDrawn += vertices.Length);
			Drawing = testResolver.RegisterMock(mockDrawing.Object);
		}

		private void SetupImage()
		{
			MockImage = new Mock<Image>("dummy", Drawing);
			MockImage.SetupGet(i => i.PixelSize).Returns(new Size(128, 128));
			MockImage.CallBase = true;
			testResolver.RegisterMock(MockImage.Object);
		}

		private void SetupRenderer()
		{
			var screen = testResolver.RegisterMock(new QuadraticScreenSpace(window));
			Renderer = new Mock<Renderer>(Drawing, screen).Object;
			testResolver.RegisterMock(Renderer);
		}
	}
}
