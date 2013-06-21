using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace CreepyTowers
{
	/// <summary>
	/// Starts a new Creepy Towers game
	/// </summary>
	public class Game
	{
		public Game(ScreenSpace screen, ContentLoader contentLoader, InputCommands input)
		{
			this.screen = screen;
			content = contentLoader;
			this.input = input;

			SetupPlayArea();
			SetupMenuScreen();
		}

		private readonly ScreenSpace screen;
		private readonly ContentLoader content;
		private readonly InputCommands input;

		private void SetupPlayArea()
		{
			screen.Window.Title = "Creepy Towers Prototype";
			screen.Window.TotalPixelSize = new Size(1000, 1000);
		}

		private void SetupMenuScreen()
		{
			var mainMenu = new MainMenu(content, screen);
			adddtutorialcommand = input.Add(MouseButton.Left, State.Releasing, mouse =>
			{
				mainMenu.Dispose();
				new TutorialStarter(content, screen, input);
				input.Remove(adddtutorialcommand);
			});
			//mainMenu.ExitGame += () => screen.Window.Dispose();
			//mainMenu.Show();
		}

		private Command adddtutorialcommand;

		private class TutorialStarter
		{
			public TutorialStarter(ContentLoader contentLoader, ScreenSpace screenSpace,
				InputCommands input)
			{
				dialogue = new TutorialDialogue(contentLoader, screenSpace);
				input.Add(MouseButton.Left, State.Releasing, mouse => { dialogue.AdvanceToNextMessage(); });
			}

			public readonly TutorialDialogue dialogue;
		}
	}
}