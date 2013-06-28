using DeltaEngine.Platforms;

namespace FountainApp
{
	public class Program : App
	{
		public Program()
		{
			new Fountain();
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}