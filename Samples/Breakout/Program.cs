using DeltaEngine.Platforms;

namespace Breakout
{
	/// <summary>
	/// Simple breakout game with more bricks to destroy each level you advance.
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<Game>();
		}
	}
}