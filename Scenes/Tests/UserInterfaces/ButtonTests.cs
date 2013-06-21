using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class ButtonTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void BeginClickInside(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
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
			});
		}

		private static Button CreateSceneWithButton(ContentLoader content)
		{
			var button = CreateButton(content);
			EntitySystem.Current.Run();
			var scene = new Scene();
			scene.Add(button);
			scene.Show();
			return button;
		}

		private static Button CreateButton(ContentLoader content, string text = "")
		{
			var logo = content.Load<Image>("DeltaEngineLogo");
			var theme = new Theme
			{
				Button = new Theme.Appearance(logo, NormalColor),
				ButtonMouseover = new Theme.Appearance(logo, MouseoverColor),
				ButtonPressed = new Theme.Appearance(logo, PressedColor),
				Font = new Font(content, "Verdana12")
			};
			return new Button(theme, Center, text);
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);
		private static readonly Color NormalColor = Color.Green;
		private static readonly Color MouseoverColor = Color.Blue;
		private static readonly Color PressedColor = Color.Red;

		private void InitializeMouse()
		{
			mockResolver.input.SetMousePosition(Point.Zero);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		private void SetMouseState(State state, Point position, float duration = 0.02f)
		{
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners(duration);
		}

		[IntegrationTest]
		public void BeginClickOutside(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
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
			});
		}

		[IntegrationTest]
		public void BeginAndEndClickInside(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
				Point tapped = Point.One;
				button.Clicked += () => tapped = button.Get<Interact.State>().RelativePointerPosition;
				PressAndReleaseMouse(new Point(0.53f, 0.52f), new Point(0.53f, 0.52f));
				Assert.AreEqual(new Point(0.6f, 0.7f), tapped);
				Assert.AreEqual(MouseoverColor, button.Color);
				Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			});
		}

		private void PressAndReleaseMouse(Point pressPosition, Point releasePosition)
		{
			InitializeMouse();
			SetMouseState(State.Pressing, pressPosition);
			SetMouseState(State.Releasing, releasePosition);
		}

		[IntegrationTest]
		public void BeginClickInsideAndEndOutside(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
				bool tapped = false;
				button.Clicked += () => tapped = true;
				PressAndReleaseMouse(Point.Half, Point.Zero);
				Assert.IsFalse(tapped);
				Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			});
		}

		[IntegrationTest]
		public void BeginClickOutsideAndEndInside(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
				bool tapped = false;
				button.Clicked += () => tapped = true;
				PressAndReleaseMouse(Point.Zero, Point.Half);
				Assert.IsFalse(tapped);
				Assert.IsFalse(button.Get<Interact.State>().IsPressed);
			});
		}

		[IntegrationTest]
		public void Enter(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
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
			});
		}

		[IntegrationTest]
		public void Exit(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
				bool exited = false;
				button.Messaged += message =>
				{
					if (message is Interact.ControlExited)
						exited = true;
				};
				MoveMouse();
				Assert.IsTrue(exited);
				Assert.IsFalse(button.Get<Interact.State>().IsInside);
			});
		}

		private void MoveMouse()
		{
			InitializeMouse();
			SetMouseState(State.Released, Point.Half);
			SetMouseState(State.Released, Point.Zero);
		}

		[IntegrationTest]
		public void ShortDelayDoesntTriggerHover(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
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
			});
		}

		[IntegrationTest]
		public void LongDelayTriggersHover(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
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
			});
		}

		[IntegrationTest]
		public void StopHover(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateSceneWithButton(content);
				bool stoppedHover = false;
				button.Messaged += message =>
				{
					if (message is Interact.ControlHoveringStopped)
						stoppedHover = true;
				};
				HoverThenMoveMouse();
				Assert.IsTrue(stoppedHover);
				Assert.IsFalse(button.Get<Interact.State>().IsHovering);
			});
		}

		private void HoverThenMoveMouse()
		{
			InitializeMouse();
			SetMouseState(State.Released, Point.Half, 2.0f);
			SetMouseState(State.Released, Point.Zero);
		}

		[VisualTest]
		public void WritePointerRelativePosition(Type resolver)
		{
			Button button = null;
			Start(resolver,
				(Scene s, ContentLoader content) => { button = CreateSceneWithButton(content); },
				(Window window) =>
				{
					window.Title = "Relative Pointer Position: " +
						button.Get<Interact.State>().RelativePointerPosition;
				});
		}
	}
}