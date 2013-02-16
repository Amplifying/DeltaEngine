using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// Displays and handles the player score, each kill = score gives 2 money.
	/// </summary>
	public class Score : Sprite
	{
		public Score(Content content, ScreenSpace screen)
			: base(null, Rectangle.Zero)
		{
			this.screen = screen;
			RenderLayer = UIRenderLayer;
			for (int digit = 0; digit < 10; digit++)
				images[digit] = content.Load<Image>(digit.ToString(CultureInfo.InvariantCulture));
			images[DollarSign] = content.Load<Image>("Dollar");
			images[RabbitKillSign] = content.Load<Image>("DeadRabbit");
			digitWidth = images[0].PixelSize.Width / QuadraticFullscreenSize.Width;
		}

		public static readonly Size QuadraticFullscreenSize = new Size(1920, 1920);

		private readonly ScreenSpace screen;
		private const int DollarSign = 10;
		private const int RabbitKillSign = 11;
		private readonly Image[] images = new Image[12];
		private readonly float digitWidth;

		internal void DrawDiggit(Point position, int digit)
		{
			var digitImage = images[digit.Clamp(0, RabbitKillSign)];
			var screenSize = digitImage.PixelSize / QuadraticFullscreenSize;
			var drawArea = Rectangle.FromCenter(FixPrefixOffset(position, digit), screenSize);
			digitImage.Draw(GetVertices(drawArea));
		}

		private static Point FixPrefixOffset(Point position, int digit)
		{
			if (digit == RabbitKillSign)
				position += new Point(-0.013f, 0);
			return position;
		}

		private VertexPositionColorTextured[] GetVertices(Rectangle drawArea)
		{
			return new[]
			{
				GetVertex(drawArea.TopLeft, Point.Zero),
				GetVertex(drawArea.TopRight, Point.UnitX),
				GetVertex(drawArea.BottomRight, Point.One),
				GetVertex(drawArea.BottomLeft, Point.UnitY)
			};
		}

		private VertexPositionColorTextured GetVertex(Point position, Point uv)
		{
			return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position), Color.White, uv);
		}

		public void ShowDollars(Point position, int dollars)
		{
			DrawDigits(DollarSign, position, dollars);
		}

		private void DrawDigits(int prefixDigit, Point position, int dollars)
		{
			var digits = GetDigits(prefixDigit, dollars);
			for (int index = 0; index < digits.Count; index++)
				DrawDiggit(position + DigitOffset(digits.Count, index), digits[index]);
		}

		private Point DigitOffset(int numberOfDigits, int index)
		{
			return new Point((0.5f + index - numberOfDigits / 2.0f) * digitWidth, 0);
		}

		private static List<int> GetDigits(int prefixDigit, int dollars)
		{
			var digits = new List<int> { prefixDigit };
			var letters = dollars.ToString(CultureInfo.InvariantCulture).ToCharArray();
			digits.AddRange(letters.Select(letter => letter - '0'));
			return digits;
		}

		public void ShowKills(Point position, int kills)
		{
			DrawDigits(RabbitKillSign, position, kills);
		}

		protected override void Render(Renderer renderer, Time time)
		{
			ShowDollars(renderer.Screen.TopLeft + new Point(0.1f, 0.042f), CurrentMoney);
			ShowKills(renderer.Screen.Viewport.TopRight + new Point(-0.09f, 0.042f), CurrentKills);
		}

		public int CurrentMoney = InitialMoney;
		private const int InitialMoney = 10;
		public int CurrentKills = 0;
	}
}