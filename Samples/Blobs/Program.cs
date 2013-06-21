using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs
{
	/// <summary>
	/// Fire your blob onto other blobs to absorb them
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<Game, Camera2DControlledQuadraticScreenSpace>();
		}
	}
}