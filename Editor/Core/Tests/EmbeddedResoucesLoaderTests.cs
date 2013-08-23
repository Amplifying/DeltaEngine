using System.IO;
using System.Text;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class EmbeddedResoucesLoaderTests
	{
		[Test]
		public void GetEmbeddedResourceThatDoesNotExistShouldThrowException()
		{
			Assert.Throws<EmbeddedResoucesLoader.EmbeddedResourceNotFound>(
				() => EmbeddedResoucesLoader.GetFullResourceName("NotAvailable"));
		}

		[Test]
		public void GetExistingEmbeddedResourceName()
		{
			const string ResourcePath = "DeltaEngine.Editor.Core.Tests.EmbeddedResources.HelloWorld.txt";
			Assert.AreEqual(ResourcePath, EmbeddedResoucesLoader.GetFullResourceName(ResourceName));
		}

		private const string ResourceName = "EmbeddedResources.HelloWorld.txt";

		[Test]
		public void GetEmbeddedResourceStream()
		{
			Stream fileStream = EmbeddedResoucesLoader.GetEmbeddedResourceStream(ResourceName);
			var data = StreamToByteArray(fileStream);
			var actualText = Encoding.UTF8.GetString(data);
			Assert.AreEqual("Hello, World!", actualText);
		}

		private static byte[] StreamToByteArray(Stream stream)
		{
			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}
	}
}