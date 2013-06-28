using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TextBoxTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderTwoTextBoxes()
		{
			CreateTextBox(Top, "Hello");
			CreateTextBox(Bottom, "World");
		}

		private static TextBox CreateTextBox(Rectangle drawArea, string text = "")
		{
			var background = ContentLoader.Load<Image>("DefaultLabel");
			var theme = new Theme
			{
				TextBox = new Theme.Appearance(background),
				TextBoxFocussed = new Theme.Appearance(background, Color.Yellow),
				Font = new Font("Verdana12")
			};
			var textbox = new TextBox(theme, drawArea, text);
			return textbox;
		}

		private static readonly Rectangle Top = Rectangle.FromCenter(0.5f, 0.4f, 0.3f, 0.1f);
		private static readonly Rectangle Bottom = Rectangle.FromCenter(0.5f, 0.6f, 0.3f, 0.1f);

		[Test]
		public void ClickingTextBoxGivesItFocus()
		{
			var textbox = CreateTextBox(Top);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsFalse(textbox.Get<Interact.State>().HasFocus);
			PressAndReleaseMouse(Point.One);
			Assert.IsFalse(textbox.Get<Interact.State>().HasFocus);
			PressAndReleaseMouse(Top.Center);
			Assert.IsTrue(textbox.Get<Interact.State>().HasFocus);
			Window.CloseAfterFrame();
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
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners(0.04f);
		}

		private void SetMouseState(State state, Point position)
		{
			Resolve<MockMouse>().SetMousePositionNextFrame(position);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, state);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void ClickingOneTextBoxCausesOtherTextBoxToLoseFocus()
		{
			var topTextbox = CreateTextBox(Top);
			var bottomTextbox = CreateTextBox(Bottom);
			PressAndReleaseMouse(Top.Center);
			Assert.IsTrue(topTextbox.Get<Interact.State>().HasFocus);
			Assert.IsFalse(bottomTextbox.Get<Interact.State>().HasFocus);
			PressAndReleaseMouse(Bottom.Center);
			Assert.IsFalse(topTextbox.Get<Interact.State>().HasFocus);
			Assert.IsTrue(bottomTextbox.Get<Interact.State>().HasFocus);
			Window.CloseAfterFrame();
		}

		[Test]
		public void TypingHasNoEffectIfTextBoxDoesNotHaveFocus()
		{
			var textbox = CreateTextBox(Top);
			PressKey(Key.A);
			Assert.AreEqual("", textbox.Text);
			Window.CloseAfterFrame();
		}

		private void PressKey(Key key)
		{
			Resolve<MockKeyboard>().SetKeyboardState(key, State.Releasing);
			if (lastKey != Key.None)
				Resolve<MockKeyboard>().SetKeyboardState(lastKey, State.Released);

			resolver.AdvanceTimeAndExecuteRunners();
			lastKey = key;
		}

		private Key lastKey = Key.None;

		[Test]
		public void TypingGoesIntoTheTextBoxWithFocus()
		{
			var topTextbox = CreateTextBox(Top);
			var bottomTextbox = CreateTextBox(Bottom);
			PressAndReleaseMouse(Bottom.Center);
			PressKeys();
			Assert.AreEqual("", topTextbox.Text);
			Assert.AreEqual("A 2", bottomTextbox.Text);
			Window.CloseAfterFrame();
		}

		private void PressKeys()
		{
			PressKey(Key.A);
			PressKey(Key.Space);
			PressKey(Key.D1);
			PressKey(Key.Backspace);
			PressKey(Key.D2);
		}
	}
}