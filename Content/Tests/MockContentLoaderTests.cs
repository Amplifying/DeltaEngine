using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	public class MockContentLoaderTests
	{
		[SetUp]
		public void Setup()
		{
			new MockContentLoader(new ContentDataResolver());
			content = ContentLoader.Load<MockContent>("Test");
		}

		private MockContent content;

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
				() => ContentLoader.Load<TestImage>("DeltaEngineLogo.png"));
		}

		[Test]
		public void LoadCachedContent()
		{
			var contentTwo = ContentLoader.Load<MockContent>("Test");
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
			var content2 = ContentLoader.Load<MockContent>("Test2");
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
			var freshContent = ContentLoader.Load<MockContent>("Test");
			Assert.IsFalse(freshContent.IsDisposed);
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new MockContent(""));
		}

		[Test]
		public void ExceptionOnInstancingFromOutsideContentLoader()
		{
			Assert.Throws<ContentData.MustBeCalledFromContentLoader>(() => new MockContent("VectorText"));
		}
	}
}