using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Grid
	/// </summary>
	public class GridTests : TestStarter
	{
		[IntegrationTest]
		public void Constructor(Type resolver)
		{
			Start(resolver, (Grid grid) => Assert.IsNotNull(grid.Random));
		}

		[IntegrationTest]
		public void AffixBlocksWhichFillOneRow(Type resolver)
		{
			Start(resolver, (TestGrid grid, BlocksContent content) =>
			{
				Assert.AreEqual(0,
					AffixBlocks(grid, content, new[] { new Point(0, 18), new Point(4, 18), new Point(7, 18) }));
				Assert.AreEqual(1, grid.AffixBlock(new Block(content, VerticalIBlock, new Point(11, 15))));
				Assert.AreEqual(3, grid.BrickCount);
				Assert.IsNotNull(grid.Bricks[11, 16]);
				Assert.IsNotNull(grid.Bricks[11, 17]);
				Assert.IsNotNull(grid.Bricks[11, 18]);
			});
		}

		private static readonly FixedRandom VerticalIBlock =
			new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f });

		private static int AffixBlocks(Grid grid, BlocksContent content, IEnumerable<Point> points)
		{
			return points.Sum(point => grid.AffixBlock(new Block(content, new FixedRandom(), point)));
		}

		[IntegrationTest]
		public void AffixBlocksWhichFillTwoRows(Type resolver)
		{
			Start(resolver, (TestGrid grid, BlocksContent content) =>
			{
				Assert.AreEqual(0, AffixBlocks(grid, content, 
					new[] { new Point(0, 17), new Point(4, 17), new Point(7, 17), new Point(0, 18),
						      new Point(4, 18), new Point(7, 18) }));
				Assert.AreEqual(2, grid.AffixBlock(new Block(content, VerticalIBlock, new Point(11, 15))));
				Assert.AreEqual(2, grid.BrickCount);
				Assert.IsNotNull(grid.Bricks[11, 17]);
				Assert.IsNotNull(grid.Bricks[11, 18]);
			});
		}

		[IntegrationTest]
		public void RowsDontSplit(Type resolver)
		{
			Start(resolver, (TestGrid grid, BlocksContent content, Renderer renderer) =>
			{
				content.DoBricksSplitInHalfWhenRowFull = false;
				Assert.AreEqual(0,
					AffixBlocks(grid, content, new[] { new Point(0, 18), new Point(4, 18), new Point(7, 18) }));
				Assert.AreEqual(1, grid.AffixBlock(new Block(content, VerticalIBlock, new Point(11, 15))));
				Assert.AreEqual(30, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void RowsSplit(Type resolver)
		{
			Start(resolver, (TestGrid grid, BlocksContent content, Renderer renderer) =>
			{
				content.DoBricksSplitInHalfWhenRowFull = true;
				Assert.AreEqual(0,
					AffixBlocks(grid, content, new[] { new Point(0, 18), new Point(4, 18), new Point(7, 18) }));
				Assert.AreEqual(1, grid.AffixBlock(new Block(content, VerticalIBlock, new Point(11, 15))));
				Assert.AreEqual(42, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void IsValidPositionInEmptyGrid(Type resolver)
		{
			Start(resolver, (Grid grid, BlocksContent content) =>
			{
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(-1, 1))));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(9, 1))));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(0, 0))));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, VerticalIBlock, new Point(0, 17))));
				Assert.IsTrue(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(2, 2))));
			});
		}

		[IntegrationTest]
		public void IsValidPositionInOccupiedGrid(Type resolver)
		{
			Start(resolver, (Grid grid, BlocksContent content) =>
			{
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(5, 1)));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(3, 0))));
				Assert.IsTrue(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(5, 2))));
			});
		}

		[IntegrationTest]
		public void IsABrickOnFirstRow(Type resolver)
		{
			Start(resolver, (Grid grid, BlocksContent content) =>
			{
				Assert.IsFalse(grid.IsABrickOnFirstRow());
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 1)));
				Assert.IsFalse(grid.IsABrickOnFirstRow());
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(2, 0)));
				Assert.IsTrue(grid.IsABrickOnFirstRow());
			});
		}

		[IntegrationTest]
		public void Clear(Type resolver)
		{
			Start(resolver, (TestGrid grid, Renderer renderer, BlocksContent content) =>
			{
				grid.AffixBlock(new Block(content, new FixedRandom(new[] { 0.8f, 0.0f }), Point.One));
				Assert.AreEqual(4, grid.BrickCount);
				Assert.AreEqual(8, renderer.NumberOfActiveRenderableObjects);
				grid.Clear();
				Assert.AreEqual(0, grid.BrickCount);
				Assert.AreEqual(8, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void GetValidStartingColumns(Type resolver)
		{
			Start(resolver, (Grid grid, BlocksContent content) =>
			{
				content.DoBlocksStartInARandomColumn = true;
				var block = new Block(content, new FixedRandom(), Point.Zero);
				Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, grid.GetValidStartingColumns(block));
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 1)));
				Assert.AreEqual(new[] { 5, 6, 7, 8 }, grid.GetValidStartingColumns(block));
			});
		}

		[IntegrationTest]
		public void GetSingleValidStartingColumn(Type resolver)
		{
			Start(resolver, (Grid grid, BlocksContent content) =>
			{
				content.DoBlocksStartInARandomColumn = false;
				var block = new Block(content, new FixedRandom(), Point.Zero);
				Assert.AreEqual(new[] { 4 }, grid.GetValidStartingColumns(block));
			});
		}
	}
}