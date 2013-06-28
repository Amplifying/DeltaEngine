using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath
{
	/// <summary>
	/// Displays and handles the player score; Each kill adds one to score and two to money
	/// </summary>
	public class Scoreboard : Entity2D
	{
		public Scoreboard(ScreenSpace screen)
			: base(Rectangle.Zero)
		{
			Add(new Score(screen) { Money = InitialMoney });
		}

		public static readonly Size QuadraticFullscreenSize = new Size(1920, 1920);
		private const int InitialMoney = 15;

		public int Money
		{
			get { return Get<Score>().Money; }
			set { Get<Score>().Money = value; }
		}

		public int Kills
		{
			get { return Get<Score>().Kills; }
			set { Get<Score>().Kills = value; }
		}

		/// <summary>
		/// Holds the amount of money and number of kills a player has
		/// </summary>
		public class Score
		{
			public Score(ScreenSpace screen)
			{
				CreateText(screen);
				CreateDeadRabbit(screen);
			}

			private void CreateText(ScreenSpace screen)
			{
				var font = new Font("Tahoma30");
				moneyText = new FontText(font, "$0", screen.TopLeft + MoneyOffset)
				{
					RenderLayer = int.MaxValue,
					Color = Color.Black
				};

				killText = new FontText(font, "0", screen.Viewport.TopRight + KillOffset)
				{
					RenderLayer = int.MaxValue,
					Color = Color.Black
				};
			}

			private FontText moneyText;
			private FontText killText;
			private static readonly Point MoneyOffset = new Point(0.1f, 0.08f);
			private static readonly Point KillOffset = new Point(-0.09f, 0.08f);

			private static void CreateDeadRabbit(ScreenSpace screen)
			{
				new Sprite(ContentLoader.Load<Image>("DeadRabbit"),
					new Rectangle(screen.Viewport.TopRight + DeadRabbitOffset, DeadRabbitSize))
				{
					RenderLayer = int.MaxValue
				};
			}

			private static readonly Point DeadRabbitOffset = new Point(-0.175f, 0.002f);
			private static readonly Size DeadRabbitSize = new Size(0.05f, 0.0667f);

			public int Money
			{
				get { return money; }
				set
				{
					money = value;
					moneyText.Text = "$" + money;
				}
			}

			private int money;

			public int Kills
			{
				get { return kills; }
				set
				{
					kills = value;
					killText.Text = value.ToString(CultureInfo.InvariantCulture);
				}
			}

			private int kills;
		}
	}
}