using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	public class MockContentLoaderTests
	{
		[SetUp]
		public void Setup()
		{
			contentLoader = new MockContentLoader(new MockContentDataResolver());
			content = contentLoader.Load<MockContent>("Test");
		}

		private MockContentLoader contentLoader;
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
				() => contentLoader.Load<TestImage>("DeltaEngineLogo.png"));
		}

		[Test]
		public void GetContentExtension()
		{
			Assert.AreEqual("Test.xml", contentLoader.GetFilenameFromContentName("Test"));
			Assert.AreEqual("DeltaEngineLogo.png",
				contentLoader.GetFilenameFromContentName("DeltaEngineLogo"));

			Assert.Throws<ContentLoader.ContentNotFound>(
				() => contentLoader.GetFilenameFromContentName("abc"));
		}

		[Test]
		public void LoadCachedContent()
		{
			var contentTwo = contentLoader.Load<MockContent>("Test");
			Assert.IsFalse(contentTwo.IsDisposed);
			Assert.AreEqual(content, contentTwo);
		}

		[Test]
		public void ForceReload()
		{
			contentLoader.ReloadContent("Test");
			Assert.Greater(content.LoadCounter, 1);
			Assert.AreEqual(1, content.changeCounter);
		}

		[Test]
		public void TwoContentFilesShouldNotReloadEachOther()
		{
			var content2 = contentLoader.Load<MockContent>("Test2");
			contentLoader.ReloadContent("Test2");
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
			var freshContent = contentLoader.Load<MockContent>("Test");
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