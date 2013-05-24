using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Fonts
{
	public class FontData
	{
		public FontData(XmlData data)
		{
			if (data == null || data.Children.Count == 0)
				throw new UnableToLoadFontDataWithoutValidXmlData();

			this.data = data;
			Load();
		}

		public class UnableToLoadFontDataWithoutValidXmlData : Exception {}

		private readonly XmlData data;
		private const char NoChar = '\0';
		private const char FallbackCharForUnsupportedCharacters = '?';
		public string FontFamilyName
		{
			get { return data.GetAttributeValue("Family", "Verdana"); }
		}
		public int SizeInPoints
		{
			get { return data.GetAttributeValue("Size", 12); }
		}
		public string Style
		{
			get { return data.GetAttributeValue("Style"); }
		}
		public int PixelLineHeight
		{
			get { return data.GetAttributeValue("LineHeight", 20); }
		}
		public string FontMapName
		{
			get
			{
				var bitmap = data.GetChild("Bitmap");
				return bitmap == null ? "Verdana9" : bitmap.GetAttributeValue("Name");
			}
		}
		public Size FontMapPixelSize
		{
			get
			{
				var bitmap = data.GetChild("Bitmap");
				return bitmap == null ? Size.One : new Size(bitmap.GetAttributeValue("Width", 256),
					bitmap.GetAttributeValue("Height", 256));
			}
		}

		private void Load()
		{
			var fontMapSize = FontMapPixelSize;
			foreach (var child in data.Children)
			{
				if (child.Name == "Glyphs")
					foreach (var glyphData in child.Children)
						LoadGlyph(glyphData, fontMapSize);
				else if (child.Name == "Kernings")
					foreach (var kerningData in child.Children)
						LoadKerning(kerningData);
			}
		}

		private void LoadGlyph(XmlData glyphData, Size fontMapSize)
		{
			char character = glyphData.GetAttributeValue("Character", ' ');
			var glyph = new Glyph
			{
				UV = new Rectangle(glyphData.GetAttributeValue("UV")),
				LeftSideBearing = glyphData.GetAttributeValue("LeftBearing", 0.0f),
				RightSideBearing = glyphData.GetAttributeValue("RightBearing", 0.0f)
			};
			glyph.AdvanceWidth = glyphData.GetAttributeValue("AdvanceWidth", glyph.UV.Width - 2.0f);
			glyph.PrecomputedFontMapUV = Rectangle.BuildUVRectangle(glyph.UV, fontMapSize);
			glyphDictionary.Add(character, glyph);
		}

		public readonly Dictionary<char, Glyph> glyphDictionary = new Dictionary<char, Glyph>();

		private void LoadKerning(XmlData kerningData)
		{
			char firstChar = kerningData.GetAttributeValue("First", '\0');
			char secondChar = kerningData.GetAttributeValue("Second", '\0');
			int kerningDistance = kerningData.GetAttributeValue("Distance", 0);
			if (firstChar == '\0' || secondChar == '\0' || kerningDistance == 0)
				throw new InvalidDataException("Unable to add kerning " + firstChar + " and " + secondChar +
					" with distance=" + kerningDistance);

			Glyph glyph;
			if (glyphDictionary.TryGetValue(firstChar, out glyph))
				glyph.Kernings.Add(secondChar, kerningDistance);
		}

		/// <summary>
		/// Gets all glyph draw areas and UVs needed to show the text on the screen (in pixel space).
		/// </summary>
		[Obsolete("TODO: split up in several methods, this is not clean code")]
		public GlyphDrawAreaAndUV[] GetGlyphDrawAreaAndUVs(string text, float lineSpacing,
			HorizontalAlignment textAlignment, bool isWordWrapOn, ref Size maxTextPixelSize)
		{
			var glyphInfos = new List<GlyphDrawAreaAndUV>();
			float finalLineHeight = PixelLineHeight * lineSpacing;
			var lastDrawInfo = new GlyphDrawAreaAndUV
			{
				DrawArea = new Rectangle(Point.Zero, new Size(0, finalLineHeight)),
				UV = new Rectangle(Point.Zero, new Size(0, finalLineHeight)),
			};

			// First split the given text in several text lines (if there are line breaks)
			List<float> textlineWidths;
			float maxTextlineWidth;
			List<List<char>> textlines = GetTextLines(text, lineSpacing, maxTextPixelSize, textAlignment,
				maxTextPixelSize != Size.Zero, isWordWrapOn, out textlineWidths, out maxTextlineWidth);

			for (int lineId = 0; lineId < textlines.Count; lineId++)
			{
				List<char> textline = textlines[lineId];

				// Sanity check, because we only need to align and handle characters if we have some
				if (textline.Count > 0)
				{
					switch (textAlignment)
					{
						// For left-aligned texts (that goes left-to-right) we need to
						// subtract the LeftSideBearing to make sure that every first
						// character of a text line starts really at the same left border
						// -> 'abcde'   instead of   'abcde'
						//    'bcd'                  ' bcd'
						//    'cdef'                 'cdef'
					case HorizontalAlignment.Left:
						char firstChar = textline[0];
						lastDrawInfo.DrawArea.Left = -glyphDictionary[firstChar].LeftSideBearing;
						break;

						// For center-aligned text, we add here the LeftSideBearing too
						// like in the left-aligned mode, but additionally we still make
						// sure that the every text line is centered (based on the
						// longest text line)
					case HorizontalAlignment.Centered:
						firstChar = textline[0];
						lastDrawInfo.DrawArea.Left = MathExtensions.Round(
							(maxTextlineWidth - textlineWidths[lineId]) * 0.5f -
								glyphDictionary[firstChar].LeftSideBearing);
						break;

						// For right-aligned texts (that goes left-to-right) we need to
						// add the RightSideBearing of the last character of a text line
						// at the beginning the text line to make sure that every last
						// character of a text line ends really at the same right border
						// (because we all add the chars from left-to-right)
						// -> 'abcde'   instead of   'abcde'
						//      'bcd'                 'bcd '
						//     'cdef'                 'cdef'
					case HorizontalAlignment.Right:
						char lastChar = textline[textline.Count - 1];
						lastDrawInfo.DrawArea.Left = MathExtensions.Round(
							(maxTextlineWidth - textlineWidths[lineId]) +
								glyphDictionary[lastChar].RightSideBearing);
						break;

					default:
						lastDrawInfo.DrawArea.Left = 0;
						break;
					}

					// Also apply the offset for each letter
					float startPositionX = lastDrawInfo.DrawArea.Left;
					float totalGlyphWidth = 0.0f;

					// Now iterate through all characters of the text
					Glyph lastGlyph = null;
					for (int charIndex = 0; charIndex < textline.Count; charIndex++)
					{
						char textChar = textline[charIndex];
						Glyph characterGlyph;
						if (!glyphDictionary.TryGetValue(textChar, out characterGlyph))
							characterGlyph = glyphDictionary['?'];

						float glyphWidth = characterGlyph.AdvanceWidth;
						int charKerning;
						if (lastGlyph != null && lastGlyph.Kernings != null &&
							lastGlyph.Kernings.TryGetValue(textChar, out charKerning))
							totalGlyphWidth += charKerning;

						// To avoid blurry text the width has to be a whole numbers
						var newDrawInfo = new GlyphDrawAreaAndUV();
						var startPosition = new Point(MathExtensions.Round(startPositionX + totalGlyphWidth +
							characterGlyph.LeftSideBearing), lastDrawInfo.DrawArea.Top);
						newDrawInfo.DrawArea = new Rectangle(startPosition, characterGlyph.UV.Size);
						newDrawInfo.UV = characterGlyph.PrecomputedFontMapUV;
						glyphInfos.Add(newDrawInfo);
						lastDrawInfo = newDrawInfo;

						// Increase the position for the next character.
						totalGlyphWidth += (float)Math.Round(glyphWidth);
						lastGlyph = characterGlyph;
					}
				}

				lastDrawInfo.DrawArea.Top += finalLineHeight;
			}

			maxTextPixelSize = new Size(maxTextlineWidth, lastDrawInfo.DrawArea.Bottom);
			return glyphInfos.ToArray();
		}

		public override string ToString()
		{
			return base.ToString() + ", Font Family=" + FontFamilyName + ", Font Size=" + SizeInPoints;
		}

		/// <summary>
		/// Parses the given text by analyzing every character and building line by line results.
		/// </summary>
		[Obsolete("TODO: split up in several methods, this is not clean code")]
		internal List<List<char>> ParseText(string text)
		{
			var finalLines = new List<List<char>>();
			var currentLine = new List<char>();
			char[] textChars = text.ToCharArray();
			for (int charIndex = 0; charIndex < textChars.Length; charIndex++)
			{
				char textChar = textChars[charIndex];
				
				// Compute the index of the next char to detect a line break and when the text has ended
				int nextCharIndex = charIndex + 1;
				bool isLineBreak = false;
				// First check for new-lines, possible values are: Windows = \r\n, Unix = \n, Mac = \r
				if (textChar == '\n')
					isLineBreak = true;
				else if (textChar == '\r')
				{
					isLineBreak = true;

					// Also check for the second part of the Windows new-line
					if (nextCharIndex < textChars.Length && textChars[nextCharIndex] == '\n')
						charIndex++;
				}

					// Just convert a tab into 2 spaces
				else if (textChar == '\t')
				{
					currentLine.Add(' ');
					currentLine.Add(' ');
				}

					// Ignore all other special characters (EOT, BackSpace, etc.)
				else if (textChar >= ' ')
				{
					// Check now if the current text character is supported by the font so we can allow it
					if (glyphDictionary.ContainsKey(textChar))
						currentLine.Add(textChar);
					else if (glyphDictionary.ContainsKey(FallbackCharForUnsupportedCharacters))
						currentLine.Add(FallbackCharForUnsupportedCharacters);
				}

				// The current line is complete if a line break or the last character was reached
				if (isLineBreak || nextCharIndex == textChars.Length)
				{
					finalLines.Add(currentLine);
					currentLine = new List<char>();
				}
			}
			return finalLines;
		}

		/// <summary>
		/// Gets the single text lines of multi line text by determining the line breaks.
		/// </summary>
		[Obsolete("TODO: split up in several methods, this is not clean code")]
		internal List<List<char>> GetTextLines(string text, float lineSpacing, Size maxTextPixelSize,
			HorizontalAlignment textAlignment, bool isClippingOn, bool isWordWrapOn,
			out List<float> textlineWidths, out float maxTextlineWidth)
		{
			textlineWidths = new List<float>();
			maxTextlineWidth = 0.0f;
			var finalTextlines = new List<List<char>>();
			if (String.IsNullOrEmpty(text))
				return finalTextlines;

			// Compute now the total height which the font would need for every text line
			float maxLineHeight = PixelLineHeight * lineSpacing;

			// If text clipping is enabled check now if the available the height of given area is smaller
			// than a character of the font. Then no text line will fit into it and we can also stop here
			if (isClippingOn && maxTextPixelSize.Height < maxLineHeight)
				return finalTextlines;

			// Parse the given text now and get the text lines of it with all supported characters
			List<List<char>> textlines = ParseText(text);
			for (int lineIndex = 0; lineIndex < textlines.Count; lineIndex++)
			{
				List<char> textlineChars = textlines[lineIndex];
				var textline = new List<char>();
				float textlineWidth = 0.0f;

				// For word-wrap we need a container which collects the characters of the current word
				var word = new List<char>();
				float wordWidth = 0.0f;

				// Keep the number of words we have already parsed to detect if a single word exceeds the
				// max. allowed text width and we need to put it into the next text line separately.
				int wordNumber = 0;

				Glyph lastGlyph = null;
				for (int charId = 0; charId < textlineChars.Count; charId++)
				{
					// Get the current char of the current text line
					char textChar = textlineChars[charId];

					// We need the next character for kerning, bearing and word- or line-ending detection
					int nextCharId = charId + 1;
					char nextChar = nextCharId < textlineChars.Count ? textlineChars[nextCharId] : NoChar;

					// Get now the width of the character which it will need to draw it
					Glyph glyph = glyphDictionary[textChar];
					float glyphWidth = glyph.AdvanceWidth;
					int charKerning;
					if (lastGlyph != null &&
						lastGlyph.Kernings != null &&
						lastGlyph.Kernings.TryGetValue(textChar, out charKerning))
						glyphWidth += charKerning;

					// Jump to next full pixel after character. Important for small fonts to not look strange.
					glyphWidth = (float)Math.Round(glyphWidth);
					bool isLastChar = nextCharId == textlineChars.Count;

					// Unlike GetGlyphDrawAreaAndUVs above we do not care about positioning, just the width
					if (nextChar != NoChar && isLastChar && textAlignment == HorizontalAlignment.Right)
						glyphWidth += glyphDictionary[nextChar].RightSideBearing;

					// Word-wrap only makes sense with clipping otherwise there would be no need to wrap text
					if (isClippingOn && isWordWrapOn)
					{
						bool isSpace = textChar == ' ' || textChar == '\t';
						bool isWordFinished = false;
						if (isSpace)
						{
							// Add space character at the end of the word independend by the set text alignment
							word.Add(textChar);
							isWordFinished = true;
						}
						else if (isLastChar)
						{
							word.Add(textChar);
							wordWidth += glyphWidth;
							isWordFinished = true;
						}
						else
						{
							word.Add(textChar);
							wordWidth += glyphWidth;
						}

						if (isWordFinished)
						{
							// Check if the current text line has still enough space to add it. Ignore clipping
							// if even the first word of the current text line wouldn't fit into it to avoid
							// that the word would "push" the rest of the text down and out of the text area.
							if (textlineWidth + wordWidth < maxTextPixelSize.Width || wordNumber == 0)
							{
								// Update the parsed result of the current text line now.
								textline.AddRange(word);
								textlineWidth += wordWidth;

								// Also add the width of the space now that we have skipped.
								if (isSpace)
									textlineWidth += glyphWidth;

								wordNumber++;
							}
							else
							{
								// If the next word would not fit anymore we also grab the remaining text line part
								while (nextCharId < textlineChars.Count)
								{
									word.Add(textlineChars[nextCharId]);
									nextCharId++;
								}
								// and insert it as next text line (-> wrapping)
								textlines.Insert(lineIndex + 1, word);
								// Indicate to the character loop of the current text line we have reached the end
								charId = textlineChars.Count;
							}
							word = new List<char>();
							wordWidth = 0.0f;
						}
					}

						// If no clipping is required or in the case that (horizontal) clipping is enabled and we
						// check if the current character would still fit in the remaining space of the text line
						// (vertical clipping will be handled more below). Note: Also add 1 pixel to the right
						// border to make sure we don't round off the right side and the full text always fits
						// into the draw area.
					else if (isClippingOn == false ||
						textlineWidth + glyph.UV.Width <= maxTextPixelSize.Width + 1)
					{
						textline.Add(textChar);
						textlineWidth += glyphWidth;
					}
					else
						// Abort the parsing of this line and go to the next one if the space limit is reached
						break;

					lastGlyph = glyph;
				}

				// Every time a text line is parsed add it as final one, including the measured width of it
				finalTextlines.Add(textline);
				textlineWidths.Add(textlineWidth);

				// Additionally check if the current text line was the longest that we had so far
				if (maxTextlineWidth < textlineWidth)
					maxTextlineWidth = textlineWidth;

				// Check if there is still enough space for the next text line if vertical clipping is on
				if (!isClippingOn)
					continue;

				float requiredHeightForNextLine = (finalTextlines.Count + 1) * maxLineHeight;
				// If no more text line would fit (make sure to add a pixel for
				// conversion errors between pixel space and quadratic space)
				if (requiredHeightForNextLine > maxTextPixelSize.Height + 1)
					// Stop here completely and are done now with parsing of the text lines
					break;
			}
			return finalTextlines;
		}
	}
}