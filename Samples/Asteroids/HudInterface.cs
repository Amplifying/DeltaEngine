using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Asteroids
{
	public class HudInterface
	{
		public HudInterface(ContentLoader contentLoader, ScreenSpace screenSpace)
		{
			hudFont = new Font(contentLoader, "Tahoma30");
			ScoreDisplay = new FontText(hudFont, "0",
				new Point(screenSpace.Viewport.Left + 0.02f, screenSpace.Viewport.Top + 0.08f));
			screenSpace.ViewportSizeChanged +=
				() =>
				{
					ScoreDisplay.SetPosition(new Point(screenSpace.Viewport.Left + 0.02f,
						screenSpace.Viewport.Top + 0.04f));
				};
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
			var gameOverText = new FontText(hudFont, "Game Over", Point.Half);
			metaInfoTexts.Add(gameOverText);
		}
	}
}