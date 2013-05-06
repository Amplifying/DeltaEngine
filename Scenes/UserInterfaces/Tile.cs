using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Manages a tile within a TileMap
	/// </summary>
	public class Tile : Label
	{
		public Tile(Image image, int column, int row)
			: base(image, Rectangle.Zero)
		{
			Column = column;
			Row = row;
		}

		public int Column { get; private set; }
		public int Row { get; private set; }

		public Point TileCoord
		{
			get { return new Point(Column, Row); }
		}
	}
}