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
			App app = new App();
			app.Start<FruitBlocksContent, Game>();
		}
	}
}