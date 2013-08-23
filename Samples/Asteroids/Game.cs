﻿using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// Game Logics and initialization for Asteroids
	/// </summary>
	public class Game
	{
		public Game()
		{
			controls = new Controls(this);
			score = 0;
			SetUpBackground();
			GameState = GameState.Playing;
			InteractionLogics = new InteractionLogics();
			SetUpEvents();
			controls.SetControlsToState(GameState);
			hudInterface = new HudInterface();
		}

		private void SetUpEvents()
		{
			InteractionLogics.GameOver += () => { GameOver(); };
			InteractionLogics.IncreaseScore += increase =>
			{
				score += increase;
				hudInterface.SetScoreText(score);
			};
		}

		private readonly Controls controls;
		private int score;
		public InteractionLogics InteractionLogics { get; private set; }
		public GameState GameState;
		private readonly HudInterface hudInterface;

		private static void SetUpBackground()
		{
			var background = new Sprite(new Material(Shader.Position2DColorUv, "black-background"), 
				new Rectangle(Point.Zero, new Size(1)));
			background.RenderLayer = (int)AsteroidsRenderLayer.Background;
		}

		public void GameOver()
		{
			if(GameState == GameState.GameOver)
				return;
			InteractionLogics.Player.IsActive = false;
			GameState = GameState.GameOver;
			controls.SetControlsToState(GameState);
			hudInterface.SetGameOverText();
		}

		public void RestartGame()
		{
			InteractionLogics.Restart();
			score = 0;
			hudInterface.SetScoreText(score);
			hudInterface.SetIngameMode();
			GameState = GameState.Playing;
			controls.SetControlsToState(GameState);
		}
	}
}