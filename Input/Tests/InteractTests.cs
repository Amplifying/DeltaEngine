using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InteractTests : TestWithMocksOrVisually
	{
		[Test]
		public void RelativePointerPositionIsCorrect()
		{
			var entity = CreateEntity();
			SetMouseState(Point.Half, State.Released);
			Assert.AreEqual(Point.Half, entity.Get<Interact.State>().RelativePointerPosition);
			SetMouseState(new Point(0.1f, 0.8f), State.Released);
			Assert.AreEqual(new Point(-0.5f, 2f), entity.Get<Interact.State>().RelativePointerPosition);
			Window.CloseAfterFrame();
		}

		private Entity2D CreateEntity()
		{
			var entity = new Entity2D(Center);
			entity.Start<Interact>().Add(new Interact.State());
			EntitySystem.Current.Run();
			InitializeMouse();
			return entity;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.2f);

		private void InitializeMouse()
		{
			Resolve<MockMouse>().SetPosition(Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		private void SetMouseState(Point position, State state, float duration = 0.02f)
		{
			Resolve<MockMouse>().SetPosition(position);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, state);
			resolver.AdvanceTimeAndExecuteRunners(duration);
		}

		[Test]
		public void IsPressedDoesNotFireOutside()
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
		}

		[Test]
		public void IsPressedFiresInside()
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
		}

		[Test]
		public void NotEnteringEntity()
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
		}

		[Test]
		public void EnteringEntity()
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
		}

		[Test]
		public void ExitingEntity()
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
		}

		[Test]
		public void HoveringOverEntityForShortTimeDoesNotStartHovering()
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
		}

		[Test]
		public void HoveringOverEntityForLongTimeStartHoverings()
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
		}

		[Test]
		public void StartThenStopHovering()
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
		}

		[Test]
		public void PressingAndReleasingEntity()
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
		}

		private void PressAndReleaseMouse(Point press, Point release)
		{
			SetMouseState(press, State.Pressing);
			SetMouseState(release, State.Releasing);
		}

		[Test]
		public void ClickingEntity()
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
		}

		private ClickableEntity CreateClickableEntity()
		{
			var entity = new ClickableEntity { DrawArea = Center };
			entity.Start<Interact, Interact.Clicking>().Add(new Interact.State());
			EntitySystem.Current.Run();
			InitializeMouse();
			return entity;
		}

		private class ClickableEntity : Entity2D, Interact.Clickable
		{
			public ClickableEntity()
				: base(Rectangle.Zero) {}

			public void Clicking()
			{
				Clicked = true;
			}

			public bool Clicked { get; private set; }
		}

		[Test]
		public void FocusableControlAcceptsFocus()
		{
			var entity = CreateFocusableEntity();
			PressAndReleaseMouse(Point.Half, Point.Half);
			Assert.IsTrue(entity.Get<Interact.State>().HasFocus);
		}

		private Entity2D CreateFocusableEntity()
		{
			var entity = CreateEntity();
			entity.Get<Interact.State>().CanHaveFocus = true;
			return entity;
		}

		[Test]
		public void ControlGrabsFocusFromPreviousControl()
		{
			var entity1 = CreateFocusableEntity();
			var entity2 = CreateFocusableEntity();
			entity2.DrawArea = Rectangle.FromCenter(Point.Zero, Size.Half);
			PressAndReleaseMouse(Point.Half, Point.Half);
			Assert.IsTrue(entity1.Get<Interact.State>().HasFocus);
			PressAndReleaseMouse(Point.Zero, Point.Zero);
			Assert.IsFalse(entity1.Get<Interact.State>().HasFocus);
			Assert.IsTrue(entity2.Get<Interact.State>().HasFocus);
		}

		[Test]
		public void NonFocussableControlDoesntAcceptsFocus()
		{
			var entity = CreateEntity();
			SetMouseState(Point.Half, State.Pressing);
			SetMouseState(Point.Half, State.Releasing);
			Assert.IsFalse(entity.Get<Interact.State>().HasFocus);
		}

		[Test]
		public void PressingWithoutMovingMouseDoesNotDrag()
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
		}

		[Test]
		public void StartDraggingMouse()
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
		}

		[Test]
		public void StopDraggingMouse()
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
		}
	}
}