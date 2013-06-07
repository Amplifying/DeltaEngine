using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class MainMenuTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void ShowDummyMenuBackground(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var menu = new MainMenu(content, screen);
				menu.Background.Image = content.Load<Image>("Background");
			});
		}

		[IntegrationTest]
		public void CheckMenuBackground(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var menu = new MainMenu(content, screen);
				Assert.AreEqual(content.Load<Image>("MainMenu"), menu.Background.Image);
			});
		}

		[VisualTest]
		public void ShowDummyGameLogo(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var menu = new MainMenu(content, screen);
				menu.CreepyTowersLogo.Image = content.Load<Image>("DeltaEngineLogo");
			});
		}

		[IntegrationTest]
		public void CheckCreepyTowersLogo(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var menu = new MainMenu(content, screen);
				Assert.AreEqual(content.Load<Image>("CreepyTowersLogo"), menu.CreepyTowersLogo.Image);
			});
		}

		[Test]
		public void CheckResizingWindowAdjustsLogoDrawArea()
		{
			Start(typeof(MockResolver), (ContentLoader content, ScreenSpace screen) =>
			{
				new MainMenu(content, screen);
				var drawAreaChanged = false;
				screen.ViewportSizeChanged += () => { drawAreaChanged = true; };
				screen.Window.SetFullscreen(new Size(1600, 900));
				Assert.IsTrue(drawAreaChanged);
			});
		}

		[VisualTest]
		public void ResizingWindowChnagesLogoDrawAreaAndColour(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var menu = new MainMenu(content, screen);
				screen.ViewportSizeChanged += () => { menu.CreepyTowersLogo.Color = Color.Red; };
			});
		}


		[VisualTest]
		public void ShowMenuWithOneButton(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var menu = new MainMenu(content, screen);
			});
		}

		//[VisualTest]
		//public void ShowMenuWithAllFiveButtons(Type resolver)
		//{
		//	Start(resolver, (Scene s, ContentLoader content, Window window) =>
		//	{
		//		var menu = new MainMenu(content, new Size(0.3f, 0.08f));
		//		menu.Show();
		//	});
		//}
	}
}