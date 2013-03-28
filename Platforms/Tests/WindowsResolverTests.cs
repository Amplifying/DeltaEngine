using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Windows;
using DeltaEngine.Rendering;
using Moq;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowsResolverTests
	{
		[Test]
		public void CreateWindowsResolver()
		{
			var resolver = new MockWindowsResolver();
			Assert.NotNull(resolver);
		}

		[Test]
		public void RegisterNonRenderableObject()
		{
			var resolver = new MockWindowsResolver();
			var rect = new Rectangle(Point.Half, Size.Half);
			resolver.RegisterRunnerInstance(rect);
		}

		[Test]
		public void RegisterRenderableObject()
		{
			var resolver = new MockWindowsResolver();

			var window = new Mock<Window>().Object;
			resolver.RegisterMockInstance(window);

			var device = new Mock<Device>().Object;
			resolver.RegisterMockInstance(device);

			var drawing = new Mock<Drawing>(device).Object;
			resolver.RegisterMockInstance(drawing);

			var line = new Line2D(Point.One, Point.Zero, Color.Red);
			resolver.RegisterRunnerInstance(line);
		}

		private class MockWindowsResolver : WindowsResolver
		{
			public void RegisterMockInstance(object instance)
			{
				RegisterInstance(instance);
			}

			public void RegisterRunnerInstance(object instance)
			{
				RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
			}
		}
	}
}