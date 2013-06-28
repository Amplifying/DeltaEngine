using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardTests : TestWithMocksOrVisually
	{
		[Test]
		public void GraphicalUnitTest()
		{
			Resolve<ScreenSpace>().Window.Title = "Press A to show ellipse";
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f),
				Color.GetRandomBrightColor());
			RunCode =
				() =>
				{
					ellipse.Center = Resolve<Keyboard>().GetKeyState(Key.A) == State.Pressed
						? Point.Half : Point.Zero;
				};
		}

		[Test]
		public void UpdateKeyboard()
		{
			var keyboard = Resolve<Keyboard>();
			keyboard.Run();
			Assert.True(keyboard.IsAvailable);
			Assert.AreEqual(keyboard.GetKeyState(Key.B), State.Released);
			Assert.AreEqual(keyboard.GetKeyState(Key.Enter), State.Released);
		}
	}
}