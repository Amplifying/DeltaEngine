using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces.Terminal
{
	public class TerminalTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestDrawAreaCalculation()
		{
			Window.ViewportPixelSize = new Size(500, 500);
			var terminal = new Scenes.UserInterfaces.Terminal.Terminal(Window);
			Assert.AreEqual(0.01f, terminal.DrawArea.Left);
			Assert.AreEqual(0.1f, terminal.DrawArea.Top);
			Assert.AreEqual(0.99f, terminal.DrawArea.Right);
			Assert.AreEqual(0.98f, terminal.DrawArea.Width);
		}
	}
}