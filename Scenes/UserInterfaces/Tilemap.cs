using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A smoothly scrolling grid of colored images
	/// </summary>
	public abstract class Tilemap : Entity2D
	{
		protected Tilemap(Size world, Size map)
			: base(Rectangle.Zero)
		{
			Add(new Interact.State());
			Start<Interact, InteractWithKeyboard, ApplyInteractions>();
			Add(new Data(world, map));
			Start<Update>();
		}

		public class Data
		{
			public Data(Size world, Size map)
			{
				CreateWorld(world);
				CreateMap(map);
			}

			private void CreateWorld(Size world)
			{
				WorldWidth = (int)world.Width;
				WorldHeight = (int)world.Height;
				World = new Tile[WorldWidth,WorldHeight];
			}

			public int WorldWidth { get; private set; }
			public int WorldHeight { get; private set; }
			public Tile[,] World { get; private set; }

			private void CreateMap(Size map)
			{
				MapWidth = (int)map.Width;
				MapHeight = (int)map.Height;
				Map = new Sprite[MapWidth,MapHeight];
			}

			public int MapWidth { get; private set; }
			public int MapHeight { get; private set; }
			public Sprite[,] Map { get; private set; }
			public Point RenderingTopLeft = new Point(0.0001f, 0.0001f);
			public Point TargetTopLeft = Point.Zero;
		}

		private class ApplyInteractions : EventListener2D
		{
			public override void ReceiveMessage(Entity2D entity, object message)
			{
				var tilemap = entity as Tilemap;
				if (message is Interact.ControlDragged)
					DragTilemap(tilemap);

				var keyHeld = message as InteractWithKeyboard.KeyHeld;
				if (keyHeld != null)
					ScrollTilemap(tilemap, keyHeld.Key);
			}

			private static void DragTilemap(Entity2D tilemap)
			{
				var dragDelta = tilemap.Get<Interact.State>().DragDelta;
				var data = tilemap.Get<Data>();
				data.TargetTopLeft.X -= dragDelta.X * data.MapWidth / tilemap.DrawArea.Width;
				data.TargetTopLeft.Y -= dragDelta.Y * data.MapHeight / tilemap.DrawArea.Height;
				RestrictScrollingToWithinWorld(data);
			}

			private static void RestrictScrollingToWithinWorld(Data data)
			{
				if (data.TargetTopLeft.X < 0)
					data.TargetTopLeft.X = 0;

				if (data.TargetTopLeft.X > data.WorldWidth - data.MapWidth)
					data.TargetTopLeft.X = data.WorldWidth - data.MapWidth;

				if (data.TargetTopLeft.Y < 0)
					data.TargetTopLeft.Y = 0;

				if (data.TargetTopLeft.Y > data.WorldHeight - data.MapHeight)
					data.TargetTopLeft.Y = data.WorldHeight - data.MapHeight;
			}

			private static void ScrollTilemap(Entity tilemap, Key key)
			{
				var data = tilemap.Get<Data>();
				if (key == Key.CursorLeft)
					data.TargetTopLeft.X -= Time.Current.Delta * ScrollSpeed;

				if (key == Key.CursorRight)
					data.TargetTopLeft.X += Time.Current.Delta * ScrollSpeed;

				if (key == Key.CursorUp)
					data.TargetTopLeft.Y -= Time.Current.Delta * ScrollSpeed;

				if (key == Key.CursorDown)
					data.TargetTopLeft.Y += Time.Current.Delta * ScrollSpeed;

				RestrictScrollingToWithinWorld(data);
			}

			private const float ScrollSpeed = 4.0f;
		}

		private class Update : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				tilemap = entity as Tilemap;
				data = tilemap.Get<Data>();
				if (data.RenderingTopLeft == data.TargetTopLeft)
					return;

				UpdateTopLeft();
				UpdateMapSprites();
			}

			private Tilemap tilemap;
			private Data data;

			private void UpdateTopLeft()
			{
				float percentage = ScrollingStiffness * Time.Current.Delta;
				tilemap.Get<Data>().RenderingTopLeft =
					new Point(MathExtensions.Lerp(data.RenderingTopLeft.X, data.TargetTopLeft.X, percentage),
						MathExtensions.Lerp(data.RenderingTopLeft.Y, data.TargetTopLeft.Y, percentage));
			}

			private const float ScrollingStiffness = 2.0f;

			private void UpdateMapSprites()
			{
				offset = new Point(data.RenderingTopLeft.X % 1.0f, data.RenderingTopLeft.Y % 1.0f);
				tileWidth = tilemap.TileWidth;
				tileHeight = tilemap.TileHeight;
				mapTop = tilemap.DrawArea.Top;
				mapLeft = tilemap.DrawArea.Left;
				for (int x = 0; x < data.MapWidth; x++)
					for (int y = 0; y < data.MapHeight; y++)
						UpdateTileSprite(x, y);
			}

			private Point offset;
			private float tileWidth;
			private float tileHeight;
			private float mapTop;
			private float mapLeft;

			private void UpdateTileSprite(int tileX, int tileY)
			{
				var worldX = (int)data.RenderingTopLeft.X + tileX;
				var worldY = (int)data.RenderingTopLeft.Y + tileY;
				data.Map[tileX, tileY].Image = data.World[worldX, worldY].Image;
				data.Map[tileX, tileY].Color = data.World[worldX, worldY].Color;
				data.Map[tileX, tileY].DrawArea = new Rectangle(mapLeft + (tileX - offset.X) * tileWidth,
					mapTop + (tileY - offset.Y) * tileHeight, tileWidth, tileHeight);
			}
		}

		public float TileWidth
		{
			get { return DrawArea.Width / Get<Data>().MapWidth; }
		}

		public float TileHeight
		{
			get { return DrawArea.Height / Get<Data>().MapHeight; }
		}
	}
}