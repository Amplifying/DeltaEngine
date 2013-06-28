using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class MenuTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreatingSetsButtonSizeAndMenuCenter()
		{
			var menu = new Menu(ButtonSize);
			Assert.AreEqual(ButtonSize, menu.ButtonSize);
			Assert.AreEqual(Point.Half, menu.Center);
			Window.CloseAfterFrame();
		}

		private static readonly Size ButtonSize = new Size(0.3f, 0.1f);

		[Test]
		public void ChangingButtonSize()
		{
			var menu = new Menu(Size.Half) { ButtonSize = ButtonSize };
			Assert.AreEqual(ButtonSize, menu.ButtonSize);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangingCenterForSetOfButtons()
		{
			var menu = new Menu(ButtonSize) { Center = Point.One };
			Assert.AreEqual(Point.One, menu.Center);
			Window.CloseAfterFrame();
		}

		[Test]
		public void AddingMenuOptionAddsButton()
		{
			var menu = new Menu(ButtonSize) { Center = new Point(0.6f, 0.6f) };
			menu.AddMenuOption(CreateTheme(), () => { });
			Assert.AreEqual(1, menu.Controls.Count);
			var button = (Button)menu.Controls[0];
			Assert.AreEqual("DeltaEngineLogo", button.Image.Name);
			Assert.AreEqual(new Rectangle(0.45f, 0.55f, 0.3f, 0.1f), button.DrawArea);
			Window.CloseAfterFrame();
		}

		private static Theme CreateTheme()
		{
			var appearance = new Theme.Appearance(ContentLoader.Load<Image>("DeltaEngineLogo"));
			return new Theme
			{
				Button = appearance,
				ButtonMouseover = appearance,
				ButtonPressed = appearance,
				Font = new Font("Verdana12")
			};
		}

		[Test]
		public void ShowMenuWithTwoButtons()
		{
			var menu = new Menu(ButtonSize);
			menu.SetBackground(ContentLoader.Load<Image>("SimpleSubMenuBackground"));
			var theme = CreateTheme();
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Top Button"; });
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Bottom Button"; });
			menu.Show();
		}

		[Test]
		public void ShowMenuWithThreeButtons()
		{
			var menu = new Menu(ButtonSize);
			menu.SetBackground(ContentLoader.Load<Image>("SimpleSubMenuBackground"));
			var theme = CreateTheme();
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Top Button"; });
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Middle Button"; });
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Bottom Button"; });
			menu.Show();
		}

		[Test]
		public void ShowMenuWithThreeTextButtons()
		{
			var menu = new Menu(ButtonSize);
			menu.SetBackground(ContentLoader.Load<Image>("SimpleSubMenuBackground"));
			var theme = CreateTextTheme();
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Top Button"; }, "Top Button");
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Middle Button"; }, "Middle Button");
			menu.AddMenuOption(theme, () => { Window.Title = "Clicked Bottom Button"; }, "Bottom Button");
			menu.Show();
		}

		private static Theme CreateTextTheme()
		{
			return new Theme
			{
				Button = new Theme.Appearance("DefaultSliderBackground", Color.LightGray),
				ButtonMouseover = new Theme.Appearance("DefaultSliderBackground"),
				ButtonPressed = new Theme.Appearance("DefaultSliderBackground", Color.Red),
				Font = new Font("Verdana12")
			};
		}

		[Test]
		public void ClearClearsButtons()
		{
			var menu = new Menu(ButtonSize);
			var theme = CreateTheme();
			menu.AddMenuOption(theme, () => { });
			Assert.AreEqual(1, menu.Buttons.Count);
			menu.Clear();
			Assert.AreEqual(0, menu.Buttons.Count);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ClearMenuOptionsLeavesOtherControlsAlone()
		{
			var menu = new Menu(ButtonSize);
			var logo = ContentLoader.Load<Image>("DeltaEngineLogo");
			menu.Add(new Sprite(logo, Rectangle.One));
			var theme = CreateTheme();
			menu.AddMenuOption(theme, () => { });
			Assert.AreEqual(1, menu.Buttons.Count);
			Assert.AreEqual(2, menu.Controls.Count);
			menu.ClearMenuOptions();
			Assert.AreEqual(0, menu.Buttons.Count);
			Assert.AreEqual(1, menu.Controls.Count);
			Window.CloseAfterFrame();
		}
	}
}