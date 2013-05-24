using DeltaEngine.Platforms;

namespace Asteroids
{
	internal static class Program
	{
		public static void Main()
		{
			var app = new App();
			app.Start<AsteroidsGame>();
		}
	}
}