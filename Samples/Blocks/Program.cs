using DeltaEngine.Platforms;

namespace Blocks
{
	/// <summary>
	/// Falling blocks can be moved and rotated. Points are scored by filling rows.
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			var app = new App();
			app.RegisterSingleton<Grid>();
			app.RegisterSingleton<Controller>();
			app.RegisterSingleton<JewelBlocksContent>();
			app.RegisterSingleton<Soundbank>();
			app.Start<Game>();
		}
	}
}