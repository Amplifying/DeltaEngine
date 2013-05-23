using DeltaEngine.Platforms;

namespace Snake
{
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<SnakeGame>();
		}
	}
}