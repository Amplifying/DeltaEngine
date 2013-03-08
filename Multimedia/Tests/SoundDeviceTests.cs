using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundDeviceTests
	{
		[Test]
		public void Run()
		{
			var device = new MockSoundDevice();
			var music = new MockMusic("DefaultMusic", device);
			Assert.False(music.RunHasBeenCalled);
			music.Play();
			device.Run();
			Assert.True(music.RunHasBeenCalled);
		}

		[Test]
		public void PlayMusicWhileOtherIsPlaying()
		{
			var device = new MockSoundDevice();
			var music1 = new MockMusic("DefaultMusic", device);
			var music2 = new MockMusic("DefaultMusic", device);
			Assert.False(music1.HasBeenStopped);
			Assert.False(music2.HasBeenStopped);
			music1.Play();
			music2.Play();
			Assert.True(music1.HasBeenStopped);
			Assert.False(music2.HasBeenStopped);
		}

		private class MockSoundDevice : SoundDevice
		{
			public override void Dispose() { }//ncrunch: no coverage
		}

		private class MockMusic : Music
		{
			public MockMusic(string filename, SoundDevice device)
				: base(filename, device) { }

			protected override void PlayNativeMusic(float volume) { }

			protected override void Run()
			{
				RunHasBeenCalled = true;
			}

			public bool RunHasBeenCalled;

			public override void Stop()
			{
				HasBeenStopped = true;
			}

			public bool HasBeenStopped;

			//ncrunch: no coverage start
			public override void Dispose() {}

			public override bool IsPlaying
			{
				get { return true; }
			}

			public override float DurationInSeconds
			{
				get { return 2f; }
			}
			public override float PositionInSeconds
			{
				get { return 1f; }
			}
		}
	}
}