using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace Blocks
{
	/// <summary>
	/// Knits the main control classes together by feeding events raised by one to another
	/// </summary>
	public class Game : Runner<Time>
	{
		public Game(Controller controller, UserInterface userInterface, InputCommands input)
		{
			this.controller = controller;
			this.input = input;
			SetControllerEvents(userInterface);
			SetInputEvents();
		}

		private readonly Controller controller;
		private readonly InputCommands input;

		private void SetControllerEvents(UserInterface userInterface)
		{
			controller.AddToScore += userInterface.AddToScore;
			controller.Lose += userInterface.Lose;
		}

		private void SetInputEvents()
		{
			SetKeyboardEvents();
			SetMouseEvents();
			SetTouchEvents();
		}

		private void SetKeyboardEvents()
		{
			input.Add(Key.CursorLeft, State.Pressing, StartMovingBlockLeft);
			input.Add(Key.CursorLeft, State.Releasing, () => { isBlockMovingLeft = false; });
			input.Add(Key.CursorRight, State.Pressing, StartMovingBlockRight);
			input.Add(Key.CursorRight, State.Releasing, () => { isBlockMovingRight = false; });
			input.Add(Key.CursorUp, State.Pressing, controller.RotateBlockAntiClockwiseIfPossible);
			input.Add(Key.CursorDown, State.Pressing, () => { controller.IsFallingFast = true; });
			input.Add(Key.CursorDown, State.Releasing, () => { controller.IsFallingFast = false; });
		}

		private void StartMovingBlockLeft()
		{
			controller.MoveBlockLeftIfPossible();
			isBlockMovingLeft = true;
		}

		private bool isBlockMovingLeft;

		private void StartMovingBlockRight()
		{
			controller.MoveBlockRightIfPossible();
			isBlockMovingRight = true;
		}

		private bool isBlockMovingRight;

		private void SetMouseEvents()
		{
			input.Add(MouseButton.Left, State.Pressing, mouse => Pressing(mouse.Position));
			input.Add(MouseButton.Left, State.Releasing, mouse => { controller.IsFallingFast = false; });
		}

		private void Pressing(Point position)
		{
			if (position.X < 0.4f)
				controller.MoveBlockLeftIfPossible();
			else if (position.X > 0.6f)
				controller.MoveBlockRightIfPossible();
			else if (position.Y < 0.5f)
				controller.RotateBlockAntiClockwiseIfPossible();
			else
				controller.IsFallingFast = true;
		}

		private void SetTouchEvents()
		{
			input.Add(State.Pressing, touch => Pressing(touch.GetPosition(0)));
			input.Add(State.Releasing, touch => { controller.IsFallingFast = false; });
		}

		public void Run(Time time)
		{
			UpdateElapsed(time);
			if (elapsed >= BlockMoveInterval)
				MoveBlock();
		}

		private const float BlockMoveInterval = 0.166f;

		private void UpdateElapsed(Time time)
		{
			if (isBlockMovingLeft || isBlockMovingRight)
				elapsed += time.CurrentDelta;
			else
				elapsed = 0;
		}

		private float elapsed;

		private void MoveBlock()
		{
			elapsed -= BlockMoveInterval;
			if (isBlockMovingLeft)
				controller.MoveBlockLeftIfPossible();

			if (isBlockMovingRight)
				controller.MoveBlockRightIfPossible();
		}
	}
}