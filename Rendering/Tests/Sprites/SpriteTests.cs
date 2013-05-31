using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	public class SpriteTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateSprite()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, screenCenter);
				Assert.AreEqual(Color.White, sprite.Color);
				Assert.AreEqual(image, sprite.Image);
				Assert.AreEqual(new Size(128, 128), sprite.Image.PixelSize);
			});
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[Test]
		public void SpriteWithNoImageThrowsException()
		{
			Assert.Throws<NullReferenceException>(() => new Sprite(null, Rectangle.Zero));
		}

		[Test]
		public void ChangeImage()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var image1 = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image1, screenCenter, Color.White);
				Assert.AreEqual(image1, sprite.Image);
				var image2 = content.Load<Image>("ImageAnimation01");
				sprite.Image = image2;
				Assert.AreEqual(image2, sprite.Image);
			});
		}

		[VisualTest]
		public void RenderSprite(Type resolver)
		{
			Start(resolver,
				(ContentLoader content) =>
				{ new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White); });
		}

		[VisualTest]
		public void RenderRedSpriteOverBlue(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				new Sprite(image, screenCenter, Color.Red) { RenderLayer = 1 };
				new Sprite(image, screenTopLeft, Color.Blue) { RenderLayer = 0 };
			});
		}

		private readonly Rectangle screenTopLeft = Rectangle.FromCenter(new Point(0.3f, 0.3f),
			Size.Half);

		[VisualTest]
		public void RenderSpriteAndLines(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new Line2D(Point.Zero, Point.One, Color.Blue);
				var image = content.Load<Image>("DeltaEngineLogo");
				new Sprite(image, screenCenter, Color.Red);
				new Line2D(Point.UnitX, Point.UnitY, Color.Purple);
			});
		}

		[VisualTest]
		public void RenderSmoothlyScrollingTileMap(Type resolver)
		{
			Start(resolver, (ContentLoader content, InputCommands input) =>
			{
				new FontText(new Font(content, "Verdana12"),
					"Hold down cursor keys or drag mouse to scroll", new Point(0.3f, 0.75f));
				logo = content.Load<Image>("DeltaEngineLogo");
				CreateWorld();
				CreateMap();
				CreateBorder();
				RespondToInput(input);
			}, ScrollMap);
		}

		private Image logo;

		private void CreateWorld()
		{
			for (int x = 0; x < WorldWidth; x++)
				for (int y = 0; y < WorldHeight; y++)
					world[x, y] = new Color(Rainbow(x, WorldWidth), Rainbow(y, WorldHeight),
						Rainbow(x + y, WorldWidth));
		}

		private readonly Color[,] world = new Color[WorldWidth,WorldHeight];
		private const int WorldWidth = 30;
		private const int WorldHeight = 30;

		private static float Rainbow(int value, int max)
		{
			return ((2.0f * value) % max) / max;
		}

		private void CreateMap()
		{
			for (int x = 0; x < MapWidth; x++)
				for (int y = 0; y < MapHeight; y++)
					map[x, y] = new Sprite(logo);

			UpdateMapSprites();
		}

		private readonly Sprite[,] map = new Sprite[MapWidth,MapHeight];
		private const int MapWidth = 10;
		private const int MapHeight = 10;
		private const float MapLeft = 0.1f;
		private const float MapTop = 0.3f;
		private const float TileWidth = 0.04f;
		private const float TileHeight = 0.04f;

		private void UpdateMapSprites()
		{
			var offset = new Point(renderingTopLeft.X % 1.0f, renderingTopLeft.Y % 1.0f);
			for (int x = 0; x < MapWidth; x++)
				for (int y = 0; y < MapHeight; y++)
					UpdateMapSprite(x, y, offset);
		}

		private Point renderingTopLeft;

		private void UpdateMapSprite(int x, int y, Point offset)
		{
			map[x, y].Color = world[(int)renderingTopLeft.X + x, (int)renderingTopLeft.Y + y];
			map[x, y].DrawArea = new Rectangle(MapLeft + (x - offset.X) * TileWidth,
				MapTop + (y - offset.Y) * TileHeight, TileWidth, TileHeight);
		}

		private static void CreateBorder()
		{
			new Rect(new Rectangle(MapLeft - TileWidth, MapTop - TileHeight, BorderWidth, TileHeight),
				Color.Gray);
			new Rect(new Rectangle(MapLeft - TileWidth, MapTop - TileHeight, TileWidth, BorderHeight),
				Color.Gray);
			new Rect(
				new Rectangle(MapLeft - TileWidth, MapTop + (MapHeight - 1) * TileHeight, BorderWidth,
					TileHeight), Color.Gray);
			new Rect(
				new Rectangle(MapLeft + (MapWidth - 1) * TileWidth, MapTop - TileHeight, TileWidth,
					BorderHeight), Color.Gray);
		}

		private const float BorderWidth = (MapWidth + 1) * TileWidth;
		private const float BorderHeight = (MapHeight + 1) * TileHeight;

		private void RespondToInput(InputCommands input)
		{
			input.Add(MouseButton.Left, State.Pressing, BeginScrollingByDragging);
			input.Add(MouseButton.Left, State.Pressed, ContinueScrollingByDragging);
			input.Add(Key.CursorLeft, State.Pressed, key => ScrollLeft());
			input.Add(Key.CursorRight, State.Pressed, key => ScrollRight());
			input.Add(Key.CursorUp, State.Pressed, key => ScrollUp());
			input.Add(Key.CursorDown, State.Pressed, key => ScrollDown());
		}

		private void BeginScrollingByDragging(Mouse mouse)
		{
			lastMousePosition = mouse.Position;
		}

		private Point lastMousePosition;

		private void ContinueScrollingByDragging(Mouse mouse)
		{
			desiredTopLeft.X += (lastMousePosition.X - mouse.Position.X) / TileWidth;
			desiredTopLeft.Y += (lastMousePosition.Y - mouse.Position.Y) / TileHeight;
			lastMousePosition = mouse.Position;
			RestrictScrollingToWithinWorld();
		}

		private Point desiredTopLeft;

		private void RestrictScrollingToWithinWorld()
		{
			if (desiredTopLeft.X < 0)
				desiredTopLeft.X = 0;

			if (desiredTopLeft.X > WorldWidth - MapWidth)
				desiredTopLeft.X = WorldWidth - MapWidth;

			if (desiredTopLeft.Y < 0)
				desiredTopLeft.Y = 0;

			if (desiredTopLeft.Y > WorldHeight - MapHeight)
				desiredTopLeft.Y = WorldHeight - MapHeight;
		}

		private void ScrollLeft()
		{
			desiredTopLeft.X -= Time.Current.Delta * ScrollSpeed;
			RestrictScrollingToWithinWorld();
		}

		private const float ScrollSpeed = 4.0f;

		private void ScrollRight()
		{
			desiredTopLeft.X += Time.Current.Delta * ScrollSpeed;
			RestrictScrollingToWithinWorld();
		}

		private void ScrollUp()
		{
			desiredTopLeft.Y -= Time.Current.Delta * ScrollSpeed;
			RestrictScrollingToWithinWorld();
		}

		private void ScrollDown()
		{
			desiredTopLeft.Y += Time.Current.Delta * ScrollSpeed;
			RestrictScrollingToWithinWorld();
		}

		private void ScrollMap()
		{
			if (renderingTopLeft == desiredTopLeft)
				return;

			renderingTopLeft.X = MathExtensions.Lerp(renderingTopLeft.X, desiredTopLeft.X,
				Time.Current.Delta);
			renderingTopLeft.Y = MathExtensions.Lerp(renderingTopLeft.Y, desiredTopLeft.Y,
				Time.Current.Delta);
			UpdateMapSprites();
		}
	}
}