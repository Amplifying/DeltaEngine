using System;
using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws vector style text in 2D space
	/// </summary>
	public class VectorText : Renderable
	{
		public VectorText(XmlContent vectorTextContent, Point topLeft, float height)
			: this(vectorTextContent.XmlData, topLeft, height) {}

		public VectorText(XmlData vectorTextData, Point topLeft, float height)
		{
			charData = vectorTextData.Children;
			TopLeft = topLeft;
			Height = height;
		}

		private readonly List<XmlData> charData;

		public Point TopLeft;

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				areCachedLinesOutOfDate = true;
			}
		}

		private string text = "";
		private bool areCachedLinesOutOfDate;

		public float Height
		{
			get { return height; }
			set
			{
				height = value;
				areCachedLinesOutOfDate = true;
			}
		}

		private float height;

		protected override void Render(Renderer renderer, Time time)
		{
			if (areCachedLinesOutOfDate)
				CacheLines();

			foreach (Line2D line in lines)
				renderer.DrawLine(TopLeft + line.StartPosition, TopLeft + line.EndPosition, Color);
		}

		public Color Color = Color.White;

		private void CacheLines()
		{
			lines.Clear();
			caretPosition = - new Point(0.64f * Height, 0.0f);
			foreach (char c in Text)
				CacheChar(c);

			areCachedLinesOutOfDate = false;
		}

		private readonly List<Line2D> lines = new List<Line2D>();
		private Point caretPosition;

		private void CacheChar(char c)
		{
			caretPosition.X += 0.64f * Height;
			if (char.IsWhiteSpace(c))
				return;

			IEnumerable<string> charLines = GetCharLines(c);
			foreach (string line in charLines)
				CacheLine(line);
		}

		private IEnumerable<string> GetCharLines(char c)
		{
			foreach (XmlData data in charData)
				if (data.GetValue("Character") == c.ToString(CultureInfo.InvariantCulture).ToUpper())
					return data.GetValue("Lines").Split(';');

			throw new VectorCharacterNotFoundException();
		}

		public class VectorCharacterNotFoundException : Exception {}

		private void CacheLine(string line)
		{
			var start = caretPosition + new Point(line.Split('-')[0]) * Height;
			var finish = caretPosition + new Point(line.Split('-')[1]) * Height;
			lines.Add(new Line2D(start, finish, Color.White));
		}
	}
}