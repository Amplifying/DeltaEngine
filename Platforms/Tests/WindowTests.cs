using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowTests
	{
		[Test]
		public void WindowTitle()
		{
			using (var resolver = new TestResolver())
				Assert.AreEqual(resolver.Resolve<Window>().Title, "WindowMock");
		}
	}
}