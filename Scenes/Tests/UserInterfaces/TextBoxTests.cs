using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TextBoxTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ClickingTextBoxGivesItFocus(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var textbox = CreateTextBox(content, Top);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsFalse(textbox.Get<Interact.State>().HasFocus);
				PressAndReleaseMouse(Point.One);
				Assert.IsFalse(textbox.Get<Interact.State>().HasFocus);
				PressAndReleaseMouse(Top.Center);
				Assert.IsTrue(textbox.Get<Interact.State>().HasFocus);
			});
		}

		private static readonly Rectangle Top = Rectangle.FromCenter(0.5f, 0.4f, 0.3f, 0.1f);

		private static TextBox CreateTextBox(ContentLoader content, Rectangle drawArea,
			string text = "")
		{
			var background = content.Load<Image>("DefaultLabel");
			var theme = new Theme
			{
				TextBox = new Theme.Appearance(background),
				TextBoxFocussed = new Theme.Appearance(background, Color.Yellow),
				Font = new Font(content, "Verdana12")
			};
			var textbox = new TextBox(theme, drawArea, text);
			return textbox;
		}

		private void PressAndReleaseMouse(Point position)
		{
			InitializeMouse();
			SetMouseState(State.Pressing, position);
			SetMouseState(State.Releasing, position);
			SetMouseState(State.Released, position);
		}

		private void InitializeMouse()
		{
			mockResolver.input.SetMousePosition(Point.Zero);
			mockResolver.AdvanceTimeAndExecuteRunners(0.04f);
		}

		private void SetMouseState(State state, Point position)
		{
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		[IntegrationTest]
		public void ClickingOneTextBoxCausesOtherTextBoxToLoseFocus(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var topTextbox = CreateTextBox(content, Top);
				var bottomTextbox = CreateTextBox(content, Bottom);
				PressAndReleaseMouse(Top.Center);
				Assert.IsTrue(topTextbox.Get<Interact.State>().HasFocus);
				Assert.IsFalse(bottomTextbox.Get<Interact.State>().HasFocus);
				PressAndReleaseMouse(Bottom.Center);
				Assert.IsFalse(topTextbox.Get<Interact.State>().HasFocus);
				Assert.IsTrue(bottomTextbox.Get<Interact.State>().HasFocus);
			});
		}

		private static readonly Rectangle Bottom = Rectangle.FromCenter(0.5f, 0.6f, 0.3f, 0.1f);

		[IntegrationTest]
		public void TypingHasNoEffectIfTextBoxDoesNotHaveFocus(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var textbox = CreateTextBox(content, Top);
				PressKey(Key.A);
				Assert.AreEqual("", textbox.Text);
			});
		}

		private void PressKey(Key key)
		{
			mockResolver.input.SetKeyboardState(key, State.Releasing);
			if (lastKey != Key.None)
				mockResolver.input.SetKeyboardState(lastKey, State.Released);

			mockResolver.AdvanceTimeAndExecuteRunners();
			lastKey = key;
		}

		private Key lastKey = Key.None;

		[IntegrationTest]
		public void TypingGoesIntoTheTextBoxWithFocus(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var topTextbox = CreateTextBox(content, Top);
				var bottomTextbox = CreateTextBox(content, Bottom);
				PressAndReleaseMouse(Bottom.Center);
				PressKeys();
				Assert.AreEqual("", topTextbox.Text);
				Assert.AreEqual("A 2", bottomTextbox.Text);
			});
		}

		private void PressKeys()
		{
			PressKey(Key.A);
			PressKey(Key.Space);
			PressKey(Key.D1);
			PressKey(Key.BackSpace);
			PressKey(Key.D2);
		}

		[VisualTest]
		public void RenderTwoTextBoxes(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				CreateTextBox(content, Top, "Hello");
				CreateTextBox(content, Bottom, "World");
			});
		}
	}
}