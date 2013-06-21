using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InteractTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void RelativePointerPositionIsCorrect(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				SetMouseState(new Point(0.3f, 0.4f), State.Released);
				Assert.AreEqual(Point.Zero, entity.Get<Interact.State>().RelativePointerPosition);
				SetMouseState(new Point(0.1f, 0.8f), State.Released);
				Assert.AreEqual(new Point(-0.5f, 2f), entity.Get<Interact.State>().RelativePointerPosition);
			});
		}

		private Entity2D CreateEntity()
		{
			var entity = new Entity2D { DrawArea = Center };
			entity.Add<Interact>().Add(new Interact.State());
			EntitySystem.Current.Run();
			InitializeMouse();
			return entity;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.2f);

		private void InitializeMouse()
		{
			mockResolver.input.SetMousePosition(Point.Zero);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		private void SetMouseState(Point position, State state, float duration = 0.02f)
		{
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners(duration);
		}

		[IntegrationTest]
		public void IsPressedDoesNotFireOutside(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool pressed = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlPressed)
						pressed = true;
				};
				SetMouseState(Point.Zero, State.Pressing);
				Assert.IsFalse(pressed);
				Assert.IsFalse(entity.Get<Interact.State>().IsPressed);
			});
		}

		[IntegrationTest]
		public void IsPressedFiresInside(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool pressed = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlPressed)
						pressed = true;
				};
				SetMouseState(Point.Half, State.Pressing);
				Assert.IsTrue(pressed);
				Assert.IsTrue(entity.Get<Interact.State>().IsPressed);
			});
		}

		[IntegrationTest]
		public void NotEnteringEntity(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool entered = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlEntered)
						entered = true;
				};
				SetMouseState(Point.Zero, State.Released);
				Assert.IsFalse(entered);
				Assert.IsFalse(entity.Get<Interact.State>().IsInside);
			});
		}

		[IntegrationTest]
		public void EnteringEntity(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool entered = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlEntered)
						entered = true;
				};
				SetMouseState(Point.Half, State.Released);
				Assert.IsTrue(entered);
				Assert.IsTrue(entity.Get<Interact.State>().IsInside);
			});
		}

		[IntegrationTest]
		public void ExitingEntity(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool exited = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlExited)
						exited = true;
				};
				SetMouseState(Point.Half, State.Released);
				SetMouseState(Point.Zero, State.Released);
				Assert.IsTrue(exited);
				Assert.IsFalse(entity.Get<Interact.State>().IsInside);
			});
		}

		[IntegrationTest]
		public void HoveringOverEntityForShortTimeDoesNotStartHovering(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool hovered = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlHoveringStarted)
						hovered = true;
				};
				SetMouseState(Point.Half, State.Released, 1.0f);
				Assert.IsFalse(hovered);
				Assert.IsFalse(entity.Get<Interact.State>().IsHovering);
			});
		}

		[IntegrationTest]
		public void HoveringOverEntityForLongTimeStartHoverings(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool hovered = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlHoveringStarted)
						hovered = true;
				};
				SetMouseState(Point.Half, State.Released, 3.0f);
				Assert.IsTrue(hovered);
				Assert.IsTrue(entity.Get<Interact.State>().IsHovering);
			});
		}

		[IntegrationTest]
		public void StartThenStopHovering(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool stoppedHovering = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlHoveringStopped)
						stoppedHovering = true;
				};
				SetMouseState(Point.Half, State.Released, 3.0f);
				SetMouseState(Point.Zero, State.Released);
				Assert.IsTrue(stoppedHovering);
				Assert.IsFalse(entity.Get<Interact.State>().IsHovering);
			});
		}

		[IntegrationTest]
		public void PressingAndReleasingEntity(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool released = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlReleased)
						released = true;
				};
				PressAndReleaseMouse(Point.Half, Point.Zero);
				Assert.IsTrue(released);
			});
		}

		private void PressAndReleaseMouse(Point press, Point release)
		{
			SetMouseState(press, State.Pressing);
			SetMouseState(release, State.Releasing);
		}

		[IntegrationTest]
		public void ClickingEntity(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateClickableEntity();
				bool clicked = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlClicked)
						clicked = true;
				};
				PressAndReleaseMouse(Point.Half, Point.Half);
				Assert.IsTrue(clicked);
				Assert.IsTrue(entity.Clicked);
			});
		}

		private ClickableEntity CreateClickableEntity()
		{
			var entity = new ClickableEntity { DrawArea = Center };
			entity.Add<Interact, Interact.Clicking>().Add(new Interact.State());
			EntitySystem.Current.Run();
			InitializeMouse();
			return entity;
		}

		private class ClickableEntity : Entity2D, Interact.Clickable
		{
			public void Clicking()
			{
				Clicked = true;
			}

			public bool Clicked { get; private set; }
		}

		[IntegrationTest]
		public void FocusableControlAcceptsFocus(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateFocusableEntity();
				PressAndReleaseMouse(Point.Half, Point.Half);
				Assert.IsTrue(entity.Get<Interact.State>().HasFocus);
			});
		}

		private Entity2D CreateFocusableEntity()
		{
			var entity = CreateEntity();
			entity.Get<Interact.State>().CanHaveFocus = true;
			return entity;
		}

		[IntegrationTest]
		public void ControlGrabsFocusFromPreviousControl(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity1 = CreateFocusableEntity();
				var entity2 = CreateFocusableEntity();
				entity2.DrawArea = Rectangle.FromCenter(Point.Zero, Size.Half);
				PressAndReleaseMouse(Point.Half, Point.Half);
				Assert.IsTrue(entity1.Get<Interact.State>().HasFocus);
				PressAndReleaseMouse(Point.Zero, Point.Zero);
				Assert.IsFalse(entity1.Get<Interact.State>().HasFocus);
				Assert.IsTrue(entity2.Get<Interact.State>().HasFocus);
			});
		}

		[IntegrationTest]
		public void NonFocussableControlDoesntAcceptsFocus(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				SetMouseState(Point.Half, State.Pressing);
				SetMouseState(Point.Half, State.Releasing);
				Assert.IsFalse(entity.Get<Interact.State>().HasFocus);
			});
		}

		[IntegrationTest]
		public void PressingWithoutMovingMouseDoesNotDrag(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity(); 
				bool dragging = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlDragStarted)
						dragging = true;
				};
				SetMouseState(Point.Half, State.Pressing);
				Assert.IsFalse(entity.Get<Interact.State>().IsDragging);
				Assert.IsFalse(dragging);
			});
		}

		[IntegrationTest]
		public void StartDraggingMouse(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool dragging = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlDragStarted)
						dragging = true;
				};
				SetMouseState(Point.Half, State.Pressing);
				SetMouseState(new Point(0.6f, 0.7f), State.Pressed);
				Assert.IsTrue(entity.Get<Interact.State>().IsDragging);
				Assert.IsTrue(dragging);
				Assert.AreEqual(new Point(0.1f, 0.2f), entity.Get<Interact.State>().DragDelta);
			});
		}

		[IntegrationTest]
		public void StopDraggingMouse(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = CreateEntity();
				bool stopDragging = false;
				entity.Messaged += message =>
				{
					if (message is Interact.ControlDragFinished)
						stopDragging = true;
				};
				SetMouseState(Point.Half, State.Pressing);
				SetMouseState(new Point(0.6f, 0.7f), State.Pressed);
				Assert.IsFalse(stopDragging);
				SetMouseState(new Point(0.6f, 0.8f), State.Releasing);
				Assert.IsFalse(entity.Get<Interact.State>().IsDragging);
				Assert.IsTrue(stopDragging);
			});
		}
	}
}