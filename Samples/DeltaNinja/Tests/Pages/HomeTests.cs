using System;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaNinja.Pages;
using NUnit.Framework;

namespace DeltaNinja.Tests.Pages
{
	internal class HomeTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateHome()
		{
			var screen = Resolve<ScreenSpace>();
			var home = new HomePage(screen, Resolve<InputCommands>());
			home.Show();

			home.ButtonClicked += (x) => { screen.Window.Title = "Clicked: " + x.ToString(); };

			if (EntitySystem.HasCurrent)
				Assert.AreEqual(9, EntitySystem.Current.NumberOfEntities);
			else
				Assert.NotNull(home);
		}
	}
}