using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
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
	public class RadioDialogTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderRadioDialogWithThreeButtons(Type resolver)
		{
			RadioDialog dialog = null;
			Start(resolver, (Scene s, ContentLoader content) => { dialog = CreateDialog(content); },
				(Window window) => { window.Title = "Button '" + dialog.SelectedButton.Text + "'"; });
		}

		private static RadioDialog CreateDialog(ContentLoader content)
		{
			var theme = CreateTheme(content);
			var dialog = new RadioDialog(theme, Center);
			EntitySystem.Current.Run();
			dialog.AddButton("Top Button");
			dialog.AddButton("Middle Button");
			dialog.AddButton("Bottom Button");
			EntitySystem.Current.Run();
			return dialog;
		}

		private static Theme CreateTheme(ContentLoader content)
		{
			return new Theme
			{
				RadioButtonBackground = new Theme.Appearance(content.Load<Image>("DefaultLabel")),
				RadioButtonNotSelected = new Theme.Appearance(content.Load<Image>("DefaultRadiobuttonOff")),
				RadioButtonNotSelectedMouseover =
					new Theme.Appearance(content.Load<Image>("DefaultRadioButtonOffHover")),
				RadioButtonSelected = new Theme.Appearance(content.Load<Image>("DefaultRadiobuttonOn")),
				RadioButtonSelectedMouseover =
					new Theme.Appearance(content.Load<Image>("DefaultRadioButtonOnHover")),
				Font = new Font(content, "Verdana12")
			};
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.6f);

		[VisualTest]
		public void RenderGrowingRadioDialog(Type resolver)
		{
			RadioDialog dialog = null;
			Start(resolver, (Scene s, ContentLoader content) => { dialog = CreateDialog(content); },
				(Window window) =>
				{
					var center = dialog.DrawArea.Center + new Point(0.01f, 0.01f) * Time.Current.Delta;
					var size = dialog.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
					dialog.DrawArea = Rectangle.FromCenter(center, size);
					window.Title = "Button '" + dialog.SelectedButton.Text + "'";
				});
		}

		[IntegrationTest]
		public void ClickingRadioButtonSelectsIt(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var dialog = CreateDialog(content);
				var buttons = dialog.Get<List<RadioButton>>();
				Assert.IsFalse(buttons[1].IsSelected);
				PressAndReleaseMouse(Point.One);
				Assert.IsFalse(buttons[1].IsSelected);
				PressAndReleaseMouse(Point.Half);
				Assert.IsTrue(buttons[1].IsSelected);
			});
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
		public void ClickingOneRadioButtonCausesTheOthersToUnselect(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var dialog = CreateDialog(content);
				var buttons = dialog.Get<List<RadioButton>>();
				PressAndReleaseMouse(Point.Half);
				PressAndReleaseMouse(new Point(0.5f,0.6f));
				Assert.IsFalse(buttons[1].IsSelected);
				Assert.IsTrue(buttons[2].IsSelected);
			});
		}
	}
}