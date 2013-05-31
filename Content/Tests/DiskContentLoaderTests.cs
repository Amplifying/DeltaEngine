using System.Collections.Generic;
using DeltaEngine.Content.Disk;
using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	/// <summary>
	/// Note: ContentMetaData.xml does not exist on purpose, it should be created automatically!
	/// </summary>
	[Category("Slow")]
	internal class DiskContentLoaderTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Setup()
		{
			contentLoader = new DiskContentLoader(new MockContentDataResolver());
			content = contentLoader.Load<TestImage>("DeltaEngineLogo");
		}

		private DiskContentLoader contentLoader;
		private ContentData content;

		[Test]
		public void MakeSureContentPathIsCorrect()
		{
			Assert.AreEqual("Content", contentLoader.ContentPath);
		}

		[Test]
		public void LoadImageContent()
		{
			Assert.AreEqual("DeltaEngineLogo", content.Name);
			Assert.IsFalse(content.IsDisposed);
		}

		[Test]
		public void LoadNonExistingImageFails()
		{
			Assert.Throws<ContentLoader.ContentNotFound>(
				() => contentLoader.Load<TestImage>("FailImage"));
		}

		[Test]
		public void GetContentExtension()
		{
			Assert.AreEqual("DeltaEngineLogo.png",
				contentLoader.GetFilenameFromContentName("DeltaEngineLogo"));
			Assert.Throws<ContentLoader.ContentNotFound>(
				() => contentLoader.GetFilenameFromContentName("abc"));
		}

		[Test]
		public void SetContentPath()
		{
			Assert.IsTrue(contentLoader.SetPathIfAcceptable("Content"));
			Assert.IsFalse(contentLoader.SetPathIfAcceptable("sdkfz,>"));
		}

		[Test]
		public void LoadListOfContentDataFromParentEntry()
		{
			List<TestImage> animationImageList =
				contentLoader.LoadRecursively<TestImage>("TestAnimation");
			Assert.AreEqual(2, animationImageList.Count);
		}

		[Test]
		public void LoadingNonExistantParentCausesException()
		{
			var animationImageList = new List<TestImage>();
			Assert.Throws<DiskContentLoader.ParentEntryForRecursiveLoadingNotExistant>(
				() => { animationImageList = contentLoader.LoadRecursively<TestImage>("NoAniForYou!"); });
		}

		//ncrunch: no coverage end

		[Test]
		public void LoadingCachedContentOfTheWrongTypeThrowsException()
		{
			Assert.DoesNotThrow(() => contentLoader.Load<TestImage>("DeltaEngineLogo"));
			Assert.Throws<ContentLoader.CachedResourceExistsButIsOfTheWrongType>(
				() => contentLoader.Load<TestSound>("DeltaEngineLogo"));
		}
	}
}