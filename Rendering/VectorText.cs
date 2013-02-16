using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Draws vector style text in 2D space
	/// </summary>
	public class VectorText : Renderable
	{
		public VectorText(string text, Point topLeft, float height)
		{
			TopLeft = topLeft;
			Text = text;
			Height = height;
		}

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

		private string text;
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
			int[] ascii = Text.ToUpper().Select(x => (int)x).ToArray();
			for (int i = 0; i < ascii.Count(); i++)
				CacheChar(ascii[i]);

			areCachedLinesOutOfDate = false;
		}

		private readonly List<Line2D> lines = new List<Line2D>();
		private Point caretPosition;

		private void CacheChar(int ascii)
		{
			caretPosition.X += 0.64f * Height;
			if (ascii >= 'A' && ascii <= 'Z')
				CacheAtoZChar(ascii);

			if (ascii >= '0' && ascii <= '9')
				Cache0To9Char(ascii);

			if (ascii == '.')
				CachePunctuation(ascii);
		}

		private void CacheAtoZChar(int ascii)
		{
			charLines = CharLineBank[ascii - 'A'];
			CacheCharLines();
		}

		private int[] charLines;

		private void CacheCharLines()
		{
			linePosition = 0;
			GetAndSaveNextPoint();
			while (charLines.Count() - linePosition > 1)
				lines.Add(new Line2D(lastPoint, GetAndSaveNextPoint(), Color.White));
		}

		private int linePosition;
		private Point lastPoint;

		private Point GetAndSaveNextPoint()
		{
			return
				lastPoint =
					new Point(caretPosition.X + 0.128f * Height * charLines[linePosition++],
						caretPosition.Y + 0.114285f * Height * charLines[linePosition++]);
		}

		private void Cache0To9Char(int ascii)
		{
			charLines = CharLineBank[26 + ascii - '0'];
			CacheCharLines();
		}

		private void CachePunctuation(int ascii)
		{
			charLines = CharLineBank[36 + ascii - '.'];
			CacheCharLines();
		}

		// This represents lines within a character via a 5x8 grid - with row 8 only used by Q
		// eg. 'A' is made of 7 lines: (0,6)->(0,1)->(1,0)->(3,0)->(4,1)->(4,6)->(4,3)->(0,3) 
		//  01234
		// 0 ...
		// 1.   .
		// 2.   .
		// 3.....
		// 4.   .
		// 5.   .
		// 6.   .
		// 7
		private static readonly int[][] CharLineBank = new[]
		{
			new[] { 0, 6, 0, 1, 1, 0, 3, 0, 4, 1, 4, 6, 4, 3, 0, 3 },                               //A
			new[] { 0, 3, 0, 0, 3, 0, 4, 1, 4, 2, 3, 3, 0, 3, 0, 6, 3, 6, 4, 5, 4, 4, 3, 3 },       //B
			new[] { 4, 0, 1, 0, 0, 1, 0, 5, 1, 6, 4, 6 },                                           //C
			new[] { 0, 6, 0, 0, 3, 0, 4, 1, 4, 5, 3, 6, 0, 6 },                                     //D
			new[] { 4, 0, 0, 0, 0, 3, 4, 3, 0, 3, 0, 6, 4, 6 },                                     //E
			new[] { 4, 0, 0, 0, 0, 3, 4, 3, 0, 3, 0, 6 },                                           //F
			new[] { 4, 0, 1, 0, 0, 1, 0, 5, 1, 6, 3, 6, 4, 5, 4, 3, 2, 3 },                         //G
			new[] { 0, 0, 0, 6, 0, 3, 4, 3, 4, 0, 4, 6 },                                           //H
			new[] { 1, 0, 3, 0, 2, 0, 2, 6, 1, 6, 3, 6 },                                           //I
			new[] { 4, 0, 4, 5, 3, 6, 1, 6, 0, 5 },                                                 //J
			new[] { 0, 0, 0, 6, 0, 3, 4, 0, 0, 3, 4, 6 },                                           //K
			new[] { 0, 0, 0, 6, 4, 6 },                                                             //L
			new[] { 0, 6, 0, 0, 2, 2, 4, 0, 4, 6 },                                                 //M
			new[] { 0, 6, 0, 0, 4, 6, 4, 0 },                                                       //N
			new[] { 1, 0, 3, 0, 4, 1, 4, 5, 3, 6, 1, 6, 0, 5, 0, 1, 1, 0 },                         //O
			new[] { 0, 6, 0, 0, 3, 0, 4, 1, 4, 2, 3, 3, 0, 3 },                                     //P
			new[] { 3, 6, 1, 6, 0, 5, 0, 1, 1, 0, 3, 0, 4, 1, 4, 5, 3, 6, 4, 7 },                   //Q
			new[] { 0, 6, 0, 0, 3, 0, 4, 1, 4, 2, 3, 3, 0, 3, 4, 6 },                               //R
			new[] { 0, 6, 3, 6, 4, 5, 4, 4, 3, 3, 1, 3, 0, 2, 0, 1, 1, 0, 4, 0 },                   //S
			new[] { 0, 0, 4, 0, 2, 0, 2, 6 },                                                       //T
			new[] { 0, 0, 0, 5, 1, 6, 3, 6, 4, 5, 4, 0 },                                           //U
			new[] { 0, 0, 0, 4, 2, 6, 4, 4, 4, 0 },                                                 //V
			new[] { 0, 0, 0, 5, 1, 6, 2, 5, 3, 6, 4, 5, 4, 0 },                                     //W
			new[] { 0, 0, 4, 6, 2, 3, 0, 6, 4, 0 },                                                 //X
			new[] { 0, 0, 2, 2, 4, 0, 2, 2, 2, 6 },                                                 //Y
			new[] { 0, 0, 4, 0, 0, 6, 4, 6 },                                                       //Z
			new[] { 1, 0, 3, 0, 4, 1, 4, 5, 3, 6, 1, 6, 0, 5, 0, 1, 1, 0 },                         //0
			new[] { 2, 0, 2, 6 },                                                                   //1
			new[] { 0, 1, 1, 0, 3, 0, 4, 1, 4, 2, 0, 6, 4, 6 },                                     //2
			new[] { 0, 1, 1, 0, 3, 0, 4, 1, 4, 2, 3, 3, 0, 3, 3, 3, 4, 4, 4, 5, 3, 6, 1, 6, 0, 5 }, //3
			new[] { 4, 3, 0, 3, 3, 0, 3, 6 },                                                       //4
			new[] { 4, 0, 0, 0, 0, 3, 3, 3, 4, 4, 4, 5, 3, 6, 0, 6 },                               //5
			new[] { 4, 0, 1, 0, 0, 1, 0, 5, 1, 6, 3, 6, 4, 5, 4, 4, 3, 3, 0, 3 },                   //6
			new[] { 0, 0, 4, 0, 0, 6 },                                                             //7
			new[] { 1, 0, 3, 0, 4, 1, 4, 2, 3, 3, 1, 3, 0, 4, 0, 5, 1, 6, 
				      3, 6, 4, 5, 4, 4, 3, 3, 1, 3, 0, 2, 0, 1, 1, 0 },                               //8
			new[] { 0, 6, 3, 6, 4, 5, 4, 1, 3, 0, 1, 0, 0, 1, 0, 2, 1, 3, 4, 3 },                   //9
			new[] { 2, 5, 3, 5, 3, 6, 2, 6, 2, 5 }                                                  //.
		};
	}
}