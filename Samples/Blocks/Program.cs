using DeltaEngine.Platforms;

namespace Blocks
{
	internal static class Program
	{
		public static void Main()
		{
			var app = new App();
			app.RegisterSingleton<Grid>();
			app.RegisterSingleton<Controller>();
			app.RegisterSingleton<JewelBlocksContent>();
			app.Start<Game>();
		}
	}
}