using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;

namespace FindTheWord
{
	public class Button : Sprite, IDisposable
	{
		public Button(InputCommands input, string imageName, Rectangle drawArea)
			: base(ContentLoader.Load<Image>(imageName), drawArea)
		{
			Input = input;
			RenderLayer = 2;
			mouseMovement = Input.AddMouseMovement(OnMouseMovementCommand);
			RegisterMouseClickCommands();
		}

		private InputCommands Input { get; set; }
		private readonly Command mouseMovement;

		private void OnMouseMovementCommand(Mouse mouse)
		{
			IsHovered = DrawArea.Contains(mouse.Position);
		}

		public bool IsHovered { get; private set; }

		private void RegisterMouseClickCommands()
		{
			leftMouseButtonPressBegin = Input.Add(MouseButton.Left, State.Pressing,
				mouse => OnLeftMouseButtonPressBegin());
			leftMouseButtonPressEnd = Input.Add(MouseButton.Left, State.Releasing,
				mouse => OnLeftMouseButtonPressEnd());
		}

		private Command leftMouseButtonPressBegin;
		private Command leftMouseButtonPressEnd;

		private void OnLeftMouseButtonPressBegin()
		{
			isPressed = IsHovered;
		}

		private bool isPressed;

		private void OnLeftMouseButtonPressEnd()
		{
			if (isPressed && IsHovered)
				RaiseClickedEvent();
			isPressed = false;
		}

		private void RaiseClickedEvent()
		{
			if (Clicked != null)
				Clicked(this);
		}

		public event Action<Button> Clicked;

		public virtual void Dispose()
		{
			Input.Remove(mouseMovement);
			Input.Remove(leftMouseButtonPressBegin);
			Input.Remove(leftMouseButtonPressEnd);
		}
	}
}