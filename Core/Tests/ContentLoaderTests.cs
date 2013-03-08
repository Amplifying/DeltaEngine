using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class ContentLoaderTests
	{
		[Test]
		public void LoadImageContent()
		{
			var resolver = new FakeResolver();
			var image = new Content(resolver).Load<FakeResolver.FakeImage>("Test");
			Assert.IsTrue(image.IsLoaded);
			image.Dispose();
			Assert.IsFalse(image.IsLoaded);
			Assert.IsNull(resolver.Resolve<ContentLoaderTests>());
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new FakeResolver.FakeImage(""));
		}
	}
}