using DeltaEngine.Input;
using DeltaEngine.Platforms;

namespace SideScroller
{
	internal class Program : App
	{
		public Program()
		{
			new SideScrollerGame(Resolve<InputCommands>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}

}