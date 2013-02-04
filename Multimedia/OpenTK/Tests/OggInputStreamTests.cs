using System.IO;
using NUnit.Framework;
using OggStreamingUtils;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class OggInputStreamTests
	{
		[Test, Category("Slow")]
		public void LoadWithTestFile()
		{
			using (var stream = File.OpenRead("../../testmusic.ogg"))
			{
				var oggStream = new OggInputStream(stream);
				Assert.AreEqual(2, oggStream.Channels);
				Assert.AreEqual(44100, oggStream.SampleRate);
				var buffer = new byte[4096 * 8];
				oggStream.Read(buffer);
			}
		}

		[Test]
		public void LoadWithWrongStream()
		{
			using (var stream = new MemoryStream())
			{
				var oggStream = new OggInputStream(stream);
				Assert.AreEqual(0, oggStream.Channels);
				Assert.AreEqual(0, oggStream.SampleRate);
			}
		}
	}
}
