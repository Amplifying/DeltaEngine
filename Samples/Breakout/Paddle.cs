using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Breakout
{
	/// <summary>
	/// Holds the paddle position
	/// </summary>
	public class Paddle : Sprite
	{
		public Paddle(Content content, InputCommands inputCommands, Time time)
			: base(content.Load<Image>("Paddle"), Rectangle.One)
		{
			RegisterInputCommands(inputCommands, time);
		}

		private void RegisterInputCommands(InputCommands inputCommands, Time time)
		{
			inputCommands.Add(Key.CursorLeft, State.Pressed,
				() => xPosition -= PaddleMovementSpeed * time.CurrentDelta);
			inputCommands.Add(Key.CursorRight, State.Pressed,
				() => xPosition += PaddleMovementSpeed * time.CurrentDelta);
			inputCommands.Add(MouseButton.Left, State.Pressed,
				mouse => xPosition += mouse.Position.X - Position.X);
			inputCommands.Add(State.Pressed, touch => xPosition += touch.GetPosition(0).X - Position.X);
			inputCommands.Add(GamePadButton.Left, State.Pressed,
				() => xPosition -= PaddleMovementSpeed * time.CurrentDelta);
			inputCommands.Add(GamePadButton.Right, State.Pressed,
				() => xPosition += PaddleMovementSpeed * time.CurrentDelta);
		}

		private float xPosition = 0.5f;
		private const float PaddleMovementSpeed = 1.5f;

		protected override void Render(Renderer renderer, Time time)
		{
			xPosition = xPosition.Clamp(HalfWidth, 1.0f - HalfWidth);
			DrawArea = Rectangle.FromCenter(xPosition, YPosition, Width, Height);
			base.Render(renderer, time);
		}

		private const float YPosition = 0.9f;
		internal const float HalfWidth = Width / 2.0f;
		private const float Width = 0.2f;
		private const float Height = 0.04f;

		public Point Position
		{
			get { return new Point(DrawArea.Center.X, DrawArea.Top); }
		}
	}
}