using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class ActiveButtonTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ChangeBaseSize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateButton(content);
				Assert.AreEqual(BaseSize, button.BaseSize);
				button.BaseSize = Size.Half;
				Assert.AreEqual(Size.Half, button.BaseSize);
			});
		}

		private static ActiveButton CreateButton(ContentLoader content)
		{
			var logo = content.Load<Image>("DeltaEngineLogo");
			var theme = new Theme
			{
				Button = new Theme.Appearance(logo, NormalColor),
				ButtonMouseover = new Theme.Appearance(logo, MouseoverColor),
				ButtonPressed = new Theme.Appearance(logo, PressedColor),
				Font = new Font(content, "Verdana12")
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

		[IntegrationTest]
		public void BeginningClickMakesItShrink(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateButton(content);
				Assert.IsFalse(button.Contains<Transition.Size>());
				SetMouseState(State.Pressing, Point.Half);
				Assert.IsTrue(button.Get<Transition.Size>().End.Width < BaseSize.Width);
			});
		}

		private void SetMouseState(State state, Point position)
		{
			mockResolver.input.SetMousePosition(Point.Zero);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Released);
			mockResolver.AdvanceTimeAndExecuteRunners();
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		[IntegrationTest]
		public void FinishingClickMakesItGrow(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateButton(content);
				Assert.IsFalse(button.Contains<Transition.Size>());
				SetMouseState(State.Pressing, Point.Half);
				SetMouseState(State.Releasing, Point.Half);
				Assert.IsTrue(button.Get<Transition.Size>().End.Width > BaseSize.Width);
			});
		}

		[IntegrationTest]
		public void EnteringMakesItGrow(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateButton(content);
				SetMouseState(State.Released, Point.Half);
				Assert.IsTrue(button.Get<Transition.Size>().End.Width > BaseSize.Width);
			});
		}

		[IntegrationTest]
		public void ExitingMakesItNormalize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var button = CreateButton(content);
				Assert.IsFalse(button.Contains<Transition.Size>());
				SetMouseState(State.Released, Point.Half);
				SetMouseState(State.Released, Point.Zero);
				Assert.AreEqual(BaseSize, button.Get<Transition.Size>().End);
			});
		}

		[VisualTest]
		public void RenderActiveButton(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) => CreateButton(content));
		}
	}
}