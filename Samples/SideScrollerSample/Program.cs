using DeltaEngine.Platforms;

namespace SideScrollerSample
{
	internal static class Program
	{
		public static void Main()
		{
			var app = new App();
			app.Start<SideScrollerGame>();
		}
	}
}