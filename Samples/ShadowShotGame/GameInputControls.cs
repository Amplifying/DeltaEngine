using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace ShadowShotGame
{
	public class GameInputControls
	{
		public GameInputControls(InputCommands input, PlayerShip ship)
		{
			this.input = input;
			this.ship = ship;
			SetupInputControls();
		}

		private readonly InputCommands input;
		private readonly PlayerShip ship;

		private void SetupInputControls()
		{
			input.Add(Key.A, State.Pressed, key => MoveLeft());
			input.Add(Key.A, State.Pressing, key => MoveLeft());
			input.Add(Key.CursorLeft, State.Pressed, key => MoveLeft());
			input.Add(Key.CursorLeft, State.Pressing, key => MoveLeft());
			input.Add(Key.D, State.Pressed, key => MoveRight());
			input.Add(Key.D, State.Pressing, key => MoveRight());
			input.Add(Key.CursorRight, State.Pressed, key => MoveRight());
			input.Add(Key.CursorRight, State.Pressing, key => MoveRight());
			input.Add(Key.S, State.Pressed, key => StopMovement());
			input.Add(Key.S, State.Pressing, key => StopMovement());
			input.Add(Key.CursorDown, State.Pressed, key => StopMovement());
			input.Add(Key.CursorDown, State.Pressing, key => StopMovement());
			input.Add(Key.Space, State.Pressed, key => FireWeapon());
			input.Add(Key.Space, State.Pressing, key => FireWeapon());
		}


		private void MoveRight()
		{
			ship.Accelerate(new Point(1.0f, 0.0f));
		}

		private void MoveLeft()
		{
			ship.Accelerate(new Point(-1.0f, 0.0f));
		}

		private void StopMovement()
		{
			ship.Deccelerate();
		}

		private void FireWeapon()
		{
			ship.Fire();
		}
	}
}