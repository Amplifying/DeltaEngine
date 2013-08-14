using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;

namespace DeltaEngine.Scenes.UserInterfaces.Controls
{
	/// <summary>
	/// Enables a control to respond to mouse and touch input.
	/// </summary>
	public class Interact : UpdateBehavior
	{
		public Interact()
			: base(Priority.High)
		{
			new Command(point => leftClick = point).Add(new MouseButtonTrigger());
			new Command(point => leftRelease = point).Add(new MouseButtonTrigger(State.Releasing));
			new Command(point => movement = point).Add(new MouseMovementTrigger());
			new Command(rectangle => dragArea = rectangle).Add(new MouseDragTrigger());
		}

		private Point leftClick = Point.Unused;
		private Point leftRelease = Point.Unused;
		private Point movement = Point.Unused;
		private Rectangle dragArea = Rectangle.Unused;

		public override void Update(IEnumerable<Entity> entities)
		{
			// ReSharper disable PossibleMultipleEnumeration
			if (dragArea == Rectangle.Unused)
				ResetDrag(entities);
			if (DidATriggerFireThisFrame())
				UpdateStateOfControls(entities);
			ProcessShiftOfFocus(entities);
			Reset();
			// ReSharper restore PossibleMultipleEnumeration
		}

		private static void ResetDrag(IEnumerable<Entity> entities)
		{
			foreach (Control control in entities.OfType<Control>())
				ResetDragForControl(control.State);
		}

		private static void ResetDragForControl(InteractiveState state)
		{
			state.DragDelta = Point.Zero;
			state.DragArea = Rectangle.Unused;
		}

		private bool DidATriggerFireThisFrame()
		{
			return leftClick != Point.Unused || leftRelease != Point.Unused || movement != Point.Unused ||
				dragArea != Rectangle.Unused;
		}

		private void UpdateStateOfControls(IEnumerable<Entity> entities)
		{
			foreach (Control control in entities.OfType<Control>())
				UpdateState(control, control.State);
		}

		private void UpdateState(Control control, InteractiveState state)
		{
			ProcessAnyLeftClick(control, state);
			ProcessAnyLeftRelease(control, state);
			ProcessAnyMovement(control, state);
			ProcessAnyDrag(state);
		}

		private void ProcessAnyLeftClick(Control control, InteractiveState state)
		{
			if (leftClick != Point.Unused)
				state.IsPressed = control.RotatedDrawAreaContains(leftClick);
		}

		private void ProcessAnyLeftRelease(Control control, InteractiveState state)
		{
			if (leftRelease == Point.Unused)
				return;

			if (state.IsPressed && control.RotatedDrawAreaContains(leftRelease))
				ClickControl(control, state);
			else
				state.IsPressed = false;
		}

		private static void ClickControl(Control control, InteractiveState state)
		{
			state.IsPressed = false;
			control.Click();
			if (state.CanHaveFocus)
				state.WantsFocus = true;
		}

		private void ProcessAnyMovement(Control control, InteractiveState state)
		{
			if (movement == Point.Unused)
				return;
			state.IsInside = control.RotatedDrawAreaContains(movement);
			Point rotatedMovement = control.Rotation == 0.0f
				? movement : movement.RotateAround(control.RotationCenter, -control.Rotation);
			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(rotatedMovement);
		}

		private void ProcessAnyDrag(InteractiveState state)
		{
			if (dragArea == Rectangle.Unused)
				return;
			if (state.DragArea == Rectangle.Unused)
				state.DragDelta = dragArea.Size;
			else
				state.DragDelta = dragArea.BottomRight - state.DragArea.BottomRight;
			state.DragArea = dragArea;
		}

		private static void ProcessShiftOfFocus(IEnumerable<Entity> entities)
		{
			// ReSharper disable PossibleMultipleEnumeration
			var entityWhichWantsFocus =
				entities.FirstOrDefault(entity => entity.Get<InteractiveState>().WantsFocus);
			if (entityWhichWantsFocus == null)
				return;
			foreach (var state in entities.Select(entity => entity.Get<InteractiveState>()))
			{
				state.WantsFocus = false;
				state.HasFocus = false;
			}
			entityWhichWantsFocus.Get<InteractiveState>().HasFocus = true;
			// ReSharper restore PossibleMultipleEnumeration
		}

		private void Reset()
		{
			leftClick = Point.Unused;
			leftRelease = Point.Unused;
			movement = Point.Unused;
			dragArea = Rectangle.Unused;
		}
	}
}