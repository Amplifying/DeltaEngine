using System.IO;

namespace DeltaEngine.Content.Tests
{
	public class MockContentLoader : ContentLoader
	{
		public MockContentLoader(ContentDataResolver resolver)
			: base(resolver)
		{
			contentFilenames.Add("Test", "Test.xml");
			contentFilenames.Add("DeltaEngineLogo", "DeltaEngineLogo.png");
		}

		protected override Stream GetContentDataStream(string contentName)
		{
			return Stream.Null;
		}
	}
}