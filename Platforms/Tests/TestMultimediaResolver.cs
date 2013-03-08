using System.Collections.Generic;
using DeltaEngine.Multimedia;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	class TestMultimediaResolver : TestModuleResolver
	{
		public TestMultimediaResolver(TestResolver testResolver) 
			: base(testResolver)
		{
			SetupMultimedia();
		}

		private void SetupMultimedia()
		{
			var soundDevice = testResolver.RegisterMock<SoundDevice>();
			SetupSoundMock(soundDevice.Object);
			SetupMusicMock(soundDevice.Object);
		}

		private void SetupSoundMock(SoundDevice soundDevice)
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

		private void SetupMusicMock(SoundDevice soundDevice)
		{
			var mockMusic = new Mock<Music>("dummy", soundDevice);
			mockMusic.SetupGet(s => s.DurationInSeconds).Returns(35.85f);
			mockMusic.SetupGet(s => s.PositionInSeconds).Returns(1.0f);
			testResolver.RegisterMock(mockMusic.Object);
		}
	}
}
