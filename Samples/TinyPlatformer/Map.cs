using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Content.Json;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace TinyPlatformer
{
	public class Map
	{
		/// <summary>
		/// Parses the level and set the object entities.
		/// </summary>
		public Map()
		{
			Parse(ContentLoader.Load<JsonContent>(MapContentName).Data);
			for (int y = 0; y < Size.Height; y++)
				for (int x = 0; x < Size.Width; x++)
					new FilledRect(new Rectangle(
						Constants.ScreenGap.Width + Constants.BlockSize * x, 
						Constants.ScreenGap.Height + Constants.BlockSize * y,
						Constants.BlockSize, Constants.BlockSize),
						GetColor(Blocks[x, y]));
		}

		private static Color GetColor(BlockType blockType)
		{
			switch (blockType)
			{
			case BlockType.Gold:
				return Color.Gold;
			case BlockType.GroundBrick:
				return Color.Orange;
			case BlockType.PlatformBrick:
				return Color.Red;
			case BlockType.PlatformTop:
				return Color.Purple;
			case BlockType.LevelBorder:
				return Color.Teal;
			default:
				return Color.TransparentBlack;
			}
		}

		private const string MapContentName = "Level";

		private void Parse(JsonNode root)
		{
			ParseStaticBlocks(root);
			ParseEntityBlocks(root);
		}

		private void ParseEntityBlocks(JsonNode root)
		{
			var entityArray = root["layers"][1]["objects"];
			for (int entity = 0; entity < entityArray.NumberOfNodes; entity++)
			{
				var entityData = entityArray[entity];
				var properties = entityData["properties"];
				float maxdx = Constants.MaxXSpeed;
				bool left = false, right = false;
				try { maxdx = properties.Get<float>("maxdx"); }	catch { }
				try { left = properties.Get<bool>("left"); }		catch { }
				try { right = properties.Get<bool>("right"); }	catch { }

				var entityActor = new Actor(Blocks)
				{
					Type = entityData.Get<string>("type"),
					position = new Point(entityData.Get<int>("x"), entityData.Get<int>("y")),
					maxVelocityX = Constants.Meter*maxdx,
					Left = left,
					Right = right
				};
				ActorList.Add(entityActor);
			}
		}

		private void ParseStaticBlocks(JsonNode root)
		{
			var width = root.Get<int>("width");
			var height = root.Get<int>("height");
			Size = new Size(width, height);
			Blocks = new BlockType[width, height];
			var blocksData = root["layers"][0]["data"].GetIntArray();
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					Blocks[x, y] = (BlockType) blocksData[x + y*width];
		}
		public Size Size { get; private set; }
		public BlockType[,] Blocks;

		public List<Actor> ActorList = new List<Actor>();
		public Actor Player { get { return ActorList.Find(actor => actor.Type.Equals("player")); } 
		}
	}
}