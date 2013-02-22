using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Multimedia.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new SoundTests().PlaySoundRightAndPitched(TestStarter.Xna);
		}
	}
}