using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace FindTheWord
{
	public class Game
	{
		public Game(Window window, StartupScreen startScreen, GameScreen gameScreen)
		{
			window.BackgroundColor = Color.Gray;
			window.TotalPixelSize = new Size(1280, 800);
			this.startScreen = startScreen;
			startScreen.GameStarted += OnStartupScreenGameStarted;
			this.gameScreen = gameScreen;
			gameScreen.Hide();
		}

		private readonly StartupScreen startScreen;
		private readonly GameScreen gameScreen;

		private void OnStartupScreenGameStarted()
		{
			startScreen.FadeOut();
			gameScreen.FadeIn();
		}
	}
}