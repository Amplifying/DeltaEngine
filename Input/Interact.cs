using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows an Entity2D to respond to pointer input (mouse or touch)
	/// </summary>
	public class Interact : Behavior2D
	{
		public Interact(InputCommands input)
		{
			input.AddMouseMovement(MouseMovement);
			input.AddMouseHover(MouseHover);
			input.Add(MouseButton.Left, Input.State.Pressing, mouse => PointerPressed(mouse.Position));
			input.Add(MouseButton.Left, mouse => PointerReleased(mouse.Position));
			input.Add(Input.State.Pressing, touch => PointerPressed(touch.GetPosition(0)));
			input.Add(Input.State.Releasing, touch => PointerReleased(touch.GetPosition(0)));
		}

		private void MouseMovement(Mouse mouse)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			var interactiveControls =
				new List<Entity2D>(entities.OfType<Entity2D>().Where(e => e.Contains<State>()));
			foreach (Entity2D control in interactiveControls)
				ProcessMouseMovement(control, mouse);
		}

		private static void ProcessMouseMovement(Entity2D control, Mouse mouse)
		{
			var state = control.Get<State>();
			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(mouse.Position);
			if (state.IsHovering)
				StopHoveringOverControl(control);

			ProcessDragging(control, mouse.Position, state);
			ProcessEntryAndExit(control, mouse.Position, state);
		}

		private static void ProcessDragging(Entity control, Point position, State state)
		{
			CheckForStartingDragging(control, position, state);
			if (!state.IsDragging || position == state.DragPosition)
				return;

			UpdateDragging(control, position, state);
		}

		private static void CheckForStartingDragging(Entity control, Point position, State state)
		{
			if (state.IsDragging || !state.IsPressed || position == state.DragPosition)
				return;

			control.MessageAllListeners(new ControlDragStarted());
			state.IsDragging = true;
		}

		public class ControlDragStarted { }

		private static void UpdateDragging(Entity control, Point position, State state)
		{
			control.MessageAllListeners(new ControlDragged());
			state.DragDelta = position - state.DragPosition;
			state.DragPosition = position;
		}

		public class ControlDragged {}

		private static void ProcessEntryAndExit(Entity2D control, Point position, State state)
		{
			bool isInside = control.RotatedDrawAreaContains(position);
			if (!state.IsInside && isInside)
				EnterControl(control);
			else if (state.IsInside && !isInside)
				ExitControl(control);
		}

		private static void StopHoveringOverControl(Entity control)
		{
			control.Get<State>().IsHovering = false;
			control.MessageAllListeners(new ControlHoveringStopped());
		}

		private static void EnterControl(Entity control)
		{
			control.Get<State>().IsInside = true;
			control.MessageAllListeners(new ControlEntered());
		}

		private static void ExitControl(Entity control)
		{
			control.Get<State>().IsInside = false;
			control.MessageAllListeners(new ControlExited());
		}

		public class State
		{
			public bool IsHovering { get; set; }
			public bool IsInside { get; set; }
			public bool IsPressed { get; set; }
			public Point RelativePointerPosition { get; set; }
			public bool CanHaveFocus { get; set; }
			public bool HasFocus { get; set; }
			public bool IsDragging { get; set; }
			public Point DragPosition { get; set; }
			public Point DragDelta { get; set; }
		}

		public class ControlHoveringStopped {}

		public class ControlEntered {}

		public class ControlExited {}

		private void MouseHover(Mouse mouse)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			var interactiveControls =
				new List<Entity2D>(entities.OfType<Entity2D>().Where(e => e.Contains<State>()));
			foreach (Entity2D control in
				interactiveControls.Where(control => control.Get<State>().IsInside))
				HoverOverControl(control);
		}

		private static void HoverOverControl(Entity control)
		{
			control.Get<State>().IsHovering = true;
			control.MessageAllListeners(new ControlHoveringStarted());
		}

		public class ControlHoveringStarted {}

		private void PointerPressed(Point position)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			var interactiveControls =
				new List<Entity2D>(entities.OfType<Entity2D>().Where(e => e.Contains<State>()));
			foreach (Entity2D control in
				interactiveControls.Where(control => control.Get<State>().IsInside))
				PressControl(control, position, control.Get<State>());
		}

		private static void PressControl(Entity2D control, Point position, State state)
		{
			state.IsPressed = true;
			state.DragPosition = position;
			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(position);
			control.MessageAllListeners(new ControlPressed());
		}

		public class ControlPressed {}

		private void PointerReleased(Point position)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			var interactiveControls =
				new List<Entity2D>(entities.OfType<Entity2D>().Where(e => e.Contains<State>()));
			foreach (Entity2D interactiveControl in interactiveControls)
				interactiveControl.Get<State>().HasFocus = false;

			foreach (Entity2D control in
				interactiveControls.Where(control => control.Get<State>().IsPressed))
				ClickOrReleaseControl(control, position, control.Get<State>());
		}

		private static void ClickOrReleaseControl(Entity2D control, Point position, State state)
		{
			state.IsPressed = false;
			if (state.IsDragging)
				StopDragging(control, state);

			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(position);
			if (control.DrawArea.Contains(position))
				SetFocusAndClickControl(control, state);
			else
				control.MessageAllListeners(new ControlReleased());
		}

		private static void StopDragging(Entity control, State state)
		{
			control.MessageAllListeners(new ControlDragFinished());
			state.IsDragging = false;
		}

		public class ControlDragFinished { }

		private static void SetFocusAndClickControl(Entity control, State state)
		{
			if (state.CanHaveFocus)
				state.HasFocus = true;

			control.MessageAllListeners(new ControlClicked());
		}

		public class ControlClicked {}

		public class ControlReleased {}

		public interface Clickable
		{
			void Clicking();
		}

		public class Clicking : EventListener2D
		{
			public override void ReceiveMessage(Entity2D entity, object message)
			{
				var clickable = entity as Clickable;
				var clicked = message as ControlClicked;
				if (clickable != null && clicked != null)
					clickable.Clicking();
			}
		}

		public override void Handle(Entity2D entity) { }
	}
}