using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	internal class ButtonTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Draw(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = new Button(content.Load<Image>("DeltaEngineLogo"), Small)
				{
					NormalColor = Color.Green,
					MouseoverColor = Color.Blue,
					PressedColor = Color.Red,
					Sprite = { Rotation = 45 },
				};
				button.Hovered += () => button.Sprite.DrawArea = Big;
				button.StoppedHover += () => button.Sprite.DrawArea = Small;
				var scene = new Scene();
				scene.Add(button);
				scene.Show(entitySystem, content, input);
			});
		}

		private static readonly Rectangle Small = new Rectangle(0.45f, 0.45f, 0.1f, 0.1f);
		private static readonly Rectangle Big = new Rectangle(0.4f, 0.4f, 0.2f, 0.2f);

		[IntegrationTest]
		public void BeginClickInside(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool pressed = false;
				button.Pressed += () => pressed = true;
				SetMouseState(State.Pressing, Point.Half);
				Assert.IsTrue(pressed);
				Assert.AreEqual(PressedColor, button.Sprite.Color);
			});
		}

		private static Button CreateSceneWithButton(EntitySystem entitySystem, ContentLoader content,
			InputCommands input)
		{
			var button = new Button(content.Load<Image>("DeltaEngineLogo"), Small)
			{
				NormalColor = NormalColor,
				MouseoverColor = MouseoverColor,
				PressedColor = PressedColor
			};

			var scene = new Scene();
			scene.Add(button);
			scene.Show(entitySystem, content, input);
			return button;
		}

		private static readonly Color NormalColor = Color.Red;
		private static readonly Color MouseoverColor = Color.Green;
		private static readonly Color PressedColor = Color.Blue;

		private void SetMouseState(State state, Point position, float duration = 0.02f)
		{
			mockResolver.input.SetMousePosition(position + Offset);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners(0.02f);
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners(duration);
		}

		private static readonly Point Offset = new Point(0.001f, 0.001f);

		[IntegrationTest]
		public void BeginClickOutside(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool pressed = false;
				button.Pressed += () => pressed = true;
				SetMouseState(State.Pressing, Point.Zero);
				Assert.IsFalse(pressed);
				Assert.IsFalse(button.IsPressed);
			});
		}

		[IntegrationTest]
		public void BeginAndEndClickInside(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool tapped = false;
				button.Tapped += position => tapped = true;
				SetMouseState(State.Pressing, Point.Half);
				SetMouseState(State.Releasing, Point.Half);
				Assert.IsTrue(tapped);
				Assert.AreEqual(MouseoverColor, button.Sprite.Color);
				Assert.IsFalse(button.IsPressed);
			});
		}

		[IntegrationTest]
		public void BeginClickInsideAndEndOutside(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool tapped = false;
				button.Tapped += position => tapped = true;
				SetMouseState(State.Pressing, Point.Half);
				SetMouseState(State.Releasing, Point.Zero);
				Assert.IsFalse(tapped);
				Assert.IsFalse(button.IsPressed);
			});
		}

		[IntegrationTest]
		public void BeginClickOutsideAndEndInside(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool tapped = false;
				button.Tapped += position => tapped = true;
				SetMouseState(State.Pressing, Point.Zero);
				SetMouseState(State.Releasing, Point.Half);
				Assert.IsFalse(tapped);
				Assert.IsFalse(button.IsPressed);
			});
		}

		[IntegrationTest]
		public void Enter(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool entered = false;
				button.Entered += () =>
				{
					entered = true;
				};
				SetMouseState(State.Released, Point.Zero);
				Assert.IsFalse(entered);
				SetMouseState(State.Released, Point.Half);
				Assert.IsTrue(entered);
				Assert.IsTrue(button.IsInside);
			});
		}

		[IntegrationTest]
		public void Exit(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool exited = false;
				button.Exited += () =>
				{
					exited = true;
				};
				SetMouseState(State.Released, Point.Half);
				SetMouseState(State.Released, Point.Zero);
				Assert.IsTrue(exited);
				Assert.IsFalse(button.IsInside);
			});
		}

		[IntegrationTest]
		public void Hover(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool hovered = false;
				button.Hovered += () =>
				{
					hovered = true;
				};
				SetMouseState(State.Released, Point.Half, 1.0f);
				Assert.IsFalse(hovered);
				SetMouseState(State.Released, Point.Half, 2.0f);
				Assert.IsTrue(hovered);
			});
		}

		[IntegrationTest]
		public void StopHover(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var button = CreateSceneWithButton(entitySystem, content, input);
				bool stoppedHover = false;
				button.StoppedHover += () =>
				{
					stoppedHover = true;
				};
				SetMouseState(State.Released, Point.Half, 2.0f);
				Assert.IsFalse(stoppedHover);
				SetMouseState(State.Released, Point.Zero);
				Assert.IsTrue(stoppedHover);
			});
		}
	}
}