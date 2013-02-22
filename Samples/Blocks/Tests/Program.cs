using DeltaEngine.Platforms.Tests;

namespace Blocks.Tests
{
	/// <summary>
	/// For running visual tests on demand
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			//new BrickTests().RenderBrick(TestStarter.OpenGL);
			new BlockTests().RenderJBlock(TestStarter.OpenGL);
			//new GridTests().RenderBlankGrid(TestStarter.OpenGL);
			//new GridTests().RenderBlockAffixedToGrid(TestStarter.OpenGL);
			//new GameTests().PlayGame(TestStarter.OpenGL);
			//new ModdableContentTests().LoadContentWithNoModDirectorySet(TestStarter.OpenGL);
			//new ModdableContentTests().LoadContentWithModDirectorySet(TestStarter.OpenGL);
		}
	}
}