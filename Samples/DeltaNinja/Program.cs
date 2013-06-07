using DeltaEngine.Platforms;

namespace DeltaNinja
{
	internal static class Program
	{
		public static void Main()
		{
			var app = new App();
			app.Start<Game>();
		}
	}
}