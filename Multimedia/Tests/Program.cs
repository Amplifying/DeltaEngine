using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Multimedia.Tests
{
	internal static class Program
	{
		//ncrunch: no coverage start
		public static void Main()
		{
			//new SoundTests().PlaySoundRightAndPitched(TestStarter.Xna);
			//new MusicTests().PlayMusic(TestStarter.Xna);
			new VideoTests().PlayVideo(TestStarter.Xna);
		}
	}
}