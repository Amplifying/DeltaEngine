using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// Game Logics and initialization for Asteroids
	/// </summary>
	public class AsteroidsGame
	{
		public AsteroidsGame(ContentLoader content, InputCommands input, ScreenSpace screenSpace)
		{
			this.content = content;
			controls = new GameControls(this, input);
			score = 0;
			SetUpBackground();
			GameState = GameState.Playing;
			GameLogic = new GameLogic(content);
			SetUpEvents();
			controls.SetControlsToState(GameState);
			hudInterface = new HudInterface(content, screenSpace);
		}

		private void SetUpEvents()
		{
			GameLogic.GameOver += () => { GameOver(); };
			GameLogic.IncreaseScore += increase =>
			{
				score += increase;
				hudInterface.SetScoreText(score);
			};
		}

		public readonly ContentLoader content;
		internal readonly GameControls controls;
		internal int score;
		public GameLogic GameLogic { get; private set; }
		public GameState GameState;
		public readonly HudInterface hudInterface;

		private void SetUpBackground()
		{
			new Sprite(content.Load<Image>("black-background"), new Rectangle(Point.Zero, new Size(1)));
		}

		public void GameOver()
		{
			GameLogic.Player.IsActive = false;
			GameState = GameState.GameOver;
			controls.SetControlsToState(GameState);
			hudInterface.SetGameOverText();
		}
	}
}