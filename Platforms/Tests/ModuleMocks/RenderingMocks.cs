using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using Moq;

namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	public class RenderingMocks : BaseMocks
	{
		internal RenderingMocks(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			SetupWindow();
			SetupGraphics();
			SetupScreen();
			SetupCapturer();
		}

		internal Drawing Drawing { get; private set; }

		private void SetupWindow()
		{
			var windowMock = resolver.RegisterMock<Window>();
			windowMock.Setup(w => w.Visibility).Returns(true);
			windowMock.Setup(w => w.IsClosing).Returns(true);
			windowMock.SetupProperty(w => w.Title, "WindowMock");
			SetupWindowSizeProperties(windowMock);
			Window = windowMock.Object;
		}

		public Window Window { get; private set; }

		private void SetupWindowSizeProperties(Mock<Window> windowMock)
		{
			var currentSize = new Size(640, 360);
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
				rememberSizeBeforeFullscreen = Window.TotalPixelSize;
				Window.TotalPixelSize = displaySize;
			});
			windowMock.Setup(w => w.SetWindowed()).Callback(() =>
			{
				isFullscreen = false;
				Window.TotalPixelSize = rememberSizeBeforeFullscreen;
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
			resolver.RegisterMock(device);
		}

		private Device device;

		private void SetupDrawing()
		{
			var mockDrawing = new Mock<Drawing>(device);
			MockDrawingSetupShapes(mockDrawing);

			MockDrawingSetupSprites(mockDrawing);
			Drawing = resolver.RegisterMock(mockDrawing.Object);
		}

		private void MockDrawingSetupSprites(Mock<Drawing> mockDrawing)
		{
			mockDrawing.Setup(
				d =>
					d.DrawVerticesForSprite(It.IsAny<VerticesMode>(),
						It.IsAny<VertexPositionColorTextured[]>())).Callback(
							(VerticesMode mode, VertexPositionColorTextured[] vertices) =>
							{
								NumberOfVerticesDrawn += vertices.Length;
								NumberOfTimesDrawn++;
							});
		}

		private void MockDrawingSetupShapes(Mock<Drawing> mockDrawing)
		{
			mockDrawing.Setup(
				d => d.DrawVertices(It.IsAny<VerticesMode>(), It.IsAny<VertexPositionColor[]>())).Callback
				((VerticesMode mode, VertexPositionColor[] vertices) =>
				{
					NumberOfVerticesDrawn += vertices.Length;
					NumberOfTimesDrawn++;
				});
		}

		public int NumberOfVerticesDrawn { get; set; }
		public int NumberOfTimesDrawn { get; set; }

		private void SetupImage()
		{
			resolver.Register<MockImage>();
		}

		public class MockImage : Image
		{
			public MockImage(string contentName)
				: base(contentName) {}

			protected override void LoadData(Stream fileData) {}
			protected override void DisposeData() {}
			public override Size PixelSize
			{
				get { return new Size(128); }
			}
			public override bool HasAlpha
			{
				get { return false; }
			}
		}

		private void SetupScreen()
		{
			resolver.RegisterMock(new QuadraticScreenSpace(Window));
		}

		private void SetupCapturer()
		{
			var capturer = new Mock<ScreenshotCapturer>().Object;
			resolver.RegisterMock(capturer);
		}
	}
}