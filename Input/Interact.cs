using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows an Entity to respond to pointer input (mouse or touch)
	/// </summary>
	public class Interact : EntityHandler
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
				ProcessMouseMovement(mouse, control);
		}

		private static void ProcessMouseMovement(Mouse mouse, Entity2D control)
		{
			var state = control.Get<State>();
			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(mouse.Position);
			if (state.IsHovering)
				StopHoveringOverControl(control);

			bool isInside = control.RotatedDrawAreaContains(mouse.Position);
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
				PressControl(control, position);
		}

		private static void PressControl(Entity2D control, Point position)
		{
			control.Get<State>().IsPressed = true;
			control.Get<State>().RelativePointerPosition = control.DrawArea.GetRelativePoint(position);
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
				ClickOrReleaseControl(control, position);
		}

		private void ClickOrReleaseControl(Entity2D control, Point position)
		{
			state = control.Get<State>();
			state.IsPressed = false;
			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(position);
			if (control.DrawArea.Contains(position))
				SetFocusAndClickControl(control);
			else
				control.MessageAllListeners(new ControlReleased());
		}

		private State state;

		private void SetFocusAndClickControl(Entity control)
		{
			if (state.CanHaveFocus)
				state.HasFocus = true;

			control.MessageAllListeners(new ControlClicked());
		}

		public class ControlClicked {}

		public class ControlReleased {}

		public override void Handle(List<Entity> entities) {}

		public interface Clickable
		{
			void Clicking();
		}

		public class Clicking : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				var clickable = entity as Clickable;
				var clicked = message as ControlClicked;
				if (clickable != null && clicked != null)
					clickable.Clicking();
			}
		}
	}
}