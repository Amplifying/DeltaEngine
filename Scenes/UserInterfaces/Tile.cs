using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Used within a tilemap
	/// </summary>
	public interface Tile
	{
		Image Image { get; set; }
		Color Color { get; set; }
	}
}