using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	/// <summary>
	/// A simple tilemap with colored deltaengine logos
	/// </summary>
	public class ColoredLogoTilemap : Tilemap
	{
		public ColoredLogoTilemap(ContentLoader content, Size world, Size map)
			: base(world, map)
		{
			var data = Get<Data>();
			var logo = content.Load<Image>("DeltaEngineLogo");
			CreateWorld(data, logo);
			CreateMap(data, logo);
		}

		private static void CreateWorld(Data data, Image logo)
		{
			for (int x = 0; x < data.WorldWidth; x++)
				for (int y = 0; y < data.WorldHeight; y++)
					data.World[x, y] = new RainbowTile(logo,
						new Color(Rainbow(x, data.WorldWidth), Rainbow(y, data.WorldHeight),
							Rainbow(x + y, data.WorldWidth)));
		}

		private static float Rainbow(int value, int max)
		{
			return ((8.0f * value) % max) / max;
		}

		private static void CreateMap(Data data, Image logo)
		{
			for (int x = 0; x < data.MapWidth; x++)
				for (int y = 0; y < data.MapHeight; y++)
					data.Map[x, y] = new Sprite(logo);
		}

		private class RainbowTile : Tile
		{
			public RainbowTile(Image image, Color color)
			{
				Image = image;
				Color = color;
			}

			public Image Image { get; set; }
			public Color Color { get; set; }
		}
	}
}