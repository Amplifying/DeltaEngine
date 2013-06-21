using DeltaEngine.Platforms.All;

namespace DeltaEngine.Multimedia.Tests
{
	internal static class Program
	{
		//ncrunch: no coverage start
		public static void Main()
		{
			//new SoundTests().PlaySoundRightAndPitched(TestWithAllFrameworks.Xna);
			//new MusicTests().PlayMusic(TestWithAllFrameworks.OpenGL);
			new MusicTests().PlayMusicWith5Fps(TestWithAllFrameworks.OpenGL);
			//new MusicTests().PlayMusicWith10Fps(TestWithAllFrameworks.OpenGL);
			//new MusicTests().PlayMusicWith30Fps(TestWithAllFrameworks.OpenGL);
			//new VideoTests().PlayVideo(TestWithAllFrameworks.OpenGL);
			//new VideoTests().StartAndStopVideo(TestWithAllFrameworks.OpenGL);
		}
	}
}