using DeltaEngine.Platforms;

namespace EmptyGame
{
	/// <summary>
	/// Just starts the Game class. For more complex examples see the other sample games.
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<Game>();
		}
	}
}