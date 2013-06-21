using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using System;

namespace DeltaNinja
{
	class Game : Entity
	{
		public Game(ContentLoader content, Window window, ScreenSpace screen, InputCommands input)
		{
			this.content = content;
			this.window = window;
			this.screen = screen;
			this.input = input;

			window.Title = "Delta Ninja - A Delta Engine sample";
			window.TotalPixelSize = new Size(1280, 720);

			InitBackground();
			InitInput();
			InitPages();

			home.Show();
		}

		private readonly ContentLoader content;
		private readonly Window window;
		private readonly ScreenSpace screen;
		private readonly InputCommands input;

		private HomePage home;
		private Match playing;
		private GameOverPage gameOver;

		private void InitBackground()
		{
			new Background(content, screen);
		}

		private void InitInput()
		{
			input.Add(Key.Escape, x => Exit());
			input.Add(Key.F, x => SwitchWindowMode());
		}

		private void InitPages()
		{
			home = new HomePage(content, screen, input);
			home.ButtonClicked += OnButtonClicked;

			this.playing = new Match(content, screen, input, new NumberFactory(content), new LogoFactory(content, screen));
			playing.GameEnded += (sender, e) => ShowGameOver(e);

			gameOver = new GameOverPage(content, screen, input);
			gameOver.ButtonClicked += OnButtonClicked;
		}

		protected void OnButtonClicked(object sender, EventArgs e)
		{
			switch (((Button)sender).Code)
			{
				case (MenuButton.Home):
					ShowHome();
					break;

				case (MenuButton.NewGame):
				case (MenuButton.Retry):
					StartNewGame();
					break;

				case (MenuButton.Exit):
					Exit();
					break;
			}
		}

		private void SwitchWindowMode()
		{
			if (window.IsFullscreen)
				window.SetWindowed();
			else
				window.SetFullscreen(new Size(1920, 1080));
		}

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
			gameOver.Show();
		}

		private void Exit()
		{
			window.Dispose();
		}
	}
}
