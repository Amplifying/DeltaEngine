using System;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class ContentTests
	{
		[Test]
		public void CreateWithInvalidResolver()
		{
			Assert.Throws<NullReferenceException>(() => new Content(null));
		}

		[Test]
		public void LoadImageContent()
		{
			var resolver = new FakeResolver();
			var image = new Content(resolver).Load<FakeResolver.FakeImage>("Test");
			Assert.IsTrue(image.IsLoaded);
			image.Dispose();
			Assert.IsFalse(image.IsLoaded);
			Assert.IsNull(resolver.Resolve<ContentTests>());
		}

		[Test]
		public void LoadCachedImageContent()
		{
			var resolver = new FakeResolver();
			var content = new Content(resolver);
			var contentOne = content.Load<FakeResolver.FakeImage>("Test");
			var contentTwo = content.Load<FakeResolver.FakeImage>("Test");
			Assert.IsTrue(contentOne.IsLoaded);
			Assert.IsTrue(contentTwo.IsLoaded);
			Assert.AreEqual(contentOne, contentTwo);
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new FakeResolver.FakeImage(""));
		}
	}
}