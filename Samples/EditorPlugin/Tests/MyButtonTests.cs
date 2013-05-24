using NUnit.Framework;

namespace DeltaEngine.Editor.EditorPlugin.Tests
{
	public class MyButtonTests
	{
		[Test]
		public void ButtonInitiallyNamedClickMe()
		{
			var button = new MyButton();
			Assert.AreEqual("Click Me", button.Text);
		}
	}
}