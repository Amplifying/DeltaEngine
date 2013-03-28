using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	public class MouseDeviceCounterTests
	{
		[Test]
		public void GetNumberOfAvailableMice()
		{
			Assert.DoesNotThrow(delegate
			{
			  var counter = new MouseDeviceCounter();
			  Assert.Greater(counter.GetNumberOfAvailableMice(), 0);
			});
		}

		[Test]
		public void DeviceCounterInMouseProperty()
		{
			var windowsMouse = GetMouse();
			Assert.IsTrue(windowsMouse.IsAvailable);
		}

		private static WindowsMouse GetMouse()
		{
			var resolver = new TestResolver();
			var window = resolver.Resolve<Window>();
			var screen = new QuadraticScreenSpace(window);
			var positionTranslater = new CursorPositionTranslater(window, screen);
			return new WindowsMouse(positionTranslater);
		}
	}
}