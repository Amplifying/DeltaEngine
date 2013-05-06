using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	public class FromNameContentLoaderTests
	{
		[Test]
		public void LoadContentFromName()
		{
			var contentLoader = new MockContentLoader(new MockContentDataResolver());
			var content = contentLoader.Load<MockContentFromName>("Test");
			Assert.NotNull(content);
		}
	}
}