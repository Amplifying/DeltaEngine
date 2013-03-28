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
	/// Renders the static elements (background, grid, score) in landscape plus keeps track of and 
	/// renders the player score.
	/// </summary>
	public class UserInterfaceLandscape : Scene
	{
		public UserInterfaceLandscape(Renderer renderer, BlocksContent content)
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
			var left = Brick.OffsetLandscape.X + GridRenderLeftOffset;
			var top = Brick.OffsetLandscape.Y - Brick.ZoomLandscape + GridRenderTopOffset;
			var width = Grid.Width * Brick.ZoomLandscape + GridRenderWidthOffset;
			var height = (Grid.Height + 1) * Brick.ZoomLandscape + GridRenderHeightOffset;
			return new Rectangle(left, top, width, height);
		}

		private const float GridRenderLeftOffset = -0.009f;
		private const float GridRenderTopOffset = -0.009f;
		private const float GridRenderWidthOffset = 0.019f;
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
			var left = Brick.OffsetLandscape.X + GridRenderLeftOffset;
			var top = Brick.OffsetLandscape.Y - Brick.ZoomLandscape + ScoreRenderTopOffset;
			var width = Grid.Width * Brick.ZoomLandscape + GridRenderWidthOffset;
			var height = width / image.PixelSize.AspectRatio;
			return new Rectangle(left, top, width, height);
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
			Message = new VectorTextControl(vectorText, GetMessageTopLeft(), GetMessageHeight())
			{
				RenderLayer = Foreground,
				Text = "Welcome"
			};
			window.ViewportSizeChanged += size => Message.TopLeft = GetMessageTopLeft();
			window.ViewportSizeChanged += size => Message.Height = GetMessageHeight();
			Add(Message);
		}

		public VectorTextControl Message { get; private set; }
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
			Scoreboard = new VectorTextControl(vectorText, GetScoreboardTopLeft(), GetScoreboardHeight())
			{
				RenderLayer = Foreground
			};
			window.ViewportSizeChanged += size => Scoreboard.TopLeft = GetScoreboardTopLeft();
			window.ViewportSizeChanged += size => Scoreboard.Height = GetScoreboardHeight();
			Add(Scoreboard);
		}

		public VectorTextControl Scoreboard { get; private set; }

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
			Message.Text = "";
			Score += points;
			Scoreboard.Text = "Score " + Score;
			Scoreboard.TopLeft = TextLine1;
		}

		public int Score { get; private set; }

		public void Lose()
		{
			Message.Text = "Game Over";
			Scoreboard.TopLeft = TextLine2;
			Score = 0;
		}
	}
}