using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Breakout
{
	/// <summary>
	/// Simple breakout game with more bricks to destroy each level you advance.
	/// </summary>
	public class Program : App
	{
		public Program()
		{			
			new Background().RenderLayer = 0;
			var input = Resolve<InputCommands>();
			var score = new Score();
			var window = Resolve<RelativeScreenSpace>().Window;
			var game = new Game(new BallInLevel(new Paddle(input), input, new Level(score)), score);
			new UI(window, input, game, Resolve<SoundDevice>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}