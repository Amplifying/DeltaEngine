using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace Breakout
{
	/// <summary>
	/// Holds the paddle position
	/// </summary>
	public class Paddle : Sprite
	{
		public Paddle(InputCommands inputCommands)
			: base(ContentLoader.Load<Image>("Paddle"), Rectangle.One)
		{
			RegisterInputCommands(inputCommands);
			Start<RunPaddle>();
			RenderLayer = 5;
		}

		private void RegisterInputCommands(InputCommands inputCommands)
		{
			inputCommands.Add(Key.CursorLeft, State.Pressed,
				key => xPosition -= PaddleMovementSpeed * Time.Current.Delta);
			inputCommands.Add(Key.CursorRight, State.Pressed,
				key => xPosition += PaddleMovementSpeed * Time.Current.Delta);
			inputCommands.Add(MouseButton.Left, State.Pressed,
				mouse => xPosition += mouse.Position.X - Position.X);
			inputCommands.Add(State.Pressed, touch => xPosition += touch.GetPosition(0).X - Position.X);
			inputCommands.Add(GamePadButton.Left, State.Pressed,
				() => xPosition -= PaddleMovementSpeed * Time.Current.Delta);
			inputCommands.Add(GamePadButton.Right, State.Pressed,
				() => xPosition += PaddleMovementSpeed * Time.Current.Delta);
		}

		private float xPosition = 0.5f;
		private const float PaddleMovementSpeed = 1.5f;

		public class RunPaddle : EventListener2D
		{
			public override void ReceiveMessage(Entity2D entity, object message)
			{
				Paddle paddle = (Paddle)entity;
				var xPosition = paddle.xPosition.Clamp(HalfWidth, 1.0f - HalfWidth);
				paddle.DrawArea = Rectangle.FromCenter(xPosition, YPosition, Width, Height);
			}
		}

		private const float YPosition = 0.9f;
		internal const float HalfWidth = Width / 2.0f;
		private const float Width = 0.2f;
		private const float Height = 0.04f;

		public Point Position
		{
			get { return new Point(DrawArea.Center.X, DrawArea.Top); }
		}
		public void Run()
		{
			
		}
	}
}