using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.UserInterfaces;

namespace Blocks
{
	/// <summary>
	/// Renders the static elements (background, grid, score) in portrait plus keeps track of and 
	/// renders the player score.
	/// </summary>
	public class UserInterfacePortrait : Scene
	{
		public UserInterfacePortrait(Renderer renderer, BlocksContent content)
		{
			this.content = content;
			window = renderer.Screen.Window;
			AddBackground();
			AddGrid();
			AddScoreWindow();
			AddText();
		}

		private readonly BlocksContent content;
		private readonly Window window;

		private void AddBackground()
		{
			var image = content.Load<Image>("Background");
			var background = new Label(image, Rectangle.One) { RenderLayer = Background };
			Add(background);
		}

		private const int Background = (int)RenderLayer.Background;

		private void AddGrid()
		{
			var image = content.Load<Image>("Grid");
			var grid = new Label(image, GetGridDrawArea()) { RenderLayer = Background };
			window.ViewportSizeChanged += size => grid.DrawArea = GetGridDrawArea();
			Add(grid);
		}

		private static Rectangle GetGridDrawArea()
		{
			var left = Brick.OffsetPortrait.X + GridRenderLeftOffset;
			var top = Brick.OffsetPortrait.Y - Brick.ZoomPortrait + GridRenderTopOffset;
			return new Rectangle(left, top, Width, Height);
		}

		private const float GridRenderLeftOffset = -0.009f;
		private const float GridRenderTopOffset = -0.009f;
		private const float GridRenderWidthOffset = 0.019f;
		private const float Width = Grid.Width * Brick.ZoomPortrait + GridRenderWidthOffset;
		private const float Height = (Grid.Height + 1) * Brick.ZoomPortrait + GridRenderHeightOffset;
		private const float GridRenderHeightOffset = 0.018f;

		private void AddScoreWindow()
		{
			var image = content.Load<Image>("ScoreWindow");
			var scoreWindow = new Label(image, GetScoreWindowDrawArea(image))
			{
				RenderLayer = Background
			};
			window.ViewportSizeChanged += size => scoreWindow.DrawArea = GetScoreWindowDrawArea(image);
			Add(scoreWindow);
		}

		private static Rectangle GetScoreWindowDrawArea(Image image)
		{
			var left = Brick.OffsetPortrait.X + GridRenderLeftOffset;
			var top = Brick.OffsetPortrait.Y - Brick.ZoomPortrait + ScoreRenderTopOffset;
			var height = Width / image.PixelSize.AspectRatio;
			return new Rectangle(left, top, Width, height);
		}

		private const float ScoreRenderTopOffset = -0.135f;

		private void AddText()
		{
			var vectorText = content.Load<XmlContent>("VectorText");
			AddMessage(vectorText);
			AddScoreboard(vectorText);
		}

		private void AddMessage(XmlContent vectorText)
		{
			message = new VectorTextControl(vectorText, GetMessageTopLeft(), GetMessageHeight())
			{
				RenderLayer = Foreground,
				Text = "Welcome"
			};
			window.ViewportSizeChanged += size => message.TopLeft = GetMessageTopLeft();
			window.ViewportSizeChanged += size => message.Height = GetMessageHeight();
			Add(message);
		}

		private VectorTextControl message;
		private const int Foreground = (int)RenderLayer.Foreground;

		private static Point GetMessageTopLeft()
		{
			return TextLine1;
		}

		private static readonly Point TextLine1 = new Point(0.395f, 0.25f);

		private static float GetMessageHeight()
		{
			return TextHeight;
		}

		private const float TextHeight = 0.02f;

		private void AddScoreboard(XmlContent vectorText)
		{
			scoreboard = new VectorTextControl(vectorText, GetScoreboardTopLeft(), GetScoreboardHeight())
			{
				RenderLayer = Foreground
			};
			window.ViewportSizeChanged += size => scoreboard.TopLeft = GetScoreboardTopLeft();
			window.ViewportSizeChanged += size => scoreboard.Height = GetScoreboardHeight();
			Add(scoreboard);
		}

		private VectorTextControl scoreboard;

		private static Point GetScoreboardTopLeft()
		{
			return TextLine2;
		}

		private static readonly Point TextLine2 = new Point(0.395f, 0.27f);

		private static float GetScoreboardHeight()
		{
			return TextHeight;
		}

		public void AddToScore(int points)
		{
			message.Text = "";
			score += points;
			scoreboard.Text = "Score " + score;
			scoreboard.TopLeft = TextLine1;
		}

		private int score;

		public void Lose()
		{
			message.Text = "Game Over";
			scoreboard.TopLeft = TextLine2;
			score = 0;
		}
	}
}