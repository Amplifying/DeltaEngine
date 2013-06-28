using System.IO;
using DeltaEngine.Content.Online;
using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	[Category("Slow"), Ignore]
	public class DeveloperOnlineContentLoaderTests
	{
		//ncrunch: no coverage start
		[Test]
		public void ConnectToOnlineContentService()
		{
			if (Directory.Exists("Content"))
				Directory.Delete("Content", true);
			new DeveloperOnlineContentLoader(new ContentDataResolver());
			Assert.IsTrue(ContentLoader.Exists("DeltaEngineLogo"));
		}
	}
}