using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Represents a grid of bricks: the blocks that have come to rest and not yet been removed
	/// </summary>
	public class Grid
	{
		public Grid(Renderer renderer, Randomizer random)
		{
			this.renderer = renderer;
			Random = random;
			AddBorder();
		}

		private readonly Renderer renderer;
		public Randomizer Random { get; private set; }

		private void AddBorder()
		{
			var topLeft = Brick.RenderOffset - Point.UnitY * Brick.RenderZoom;
			var topRight = Brick.RenderOffset + new Point(GridWidth, -1) * Brick.RenderZoom;
			var bottomLeft = Brick.RenderOffset + new Point(0, GridHeight) * Brick.RenderZoom;
			var bottomRight = Brick.RenderOffset + new Point(GridWidth, GridHeight) * Brick.RenderZoom;

			renderer.Add(new Line2D(topLeft, topRight, Color.Red) { RenderLayer = BackgroundLayer });
			renderer.Add(new Line2D(topRight, bottomRight, Color.Red) { RenderLayer = BackgroundLayer });
			renderer.Add(new Line2D(bottomRight, bottomLeft, Color.Red) { RenderLayer = BackgroundLayer });
			renderer.Add(new Line2D(bottomLeft, topLeft, Color.Red) { RenderLayer = BackgroundLayer });
		}

		private const byte BackgroundLayer = (byte)RenderLayer.Background;

		public int AffixBlock(Block block)
		{
			foreach (Brick brick in block.Bricks)
				AffixBrick(brick);

			RemoveFilledRows();
			return removedRows;
		}

		protected Brick[,] bricks = new Brick[GridWidth,GridHeight];
		private const int GridWidth = 11;
		private const int GridHeight = 20;

		private void AffixBrick(Brick brick)
		{
			brick.TopLeft = new Point((int)brick.TopLeft.X, (int)brick.TopLeft.Y);
			bricks[(int)brick.Position.X, (int)brick.Position.Y - 1] = brick;
			renderer.Add(brick);
			renderer.Add(new ZoomingEffect(brick.Image, brick.DrawArea,
				Rectangle.FromCenter(brick.DrawArea.Center, brick.DrawArea.Size * 2), 0.2f)
			{
				Color = brick.Color,
				RenderLayer = (byte)RenderLayer.ZoomingBrick
			});
		}

		private void RemoveFilledRows()
		{
			removedRows = 0;

			for (int y = 0; y < GridHeight; y++)
				if (IsRowFilled(y))
					RemoveRow(y);
		}

		private int removedRows;

		private bool IsRowFilled(int y)
		{
			for (int x = 0; x < GridWidth; x++)
				if (bricks[x, y] == null)
					return false;

			return true;
		}

		private void RemoveRow(int row)
		{
			for (int x = 0; x < GridWidth; x++)
				RemoveBrick(x, row);

			MoveRowsDown(row);
			removedRows++;
		}

		private void RemoveBrick(int x, int y)
		{
			var brick = bricks[x, y];
			renderer.Remove(brick);
			bricks[x, y] = null;
			renderer.Add(new FallingEffect(brick.Image, brick.DrawArea, 5.0f)
			{
				Color = brick.Color,
				Velocity = new Point(Random.Get(-0.5f, 0.5f), Random.Get(-1.0f, 0.0f)),
				RotationSpeed = Random.Get(-360, 360),
				RenderLayer = (byte)RenderLayer.FallingBrick
			});
		}

		private void MoveRowsDown(int row)
		{
			for (int x = 0; x < GridWidth; x++)
				for (int y = row; y > 0; y--)
					MoveBrickDown(x, y);
		}

		private void MoveBrickDown(int x, int y)
		{
			bricks[x, y] = bricks[x, y - 1];
			if (bricks[x, y] != null)
				bricks[x, y].TopLeft.Y++;
		}

		public bool IsValidPosition(Block block)
		{
			foreach (Brick brick in block.Bricks)
				if (IsOutsideTheGrid(brick) || IsOccupied(brick))
					return false;

			return true;
		}

		private bool IsOutsideTheGrid(Brick brick)
		{
			return brick.Position.X < 0 || brick.Position.X >= GridWidth || brick.Position.Y < 1 ||
				brick.Position.Y >= GridHeight;
		}

		private bool IsOccupied(Brick brick)
		{
			return bricks[(int)brick.Position.X, (int)brick.Position.Y] != null;
		}

		public List<int> GetValidStartingColumns(Block block)
		{
			block.Top = 1;
			var validStartingColumns = new List<int>();

			for (int x = 0; x < GridWidth; x++)
				if (IsAValidStartingColumn(block, x))
					validStartingColumns.Add(x);

			return validStartingColumns;
		}

		private bool IsAValidStartingColumn(Block block, int column)
		{
			block.Left = column;
			return IsValidPosition(block);
		}

		public bool IsABrickOnFirstRow()
		{
			for (int x = 0; x < GridWidth; x++)
				if (bricks[x, 0] != null)
					return true;

			return false;
		}

		public void Clear()
		{
			for (int x = 0; x < GridWidth; x++)
				for (int y = 0; y < GridHeight; y++)
					if (bricks[x, y] != null)
						RemoveBrick(x, y);
		}
	}
}