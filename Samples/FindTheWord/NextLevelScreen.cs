using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace FindTheWord
{
	public class NextLevelScreen
	{
		public NextLevelScreen(InputCommands input)
		{
			InitializeBackground();
			this.input = input;
		}

		private void InitializeBackground()
		{
			background = new Sprite(ContentLoader.Load<Image>("NextLevel"), GetDrawArea());
			background.Visibility = Visibility.Hide;
			background.RenderLayer = 10;
		}

		private static Rectangle GetDrawArea()
		{
			return new Rectangle(0, 0.1875f, 1, 0.625f);
		}

		private readonly InputCommands input;

		public void ShowAndWaitForInput()
		{
			background.Visibility = Visibility.Show;
			InitializeButton();
		}

		private void InitializeButton()
		{
			advanceButton = new Button(input, "NextLevelButton",
				Rectangle.FromCenter(0.5f, 0.6f, 0.2f, 0.1f));
			advanceButton.RenderLayer = 11;
			advanceButton.Clicked += button => { HideAndStartNextLevel(); };
		}

		public void HideAndStartNextLevel()
		{
			background.Visibility = Visibility.Hide;
			advanceButton.IsActive = false;
			if(StartNextLevel != null)
				StartNextLevel();
		}

		public event Action StartNextLevel;
		private Sprite background;
		private Button advanceButton;
	}
}