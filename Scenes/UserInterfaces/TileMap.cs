using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Manages and renders a grid of tiles as part of a Scene
	/// </summary>
	public class TileMap : InteractiveControl
	{
		public TileMap(Image defaultTileImage, int gridWidth, int gridHeight)
		{
			GridWidth = gridWidth;
			GridHeight = gridHeight;
			CreateTiles(defaultTileImage);
		}

		public int GridWidth { get; set; }
		public int GridHeight { get; set; }

		private void CreateTiles(Image defaultTileImage)
		{
			Tiles = new Tile[GridWidth,GridHeight];
			for (int row = 0; row < GridHeight; row++)
				for (int column = 0; column < GridWidth; column++)
					CreateTile(defaultTileImage, column, row);
		}

		public Tile[,] Tiles { get; private set; }

		private void CreateTile(Image image, int column, int row)
		{
			Tiles[column, row] = new Tile(image, column, row);
			SetTileDrawAreaAndVisibility(Tiles[column, row]);
		}

		private void SetTileDrawAreaAndVisibility(Tile tile)
		{
			tile.Sprite.DrawArea = GetTileDrawArea(tile.TileCoord);
			tile.Sprite.Visibility = drawArea.Contains(tile.Sprite.DrawArea.Center)
				? Visibility.Show : Visibility.Hide;
		}

		private Rectangle drawArea = Rectangle.Zero;
		private Point topLeftTile = Point.Zero;

		public Rectangle GetTileDrawArea(Point tileCoord)
		{
			var tileTopLeft = new Point(drawArea.Left + TileWidth * (tileCoord.X - topLeftTile.X),
				drawArea.Top + TileHeight * (tileCoord.Y - topLeftTile.Y));
			return new Rectangle(tileTopLeft, tileSize);
		}

		private TileMap() {}

		public Visibility Visibility
		{
			get { return Visibility; }
			set
			{
				Visibility = value;
				foreach (Tile tile in Tiles)
					tile.Sprite.Visibility = Visibility == Visibility.Show &&
						drawArea.Contains(tile.Sprite.DrawArea.Center) ? Visibility.Show : Visibility.Hide;
			}
		}

		//private bool Visibility = true;

		public Point TopLeftTile
		{
			get { return topLeftTile; }
			set
			{
				topLeftTile = value;
				foreach (Tile tile in Tiles)
					SetTileDrawAreaAndVisibility(tile);
			}
		}

		public float TileWidth
		{
			get { return tileWidth; }
			set
			{
				tileWidth = value;
				tileSize = new Size(tileWidth, tileHeight);
				foreach (Tile tile in Tiles)
					SetTileDrawAreaAndVisibility(tile);
			}
		}

		private float tileWidth = DefaultTileWidth;
		public const float DefaultTileWidth = 0.1f;
		private Size tileSize = new Size(DefaultTileWidth, DefaultTileHeight);
		public const float DefaultTileHeight = 0.1f;

		public float TileHeight
		{
			get { return tileHeight; }
			set
			{
				tileHeight = value;
				tileSize = new Size(tileWidth, tileHeight);
				foreach (Tile tile in Tiles)
					SetTileDrawAreaAndVisibility(tile);
			}
		}

		private float tileHeight = DefaultTileHeight;

		public Rectangle DrawArea
		{
			get { return drawArea; }
			set
			{
				drawArea = value;
				foreach (Tile tile in Tiles)
					SetTileDrawAreaAndVisibility(tile);
			}
		}

		internal override bool Contains(Point point)
		{
			return drawArea.Contains(point);
		}

		public override void Tap(Point position)
		{
			var tileCoord = new Point((position.X - drawArea.TopLeft.X) / drawArea.Width,
				(position.Y - drawArea.TopLeft.Y) / drawArea.Height);
			base.Tap(tileCoord);
		}

		public int RenderLayer
		{
			get { return Tiles[0, 0].Sprite.RenderLayer; }
			set
			{
				foreach (Tile tile in Tiles)
					tile.Sprite.RenderLayer = value;
			}
		}

		internal override void Show()
		{
			foreach (Tile tile in Tiles)
				tile.Show();
		}

		internal override void Hide()
		{
			foreach (Tile tile in Tiles)
				tile.Hide();
		}

		public override void Dispose()
		{
			foreach (Tile tile in Tiles)
				tile.Dispose();
		}
	}
}