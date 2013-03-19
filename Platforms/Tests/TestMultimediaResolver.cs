using System.Collections.Generic;
using DeltaEngine.Multimedia;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Mocks common Multimedia objects for testing
	/// </summary>
	class TestMultimediaResolver : TestModuleResolver
	{
		public TestMultimediaResolver(TestResolver testResolver) 
			: base(testResolver)
		{
			SetupSoundDeviceMock();
			SetupSoundMock();
			SetupMusicMock();
		}

		public bool MusicStopCalled { get; private set; }

		private SoundDevice soundDevice;

		private void SetupSoundDeviceMock()
		{
			var mockSoundDevice = testResolver.RegisterMock<SoundDevice>();
			soundDevice = mockSoundDevice.Object;			
		}

		private void SetupSoundMock()
		{
			var mockSound = new Mock<Sound>("dummy", soundDevice) { CallBase = true };
			mockSound.SetupGet(s => s.LengthInSeconds).Returns(0.48f);
			var playingSoundInstances = new List<SoundInstance>();
			SetupSoundMockPlayInstance(mockSound, playingSoundInstances);
			SetupSoundMockStopInstance(mockSound, playingSoundInstances);
			SetupSoundMockIsPlaying(mockSound, playingSoundInstances);
			testResolver.RegisterMock(mockSound.Object);
		}

		private static void SetupSoundMockPlayInstance(Mock<Sound> mockSound,
			ICollection<SoundInstance> playingSoundInstances)
		{
			mockSound.Setup(s => s.PlayInstance(It.IsAny<SoundInstance>())).Callback(
				(SoundInstance instance) =>
				{
					mockSound.Object.RaisePlayEvent(instance);
					playingSoundInstances.Add(instance);
				});			
		}

		private static void SetupSoundMockStopInstance(Mock<Sound> mockSound, 
			ICollection<SoundInstance> playingSoundInstances)
		{
			mockSound.Setup(s => s.StopInstance(It.IsAny<SoundInstance>())).Callback(
				(SoundInstance instance) =>
				{
					mockSound.Object.RaiseStopEvent(instance);
					playingSoundInstances.Remove(instance);
				});
		}

		private static void SetupSoundMockIsPlaying(Mock<Sound> mockSound, 
			ICollection<SoundInstance> playingSoundInstances)
		{
			mockSound.Setup(s => s.IsPlaying(It.IsAny<SoundInstance>())).Returns(
				(SoundInstance instance) => playingSoundInstances.Contains(instance));
		}

		private void SetupMusicMock()
		{
			var mockMusic = new Mock<Music>("dummy", soundDevice);
			mockMusic.SetupGet(m => m.DurationInSeconds).Returns(4.13f);
			mockMusic.SetupGet(m => m.PositionInSeconds).Returns(1.0f);
			mockMusic.SetupGet(m => m.IsPlaying).Returns(true);
			mockMusic.Setup(m => m.Stop()).Callback(() => MusicStopCalled = true);
			testResolver.RegisterMock(mockMusic.Object);
		}
	}
}
