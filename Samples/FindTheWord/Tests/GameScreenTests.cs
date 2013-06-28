using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	internal class GameScreenTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowScreen()
		{
			Window.ViewportPixelSize = new Size(1280, 800);
			var screen = new GameScreen(Resolve<InputCommands>());
			screen.FadeIn();
			screen.StartNextLevel();
		}
	}
}