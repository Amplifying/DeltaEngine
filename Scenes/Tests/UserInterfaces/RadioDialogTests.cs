using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class RadioDialogTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderRadioDialogWithThreeButtons()
		{
			RadioDialog dialog = CreateDialog();
			RunCode = () => { Window.Title = "Button '" + dialog.SelectedButton.Text + "'"; };
		}

		private static RadioDialog CreateDialog()
		{
			var theme = CreateTheme();
			var dialog = new RadioDialog(theme, Center);
			EntitySystem.Current.Run();
			dialog.AddButton("Top Button");
			dialog.AddButton("Middle Button");
			dialog.AddButton("Bottom Button");
			EntitySystem.Current.Run();
			return dialog;
		}

		private static Theme CreateTheme()
		{
			return new Theme
			{
				RadioButtonBackground = new Theme.Appearance("DefaultLabel"),
				RadioButtonNotSelected = new Theme.Appearance("DefaultRadiobuttonOff"),
				RadioButtonNotSelectedMouseover = new Theme.Appearance("DefaultRadioButtonOffHover"),
				RadioButtonSelected = new Theme.Appearance("DefaultRadiobuttonOn"),
				RadioButtonSelectedMouseover = new Theme.Appearance("DefaultRadioButtonOnHover"),
				Font = new Font("Verdana12")
			};
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.3f);

		[Test]
		public void RenderGrowingRadioDialog()
		{
			RadioDialog dialog = CreateDialog();
			RunCode = () =>
			{
				var center = dialog.DrawArea.Center + new Point(0.01f, 0.01f) * Time.Current.Delta;
				var size = dialog.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
				dialog.DrawArea = Rectangle.FromCenter(center, size);
				Window.Title = "Button '" + dialog.SelectedButton.Text + "'";
			};
		}

		[Test]
		public void ClickingRadioButtonSelectsIt()
		{
			var dialog = CreateDialog();
			var buttons = dialog.Get<List<RadioButton>>();
			Assert.IsFalse(buttons[1].IsSelected);
			PressAndReleaseMouse(Point.One);
			Assert.IsFalse(buttons[1].IsSelected);
			PressAndReleaseMouse(new Point(0.35f, 0.5f));
			Assert.IsTrue(buttons[1].IsSelected);
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
		public void ClickingOneRadioButtonCausesTheOthersToUnselect()
		{
			var dialog = CreateDialog();
			var buttons = dialog.Get<List<RadioButton>>();
			PressAndReleaseMouse(Point.Half);
			PressAndReleaseMouse(new Point(0.35f, 0.6f));
			Assert.IsFalse(buttons[1].IsSelected);
			Assert.IsTrue(buttons[2].IsSelected);
			Window.CloseAfterFrame();
		}
	}
}