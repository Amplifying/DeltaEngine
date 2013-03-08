using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Renders the static elements (background, grid, score) plus keeps track of and renders 
	/// the player score.
	/// </summary>
	public class UserInterface
	{
		public UserInterface(BlocksContent content, Renderer renderer)
		{
			this.renderer = renderer;
			this.content = content;
			AddBackground();
			AddGrid();
			AddScoreWindow();
			AddText();
			if (!ExceptionExtensions.IsReleaseMode)
				AddGridOutline();
		}

		private readonly Renderer renderer;
		private readonly Content content;

		private void AddBackground()
		{
			var image = content.Load<Image>("Background");
			var background = new Sprite(image, renderer.Screen.Viewport) { RenderLayer = Background };
			renderer.Add(background);
			renderer.Screen.ViewportSizeChanged += () => background.DrawArea = renderer.Screen.Viewport;
		}

		private const byte Background = (byte)RenderLayer.Background;

		private void AddGrid()
		{
			var image = content.Load<Image>("Grid");
			var left = Brick.RenderOffset.X + GridRenderLeftOffset;
			var top = Brick.RenderOffset.Y - Brick.RenderZoom + GridRenderTopOffset;
			var width = Grid.Width * Brick.RenderZoom + GridRenderWidthOffset;
			var height = (Grid.Height + 1) * Brick.RenderZoom + GridRenderHeightOffset;
			var drawArea = new Rectangle(left, top, width, height);
			renderer.Add(new Sprite(image, drawArea) { RenderLayer = (byte)RenderLayer.Background });
		}

		private const float GridRenderLeftOffset = -0.009f;
		private const float GridRenderTopOffset = -0.009f;
		private const float GridRenderWidthOffset = 0.019f;
		private const float GridRenderHeightOffset = 0.018f;

		private void AddGridOutline()
		{
			var topLeft = Brick.RenderOffset - Point.UnitY * Brick.RenderZoom;
			var topRight = Brick.RenderOffset + new Point(Grid.Width, -1) * Brick.RenderZoom;
			var bottomLeft = Brick.RenderOffset + new Point(0, Grid.Height) * Brick.RenderZoom;
			var bottomRight = Brick.RenderOffset + new Point(Grid.Width, Grid.Height) * Brick.RenderZoom;
			renderer.Add(new Line2D(topLeft, topRight, Color.Yellow) { RenderLayer = Foreground });
			renderer.Add(new Line2D(topRight, bottomRight, Color.Yellow) { RenderLayer = Foreground });
			renderer.Add(new Line2D(bottomRight, bottomLeft, Color.Yellow) { RenderLayer = Foreground });
			renderer.Add(new Line2D(bottomLeft, topLeft, Color.Yellow) { RenderLayer = Foreground });
		}

		private const byte Foreground = (byte)RenderLayer.Foreground;

		private void AddScoreWindow()
		{
			var image = content.Load<Image>("ScoreWindow");
			var left = Brick.RenderOffset.X + GridRenderLeftOffset;
			var top = Brick.RenderOffset.Y - Brick.RenderZoom + ScoreRenderTopOffset;
			var width = Grid.Width * Brick.RenderZoom + GridRenderWidthOffset;
			var height = width / image.PixelSize.AspectRatio;
			var drawArea = new Rectangle(left, top, width, height);
			renderer.Add(new Sprite(image, drawArea) { RenderLayer = (byte)RenderLayer.Background });
		}

		private const float ScoreRenderTopOffset = -0.135f;

		private void AddText()
		{
			var vectorTextContent = content.Load<XmlContent>("VectorText");
			Message = new VectorText(vectorTextContent, TextLine1, TextHeight)
			{
				Text = "Welcome",
				RenderLayer = Foreground
			};

			Scoreboard = new VectorText(vectorTextContent, TextLine2, TextHeight)
			{
				RenderLayer = Foreground
			};

			renderer.Add(Message);
			renderer.Add(Scoreboard);
		}

		public VectorText Message { get; private set; }
		public VectorText Scoreboard { get; private set; }
		private static readonly Point TextLine1 = new Point(0.395f, 0.25f);
		private static readonly Point TextLine2 = new Point(0.395f, 0.27f);
		private const float TextHeight = 0.02f;

		public void AddToScore(int points)
		{
			Message.Text = "";
			Score += points;
			Scoreboard.Text = "Score " + Score;
			Scoreboard.TopLeft = TextLine1;
		}

		public int Score { get; private set; }

		public void Lose()
		{
			Message.Text = "You lost";
			Scoreboard.TopLeft = TextLine2;
			Score = 0;
		}
	}
}