using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blocks
{
	/// <summary>
	/// Knits the main control classes together by feeding events raised by one to another
	/// </summary>
	public class Game
	{
		public Game(ScreenSpace screen, InputCommands input, BlocksContent content,
			SoundDevice soundDevice)
		{
			screen.Window.WindowClosing += soundDevice.Dispose;
			this.screen = screen;
			this.input = input;
			UserInterface = new UserInterface(content);
			Controller = new Controller(DisplayMode, content);
			screen.Window.ViewportSizeChanged += ShowCorrectSceneForAspect;
			Initialize();
		}

		private readonly ScreenSpace screen;
		private readonly InputCommands input;
		public UserInterface UserInterface { get; private set; }
		public Controller Controller { get; private set; }

		private void Initialize()
		{
			SetDisplayMode();
			ShowCorrectSceneForAspect(screen.Window.ViewportPixelSize);
			SetControllerEvents();
			SetInputEvents();
		}

		private void SetDisplayMode()
		{
			screen.Window.TotalPixelSize = new Size(700, 700);
			screen.Window.Title = "Sample Blocks Game";
			var aspectRatio = screen.Viewport.Aspect;
			DisplayMode = aspectRatio >= 1.0f
				? Orientation.Landscape : Orientation.Portrait;
		}

		protected Orientation DisplayMode { get; set; }

		private void ShowCorrectSceneForAspect(Size size)
		{
			if (size.AspectRatio >= 1.0f)
				UserInterface.ShowUserInterfaceLandscape();
			else
				UserInterface.ShowUserInterfacePortrait();
		}

		private void SetControllerEvents()
		{
			Controller.AddToScore += UserInterface.AddToScore;
			Controller.Lose += UserInterface.Lose;
		}

		private void SetInputEvents()
		{
			SetKeyboardEvents();
			SetMouseEvents();
			SetTouchEvents();
		}

		private void SetKeyboardEvents()
		{
			input.Add(Key.CursorLeft, State.Pressing, key => StartMovingBlockLeft());
			input.Add(Key.CursorLeft, State.Releasing, key => { Controller.isBlockMovingLeft = false; });
			input.Add(Key.CursorRight, State.Pressing, key => StartMovingBlockRight());
			input.Add(Key.CursorRight, State.Releasing,
				key => { Controller.isBlockMovingRight = false; });
			input.Add(Key.CursorUp, State.Pressing,
				key => Controller.RotateBlockAntiClockwiseIfPossible());
			input.Add(Key.CursorDown, State.Pressing, key => { Controller.IsFallingFast = true; });
			input.Add(Key.CursorDown, State.Releasing, key => { Controller.IsFallingFast = false; });
		}

		private void StartMovingBlockLeft()
		{
			Controller.MoveBlockLeftIfPossible();
			Controller.isBlockMovingLeft = true;
		}

		private void StartMovingBlockRight()
		{
			Controller.MoveBlockRightIfPossible();
			Controller.isBlockMovingRight = true;
		}

		private void SetMouseEvents()
		{
			input.Add(MouseButton.Left, State.Pressing, mouse => Pressing(mouse.Position));
			input.Add(MouseButton.Left, State.Releasing, mouse => { Controller.IsFallingFast = false; });
		}

		private void Pressing(Point position)
		{
			if (position.X < 0.4f)
				Controller.MoveBlockLeftIfPossible();
			else if (position.X > 0.6f)
				Controller.MoveBlockRightIfPossible();
			else if (position.Y < 0.5f)
				Controller.RotateBlockAntiClockwiseIfPossible();
			else
				Controller.IsFallingFast = true;
		}

		private void SetTouchEvents()
		{
			input.Add(State.Pressing, touch => Pressing(touch.GetPosition(0)));
			input.Add(State.Releasing, touch => { Controller.IsFallingFast = false; });
		}
	}
}