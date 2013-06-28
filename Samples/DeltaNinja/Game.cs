using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using DeltaNinja.Entities;
using DeltaNinja.Pages;

namespace DeltaNinja
{
	class Game : Entity
	{
		public Game(ScreenSpace screen, InputCommands input)
		{
			this.screen = screen;
			window = screen.Window;
			this.input = input;

			window.Title = "Delta Ninja - A Delta Engine sample";
			window.ViewportPixelSize = new Size(1280, 720);

			InitBackground();
			InitInput();
			InitPages();
						
			home.Show();
		}

		private readonly Window window;
		private readonly ScreenSpace screen;
		private readonly InputCommands input;

		private HomePage home;
		private Match playing;
		private GameOverPage gameOver;
		
		private void InitBackground()
		{
			new Background(screen);
		}

		private void InitInput()
		{
			// input.Add(Key.Escape, m => { Exit(); });
			
			// TODO: seems not work properly
			// input.Add(Key.F, x => SwitchWindowMode());			
		}

		private void InitPages()
		{
			home = new HomePage(screen, input);			
			home.ButtonClicked += OnButtonClicked;

			this.playing = new Match(screen, input, new NumberFactory(), new LogoFactory(screen));
			playing.GameEnded += (sender, e) => ShowGameOver(e);

			gameOver = new GameOverPage(screen, input);
			gameOver.ButtonClicked += OnButtonClicked;
		}

		protected void OnButtonClicked(MenuButton code)
		{
			switch (code)
			{
				case MenuButton.Home:
					ShowHome();
					break;

				case MenuButton.NewGame:
				case MenuButton.Retry:
					StartNewGame();
					break;

				case MenuButton.Exit:
					Exit();
					break;
			}
		}

		//private void SwitchWindowMode()
		//{
		//	if (window.IsFullscreen)
		//		window.SetWindowed();
		//	else
		//		window.SetFullscreen(new Size(1920, 1080));
		//}

		private void StartNewGame()
		{
			home.Hide();
			gameOver.Hide();
			playing.Start();
		}

		private void ShowHome()
		{
			playing.HideScore();
			gameOver.Hide();
			home.Show();
		}

		private void ShowGameOver(GameOverEventArgs e)
		{
			if (e == null)
				ShowHome();
			else
				gameOver.Show();
		}

		private void Exit()
		{
			window.Dispose();
		}
	}
}
