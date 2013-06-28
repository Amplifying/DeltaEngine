using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Scenes.UserInterfaces.Terminal
{
	public class TextLine : IDisposable
	{
		public TextLine(string text, float fontHeight)
		{
			this.fontHeight = fontHeight;
			var font = new Font("Verdana12");
			fontText = new FontText(font, text, Point.Zero) { Color = FontColor };
			LeftAlignFontText();
		}

		private readonly float fontHeight;
		private static readonly Color FontColor = new Color(200, 220, 255);
		internal readonly FontText fontText;

		private void LeftAlignFontText()
		{
			fontText.DrawArea = new Rectangle(position.X + (fontText.Text.Length * fontHeight) / 2,
				position.Y + fontHeight, fontHeight, fontHeight);
		}

		private Point position;

		public Point Position
		{
			get { return position; }
			set
			{
				position = value;
				LeftAlignFontText();
			}
		}

		public string Text
		{
			get { return fontText.Text; }
			set
			{
				fontText.Text = value;
				LeftAlignFontText();
			}
		}

		public void Dispose()
		{
			fontText.IsActive = false;
		}
	}
}