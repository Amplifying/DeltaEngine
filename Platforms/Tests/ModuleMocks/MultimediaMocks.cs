using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	public class MultimediaMocks : BaseMocks
	{
		internal MultimediaMocks(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			SetupMultimedia();
		}

		private void SetupMultimedia()
		{
			resolver.RegisterMock<SoundDevice>().CallBase = true;
			resolver.Register<MockSound>();
			resolver.Register<MockMusic>();
			resolver.Register<MockVideo>();
		}

		public class MockSound : Sound
		{
			public MockSound(string contentName, SoundDevice device)
				: base(contentName, device) {}

			protected override void LoadData(Stream fileData) {}
			public override float LengthInSeconds
			{
				get { return 0.48f; }
			}

			public override void PlayInstance(SoundInstance instanceToPlay)
			{
				playingInstances.Add(instanceToPlay);
			}

			private readonly List<SoundInstance> playingInstances = new List<SoundInstance>();

			public override void StopInstance(SoundInstance instanceToStop)
			{
				playingInstances.Remove(instanceToStop);
			}

			protected override void CreateChannel(SoundInstance instanceToFill) {}
			protected override void RemoveChannel(SoundInstance instanceToRemove) {}

			public override bool IsPlaying(SoundInstance instance)
			{
				return playingInstances.Contains(instance);
			}
		}

		public class MockMusic : Music
		{
			public MockMusic(string filename, SoundDevice device)
				: base(filename, device) {}

			protected override void LoadData(Stream fileData) {}
			protected override void PlayNativeMusic(float volume)
			{
				MusicStopCalled = false;
			}
			protected internal override void Run() {}

			protected override void StopNativeMusic()
			{
				MusicStopCalled = true;
			}

			public override bool IsPlaying()
			{
				return !MusicStopCalled;
			}

			public override float DurationInSeconds
			{
				get { return 4.13f; }
			}

			public override float PositionInSeconds
			{
				get { return 1.0f; }
			}
		}

		public static bool MusicStopCalled { get; private set; }

		public class MockVideoSurface : Entity
		{
			public MockVideoSurface()
			{
				IsActive = true;
			}
		}

		public class MockVideo : Video
		{
			public MockVideo(string filename, SoundDevice device, EntitySystem entitySystem)
				: base(filename, device)
			{
				this.entitySystem = entitySystem;
			}

			private readonly EntitySystem entitySystem;

			protected override void LoadData(Stream fileData) {}
			protected override void PlayNativeVideo(float volume)
			{
				VideoStopCalled = false;
				surface = new MockVideoSurface();
				entitySystem.Add(surface);
			}

			private MockVideoSurface surface;

			protected internal override void Run() {}

			protected override void StopNativeVideo()
			{
				VideoStopCalled = true;
				if (surface != null)
					entitySystem.Remove(surface);
				surface = null;
			}

			public override bool IsPlaying()
			{
				return !VideoStopCalled;
			}

			public override float DurationInSeconds
			{
				get { return 3.33333325f; }
			}

			public override float PositionInSeconds
			{
				get { return 1.0f; }
			}
		}

		public static bool VideoStopCalled { get; private set; }
	}
}