using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class ButtonTests : TestWithMocksOrVisually
	{
		[Test]
		public void WritePointerRelativePosition()
		{
			var button = CreateSceneWithButton();
			RunCode = () => Window.Title =
				"Relative Pointer Position: " + button.Get<Interact.State>().RelativePointerPosition;
		}

		private static Button CreateSceneWithButton()
		{
			var button = CreateButton();
			EntitySystem.Current.Run();
			var scene = new Scene();
			scene.Add(button);
			scene.Show();
			return button;
		}

		private static Button CreateButton(string text = "")
		{
			var logo = ContentLoader.Load<Image>("DeltaEngineLogo");
			var theme = new Theme
			{
				Button = new Theme.Appearance(logo, NormalColor),
				ButtonMouseover = new Theme.Appearance(logo, MouseoverColor),
				ButtonPressed = new Theme.Appearance(logo, PressedColor),
				Font = new Font("Verdana12")
			};
			return new Button(theme, Center, text);
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);
		private static readonly Color NormalColor = Color.Green;
		private static readonly Color MouseoverColor = Color.Blue;
		private static readonly Color PressedColor = Color.Red;

		[Test]
		public void BeginClickInside()
		{
			var button = CreateSceneWithButton();
			bool pressed = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlPressed)
					pressed = true;
			};
			InitializeMouse();
			SetMouseState(State.Pressing, Point.Half);
			Assert.IsTrue(pressed);
			Assert.AreEqual(PressedColor, button.Color);
			Window.CloseAfterFrame();
		}

		private void InitializeMouse()
		{
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		private void SetMouseState(State state, Point position, float duration = 0.04f)
		{
			Resolve<MockMouse>().SetMousePositionNextFrame(position);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, state);
			resolver.AdvanceTimeAndExecuteRunners(duration);
		}

		[Test]
		public void BeginClickOutside()
		{
			var button = CreateSceneWithButton();
			bool pressed = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlPressed)
					pressed = true;
			};
			InitializeMouse();
			SetMouseState(State.Pressing, Point.One);
			Assert.IsFalse(pressed);
			Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			Window.CloseAfterFrame();
		}

		[Test]
		public void BeginAndEndClickInside()
		{
			var button = CreateSceneWithButton();
			Point tapped = Point.One;
			button.Clicked += () => tapped = button.Get<Interact.State>().RelativePointerPosition;
			PressAndReleaseMouse(new Point(0.53f, 0.52f), new Point(0.53f, 0.52f));
			Assert.AreEqual(new Point(0.6f, 0.7f), tapped);
			Assert.AreEqual(MouseoverColor, button.Color);
			Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			Window.CloseAfterFrame();
		}

		private void PressAndReleaseMouse(Point pressPosition, Point releasePosition)
		{
			InitializeMouse();
			SetMouseState(State.Pressing, pressPosition);
			SetMouseState(State.Releasing, releasePosition);
		}

		[Test]
		public void BeginClickInsideAndEndOutside()
		{
			var button = CreateSceneWithButton();
			bool tapped = false;
			button.Clicked += () => tapped = true;
			PressAndReleaseMouse(Point.Half, Point.Zero);
			Assert.IsFalse(tapped);
			Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			Window.CloseAfterFrame();
		}

		[Test]
		public void BeginClickOutsideAndEndInside()
		{
			var button = CreateSceneWithButton();
			bool tapped = false;
			button.Clicked += () => tapped = true;
			PressAndReleaseMouse(Point.Zero, Point.Half);
			Assert.IsFalse(tapped);
			Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			Window.CloseAfterFrame();
		}

		[Test]
		public void Enter()
		{
			var button = CreateSceneWithButton();
			bool entered = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlEntered)
					entered = true;
			};
			SetMouseState(State.Released, Point.Zero);
			Assert.IsFalse(entered);
			SetMouseState(State.Released, Point.Half);
			Assert.IsTrue(entered);
			Assert.IsTrue(button.Get<Interact.State>().IsInside);
			Window.CloseAfterFrame();
		}

		[Test]
		public void Exit()
		{
			var button = CreateSceneWithButton();
			bool exited = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlExited)
					exited = true;
			};
			MoveMouse();
			Assert.IsTrue(exited);
			Assert.IsFalse(button.Get<Interact.State>().IsInside);
			Window.CloseAfterFrame();
		}

		private void MoveMouse()
		{
			InitializeMouse();
			SetMouseState(State.Released, Point.Half);
			SetMouseState(State.Released, Point.Zero);
		}

		[Test]
		public void ShortDelayDoesntTriggerHover()
		{
			var button = CreateSceneWithButton();
			bool hovered = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlHoveringStarted)
					hovered = true;
			};
			InitializeMouse();
			SetMouseState(State.Released, Point.Half, 1.0f);
			Assert.IsFalse(hovered);
			Assert.IsFalse(button.Get<Interact.State>().IsHovering);
			Window.CloseAfterFrame();
		}

		[Test]
		public void LongDelayTriggersHover()
		{
			var button = CreateSceneWithButton();
			bool hovered = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlHoveringStarted)
					hovered = true;
			};
			InitializeMouse();
			SetMouseState(State.Released, Point.Half, 2.0f);
			Assert.IsTrue(hovered);
			Assert.IsTrue(button.Get<Interact.State>().IsHovering);
			Window.CloseAfterFrame();
		}

		[Test]
		public void StopHover()
		{
			var button = CreateSceneWithButton();
			bool stoppedHover = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlHoveringStopped)
					stoppedHover = true;
			};
			HoverThenMoveMouse();
			Assert.IsTrue(stoppedHover);
			Assert.IsFalse(button.Get<Interact.State>().IsHovering);
			Window.CloseAfterFrame();
		}

		private void HoverThenMoveMouse()
		{
			InitializeMouse();
			SetMouseState(State.Released, Point.Half, 2.0f);
			SetMouseState(State.Released, Point.Zero);
		}
	}
}