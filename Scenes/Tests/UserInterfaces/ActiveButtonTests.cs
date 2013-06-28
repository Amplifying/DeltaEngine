using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class ActiveButtonTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderActiveButton()
		{
			CreateButton();
		}

		[Test]
		public void ChangeBaseSize()
		{
			var button = CreateButton();
			Assert.AreEqual(BaseSize, button.BaseSize);
			button.BaseSize = Size.Half;
			Assert.AreEqual(Size.Half, button.BaseSize);
			Window.CloseAfterFrame();
		}

		private static ActiveButton CreateButton()
		{
			var logo = ContentLoader.Load<Image>("DeltaEngineLogo");
			var theme = new Theme
			{
				Button = new Theme.Appearance(logo, NormalColor),
				ButtonMouseover = new Theme.Appearance(logo, MouseoverColor),
				ButtonPressed = new Theme.Appearance(logo, PressedColor),
				Font = new Font("Verdana12")
			};
			var button = new ActiveButton(theme, Center);
			EntitySystem.Current.Run();
			return button;
		}

		private static readonly Size BaseSize = new Size(0.3f, 0.1f);
		private static readonly Rectangle Center = Rectangle.FromCenter(Point.Half, BaseSize);
		private static readonly Color NormalColor = Color.Green;
		private static readonly Color MouseoverColor = Color.Blue;
		private static readonly Color PressedColor = Color.Red;

		[Test]
		public void BeginningClickMakesItShrink()
		{
			var button = CreateButton();
			Assert.IsFalse(button.Contains<Transition.Size>());
			SetMouseState(State.Pressing, Point.Half);
			Assert.IsTrue(button.Get<Transition.Size>().End.Width < BaseSize.Width);
			Window.CloseAfterFrame();
		}

		private void SetMouseState(State state, Point position)
		{
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Zero);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, State.Released);
			resolver.AdvanceTimeAndExecuteRunners();
			resolver.AdvanceTimeAndExecuteRunners();
			Resolve<MockMouse>().SetMousePositionNextFrame(position);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, state);
			resolver.AdvanceTimeAndExecuteRunners();
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void FinishingClickMakesItGrow()
		{
			var button = CreateButton();
			Assert.IsFalse(button.Contains<Transition.Size>());
			SetMouseState(State.Pressing, Point.Half);
			SetMouseState(State.Releasing, Point.Half);
			Assert.IsTrue(button.Get<Transition.Size>().End.Width > BaseSize.Width);
			Window.CloseAfterFrame();
		}

		[Test]
		public void EnteringMakesItGrow()
		{
			var button = CreateButton();
			SetMouseState(State.Released, Point.Half);
			Assert.IsTrue(button.Get<Transition.Size>().End.Width > BaseSize.Width);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ExitingMakesItNormalize()
		{
			var button = CreateButton();
			Assert.IsFalse(button.Contains<Transition.Size>());
			SetMouseState(State.Released, Point.Half);
			SetMouseState(State.Released, Point.Zero);
			Assert.AreEqual(BaseSize, button.Get<Transition.Size>().End);
			Window.CloseAfterFrame();
		}
	}
}