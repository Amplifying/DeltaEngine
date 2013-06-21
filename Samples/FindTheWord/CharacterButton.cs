using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;

namespace FindTheWord
{
	public class CharacterButton : Button
	{
		public CharacterButton(InputCommands input, ContentLoader content, float xCenter, float yCenter)
			: base(input, content, "CharBackground", GetDrawArea(xCenter, yCenter))
		{
			font = new Font(content, "Tahoma30");
			letter = NoCharacter;
			currentFontText = new FontText(font, "", Point.Half);
			currentFontText.RenderLayer = 4;
		}

		private static Rectangle GetDrawArea(float xCenter, float yCenter)
		{
			return Rectangle.FromCenter(xCenter, yCenter, Dimension, Dimension);
		}

		internal const float Dimension = 64.0f / 1280.0f;
		private readonly Font font;
		private char letter;
		internal const char NoCharacter = '\0';
		private readonly FontText currentFontText;

		public char Letter
		{
			get { return letter; }
			set
			{
				letter = value;
				currentFontText.Text = letter.ToString(CultureInfo.InvariantCulture);
				SetNewCenter(DrawArea.Center);
			}
		}

		private void SetNewCenter(Point newCenter)
		{
			Center = newCenter;
			var newLetterArea = DrawArea;
			newLetterArea.Top += Dimension * 0.4f;
			newLetterArea.Left += Dimension * 0.05f;
			currentFontText.DrawArea = newLetterArea;
		}

		public void RemoveLetter()
		{
			currentFontText.Text = "";
		}

		public void ShowLetter()
		{
			currentFontText.Text = letter.ToString(CultureInfo.InvariantCulture);
		}

		public int Index { get; set; }

		public override string ToString()
		{
			return GetType().Name + "(" + Index + " - " + Letter + ")";
		}

		public bool IsClickable()
		{
			return Visibility == Visibility.Show && currentFontText.Text != "";
		}

		public override void Dispose()
		{
			currentFontText.Text = "";
			currentFontText.IsActive = false;
			IsActive = false;
			base.Dispose();
		}
	}
}