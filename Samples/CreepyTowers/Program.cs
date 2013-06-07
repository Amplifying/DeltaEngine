using DeltaEngine.Platforms;

namespace CreepyTowers
{
	/// <summary>
	/// CreepyTowers Tower Defense game
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<Game>();
		}
	}
}