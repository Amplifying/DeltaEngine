using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	/// <summary>
	/// Displays a number of colored moving logo sprites bouncing around.
	/// </summary>
	internal static class Program
	{
		public static void Main()
		{
			new App().Start<BouncingLogo>(100);
		}
	}
}