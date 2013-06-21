using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace CreepyTowers
{
	/// <summary>
	/// TutorialDialogue telling the player how to do things in a visual way
	/// </summary>
	public class TutorialDialogue
	{
		public TutorialDialogue(ContentLoader contentLoader, ScreenSpace screenSpace)
		{
			messageCount = 0;
			SetUpMessages();

			var characterImage = contentLoader.Load<Image>("ComicStripDragonTalk");
			var speechbubbleImage = contentLoader.Load<Image>("ComicStripBubble");
			var charAspect = characterImage.PixelSize.Width / characterImage.PixelSize.Height;
			var bubbleAspect = speechbubbleImage.PixelSize.Width / speechbubbleImage.PixelSize.Height;

			var portraitDrawArea = new Rectangle(screenSpace.Viewport.Left,
				screenSpace.Viewport.Top + 0.05f, 0.2f * charAspect, 0.2f);
			var speechbubbleDrawArea = new Rectangle(portraitDrawArea.TopRight.X - 0.07f,
				screenSpace.Viewport.Top, 0.2f * bubbleAspect, 0.2f);
			var textPosition = speechbubbleDrawArea.Center;
			characterPortrait = new Sprite(characterImage, portraitDrawArea);
			speechbubble = new Sprite(speechbubbleImage, speechbubbleDrawArea);
			var messageFont = new Font(contentLoader, "Verdana12");
			messageText = new FontText(messageFont, messages[messageCount], textPosition);
		}

		private int messageCount;
		private string[] messages;
		private readonly FontText messageText;
		private readonly Sprite characterPortrait;
		private readonly Sprite speechbubble;
		public event Action DialogueAdvanced;
		public event Action DialogueEnded;

		private void SetUpMessages()
		{
			messages = new[]
			{
				"Welcome to \n Creepy Towers prototype!",
				"Defeat the Sandcreeps \n before they reach their goal!",
				"To build a Tower, \n select one of them  \n from the menu.",
				"Now click on a spot \n where you wish to build it.",
				"Each type of Tower \n has a different \n elemental attack. ",
				"Each Tower attack \n creates different reactions \n upon hitting a Creep. ",
				"Hit the Creep Button \n to spawn a new Sandcreep!",
				"To reset the level, \n simply press the 'Reset' Button.",
				"Experiment with different  \n chain reactions and have fun!"
			};
		}

		public void AdvanceToNextMessage()
		{
			if (++messageCount >= messages.Length)
				Dispose();
			else
			{
				messageText.Text = messages[messageCount];
				if (DialogueAdvanced != null)
					DialogueAdvanced();
			}
		}

		public void Skip()
		{
			Dispose();
		}

		public void Dispose()
		{
			characterPortrait.IsActive = false;
			speechbubble.IsActive = false;
			messageText.IsActive = false;
			if (DialogueEnded != null)
				DialogueEnded();
		}
	}
}