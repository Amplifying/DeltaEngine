using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class AsteroidsGame
	{
		public AsteroidsGame()
		{
			controls = new GameControls(this);
			score = 0;
			SetUpBackground();
			GameState = GameState.Playing;
			GameLogic = new GameLogic();
			SetUpEvents();
			controls.SetControlsToState(GameState);
			hudInterface = new HudInterface();
		}

		private void SetUpEvents()
		{
			GameLogic.GameOver += () => 
			{
				GameOver();
			};
			GameLogic.IncreaseScore += increase => 
			{
				score += increase;
				hudInterface.SetScoreText(score);
			};
		}

		internal readonly GameControls controls;
		internal int score;

		public GameLogic GameLogic
		{
			get;
			private set;
		}

		public GameState GameState;
		public readonly HudInterface hudInterface;

		private static void SetUpBackground()
		{
			var background = new Sprite(new Material(Shader.Position2DColorUv, "black-background"), new 
				Rectangle(Point.Zero, new Size(1)));
			background.RenderLayer = (int)AsteroidsRenderLayer.Background;
		}

		public void GameOver()
		{
			GameLogic.Player.IsActive = false;
			GameState = GameState.GameOver;
			controls.SetControlsToState(GameState);
			hudInterface.SetGameOverText();
		}

		public void RestartGame()
		{
			GameLogic.Restart();
			score = 0;
			hudInterface.SetScoreText(score);
			hudInterface.SetIngameMode();
			GameState = GameState.Playing;
			controls.SetControlsToState(GameState);
		}
	}
}