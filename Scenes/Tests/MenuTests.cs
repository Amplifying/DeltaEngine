using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class MenuTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void CreatingSetsButtonSizeAndMenuCenter(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(ButtonSize);
				Assert.AreEqual(ButtonSize, menu.ButtonSize);
				Assert.AreEqual(Point.Half, menu.Center);
			});
		}

		private static readonly Size ButtonSize = new Size(0.3f, 0.1f);

		[IntegrationTest]
		public void ChangingButtonSize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(Size.Half) { ButtonSize = ButtonSize };
				Assert.AreEqual(ButtonSize, menu.ButtonSize);
			});
		}

		[IntegrationTest]
		public void ChangingCenterForSetOfButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(ButtonSize) { Center = Point.One };
				Assert.AreEqual(Point.One, menu.Center);
			});
		}

		[IntegrationTest]
		public void AddingMenuOptionAddsButton(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(ButtonSize) { Center = new Point(0.6f, 0.6f) };
				menu.AddMenuOption(CreateTheme(content), () => { });
				Assert.AreEqual(1, menu.Controls.Count);
				var button = (Button)menu.Controls[0];
				Assert.AreEqual("DeltaEngineLogo", button.Image.Name);
				Assert.AreEqual(new Rectangle(0.45f, 0.55f, 0.3f, 0.1f), button.DrawArea);
			});
		}

		private static Theme CreateTheme(ContentLoader content)
		{
			var appearance = new Theme.Appearance(content.Load<Image>("DeltaEngineLogo"));
			return new Theme
			{
				Button = appearance,
				ButtonMouseover = appearance,
				ButtonPressed = appearance,
				Font = new Font(content, "Verdana12")
			};
		}

		[VisualTest]
		public void ShowMenuWithTwoButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(ButtonSize);
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var theme = CreateTheme(content);
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Top Button"; });
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Bottom Button"; });
				menu.Show();
			});
		}

		[VisualTest]
		public void ShowMenuWithThreeButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(ButtonSize);
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var theme = CreateTheme(content);
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Top Button"; });
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Middle Button"; });
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Bottom Button"; });
				menu.Show();
			});
		}

		[VisualTest]
		public void ShowMenuWithThreeTextButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(ButtonSize);
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var theme = CreateTextTheme(content);
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Top Button"; }, "Top Button");
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Middle Button"; },
					"Middle Button");
				menu.AddMenuOption(theme, () => { window.Title = "Clicked Bottom Button"; },
					"Bottom Button");
				menu.Show();
			});
		}

		private static Theme CreateTextTheme(ContentLoader content)
		{
			return new Theme
			{
				Button =
					new Theme.Appearance(content.Load<Image>("DefaultSliderBackground"), Color.LightGray),
				ButtonMouseover = new Theme.Appearance(content.Load<Image>("DefaultSliderBackground")),
				ButtonPressed =
					new Theme.Appearance(content.Load<Image>("DefaultSliderBackground"), Color.Red),
				Font = new Font(content, "Verdana12")
			};
		}

		[IntegrationTest]
		public void ClearClearsButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(ButtonSize);
				var theme = CreateTheme(content);
				menu.AddMenuOption(theme, () => { });
				Assert.AreEqual(1, menu.Buttons.Count);
				menu.Clear();
				Assert.AreEqual(0, menu.Buttons.Count);
			});
		}

		[IntegrationTest]
		public void ClearMenuOptionsLeavesOtherControlsAlone(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(ButtonSize);
				var logo = content.Load<Image>("DeltaEngineLogo");
				menu.Add(new Sprite(logo, Rectangle.One));
				var theme = CreateTheme(content);
				menu.AddMenuOption(theme, () => { });
				Assert.AreEqual(1, menu.Buttons.Count);
				Assert.AreEqual(2, menu.Controls.Count);
				menu.ClearMenuOptions();
				Assert.AreEqual(0, menu.Buttons.Count);
				Assert.AreEqual(1, menu.Controls.Count);
			});
		}
	}
}