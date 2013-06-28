using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Asteroids
{
	public class HudInterface
	{
		public HudInterface(ScreenSpace screenSpace)
		{
			hudFont = new Font("Tahoma30");
			ScoreDisplay = new FontText(hudFont, "0",
				new Point(screenSpace.Viewport.Left + 0.02f, screenSpace.Viewport.Top + 0.08f));
			ScoreDisplay.RenderLayer = (int)AsteroidsRenderLayer.UserInterface;
			gameOverText = new FontText(hudFont, "", Point.Half);
			gameOverText.RenderLayer = (int)AsteroidsRenderLayer.UserInterface;

		}

		private readonly Font hudFont;

		public FontText ScoreDisplay { get; private set; }

		public List<FontText> metaInfoTexts = new List<FontText>();

		public void SetScoreText(int score)
		{
			ScoreDisplay.Text = score.ToString(CultureInfo.InvariantCulture);
		}

		public void SetGameOverText()
		{
			gameOverText.Text = "Game Over! \n [Space] \n to restart";
			gameOverText.Visibility = Visibility.Show;
		}

		private readonly FontText gameOverText;

		public void SetIngameMode()
		{
			gameOverText.Visibility = Visibility.Hide;
		}
	}
}