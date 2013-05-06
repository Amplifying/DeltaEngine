#if TODO
using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TileMapTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateTileMapWithDefaultTileType()
		{
			var tileMap = new TileMap(content.Load<Image>("DeltaEngineLogo"), 1, 2);
			Assert.AreEqual(1, tileMap.GridWidth);
			Assert.AreEqual(2, tileMap.GridHeight);
			Assert.AreEqual(TileMap.DefaultTileWidth, tileMap.TileWidth);
			Assert.AreEqual(TileMap.DefaultTileHeight, tileMap.TileHeight);
		}

		[Test]
		public void CreateTileMapFilledWithSpecificTileType()
		{
			var tileMap = new TileMap(2, 2, "NewType");
			Assert.AreEqual("NewType", tileMap.Tiles[0, 0].Type);
			Assert.AreEqual("NewType", tileMap.Tiles[1, 1].Type);
		}

		[Test]
		public void Tiles()
		{
			const int Columns = 5;
			const int Rows = 10;
			var tileMap = new TileMap(Columns, Rows);
			Assert.AreEqual(Columns, tileMap.Tiles.GetLength(0));
			Assert.AreEqual(Rows, tileMap.Tiles.GetLength(1));
			for (int column = 0; column < Columns; column++)
				for (int row = 0; row < Rows; row++)
					Assert.IsTrue(tileMap.Tiles[column, row].Column == column &&
						tileMap.Tiles[column, row].Row == row);
		}

		[Test]
		public void GridWidth()
		{
			var tileMap = new TileMap(1, 1) { GridWidth = 2 };
			Assert.AreEqual(2, tileMap.GridWidth);
		}

		[Test]
		public void GridHeight()
		{
			var tileMap = new TileMap(1, 1) { GridHeight = 2 };
			Assert.AreEqual(2, tileMap.GridHeight);
		}

		[Test]
		public void ToggleVisibility()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			Assert.IsTrue(tileMap.Visibility);
			tileMap.Visibility = false;
			Assert.IsFalse(tileMap.Visibility);
			tileMap.Visibility = true;
			Assert.IsTrue(tileMap.Visibility);
		}

		private static TileMap CreateDeltaEngineLogoTileMap()
		{
			var tileMap = new TileMap(10, 10)
			{
				DrawArea = new Rectangle(0.2f, 0.3f, 0.4f, 0.5f),
				TopLeftTile = new Point(1, 2)
			};

			foreach (Tile tile in tileMap.Tiles)
				tile.Type = "DeltaEngineLogo";

			return tileMap;
		}

		[Test]
		public void TileIsNotVisibleIfTileMapIsNot()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.Visibility = false;
			foreach (Tile tile in tileMap.Tiles)
				Assert.AreEqual(Visibility.Hide, tile.Sprite.Visibility);
		}

		[Test]
		public void IfTileMapVisibilityOnlyTilesWithinDrawAreaAreVisible()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[0, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[1, 1].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[1, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[4, 6].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[4, 7].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[5, 6].Sprite.Visibility);
		}

		[Test]
		public void TileWidth()
		{
			var tileMap = new TileMap(1, 1) { TileWidth = 2 };
			Assert.AreEqual(2, tileMap.TileWidth);
		}

		[Test]
		public void IncreasingTileWidthMakesLessTilesVisible()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.TileWidth = 0.2f;
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[0, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[1, 1].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[1, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[2, 6].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[2, 7].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[3, 6].Sprite.Visibility);
		}

		[Test]
		public void TileHeight()
		{
			var tileMap = new TileMap(1, 1) { TileHeight = 2 };
			Assert.AreEqual(2, tileMap.TileHeight);
		}

		[Test]
		public void IncreasingTileHeightMakesLessTilesVisible()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.TileHeight = 0.25f;
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[0, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[1, 1].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[1, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[4, 3].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[4, 4].Sprite.Visibility);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[5, 3].Sprite.Visibility);
		}

		[Test]
		public void TopLeftTile()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.TopLeftTile = new Point(1, 2);
			Assert.AreEqual(new Point(1, 2), tileMap.TopLeftTile);
		}

		[Test]
		public void DrawArea()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.DrawArea = new Rectangle(0.2f, 0.3f, 0.4f, 0.5f);
			Assert.AreEqual(new Rectangle(0.2f, 0.3f, 0.4f, 0.5f), tileMap.DrawArea);
		}

		[Test]
		public void TileDrawArea()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			Assert.AreEqual(new Rectangle(0.2f, 0.3f, 0.1f, 0.1f), tileMap.Tiles[1, 2].Sprite.DrawArea);
			Assert.AreEqual(new Rectangle(0.3f, 0.5f, 0.1f, 0.1f), tileMap.Tiles[2, 4].Sprite.DrawArea);
		}

		[Test]
		public void ChangingTopLeftTileChangesTileDrawArea()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.TopLeftTile = new Point(1, 3);
			Assert.AreEqual(new Rectangle(0.2f, 0.2f, 0.1f, 0.1f), tileMap.Tiles[1, 2].Sprite.DrawArea);
			Assert.AreEqual(new Rectangle(0.3f, 0.4f, 0.1f, 0.1f), tileMap.Tiles[2, 4].Sprite.DrawArea);
		}

		[Test]
		public void ChangingTopLeftTileChangesTileVisibility()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.TopLeftTile = new Point(1, 3);
			Assert.AreEqual(Visibility.Hide, tileMap.Tiles[1, 2].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[4, 6].Sprite.Visibility);
			Assert.AreEqual(Visibility.Show, tileMap.Tiles[4, 7].Sprite.Visibility);
		}

		[Test]
		public void Contains()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			Assert.IsFalse(tileMap.Contains(new Point(0.15f, 0.3f)));
			Assert.IsTrue(tileMap.Contains(new Point(0.2f, 0.3f)));
			Assert.IsTrue(tileMap.Contains(new Point(0.59f, 0.79f)));
			Assert.IsFalse(tileMap.Contains(new Point(0.59f, 0.85f)));
		}

		[Test]
		public void TappingTopLeft()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.Tapped += position => Assert.AreEqual(Point.Zero, position);
			tileMap.Tap(new Point(0.2f, 0.3f));
		}

		[Test]
		public void TappingBottomRight()
		{
			var tileMap = CreateDeltaEngineLogoTileMap();
			tileMap.Tapped += position => Assert.AreEqual(Point.One, position);
			tileMap.Tap(new Point(0.6f, 0.8f));
		}

		[Test]
		public void RenderLayer()
		{
			var tileMap = new TileMap(10, 10);
			Assert.AreEqual(Entity2D.DefaultRenderLayer, tileMap.RenderLayer);
			tileMap.RenderLayer = 10;
			Assert.AreEqual(10, tileMap.RenderLayer);
		}

		[Test]
		public void ShowAndHide()
		{
			Start(typeof(MockResolver),
				(EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
				{
					using (var scene = new Scene())
					{
						scene.Add(CreateDeltaEngineLogoTileMap());
						scene.Show(entitySystem, content, input);
						entitySystem.Run();
						var renderer = entitySystem.GetHandler<SortAndRenderEntity2D>();
						Assert.AreEqual(100, renderer.NumberOfActiveRenderableObjects);
						scene.Hide();
						entitySystem.Run();
						Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
					}
				});
		}

		[VisualTest]
		public void Show4X5DeltaEngineLogoTileMap(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var scene = new Scene();
				scene.Add(CreateDeltaEngineLogoTileMap());
				scene.Show(entitySystem, content, input);
			});
		}
	}
}
#endif