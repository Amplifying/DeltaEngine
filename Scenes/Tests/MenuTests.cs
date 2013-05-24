using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
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
				var menu = new Menu(content, Size.Half);
				Assert.AreEqual(Size.Half, menu.ButtonSize);
			});
		}

		[IntegrationTest]
		public void ChangingButtonSize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(content, Size.Half) { ButtonSize = Size.One };
				Assert.AreEqual(Size.One, menu.ButtonSize);
			});
		}

		[IntegrationTest]
		public void AddingMenuOptionAddsButton(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(content, Size.Half);
				var logo = content.Load<Image>("DeltaEngineLogo");
				menu.AddMenuOption(logo, () => { });
				Assert.AreEqual(1, menu.Controls.Count);
				Assert.AreEqual(logo, ((Button)menu.Controls[0]).Image);
			});
		}

		[VisualTest]
		public void ShowMenuWithOneButton(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(content, new Size(0.3f, 0.1f));
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var logo = content.Load<Image>("DeltaEngineLogo");
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Button"; });
				menu.Show();
			});
		}

		[VisualTest]
		public void ShowMenuWithTwoButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(content, new Size(0.3f, 0.1f));
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var logo = content.Load<Image>("DeltaEngineLogo");
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Top Button"; });
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Bottom Button"; });
				menu.Show();
			});
		}

		[VisualTest]
		public void ShowMenuWithThreeButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(content, new Size(0.3f, 0.1f));
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var logo = content.Load<Image>("DeltaEngineLogo");
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Top Button"; });
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Middle Button"; });
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Bottom Button"; });
				menu.Show();
			});
		}

		[VisualTest]
		public void ShowMenuWithThreeButtonsAndText(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Window window) =>
			{
				var menu = new Menu(content, new Size(0.3f, 0.1f));
				menu.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var logo = content.Load<Image>("DefaultButtonBackground");
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Top Button"; }, "Top Button");
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Middle Button"; }, "Middle Button");
				menu.AddMenuOption(logo, () => { window.Title = "Clicked Bottom Button"; }, "Bottom Button");
				menu.Show();
			});
		}

		[IntegrationTest]
		public void ClearClearsButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var menu = new Menu(content, Size.Half);
				var logo = content.Load<Image>("DefaultButtonBackground");
				menu.AddMenuOption(logo, () => { });
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
				var menu = new Menu(content, Size.Half);
				var logo = content.Load<Image>("DefaultButtonBackground");
				menu.Add(new Sprite(logo,Rectangle.One));
				menu.AddMenuOption(logo, () => { });
				Assert.AreEqual(1, menu.Buttons.Count);
				Assert.AreEqual(2, menu.Controls.Count);
				menu.ClearMenuOptions();
				Assert.AreEqual(0, menu.Buttons.Count);
				Assert.AreEqual(1, menu.Controls.Count);
			});
		}
	}
}