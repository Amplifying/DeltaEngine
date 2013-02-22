using System;
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
			Start(typeof(TestResolver),
				(Grid grid, Renderer renderer) =>
					Assert.AreEqual(4, renderer.NumberOfActiveRenderableObjects));
		}

		[VisualTest]
		public void RenderBlankGrid(Type resolver)
		{
			Start(resolver, (Grid grid) => { });
		}

		[VisualTest]
		public void RenderBlockAffixedToGrid(Type resolver)
		{
			Start(resolver, (Grid grid, Renderer renderer, Content content) =>
			{
				var block = new Block(content, new FixedRandom(new[] { 0.8f, 0.0f }), Point.One);
				renderer.Add(block);
				grid.AffixBlock(block);
			});
		}

		[Test]
		public void AffixBlocksWhichFillOneRow()
		{
			Start(typeof(TestResolver), (TestGrid grid, Content content) =>
			{
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 20))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 20))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(6, 20))));
				Assert.AreEqual(1,
					grid.AffixBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(10, 17))));

				Assert.AreEqual(3, grid.BrickCount);
				Assert.IsNotNull(grid.Bricks[10, 17]);
				Assert.IsNotNull(grid.Bricks[10, 18]);
				Assert.IsNotNull(grid.Bricks[10, 19]);
			});
		}

		[Test]
		public void AffixBlocksWhichFillTwoRows()
		{
			Start(typeof(TestResolver), (TestGrid grid, Content content) =>
			{
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 19))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 19))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(6, 19))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 20))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 20))));
				Assert.AreEqual(0, grid.AffixBlock(new Block(content, new FixedRandom(), new Point(6, 20))));
				Assert.AreEqual(2,
					grid.AffixBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(10, 17))));
				Assert.AreEqual(2, grid.BrickCount);
				Assert.IsNotNull(grid.Bricks[10, 18]);
				Assert.IsNotNull(grid.Bricks[10, 19]);
			});
		}

		[Test]
		public void IsValidPositionInEmptyGrid()
		{
			Start(typeof(TestResolver), (Grid grid, Content content) =>
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
			Start(typeof(TestResolver), (Grid grid, Content content) =>
			{
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(5, 2)));
				Assert.IsFalse(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(3, 1))));
				Assert.IsTrue(grid.IsValidPosition(new Block(content, new FixedRandom(), new Point(5, 2))));
			});
		}

		[Test]
		public void IsABrickOnFirstRow()
		{
			Start(typeof(TestResolver), (Grid grid, Content content) =>
			{
				Assert.IsFalse(grid.IsABrickOnFirstRow());
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 2)));
				Assert.IsFalse(grid.IsABrickOnFirstRow());
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(2, 1)));
				Assert.IsTrue(grid.IsABrickOnFirstRow());
			});
		}

		[Test]
		public void Clear()
		{
			Start(typeof(TestResolver), (TestGrid grid, Renderer renderer, Content content) =>
			{
				grid.AffixBlock(new Block(content, new FixedRandom(new[] { 0.8f, 0.0f }), Point.One));
				Assert.AreEqual(4, grid.BrickCount);
				Assert.AreEqual(12, renderer.NumberOfActiveRenderableObjects);

				grid.Clear();
				Assert.AreEqual(0, grid.BrickCount);
				Assert.AreEqual(12, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[Test]
		public void GetValidStartingColumns()
		{
			Start(typeof(TestResolver), (Grid grid, Content content) =>
			{
				Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7 },
					grid.GetValidStartingColumns(new Block(content, new FixedRandom(), Point.Zero)));
				grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 2)));
				Assert.AreEqual(new[] { 5, 6, 7 },
					grid.GetValidStartingColumns(new Block(content, new FixedRandom(), Point.Zero)));
			});
		}
	}
}