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
		[Test]
		public void Constructor()
		{
			Start(typeof(TestResolver), (Grid grid) => Assert.IsNotNull(grid.Random));
		}

		[Test]
		public void AffixBlocksWhichFillOneRow()
		{
			Start(typeof(TestResolver), (TestGrid grid, BlocksContent content) =>
			{
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 18))));
				Assert.AreEqual(1,
					grid.AffixBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(11, 15))));

				Assert.AreEqual(3, grid.BrickCount);
				Assert.IsNotNull(grid.Bricks[11, 16]);
				Assert.IsNotNull(grid.Bricks[11, 17]);
				Assert.IsNotNull(grid.Bricks[11, 18]);
			});
		}

		[Test]
		public void AffixBlocksWhichFillTwoRows()
		{
			Start(typeof(TestResolver), (TestGrid grid, BlocksContent content) =>
			{
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 17))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 17))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 17))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 18))));
				Assert.AreEqual(2,
					grid.AffixBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(11, 15))));
				Assert.AreEqual(2, grid.BrickCount);
				Assert.IsNotNull(grid.Bricks[11, 17]);
				Assert.IsNotNull(grid.Bricks[11, 18]);
			});
		}

		[Test]
		public void RowsDontSplit()
		{
			Start(typeof(TestResolver),
				(TestGrid grid, BlocksContent content, Renderer renderer) =>
				{
					content.DoBricksSplitInHalfWhenRowFull = false;
					Assert.AreEqual(0,
						grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18))));
					Assert.AreEqual(0,
						grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18))));
					Assert.AreEqual(0,
						grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 18))));
					Assert.AreEqual(1,
						grid.AffixBlock(new Block(content,
							new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
							new Point(11, 15))));

					Assert.AreEqual(32, renderer.NumberOfActiveRenderableObjects);
				});
		}

		[Test]
		public void RowsSplit()
		{
			Start(typeof(TestResolver),
				(TestGrid grid, BlocksContent content, Renderer renderer) =>
				{
					content.DoBricksSplitInHalfWhenRowFull = true;
					Assert.AreEqual(0,
						grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18))));
					Assert.AreEqual(0,
						grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18))));
					Assert.AreEqual(0,
						grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 18))));
					Assert.AreEqual(1,
						grid.AffixBlock(new Block(content,
							new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
							new Point(11, 15))));

					Assert.AreEqual(44, renderer.NumberOfActiveRenderableObjects);
				});
		}

		[Test]
		public void IsValidPositionInEmptyGrid()
		{
			Start(typeof(TestResolver), (Grid grid, BlocksContent content) =>
			{
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(-1, 1))));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(9, 1))));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(0, 0))));
				Assert.IsFalse(
					grid.IsValidPosition(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(0, 17))));
				Assert.IsTrue(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(2, 2))));
			});
		}

		[Test]
		public void IsValidPositionInOccupiedGrid()
		{
			Start(typeof(TestResolver), (Grid grid, BlocksContent content) =>
			{
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(5, 1)));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(3, 0))));
				Assert.IsTrue(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(5, 2))));
			});
		}

		[Test]
		public void IsABrickOnFirstRow()
		{
			Start(typeof(TestResolver), (Grid grid, BlocksContent content) =>
			{
				Assert.IsFalse(grid.IsABrickOnFirstRow());
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 1)));
				Assert.IsFalse(grid.IsABrickOnFirstRow());
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(2, 0)));
				Assert.IsTrue(grid.IsABrickOnFirstRow());
			});
		}

		[Test]
		public void Clear()
		{
			Start(typeof(TestResolver), (TestGrid grid, Renderer renderer, BlocksContent content) =>
			{
				grid.AffixBlock(new Block(content, new FixedRandom(new[] { 0.8f, 0.0f }), Point.One));
				Assert.AreEqual(4, grid.BrickCount);
				Assert.AreEqual(8, renderer.NumberOfActiveRenderableObjects);

				grid.Clear();
				Assert.AreEqual(0, grid.BrickCount);
				Assert.AreEqual(8, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[Test]
		public void GetValidStartingColumns()
		{
			Start(typeof(TestResolver), (Grid grid, BlocksContent content) =>
			{
				Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
					grid.GetValidStartingColumns(new Block(content, new FixedRandom(), Point.Zero)));
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 1)));
				Assert.AreEqual(new[] { 5, 6, 7, 8 },
					grid.GetValidStartingColumns(new Block(content, new FixedRandom(), Point.Zero)));
			});
		}
	}
}