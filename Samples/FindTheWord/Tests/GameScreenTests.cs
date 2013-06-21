using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;

namespace FindTheWord.Tests
{
	internal class GameScreenTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void ShowScreen(Type resolver)
		{
			Start(resolver, (Window window, InputCommands input, ContentLoader content) =>
			{
				window.TotalPixelSize = new Size(1280, 800);
				var screen = new GameScreen(input, content);
				screen.FadeIn();
				screen.StartNextLevel();
			});
		}
	}
}