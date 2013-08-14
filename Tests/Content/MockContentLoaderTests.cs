using DeltaEngine.Content;
using DeltaEngine.Content.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class MockContentLoaderTests
	{
		[SetUp]
		public void Setup()
		{
			new MockContentLoader(new ContentDataResolver());
			content = ContentLoader.Load<MockXmlContent>("Test");
		}

		private MockXmlContent content;

		[Test]
		public void LoadContent()
		{
			Assert.AreEqual("Test", content.Name);
			Assert.IsFalse(content.IsDisposed);
			Assert.Greater(content.LoadCounter, 0);
		}

		[Test]
		public void LoadWithExtensionNotAllowed()
		{
			Assert.Throws<ContentLoader.ContentNameShouldNotHaveExtension>(
				() => ContentLoader.Load<MockXmlContent>("Test.xml"));
		}

		[Test]
		public void LoadCachedContent()
		{
			var contentTwo = ContentLoader.Load<MockXmlContent>("Test");
			Assert.IsFalse(contentTwo.IsDisposed);
			Assert.AreEqual(content, contentTwo);
		}

		[Test]
		public void ForceReload()
		{
			ContentLoader.ReloadContent("Test");
			Assert.Greater(content.LoadCounter, 1);
			Assert.AreEqual(1, content.changeCounter);
		}

		[Test]
		public void TwoContentFilesShouldNotReloadEachOther()
		{
			var content2 = ContentLoader.Load<MockXmlContent>("Test2");
			ContentLoader.ReloadContent("Test2");
			Assert.AreEqual(2, content2.LoadCounter);
			Assert.AreEqual(1, content.LoadCounter);
		}

		[Test]
		public void DisposeContent()
		{
			Assert.IsFalse(content.IsDisposed);
			content.Dispose();
			Assert.IsTrue(content.IsDisposed);
		}

		[Test]
		public void DisposeAndLoadAgainShouldReturnFreshInstance()
		{
			content.Dispose();
			var freshContent = ContentLoader.Load<MockXmlContent>("Test");
			Assert.IsFalse(freshContent.IsDisposed);
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new MockXmlContent(""));
		}

		[Test]
		public void ExceptionOnInstancingFromOutsideContentLoader()
		{
			Assert.Throws<ContentData.MustBeCalledFromContentLoader>(
				() => new MockXmlContent("VectorText"));
		}
	}
}