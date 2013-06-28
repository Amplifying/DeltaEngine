using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Grid
	/// </summary>
	public class GridTests : TestWithMocksOrVisually
	{
		public void Initialize(ScreenSpace screen)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f ? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
			var controller = new Controller(displayMode, content);
			grid = controller.Get<Grid>();
			fixedRandomScope = Randomizer.Use(new FixedRandom());
		}

		private Orientation displayMode;
		private IDisposable fixedRandomScope;
		private JewelBlocksContent content;
		private Grid grid;

		[Test]
		public void AffixBlocksWhichFillOneRow()
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.AreEqual(0,
				AffixBlocks(grid, new[] { new Point(0, 18), new Point(4, 18), new Point(7, 18) }));
			using (Randomizer.Use(new FixedRandom(IBlock)))
			{
				Assert.AreEqual(1, grid.AffixBlock(new Block(displayMode, content, new Point(11, 15))));
				Assert.AreEqual(3, ControllerTests.CountBricks(grid));
				Assert.IsNotNull(grid.bricks[11, 16]);
				Assert.IsNotNull(grid.bricks[11, 17]);
				Assert.IsNotNull(grid.bricks[11, 18]);
			}
		}

		private static readonly float[] IBlock = new[]
		{ 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f };

		private int AffixBlocks(Grid gameGrid, IEnumerable<Point> points)
		{
			return points.Sum(point => gameGrid.AffixBlock(new Block(displayMode, content, point)));
		}

		[Test]
		public void AffixBlocksWhichFillTwoRows()
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.AreEqual(0,
				AffixBlocks(grid,
					new[]
					{
						new Point(0, 17), new Point(4, 17), new Point(7, 17), new Point(0, 18), new Point(4, 18)
						, new Point(7, 18)
					}));
			using (Randomizer.Use(new FixedRandom(IBlock)))
			{
				Assert.AreEqual(2, grid.AffixBlock(new Block(displayMode, content, new Point(11, 15))));
				Assert.AreEqual(2, ControllerTests.CountBricks(grid));
				Assert.IsNotNull(grid.bricks[11, 17]);
				Assert.IsNotNull(grid.bricks[11, 18]);
			}
		}

		[Test]
		public void RowsDontSplit()
		{
			Initialize(Resolve<ScreenSpace>());
			content.DoBricksSplitInHalfWhenRowFull = false;
			Assert.AreEqual(0,
				AffixBlocks(grid, new[] { new Point(0, 18), new Point(4, 18), new Point(7, 18) }));
			using (Randomizer.Use(new FixedRandom(IBlock)))
			{
				Assert.AreEqual(1, grid.AffixBlock(new Block(displayMode, content, new Point(11, 15))));
				EntitySystem.Current.Run();
				//TODO: Don't understand why blocks are not in Render
				//Assert.AreEqual(30, entitySystem.GetHandler<Render>().NumberOfActiveRenderableObjects);
			}
		}

		[Test]
		public void RowsSplit()
		{
			Initialize(Resolve<ScreenSpace>());
			content.DoBricksSplitInHalfWhenRowFull = true;
			Assert.AreEqual(0,
				AffixBlocks(grid, new[] { new Point(0, 18), new Point(4, 18), new Point(7, 18) }));
			using (Randomizer.Use(new FixedRandom(IBlock)))
			{
				Assert.AreEqual(1, grid.AffixBlock(new Block(displayMode, content, new Point(11, 15))));
				EntitySystem.Current.Run();
				//TODO: Don't understand why blocks are not in Render
				//Assert.AreEqual(42, entitySystem.GetHandler<Render>().NumberOfActiveRenderableObjects);
			}
		}

		[Test]
		public void IsValidPositionInEmptyGrid()
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Point(-1, 1))));
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Point(9, 1))));
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Point(0, 0))));
			using (Randomizer.Use(new FixedRandom(IBlock)))
				Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Point(0, 17))));

			Assert.IsTrue(grid.IsValidPosition(new Block(displayMode, content, new Point(2, 2))));
		}

		[Test]
		public void IsValidPositionInOccupiedGrid()
		{
			Initialize(Resolve<ScreenSpace>());
			grid.AffixBlock(new Block(displayMode, content, new Point(5, 1)));
			Assert.IsFalse(grid.IsValidPosition(new Block(displayMode, content, new Point(3, 0))));
			Assert.IsTrue(grid.IsValidPosition(new Block(displayMode, content, new Point(5, 2))));
		}

		[Test]
		public void IsABrickOnFirstRow()
		{
			Initialize(Resolve<ScreenSpace>());
			Assert.IsFalse(grid.IsABrickOnFirstRow());
			grid.AffixBlock(new Block(displayMode, content, new Point(1, 1)));
			Assert.IsFalse(grid.IsABrickOnFirstRow());
			grid.AffixBlock(new Block(displayMode, content, new Point(2, 0)));
			Assert.IsTrue(grid.IsABrickOnFirstRow());
		}

		[Test]
		public void Clear()
		{
			Initialize(Resolve<ScreenSpace>());
			using (Randomizer.Use(new FixedRandom(new[] { 0.8f, 0.0f })))
			{
				grid.AffixBlock(new Block(displayMode, content, Point.One));
				Assert.AreEqual(4, ControllerTests.CountBricks(grid));
				EntitySystem.Current.Run();
				grid.Clear();
				Assert.AreEqual(0, ControllerTests.CountBricks(grid));
			}
		}

		[Test]
		public void GetValidStartingColumns()
		{
			Initialize(Resolve<ScreenSpace>());
			content.DoBlocksStartInARandomColumn = true;
			var block = new Block(displayMode, content, Point.Zero);
			Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, grid.GetValidStartingColumns(block));
			grid.AffixBlock(new Block(displayMode, content, new Point(1, 1)));
			Assert.AreEqual(new[] { 5, 6, 7, 8 }, grid.GetValidStartingColumns(block));
		}

		[Test]
		public void GetSingleValidStartingColumn()
		{
			Initialize(Resolve<ScreenSpace>());
			content.DoBlocksStartInARandomColumn = false;
			var block = new Block(displayMode, content, Point.Zero);
			Assert.AreEqual(new[] { 4 }, grid.GetValidStartingColumns(block));
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			if (fixedRandomScope != null)
				fixedRandomScope.Dispose();
		}
	}
}