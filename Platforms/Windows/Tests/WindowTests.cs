using NUnit.Framework;

namespace DeltaEngine.Platforms.Windows.Tests
{
	public class WindowTests : WindowsResolver
	{
		[Test, Category("Slow")]
		public void WindowTitle()
		{
			const string TestTitle = "Hi there";
			using (var window = new FormsWindow { Title = TestTitle })
				Assert.AreEqual(window.Title, TestTitle);
		}

		[Test, Ignore]
		public void ShowWindow()
		{
			Run();
		}
	}
}