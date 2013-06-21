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
		public NextLevelScreen(InputCommands input, ContentLoader content)
		{
			this.input = input;
			this.content = content;
			InitializeBackground();
		}

		private readonly InputCommands input;
		private readonly ContentLoader content;

		private void InitializeBackground()
		{
			background = new Sprite(content.Load<Image>("NextLevel"), DefaultDrawArea());
			background.Visibility = Visibility.Hide;
			background.RenderLayer = 10;
		}

		private static Rectangle DefaultDrawArea()
		{
			return new Rectangle(0, 0.1875f, 1, 0.625f);
		}

		public void ShowAndWaitForInput()
		{
			background.Visibility = Visibility.Show;
			InitializeButton();
		}

		private void InitializeButton()
		{
			advanceButton = new Button(input, content, "NextLevelButton",
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