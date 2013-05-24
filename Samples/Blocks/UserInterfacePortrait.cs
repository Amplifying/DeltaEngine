using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes;

namespace Blocks
{
	/// <summary>
	/// Renders the static elements (background, grid, score) in portrait plus keeps track of and 
	/// renders the player score.
	/// </summary>
	public class UserInterfacePortrait : Scene
	{
		public UserInterfacePortrait(BlocksContent content)
		{
			this.content = content;
			AddBackground();
			AddGrid();
			AddScoreWindow();
		}

		private readonly BlocksContent content;

		private void AddBackground()
		{
			var image = content.Load<Image>("Background");
			Add(new Sprite(image, Rectangle.One) { RenderLayer = Background });
		}

		private const int Background = (int)RenderLayer.Background;

		private void AddGrid()
		{
			var image = content.Load<Image>("Grid");
			grid = new Sprite(image, GetGridDrawArea()) { RenderLayer = Background };
			Add(grid);
		}

		private Sprite grid;

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
			scoreWindow = new Sprite(image, GetScoreWindowDrawArea(image));
			scoreWindow.RenderLayer = Background;
			Add(scoreWindow);
		}

		private Sprite scoreWindow;

		private static Rectangle GetScoreWindowDrawArea(Image image)
		{
			var left = Brick.OffsetPortrait.X + GridRenderLeftOffset;
			var top = Brick.OffsetPortrait.Y - Brick.ZoomPortrait + ScoreRenderTopOffset;
			var height = Width / image.PixelSize.AspectRatio;
			return new Rectangle(left, top, Width, height);
		}

		private const float ScoreRenderTopOffset = -0.135f;
	}
}