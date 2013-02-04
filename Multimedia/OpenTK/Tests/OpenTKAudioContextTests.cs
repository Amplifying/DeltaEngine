using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class OpenTKAudioContextTests
	{
		[Test, Category("Slow")]
		public void TestCreation()
		{
			App.Start((OpenTKAudioContext context) => Assert.NotNull(context));
		}

		[Test, Category("Slow")]
		public void PreventMultipleDisposes()
		{
			var context = new OpenTKAudioContext();
			context.Dispose();
			context.Dispose();
		}
	}
}
