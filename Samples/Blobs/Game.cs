using Blobs.Levels;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs
{
	/// <summary>
	/// Start a new game of Blobs
	/// </summary>
	public class Game : Runner
	{
		public Game(ScreenSpace screen, InputCommands input, ContentLoader content)
		{
			this.screen = screen;
			this.input = input;
			this.content = content;
			CreateLevels();
			SwitchScene(intro);
		}

		private readonly ScreenSpace screen;
		private readonly InputCommands input;
		private readonly ContentLoader content;

		private void CreateLevels()
		{
			CreateIntro();
			CreateLevel1();
			CreateLevel2();
		}

		private void CreateIntro()
		{
			intro = new Intro(screen, input, content);
			intro.Passed += () => SwitchScene(level1);
		}

		private Level intro;

		private void CreateLevel1()
		{
			level1 = new Level1(screen, input, content);
			level1.Passed += () => SwitchScene(level2);
			level1.Failed += () => SwitchScene(intro);
		}

		private Level level1;

		private void CreateLevel2()
		{
			level2 = new Level2(screen, input, content);
			level2.Passed += () => SwitchScene(intro);
			level2.Failed += () => SwitchScene(intro);
		}

		private Level level2;

		private void SwitchScene(Level level)
		{
			CurrentLevel = level;
			CurrentLevel.Reset();
		}

		private Level CurrentLevel { get; set; }

		public void Run()
		{
			CurrentLevel.Run();
		}
	}
}