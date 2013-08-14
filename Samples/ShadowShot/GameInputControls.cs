using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace ShadowShot
{
	public class GameInputControls
	{
		public GameInputControls(PlayerShip ship)
		{
			this.ship = ship;
			SetupInputControls();
		}
		private readonly PlayerShip ship;

		private void SetupInputControls()
		{
			var leftCommand = new Command(MoveLeft);
			leftCommand.Add(new KeyTrigger(Key.A, State.Pressed));
			leftCommand.Add(new KeyTrigger(Key.A));
			leftCommand.Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			leftCommand.Add(new KeyTrigger(Key.CursorLeft));

			var rightCommand = new Command(MoveRight);
			rightCommand.Add(new KeyTrigger(Key.D, State.Pressed));
			rightCommand.Add(new KeyTrigger(Key.D));
			rightCommand.Add(new KeyTrigger(Key.CursorRight, State.Pressed));
			rightCommand.Add(new KeyTrigger(Key.CursorRight));

			var stopCommand = new Command(StopMovement);
			stopCommand.Add(new KeyTrigger(Key.S, State.Pressed));
			stopCommand.Add(new KeyTrigger(Key.S));
			stopCommand.Add(new KeyTrigger(Key.CursorDown, State.Pressed));
			stopCommand.Add(new KeyTrigger(Key.CursorDown));

			var fireCommand = new Command(FireWeapon);
			fireCommand.Add(new KeyTrigger(Key.Space, State.Pressed));
			fireCommand.Add(new KeyTrigger(Key.Space));
		}

		private void MoveLeft()
		{
			ship.Accelerate(new Point(-1.0f, 0.0f));
		}

		private void MoveRight()
		{
			ship.Accelerate(new Point(1.0f, 0.0f));
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