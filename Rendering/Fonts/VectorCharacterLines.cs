using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Fonts
{
	/// <summary>
	/// Points for the VectorText lines for each character supported.
	/// </summary>
	internal class VectorCharacterLines
	{
		public VectorCharacterLines()
		{
			AddLetters();
			AddNumbers();
			AddPoint();
		}

		private void AddLetters()
		{
			linePoints.Add('A',
				new[]
				{
					new Point(0, 0.68571f), new Point(0, 0.114285f), new Point(0, 0.114285f),
					new Point(0.128f, 0), new Point(0.128f, 0), new Point(0.384f, 0), new Point(0.384f, 0),
					new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f), new Point(0.512f, 0.68571f),
					new Point(0.512f, 0.68571f), new Point(0.512f, 0.342855f), new Point(0.512f, 0.342855f),
					new Point(0, 0.342855f)
				});
			linePoints.Add('B',
				new[]
				{
					new Point(0, 0.342855f), new Point(0, 0), new Point(0, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.22857f), new Point(0.512f, 0.22857f), new Point(0.384f, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0, 0.342855f), new Point(0, 0.342855f),
					new Point(0, 0.68571f), new Point(0, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.45714f), new Point(0.512f, 0.45714f), new Point(0.384f, 0.342855f)
				});
			linePoints.Add('C',
				new[]
				{
					new Point(0.512f, 0), new Point(0.128f, 0), new Point(0.128f, 0), new Point(0, 0.114285f),
					new Point(0, 0.114285f), new Point(0, 0.571425f), new Point(0, 0.571425f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.512f, 0.68571f)
				});
			linePoints.Add('D',
				new[]
				{
					new Point(0, 0.68571f), new Point(0, 0), new Point(0, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0, 0.68571f)
				});
			linePoints.Add('E',
				new[]
				{
					new Point(0.512f, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0.342855f),
					new Point(0, 0.342855f), new Point(0.512f, 0.342855f), new Point(0.512f, 0.342855f),
					new Point(0, 0.342855f), new Point(0, 0.342855f), new Point(0, 0.68571f),
					new Point(0, 0.68571f), new Point(0.512f, 0.68571f)
				});
			linePoints.Add('F',
				new[]
				{
					new Point(0.512f, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0.342855f),
					new Point(0, 0.342855f), new Point(0.512f, 0.342855f), new Point(0.512f, 0.342855f),
					new Point(0, 0.342855f), new Point(0, 0.342855f), new Point(0, 0.68571f)
				});
			linePoints.Add('G',
				new[]
				{
					new Point(0.512f, 0), new Point(0.128f, 0), new Point(0.128f, 0), new Point(0, 0.114285f),
					new Point(0, 0.114285f), new Point(0, 0.571425f), new Point(0, 0.571425f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.342855f), new Point(0.512f, 0.342855f), new Point(0.256f, 0.342855f)
				});
			linePoints.Add('H',
				new[]
				{
					new Point(0, 0), new Point(0, 0.68571f), new Point(0, 0.68571f), new Point(0, 0.342855f),
					new Point(0, 0.342855f), new Point(0.512f, 0.342855f), new Point(0.512f, 0.342855f),
					new Point(0.512f, 0), new Point(0.512f, 0), new Point(0.512f, 0.68571f)
				});
			linePoints.Add('I',
				new[]
				{
					new Point(0.128f, 0), new Point(0.384f, 0), new Point(0.384f, 0), new Point(0.256f, 0),
					new Point(0.256f, 0), new Point(0.256f, 0.68571f), new Point(0.256f, 0.68571f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.384f, 0.68571f)
				});
			linePoints.Add('J',
				new[]
				{
					new Point(0.512f, 0), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f), new Point(0.128f, 0.68571f),
					new Point(0.128f, 0.68571f), new Point(0, 0.571425f)
				});
			linePoints.Add('K',
				new[]
				{
					new Point(0, 0), new Point(0, 0.68571f), new Point(0, 0.68571f), new Point(0, 0.342855f),
					new Point(0, 0.342855f), new Point(0.512f, 0), new Point(0.512f, 0),
					new Point(0, 0.342855f), new Point(0, 0.342855f), new Point(0.512f, 0.68571f)
				});
			linePoints.Add('L',
				new[]
				{
					new Point(0, 0), new Point(0, 0.68571f), new Point(0, 0.68571f),
					new Point(0.512f, 0.68571f)
				});
			linePoints.Add('M',
				new[]
				{
					new Point(0, 0.68571f), new Point(0, 0), new Point(0, 0), new Point(0.256f, 0.22857f),
					new Point(0.256f, 0.22857f), new Point(0.512f, 0), new Point(0.512f, 0),
					new Point(0.512f, 0.68571f)
				});
			linePoints.Add('N',
				new[]
				{
					new Point(0, 0.68571f), new Point(0, 0), new Point(0, 0), new Point(0.512f, 0.68571f),
					new Point(0.512f, 0.68571f), new Point(0.512f, 0)
				});
			linePoints.Add('O',
				new[]
				{
					new Point(0.128f, 0), new Point(0.384f, 0), new Point(0.384f, 0),
					new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.571425f), new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0, 0.571425f),
					new Point(0, 0.571425f), new Point(0, 0.114285f), new Point(0, 0.114285f),
					new Point(0.128f, 0)
				});
			linePoints.Add('P',
				new[]
				{
					new Point(0, 0.68571f), new Point(0, 0), new Point(0, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.22857f), new Point(0.512f, 0.22857f), new Point(0.384f, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0, 0.342855f)
				});
			linePoints.Add('Q',
				new[]
				{
					new Point(0.384f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f),
					new Point(0, 0.571425f), new Point(0, 0.571425f), new Point(0, 0.114285f),
					new Point(0, 0.114285f), new Point(0.128f, 0), new Point(0.128f, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0.512f, 0.799995f)
				});
			linePoints.Add('R',
				new[]
				{
					new Point(0, 0.68571f), new Point(0, 0), new Point(0, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.22857f), new Point(0.512f, 0.22857f), new Point(0.384f, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0, 0.342855f), new Point(0, 0.342855f),
					new Point(0.512f, 0.68571f)
				});
			linePoints.Add('S',
				new[]
				{
					new Point(0, 0.68571f), new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.45714f),
					new Point(0.512f, 0.45714f), new Point(0.384f, 0.342855f), new Point(0.384f, 0.342855f),
					new Point(0.128f, 0.342855f), new Point(0.128f, 0.342855f), new Point(0, 0.22857f),
					new Point(0, 0.22857f), new Point(0, 0.114285f), new Point(0, 0.114285f),
					new Point(0.128f, 0), new Point(0.128f, 0), new Point(0.512f, 0)
				});
			linePoints.Add('T',
				new[]
				{
					new Point(0, 0), new Point(0.512f, 0), new Point(0.512f, 0), new Point(0.256f, 0),
					new Point(0.256f, 0), new Point(0.256f, 0.68571f)
				});
			linePoints.Add('U',
				new[]
				{
					new Point(0, 0), new Point(0, 0.571425f), new Point(0, 0.571425f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0)
				});
			linePoints.Add('V',
				new[]
				{
					new Point(0, 0), new Point(0, 0.45714f), new Point(0, 0.45714f),
					new Point(0.256f, 0.68571f), new Point(0.256f, 0.68571f), new Point(0.512f, 0.45714f),
					new Point(0.512f, 0.45714f), new Point(0.512f, 0)
				});
			linePoints.Add('W',
				new[]
				{
					new Point(0, 0), new Point(0, 0.571425f), new Point(0, 0.571425f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.256f, 0.571425f),
					new Point(0.256f, 0.571425f), new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f), new Point(0.512f, 0)
				});
			linePoints.Add('X',
				new[]
				{
					new Point(0, 0), new Point(0.512f, 0.68571f), new Point(0.512f, 0.68571f),
					new Point(0.256f, 0.342855f), new Point(0.256f, 0.342855f), new Point(0, 0.68571f),
					new Point(0, 0.68571f), new Point(0.512f, 0)
				});
			linePoints.Add('Y',
				new[]
				{
					new Point(0, 0), new Point(0.256f, 0.22857f), new Point(0.256f, 0.22857f),
					new Point(0.512f, 0), new Point(0.512f, 0), new Point(0.256f, 0.22857f),
					new Point(0.256f, 0.22857f), new Point(0.256f, 0.68571f)
				});
			linePoints.Add('Z',
				new[]
				{
					new Point(0, 0), new Point(0.512f, 0), new Point(0.512f, 0), new Point(0, 0.68571f),
					new Point(0, 0.68571f), new Point(0.512f, 0.68571f)
				});
		}

		private readonly Dictionary<char, Point[]> linePoints = new Dictionary<char, Point[]>();

		private void AddNumbers()
		{
			linePoints.Add('0',
				new[]
				{
					new Point(0.128f, 0), new Point(0.384f, 0), new Point(0.384f, 0),
					new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.571425f), new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0, 0.571425f),
					new Point(0, 0.571425f), new Point(0, 0.114285f), new Point(0, 0.114285f),
					new Point(0.128f, 0)
				});
			linePoints.Add('1', new[] { new Point(0.256f, 0), new Point(0.256f, 0.68571f) });
			linePoints.Add('2',
				new[]
				{
					new Point(0, 0.114285f), new Point(0.128f, 0), new Point(0.128f, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.22857f), new Point(0.512f, 0.22857f), new Point(0, 0.68571f),
					new Point(0, 0.68571f), new Point(0.512f, 0.68571f)
				});
			linePoints.Add('3',
				new[]
				{
					new Point(0, 0.114285f), new Point(0.128f, 0), new Point(0.128f, 0), new Point(0.384f, 0),
					new Point(0.384f, 0), new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.22857f), new Point(0.512f, 0.22857f), new Point(0.384f, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0, 0.342855f), new Point(0, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0.384f, 0.342855f), new Point(0.512f, 0.45714f),
					new Point(0.512f, 0.45714f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f), new Point(0.128f, 0.68571f),
					new Point(0.128f, 0.68571f), new Point(0, 0.571425f)
				});
			linePoints.Add('4',
				new[]
				{
					new Point(0.512f, 0.342855f), new Point(0, 0.342855f), new Point(0, 0.342855f),
					new Point(0.384f, 0), new Point(0.384f, 0), new Point(0.384f, 0.68571f)
				});
			linePoints.Add('5',
				new[]
				{
					new Point(0.512f, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0.342855f),
					new Point(0, 0.342855f), new Point(0.384f, 0.342855f), new Point(0.384f, 0.342855f),
					new Point(0.512f, 0.45714f), new Point(0.512f, 0.45714f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.571425f), new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0, 0.68571f)
				});
			linePoints.Add('6',
				new[]
				{
					new Point(0.512f, 0), new Point(0.128f, 0), new Point(0.128f, 0), new Point(0, 0.114285f),
					new Point(0, 0.114285f), new Point(0, 0.571425f), new Point(0, 0.571425f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.45714f), new Point(0.512f, 0.45714f), new Point(0.384f, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0, 0.342855f)
				});
			linePoints.Add('7',
				new[]
				{ new Point(0, 0), new Point(0.512f, 0), new Point(0.512f, 0), new Point(0, 0.68571f) });
			linePoints.Add('8',
				new[]
				{
					new Point(0.128f, 0), new Point(0.384f, 0), new Point(0.384f, 0),
					new Point(0.512f, 0.114285f), new Point(0.512f, 0.114285f), new Point(0.512f, 0.22857f),
					new Point(0.512f, 0.22857f), new Point(0.384f, 0.342855f), new Point(0.384f, 0.342855f),
					new Point(0.128f, 0.342855f), new Point(0.128f, 0.342855f), new Point(0, 0.45714f),
					new Point(0, 0.45714f), new Point(0, 0.571425f), new Point(0, 0.571425f),
					new Point(0.128f, 0.68571f), new Point(0.128f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.384f, 0.68571f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f),
					new Point(0.512f, 0.45714f), new Point(0.512f, 0.45714f), new Point(0.384f, 0.342855f),
					new Point(0.384f, 0.342855f), new Point(0.128f, 0.342855f), new Point(0.128f, 0.342855f),
					new Point(0, 0.22857f), new Point(0, 0.22857f), new Point(0, 0.114285f),
					new Point(0, 0.114285f), new Point(0.128f, 0)
				});
			linePoints.Add('9',
				new[]
				{
					new Point(0, 0.68571f), new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f),
					new Point(0.512f, 0.571425f), new Point(0.512f, 0.571425f), new Point(0.512f, 0.114285f),
					new Point(0.512f, 0.114285f), new Point(0.384f, 0), new Point(0.384f, 0),
					new Point(0.128f, 0), new Point(0.128f, 0), new Point(0, 0.114285f),
					new Point(0, 0.114285f), new Point(0, 0.22857f), new Point(0, 0.22857f),
					new Point(0.128f, 0.342855f), new Point(0.128f, 0.342855f), new Point(0.512f, 0.342855f)
				});
		}

		private void AddPoint()
		{
			linePoints.Add('.',
				new[]
				{
					new Point(0.256f, 0.571425f), new Point(0.384f, 0.571425f), new Point(0.384f, 0.571425f),
					new Point(0.384f, 0.68571f), new Point(0.384f, 0.68571f), new Point(0.256f, 0.68571f),
					new Point(0.256f, 0.68571f), new Point(0.256f, 0.571425f)
				});
		}

		public Point[] GetPoints(char c)
		{
			c = char.ToUpperInvariant(c);
			Point[] points;
			return linePoints.TryGetValue(c, out points) ? points : new Point[0];
		}
	}
}