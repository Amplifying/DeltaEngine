using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.OpenTK.Tests
{
	public class OpenTKSoundInstanceTests
	{
		[Test]
		public void TestCreationAndDisposeWithoutParent()
		{
			Assert.DoesNotThrow(delegate
			{
				var soundInstance = new OpenTKSoundInstance(null);
				Assert.False(soundInstance.IsPlaying);
			  soundInstance.Dispose();
			});
		}

		[Test, Category("Slow")]
		public void Creation()
		{
			App.Start(delegate(OpenTKSound sound)
			{
				var instance = new OpenTKSoundInstance(sound);
				Assert.False(instance.IsPlaying);
				instance.Dispose();
			});
		}
	}
}
