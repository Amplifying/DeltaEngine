using DeltaEngine.Platforms.All;

namespace DeltaEngine.Scenes.Tests
{
	public static class Program
	{
		public static void Main()
		{
			new SceneTests().DrawButtonWhichChangesColorAndSizeAndSpinsOnHover(
				TestWithAllFrameworks.OpenGL);
		}
	}
}