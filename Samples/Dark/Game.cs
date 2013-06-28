using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Dark
{
	public enum Scenes
	{
		Intro,
		Level,
		SceneCount
	}

	public class Game : Runner
	{
		public Game(Window window, InputCommands input, ScreenSpace screenSpace, SoundEffects sounds)
		{
			this.window = window;
			this.input = input;
			this.screenSpace = screenSpace;
			SetupWindow();
			CreateScenes();
			SetScene(Scenes.Level);
		}

		private readonly Window window;
		private readonly InputCommands input;
		private readonly ScreenSpace screenSpace;

		public Window Window
		{
			get { return window; }
		}

		private void SetupWindow()
		{
			window.ViewportPixelSize = new Size(1024, 768);
		}

		private void CreateScenes()
		{
			scenes = new Scene[(int)Scenes.SceneCount];
			scenes[(int)Scenes.Intro] = new SceneIntro(this, input, screenSpace);
			scenes[(int)Scenes.Level] = new SceneLevel(this, input, screenSpace);
		}

		private Scene[] scenes;
		private int currentScene = -1;

		public void Run()
		{
			scenes[currentScene].Update();
		}

		public void SetScene(Scenes nextScene)
		{
			if (currentScene != -1)
				scenes[currentScene].Release();

			currentScene = (int)nextScene;
			scenes[currentScene].Load();
		}
	}
}