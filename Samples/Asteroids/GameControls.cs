using System.Collections.Generic;
using DeltaEngine.Input;

namespace Asteroids
{
	/// <summary>
	/// Controls, can be set according to a gameState
	/// </summary>
	public class GameControls
	{
		public GameControls(AsteroidsGame game, InputCommands input)
		{
			this.game = game;
			this.input = input;
		}

		private readonly InputCommands input;
		private readonly List<Command> addedCommands = new List<Command>();
		private readonly AsteroidsGame game;

		public void SetControlsToState(GameState state)
		{
			input.Clear();
			addedCommands.Clear();
			switch (state)
			{
			case GameState.Playing:
				AddPlayerForward(input);
				AddPlayerSteerLeft(input);
				AddPlayerSteerRight(input);
				AddPlayerFireAndHoldFire(input);
				break;
			case GameState.GameOver:
				AddRestartCommand(input);
				break;
			default:
				return;
			}
		}

		private void AddPlayerForward(InputCommands input)
		{
			addedCommands.Add(input.Add(Key.W, State.Pressed, key => PlayerForward()));
			addedCommands.Add(input.Add(Key.W, State.Pressing, key => PlayerForward()));
			addedCommands.Add(input.Add(Key.CursorUp, State.Pressed, key => PlayerForward()));
			addedCommands.Add(input.Add(Key.CursorUp, State.Pressing, key => PlayerForward()));
		}

		private void AddPlayerSteerRight(InputCommands input)
		{
			addedCommands.Add(input.Add(Key.D, State.Pressed, key => PlayerSteerRight()));
			addedCommands.Add(input.Add(Key.D, State.Pressing, key => PlayerSteerRight()));
			addedCommands.Add(input.Add(Key.CursorRight, State.Pressed, key => PlayerSteerRight()));
			addedCommands.Add(input.Add(Key.CursorRight, State.Pressing, key => PlayerSteerRight()));
		}

		private void AddPlayerSteerLeft(InputCommands input)
		{
			addedCommands.Add(input.Add(Key.A, State.Pressed, key => PlayerSteerLeft()));
			addedCommands.Add(input.Add(Key.A, State.Pressing, key => PlayerSteerLeft()));
			addedCommands.Add(input.Add(Key.CursorLeft, State.Pressed, key => PlayerSteerLeft()));
			addedCommands.Add(input.Add(Key.CursorLeft, State.Pressing, key => PlayerSteerLeft()));
		}

		private void AddPlayerFireAndHoldFire(InputCommands input)
		{
			addedCommands.Add(input.Add(Key.Space, State.Pressing, key => PlayerBeginFireing()));
			addedCommands.Add(input.Add(Key.Space, State.Releasing, key => PlayerHoldFire()));
		}

		private void PlayerForward()
		{
			game.GameLogic.Player.ShipAccelerate();
		}

		private void PlayerSteerLeft()
		{
			game.GameLogic.Player.SteerLeft();
		}

		private void PlayerSteerRight()
		{
			game.GameLogic.Player.SteerRight();
		}

		private void PlayerBeginFireing()
		{
			game.GameLogic.Player.IsFiring = true;
		}

		private void PlayerHoldFire()
		{
			game.GameLogic.Player.IsFiring = false;
		}

		private void AddRestartCommand(InputCommands input)
		{
			input.Add(Key.Space, State.Pressing, key => {game.RestartGame(); });
		}
	}
}