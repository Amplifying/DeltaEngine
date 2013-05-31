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
		public void CreatingSetsButtonSize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(Size.Half);
				Assert.AreEqual(Size.Half, menu.ButtonSize);
			});
		}

		[IntegrationTest]
		public void ChangingButtonSize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(Size.Half) { ButtonSize = Size.One };
				Assert.AreEqual(Size.One, menu.ButtonSize);
			});
		}

		[IntegrationTest]
		public void AddingMenuOptionAddsButton(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(Size.Half);
				menu.AddMenuOption(CreateTheme(content), () => { });
				Assert.AreEqual(1, menu.Controls.Count);
				Assert.AreEqual("DeltaEngineLogo", ((Button)menu.Controls[0]).Image.Name);
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
				var menu = new Menu(new Size(0.3f, 0.1f));
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
				var menu = new Menu(new Size(0.3f, 0.1f));
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
				var menu = new Menu(new Size(0.3f, 0.1f));
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
				var menu = new Menu(Size.Half);
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
				var menu = new Menu(Size.Half);
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