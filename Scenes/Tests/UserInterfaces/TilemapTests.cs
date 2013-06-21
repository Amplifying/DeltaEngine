using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TilemapTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderColoredLogoTilemap(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new ColoredLogoTilemap(content, World, Map) { DrawArea = Center };
				CreateBorder();
			});
		}

		private static readonly Size World = new Size(100, 100);
		private static readonly Size Map = new Size(10, 10);
		private static readonly Rectangle Center = new Rectangle(Left, Top, Width, Height);
		private const float Left = 0.3f;
		private const float Top = 0.3f;
		private const float Width = 0.4f;
		private const float Height = 0.4f;

		private static void CreateBorder()
		{
			new Rect(new Rectangle(Left - TileWidth, Top - TileHeight, BorderWidth, TileHeight),
				Color.Black);
			new Rect(new Rectangle(Left - TileWidth, Top - TileHeight, TileWidth, BorderHeight),
				Color.Black);
			new Rect(
				new Rectangle(Left - TileWidth, Top + (Map.Height - 1) * TileHeight, BorderWidth,
					TileHeight), Color.Black);
			new Rect(
				new Rectangle(Left + (Map.Width - 1) * TileWidth, Top - TileHeight, TileWidth, BorderHeight),
				Color.Black);
		}

		private static readonly float TileWidth = Center.Width / Map.Width;
		private static readonly float TileHeight = Center.Height / Map.Height;
		private static readonly float BorderWidth = (Map.Width + 1) * TileWidth;
		private static readonly float BorderHeight = (Map.Height + 1) * TileHeight;
	}
}